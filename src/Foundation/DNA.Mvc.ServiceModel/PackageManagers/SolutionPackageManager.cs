//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Xml.Solutions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DNA.Web.ServiceModel
{
    public sealed class SolutionPackageManager : PackageManager<SolutionTemplatePackage, SolutionPackageFactory, WebElement>
    {
        private const string SOL_PKG_PATH = "~/content/packages/";

        /// <summary>
        /// Gets/Sets the current data context object.
        /// </summary>
        public IDataContext DataContext { get; set; }

        private HttpContextBase httpContext { get; set; }

        public SolutionPackageManager() : this(new HttpContextWrapper(HttpContext.Current)) { }

        public SolutionPackageManager(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            this.httpContext = httpContext;
            Init(httpContext.Server.MapPath(SOL_PKG_PATH));
        }

        public SolutionPackageManager(HttpContextBase httpContext, IDataContext context)
            : this(httpContext)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.DataContext = context;
        }

        public SolutionPackageManager(string path)
        {
            Init(path);
        }

        public WebDecorator Install(string name,
            string webName,
            string owner,
            string title,
            string desc = "",
            string theme = "default", string lang = "")
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(webName))
                throw new ArgumentNullException("webName");

            if (DataContext == null)
                throw new ArgumentNullException("DataContext could not be null");

            var defaultLocale = App.Settings.DefaultLocale;

            var pkg = Packages[name];
            if (pkg.GetParams().Count == 0)
            {
                pkg.AddParam("app_path", httpContext.Request.ApplicationPath);
                pkg.AddParam("web_name", webName);
                pkg.AddParam("web_title", webName);
            }

            if (pkg == null)
                throw new Exception("\"" + name + "\" solution package not found");

            var supports = pkg.GetSupportLanguages();
            var installLocale = string.IsNullOrEmpty(lang) ? defaultLocale.ToLower() : lang.ToLower();
            var tmpl = pkg.Locale(installLocale);


            var web = this.DataContext.Find<Web>(w => w.Name.Equals(webName, StringComparison.OrdinalIgnoreCase));
            var solutionName = name + "." + installLocale;

            #region deserialize the web element and create new instance

            if (web == null)
            {
                web = new Web()
                {
                    Created = DateTime.UtcNow,
                    MostOnlined = DateTime.UtcNow,
                    MostOnlineUserCount = 1,
                    IsEnabled = true
                };

                //web.Popuplate(tmpl);
                web.Name = webName;
                web.InstalledSolutions = solutionName;

                if (!string.IsNullOrEmpty(title))
                    web.Title = title;

                if (!string.IsNullOrEmpty(desc))
                    web.Description = desc;

                if (tmpl.LogoImage != null)
                    web.LogoImageUrl = tmpl.LogoImage.Ref;

                if (tmpl.ShortcutIcon != null)
                    web.ShortcutIconUrl = tmpl.ShortcutIcon.Ref;

                if (!string.IsNullOrEmpty(tmpl.Theme))
                    web.Theme = tmpl.Theme;

                if (string.IsNullOrEmpty(web.Theme))
                    web.Theme = theme;

                web.Owner = owner;

                if (!string.IsNullOrEmpty(lang))
                    web.DefaultLocale = lang;
                else
                    web.DefaultLocale = App.Settings.DefaultLocale;

                //else
                //{
                //    if (string.IsNullOrEmpty(web.DefaultLocale) && !string.IsNullOrEmpty(tmpl.DefaultLanguage))
                //    {
                //        web.DefaultLocale = tmpl.DefaultLanguage;
                //    }
                //    else
                //        web.DefaultLocale = defaultLocale;
                //}

                DataContext.Add(web);
            }
            else
            {
                if (!web.Owner.Equals(owner))
                    throw new UnauthorizedAccessException(string.Format("You are not the owner of the {0}.", web.Name));

                var solutionArgs = web.InstalledSolutions.Split(',');
                if (solutionArgs.Contains(solutionName))
                    throw new Exception("The solution had already installed.");

                var wrapper = new WebDecorator(web, DataContext);
                //wrapper.SwitchLocale(lang);

                if (!string.IsNullOrEmpty(title))
                    web.Title = title;

                if (!string.IsNullOrEmpty(desc))
                    web.Description = desc;

                if (tmpl.LogoImage != null)
                    web.LogoImageUrl = tmpl.LogoImage.Ref;

                if (tmpl.ShortcutIcon != null)
                    web.ShortcutIconUrl = tmpl.ShortcutIcon.Ref;

                web.InstalledSolutions += "," + solutionName;
            }

            DataContext.SaveChanges();

            #endregion

            foreach (var wpTmpl in tmpl.Pages)
            {
                try
                {
                    wpTmpl.Install(DataContext, web);
                }
                catch (Exception e)
                {
                    throw new Exception("There is an error occur in creating the \"" + wpTmpl.Title.Text + "\" page", e);
                }
            }

            #region create categories
            var catsHolder = new Dictionary<int, Category>();

            if (tmpl.Categories != null && tmpl.Categories.Categories != null)
            {
                foreach (var cat in tmpl.Categories.Categories)
                {
                    var c = new Category()
                    {
                        //ID = cat.ID,
                        //       ParentID = cat.ParentID,
                        Name = cat.Name,
                        WebID = web.Id,
                        Locale = web.DefaultLocale
                    };

                    if (cat.ID > 0)
                        c.ID = cat.ID;

                    DataContext.Add(c);
                    DataContext.SaveChanges();

                    catsHolder.Add(cat.ID > 0 ? cat.ID : c.ID, c);
                }

                var waitForFixed = tmpl.Categories.Categories.Where(c => c.ParentID > 0);
                foreach (var w in waitForFixed)
                {
                    if (catsHolder.ContainsKey(w.ID))
                        catsHolder[w.ID].ParentID = catsHolder[w.ParentID].ID;
                }
                DataContext.SaveChanges();
            }
            #endregion

            #region create list
            if (tmpl.ListRefs != null && tmpl.ListRefs.Count > 0)
            {
                var wrapperWeb = App.Get().Wrap(web);
                var listTmpls = pkg.LoadContentLists(installLocale);

                foreach (var listTmpl in listTmpls)
                {
                    try
                    {
                        var list = wrapperWeb.CreateList(listTmpl);

                        //if (list.Items != null && list.TotalItems > 0)
                        //{
                        //    foreach (var item in list.Items)
                        //    {
                        //        var catArgs = string.IsNullOrEmpty(item.Categories) ? null : item.Categories.Split(',');
                        //        if (catArgs != null)
                        //        {
                        //            var fixedIDs = new List<int>();
                        //            for (int i = 0; i < catArgs.Length; i++)
                        //            {
                        //                var catID = 0;
                        //                if (int.TryParse(catArgs[i], out catID))
                        //                {
                        //                    if (catsHolder.ContainsKey(catID))
                        //                        fixedIDs.Add(catsHolder[catID].ID);
                        //                }
                        //            }
                        //            if (fixedIDs.Count > 0)
                        //                item.Categories = string.Join(",", fixedIDs.ToArray());
                        //        }
                        //    }
                        //}
                        DataContext.SaveChanges();
                        list.ClearCache();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("There is an error occur in creating the \"" + tmpl.Title.Text + "\" list", e);
                    }
                }
            }
            #endregion

            #region Copy resources
            var ndRoot = "~/app_data/files/" + (web.Name.Equals("home") ? "public" : "personal/" + web.Name) + "/";
            var storagePath = pkg.InstalledPath + "\\files";
            if (System.IO.Directory.Exists(storagePath))
            {
                var dirs = System.IO.Directory.GetDirectories(storagePath);
                foreach (var dir in dirs)
                {
                    //Copy all files to web storage
                    var srcInfo = new DirectoryInfo(dir);
                    var dest = httpContext.Server.MapPath(ndRoot + srcInfo.Name);
                    DNA.Utility.FileUtility.CopyDirectory(dir, dest);
                }
            }
            #endregion
            return new WebDecorator(web, DataContext);
        }

        //private WebPage CreateWebPageFromTemplate(PageElement pageData, Web parentWeb, int parentWebPageID = 0)
        //{
        //    if (parentWeb == null)
        //        throw new ArgumentNullException("parentWeb");

        //    if (pageData.Title == null)
        //        throw new ArgumentNullException("pageData.Title");

        //    if (string.IsNullOrEmpty(pageData.Title.Text))
        //        throw new ArgumentNullException("pageData.Title");

        //    var webName = parentWeb.Name;

        //    var thisPage = DataContext.WebPages.Create(parentWeb, parentWebPageID, pageData);
        //    DataContext.SaveChanges();

        //    #region Create widgets
        //    if (pageData.Widgets != null)
        //    {
        //        if (pageData.Widgets.Count > 0)
        //        {
        //            var groupWidgets = pageData.Widgets.GroupBy(w => w.Zone);

        //            foreach (var orderedTmpl in groupWidgets)
        //            {
        //                var orderedWidgets = orderedTmpl.OrderBy(o => o.Sequence);
        //                foreach (var widgetTmpl in orderedWidgets)
        //                {
        //                    widgetTmpl.Install(DataContext, thisPage);
        //                }
        //            }
        //            DataContext.SaveChanges();
        //        }
        //    }
        //    #endregion

        //    if (pageData.Children != null)
        //    {
        //        foreach (var child in pageData.Children)
        //            CreateWebPageFromTemplate(child, parentWeb, thisPage.ID);
        //    }

        //    return thisPage;
        //}
    }
}
