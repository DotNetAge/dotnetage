//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DNA.Web;
using DNA.Web.ServiceModel;
using System.Xml.Linq;

namespace DNA.Modules.WebManager.Controllers
{
    public class HouseController : Controller
    {
        [HostDashboard(Text = "Webs", Icon = "d-icon-earth")]
        public ActionResult Index()
        {
            return View(App.Get().Webs);
        }

        [HostDashboard]
        public ActionResult Detail(string id)
        {
            return PartialView(App.Get().Webs[id]);
        }

        //[MyDashboard(Text = "My webs", Icon = "d-icon-earth")]
        //public ActionResult MyWebs()
        //{
        //    return View("~/Views/Account/MyWebs.cshtml", App.Get().User.Webs);
        //}

        /// <summary>
        /// Backup all the website
        /// </summary>
        /// <param name="name"></param>
        /// <param name="locale"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        [HostOnly, Authorize, Loc]
        public ActionResult Export(string web, string name, string desc = "")
        {
            if (!User.IsAdministrator())
                return new HttpUnauthorizedResult();

            var app = App.Get();
            var netdrive = app.NetDrive;
            //var name = app.CurrentWeb.Name;
            var currentWeb = app.Webs[web];
            //var xmlFile = netdrive.MapPath(new Uri(String.Format("{0}webshared/{1}/_backups/{2}/", app.Context.AppUrl.ToString(), app.Context.Website, name))) + "\\config.xml";

            //var path = System.IO.Path.Combine(Server.MapPath("~/Content/Packages/"), name);
            //if (!System.IO.Directory.Exists(path))
            //    System.IO.Directory.CreateDirectory(path);
            var basePath = "~/Content/Packages/";
            var formattedName = DNA.Utility.TextUtility.Slug(name);
            var solPath = basePath + formattedName;
            var path = PreparePath(name, basePath);
            var xmlFile = System.IO.Path.Combine(path, "config.xml");
            var defaultLocale = currentWeb.Culture.ToLower();
            var defaultNamespace = "http://www.dotnetage.com/XML/Schema/solution";
            XNamespace ns = defaultNamespace;

            #region Build solution xml

            var element = new XElement(ns + "solution",
                 new XAttribute("xmlns", defaultNamespace),
                new XElement(ns + "title", name),
                new XElement(ns + "description", desc),
                //new XAttribute("name", name),
                new XAttribute("theme", currentWeb.Theme),
                new XAttribute("defaultLocale", currentWeb.Culture.ToLower()));

            if (!string.IsNullOrEmpty(app.CurrentWeb.LogoImageUrl))
            {
                var src = app.CurrentWeb.LogoImageUrl;

                element.Add(new XElement(ns + "logo", new XAttribute("src", src.StartsWith("data:image") ? src : System.IO.Path.GetFileName(currentWeb.LogoImageUrl))));
                if (!src.StartsWith("data:image"))
                {
                    CopyResource(netdrive, PreparePath("images", solPath), currentWeb.LogoImageUrl);
                }
            }


            #endregion

            #region Build page xmls
            var topPages = currentWeb.Pages.Where(p => p.ParentID == 0);

            foreach (var page in topPages)
            {
                if (page.IsShared && !page.ShowInMenu)
                    continue;

                var pageLocale = page.Locale.ToLower();
                var isDefaultLocale = pageLocale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase);
                var buildPagePath = isDefaultLocale ? PreparePath("pages", solPath) : PreparePath("pages", solPath + "/locales/" + pageLocale + "/pages");
                var buildImagePath = isDefaultLocale ? PreparePath("images", solPath) : PreparePath("images", solPath + "/locales/" + pageLocale + "/images");

                var slug = page.Slug.Replace("/", "-");
                //var pageFile = netdrive.MapPath(buildPageUri) + "\\" + slug + ".xml";
                var pageFile = System.IO.Path.Combine(buildPagePath, slug + ".xml");

                if (isDefaultLocale)
                    element.Add(new XElement(ns + "page", new XAttribute("src", "pages/" + slug + ".xml")));
                else
                    element.Add(new XElement(ns + "page", new XAttribute(XNamespace.Xml + "lang", pageLocale), new XAttribute("src", "pages/" + slug + ".xml")));

                if (!string.IsNullOrEmpty(page.IconUrl) && !page.ImageUrl.StartsWith("data:image"))
                    CopyResource(netdrive, buildImagePath, page.IconUrl);

                if (!string.IsNullOrEmpty(page.ImageUrl) && !page.ImageUrl.StartsWith("data:image"))
                    CopyResource(netdrive, buildImagePath, page.ImageUrl);

                if (page.Scripts != null && page.Scripts.Length > 0)
                {
                    var buildScriptUri = pageLocale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase) ? PreparePath(name, "scripts") : PreparePath(name, "locales/" + pageLocale + "/scripts");

                    foreach (var script in page.Scripts)
                        CopyResource(netdrive, buildScriptUri, script);
                }

