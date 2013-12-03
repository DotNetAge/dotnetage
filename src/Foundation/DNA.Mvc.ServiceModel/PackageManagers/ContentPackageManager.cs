//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using DNA.Web.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNA.Web.ServiceModel
{
    public sealed class ContentPackageManager : PackageManager<ContentPackage, ContentPackageFactory, ContentList>
    {
        private const string CONTENT_PKG_PATH = "~/content/types";

        /// <summary>
        /// Gets/Sets the current data context object.
        /// </summary>
        public IDataContext DataContext { get; set; }

        private HttpContextBase HttpContext { get; set; }

        public ContentPackageManager() : this(new HttpContextWrapper(System.Web.HttpContext.Current)) { }

        public ContentPackageManager(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            this.HttpContext = httpContext;
            Init(httpContext.Server.MapPath(CONTENT_PKG_PATH));
        }

        public ContentPackageManager(string path) { Init(path); }

        public ContentPackageManager(HttpContextBase httpContext, IDataContext context)
            : this(httpContext)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.DataContext = context;
        }

        public ContentListDecorator InstantiateIn(string baseName, string title = "", string listName = "", string webName = "home", string culture = "", bool showInMenu = false)
        {
            var pkg = Packages[baseName];
            if (pkg == null)
                return null;
            var app = App.Get();
            var web = DataContext.Find<Web>(w => w.Name.Equals(webName, StringComparison.OrdinalIgnoreCase));
            var lang = string.IsNullOrEmpty(web.DefaultLocale) ? App.Settings.DefaultLocale : web.DefaultLocale.ToLower();

            if (!string.IsNullOrEmpty(culture))
            {
                lang = culture.ToLower();
            }

            var contentType = pkg.Locale(lang);
            var listNames = web.Lists.Select(l => l.Name).ToArray();

            if (contentType.IsSingle)
            {
                if (DataContext.Count<ContentList>(c => c.Name.Equals(pkg.Name)) > 0)
                    throw new Exception("ContentType already created.");
            }

            if (string.IsNullOrEmpty(contentType.Name))
                contentType.Name = pkg.Name;

            if (!string.IsNullOrEmpty(title))
            {
                contentType.Title = title;
                if (string.IsNullOrEmpty(listName))
                    contentType.Name = TextUtility.Slug(title);
            }

            if (!string.IsNullOrEmpty(listName))
                contentType.Name = TextUtility.Slug(listName);

            contentType.WebID = web.Id;
            contentType.BaseType = pkg.Name;
            contentType.Owner = web.Owner;
            contentType.LastModified = DateTime.Now;

            //if (!string.IsNullOrEmpty(title))
            //contentType.Name = TextUtility.Slug(title);

            var uniqueName = contentType.Name;
            var i = 0;

            while (listNames.Contains(uniqueName))
                uniqueName = contentType.Name + (++i).ToString();

            if (uniqueName != contentType.Name)
                contentType.Name = uniqueName;

            var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
            var names = new List<string>();

            //        if (v.StartupScripts != null && !string.IsNullOrEmpty(v.StartupScripts.Source))
            //            view.StartupScripts = string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Url.Content(pkg.ResolveUri(v.StartupScripts.Source, v.StartupScripts.Language.Equals(contentType.DefaultLocale, StringComparison.OrdinalIgnoreCase) ? "" : lang)));

            //        if (v.StyleSheet != null && !string.IsNullOrEmpty(v.StyleSheet.Source))
            //            view.StyleSheet = string.Format("<link type=\"text/css\" rel=\"StyleSheet\" href=\"{0}\"></script>", Url.Content(pkg.ResolveUri(v.StyleSheet.Source, v.StyleSheet.Language.Equals(contentType.DefaultLocale, StringComparison.OrdinalIgnoreCase) ? "" : lang)));

            #region unqurie slug
            if (contentType.Views != null)
            {
                foreach (var v in contentType.Views)
                {
                    var slug = v.Name;
                    var flex = slug;
                    var j = 0;
                    while (names.Contains(slug))
                        slug = flex + (++j).ToString();
                    names.Add(slug);

                    if (v.Name != slug)
                        v.Name = slug;
                }

                if (contentType.Views.Count(v => v.IsDefault) == 0)
                    contentType.Views.FirstOrDefault().IsDefault = true;
            }
            #endregion

            var owner = this.HttpContext.User.Identity.Name;
            if (contentType.Items != null)
            {
                contentType.Items.AsParallel().ForAll(it =>
                {
                    if (string.IsNullOrEmpty(it.Owner))
                        it.Owner = owner;
                });
            }

            DataContext.Add(contentType);
            DataContext.SaveChanges();

            var webWrapper = App.Get().Wrap(web);
            var defaultViewPageID = 0;

            if (contentType.Views != null)
            {
                foreach (var v in contentType.Views)
                {
                    #region new page

                    if (!v.NoPage)
                    {
                        var newpage = webWrapper.CreatePage(new ContentViewDecorator(v, DataContext), defaultViewPageID);
                        if (v.IsDefault)
                        {
                            defaultViewPageID = newpage.ID;
                            if (showInMenu)
                            {
                                newpage.Model.ShowInMenu = true;
                                //newpage.Save();
                            }
                        }
                    }

                    #endregion

                    DataContext.SaveChanges();

                }
            }

            //foreach (var f in contentType.Forms)
            //{
            //}

            var result = new ContentListDecorator(DataContext, contentType);
            App.Trigger("ContentListCreated", this, new ContentListEventArgs()
            {
                List = result
            });
            //new ContentListCreatedEvent(result).Raise(HttpContext);
            return result;
        }
    }
}
