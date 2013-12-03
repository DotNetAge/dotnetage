//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    //[Log]
    //[MasterLayout]
    [HandleError(View = "Error")]
    public class DynamicUIController : Controller
    {
        private const string DESCRIPTOR_PATH = "~/content/widgets/";
        private IDataContext dataContext;

        public DynamicUIController(IDataContext context)
        {
            this.dataContext = context;
        }

        private string[] GetPostRoles()
        {
            var strRoles = HttpContext.Request.Form["AccessableRoles"];
            if (!string.IsNullOrEmpty(strRoles))
                return strRoles.Split(',');
            else
                return new string[0];
        }

        /// <summary>
        /// Return the edit page view
        /// </summary>
        /// <param name="id">Specified the page id to edit.</param>
        /// <param name="forms"></param>
        /// <returns>The edit page view</returns>
        [HttpPost, Loc, ValidateInput(false),
        SecurityAction("Pages", "Edit page", "Allows user to edit the page data.",
    TitleResName = "SA_EditPage",
    DescResName = "SA_EditPageDesc",
    PermssionSetResName = "SA_PAGES")]
        public ActionResult Edit(int id, string[] roles, string slug, bool designModel = false)
        {
            var orgUrl = "";
            var newUrl = "";
            ViewBag.DesignModel = designModel;

            if (id == 0) throw new ArgumentNullException("id");
            var page = dataContext.WebPages.Find(id);

            if (page == null)
                return HttpNotFound();

            if (!page.Owner.Equals(User.Identity.Name))
                return new HttpUnauthorizedResult();

            if (!string.IsNullOrEmpty(slug) && !page.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
            {
                orgUrl = App.Get().Wrap(page).FullUrl;
                //var ps = dataContext.WebPages.Filter(w => w.ID != id && w.WebID == page.WebID && w.Locale.Equals(page.Locale, StringComparison.OrdinalIgnoreCase)).ToList();
                var slugs = dataContext.WebPages.Filter(w => w.ID != id && w.WebID == page.WebID && w.Locale.Equals(page.Locale, StringComparison.OrdinalIgnoreCase)).Select(p => p.Slug).ToArray();
                var i = 0;
                page.Slug = slug;
                while (slugs.Contains(page.Slug))
                    page.Slug = slug + (++i).ToString();
            }

            if (ModelState.IsValid)
            {
                if (TryUpdateModel(page, new string[] { "Title", "Description", "Keywords", "LinkTo", "Target", "ShowInMenu", 
                    "ShowInSitemap", "NoFollow", "AllowAnonymous","Dir","ImageUrl","ViewData","CssText","StartupScripts" }))
                {
                    page.Roles.Clear();
                    if ((roles != null) && (roles.Length > 0))
                        dataContext.WebPages.AddRoles(page, roles);
                    page.LastModified = DateTime.Now;
                    dataContext.SaveChanges();

                    if (!string.IsNullOrEmpty(orgUrl))
                    {
                        newUrl = App.Get().Wrap(page).FullUrl;
                        App.Get().Urls.Rename(orgUrl, newUrl);
                    }

                    #region update template file
                    var webName = this.CurrentWebName();
                    var path = string.Format("~/content/layouts/{0}/{1}.cshtml", webName, page.ID.ToString());
                    var _dir = Server.MapPath(string.Format("~/content/layouts/{0}", webName));

                    if (!Directory.Exists(_dir))
                        Directory.CreateDirectory(_dir);

                    System.IO.File.WriteAllText(Server.MapPath(path), page.ViewData, Encoding.UTF8);
                    page.ViewName = path;
                    #endregion

                    App.Get().CurrentWeb.ClearCache();
                }

                return PartialView(page);
            }
            else
                return PartialView(page);
        }

        /// <summary>
        /// Save the page changes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SecurityAction("Pages", "Edit page", "Allows user to edit the page data.",
            TitleResName = "SA_EditPage",
            DescResName = "SA_EditPageDesc",
            PermssionSetResName = "SA_PAGES"
        )]
        public ActionResult Edit(int id, string locale = "", bool designModel = false)
        {
            var page = dataContext.WebPages.Find(id);

            if (page == null)
                return HttpNotFound();

            App.Get().SetCulture(locale);

            ViewBag.DesignModel = designModel;
            return PartialView(page);
        }

        //[SecurityAction("Pages", "Edit page", "Allows user to edit the page data.",
        //    TitleResName = "SA_EditPage",
        //    DescResName = "SA_EditPageDesc",
        //    PermssionSetResName = "SA_PAGES"
        //)]
        [Loc]
        public ActionResult Create() { return PartialView("CreatePage"); }

        /// <summary>
        /// Display the dyanmic page by specified page name.
        /// </summary>
        /// <remarks>
        ///  If the specified page name not found this action will redirect to the NotFound page.
        /// </remarks>
        /// <param name="layout">The dynamic page name</param>
        /// <returns></returns>
        [SiteMapAction]
        public ActionResult Index(string website, string locale, string slug)
        {
            if (Request.IsAjaxRequest())
                return PartialView();
            else
                return View();
        }

        [Authorize, Loc]
        public ActionResult DesignLayout(int id)
        {
            return PartialView(dataContext.Find<WebPage>(id));
        }

        [Authorize, Loc]
        public ActionResult EditCss(int id)
        {
            return PartialView(dataContext.Find<WebPage>(id));
        }

        [Authorize, Loc]
        public ActionResult EditPro(int id)
        {
            return PartialView(dataContext.Find<WebPage>(id));
        }

        [Authorize, Loc]
        public ActionResult Layout(int id)
        {
            if (Request.IsAjaxRequest())
            {
                var file = string.Format("~/Views/DynamicUI/Layouts/Layout_{0}.cshtml", id);
                return PartialView(file);
            }
            else
                return HttpNotFound();
        }

        [Authorize, Loc]
        public ActionResult Layouts(int id)
        {
            var page = AppModel.Get().CurrentWeb.FindPage(id);
            return PartialView(page);
        }

        [SecurityAction("Pages", "Page manager", "Allows the users can access the page manager",
            TitleResName = "SA_PageMan",
            DescResName = "SA_PageManDesc",
            PermssionSetResName = "SA_PAGES"
        )]
        [SiteDashboard(ResKey = "Pages",
            Sequence = 2,
            RouteName = "dna_pages_mgr",
            Icon = "d-icon-sitemap")]
        public ActionResult Sitemap()
        {
            return View();
        }

        [Authorize]
        public ActionResult PageDetail(int id)
        {
            return PartialView(dataContext.WebPages.Find(id));
        }

        [HttpGet, Loc, Authorize]
        public ActionResult InstallSolution()
        {
            return PartialView();
        }

        [Loc, Authorize, HttpPost]
        public ActionResult InstallSolution(string name, string locale)
        {
            var web = App.Get().CurrentWeb;
            if (!web.IsOwner(HttpContext))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);

            App.Get().Solutions.Install(name, web.Name, web.Owner, web.Title, web.Description, web.Theme, locale);
            App.Get().CurrentWeb.ClearCache();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        public ActionResult ThemeLinks(string id)
        {
            var styles = System.Web.Optimization.Styles.Render(string.Format("~/Content/themes/{0}/css", id));
            return Content(styles.ToHtmlString());
        }

        [Loc]
        public ActionResult CreateWeb()
        {
            return PartialView();
        }

        //[Loc]
        //public ActionResult Languages()
        //{
        //    return PartialView();
        //}

        /// <summary>
        /// This action renders for page dialog.
        /// </summary>
        /// <returns></returns>
        [Loc]
        public ActionResult Pages()
        {
            return PartialView();
        }

        /// <summary>
        /// This action renders for links dialog.
        /// </summary>
        /// <returns></returns>
        [Loc]
        public ActionResult Links() { return PartialView(); }

        [Loc]
        public ActionResult LinksEditor(string links)
        {
            ViewBag.ObjectString = string.IsNullOrEmpty(links) ? "[]" : links;
            return PartialView();
        }

        /// <summary>
        /// This action renders for image gallery dialog.
        /// </summary>
        /// <returns></returns>
        [Loc]
        public ActionResult Gallery(string objectStr)
        {
            ViewBag.ObjectString = string.IsNullOrEmpty(objectStr) ? "[]" : objectStr;
            return PartialView();
        }

        [Loc]
        public ActionResult Simulator(string url)
        {
            ViewBag.Url = url;
            return View();
        }
    }
}