                if (page.StyleSheets != null && page.StyleSheets.Length > 0)
                {
                    var buildCssUri = pageLocale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase) ? PreparePath(name, "css") : PreparePath(name, "locales/" + pageLocale + "/css");

                    foreach (var styleSheet in page.StyleSheets)
                        CopyResource(netdrive, buildPagePath, styleSheet);
                }

                page.Save(pageFile);
            }

            #endregion

            #region Build list xmls

            var lists = currentWeb.Lists;
            foreach (var list in lists)
            {
                var listLocale = list.Locale.ToLower();
                var isDefaultLocale = listLocale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase);
                var buildListUri = isDefaultLocale ? PreparePath("lists", solPath) : PreparePath("list", solPath + "/locales/" + listLocale + "/lists");
                //var listFile = netdrive.MapPath(buildListUri) + "\\" + list.Name + ".xml";
                var listFile = System.IO.Path.Combine(buildListUri, list.Name + ".xml");

                if (isDefaultLocale)
                    element.Add(new XElement(ns + "list", new XAttribute("src", "lists/" + list.Name + ".xml")));
                else
                    element.Add(new XElement(ns + "list", new XAttribute(XNamespace.Xml + "lang", listLocale), new XAttribute("src", "lists/" + list.Name + ".xml")));

                list.Save(listFile);
            }

            #endregion

            #region Build cats
            var cats = app.DataContext.Where<Category>(c => c.WebID == currentWeb.Id).ToList();
            if (cats != null && cats.Count() > 0)
            {
                var catsEle = new XElement("categories");
                element.Add(catsEle);
                foreach (var cat in cats)
                {
                    var catLocale = cat.Locale.ToLower();
                    var isDefaultLocale = catLocale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase);
                    var catEle = new XElement(ns + "category", new XAttribute("id", cat.ID),
                        new XAttribute("name", cat.Name),
                        new XAttribute("parentId", cat.ParentID));

                    catsEle.Add(catEle);
                    if (!isDefaultLocale)
                        catEle.Add(new XAttribute(XNamespace.Xml + "lang", catLocale));
                }
            }
            #endregion

            #region Copy files
            var filesPath = PreparePath("files", solPath);
            var ndRoot = app.NetDrive.MapPath(new Uri(app.Context.AppUrl.ToString() + "webshared/" + web));
            DNA.Utility.FileUtility.CopyDirectory(ndRoot, filesPath);
            #endregion

            //if (s)
            var listIndexPath = System.IO.Path.Combine(filesPath, "lists");
            if (System.IO.Directory.Exists(listIndexPath))
                System.IO.Directory.Delete(listIndexPath, true);

            element.Save(xmlFile);
            return new HttpStatusCodeResult(200);
        }

        [HostDashboard]
        public ActionResult Templates()
        {
            return View();
        }

        private void CopyResource(INetDriveService netdrive, string basePath, string targetFile)
        {
            var orgPath = Server.MapPath(targetFile);
            var file = System.IO.Path.Combine(basePath, System.IO.Path.GetFileName(targetFile));

            if (System.IO.File.Exists(orgPath) && !System.IO.File.Exists(file))
                System.IO.File.Copy(orgPath, file);
        }

        private string PreparePath(string name, string path)
        {
            var destPath = System.IO.Path.Combine(path.StartsWith("~") ? Server.MapPath(path) : path, name);
            if (!System.IO.Directory.Exists(destPath))
                System.IO.Directory.CreateDirectory(destPath);
            return destPath;

            //var app = App.Get();
            //var netdrive = app.NetDrive;
            //var basePath = String.Format("{0}webshared/{1}/_backups/{2}/", app.Context.AppUrl.ToString(), app.Context.Website, name);
            //var baseUri = new Uri(basePath);
            //if (!netdrive.Exists(baseUri))
            //    netdrive.CreatePath(baseUri);

            //var buildingPath = basePath + path;
            //var buildUri = new Uri(buildingPath);
            //if (!netdrive.Exists(buildUri))
            //    netdrive.CreatePath(buildUri);
            //return buildUri;
        }

    }
}
