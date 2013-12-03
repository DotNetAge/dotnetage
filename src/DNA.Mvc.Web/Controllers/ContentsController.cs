//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class ContentsController : Controller
    {
        [HostDashboard("Contents",
            Group = "Extensions",
            Icon = "d-icon-table-2",
            RouteName = "dna_host_ext_contents",
            GroupResKey = "Extensions",
            ResKey = "Contents")]
        public ActionResult Packages()
        {
            return View();
        }

        [SiteDashboard(
            ResKey = "Contents",
            Icon = "d-icon-table-2",
            RouteName = "dna_contents_mgr")]
        [SecurityAction("Manage content lists",
             PermssionSet = "Contents",
             Description = "Allows users to use content manager",
            TitleResName = "SA_ContentMan",
            DescResName = "SA_ContentManDesc",
            PermssionSetResName = "SA_Contents")]
        public ActionResult Lists()
        {
            return View();
        }

        [HostOnly, Loc]
        public ActionResult EditPackage(string name)
        {
            var type = App.Get().ContentTypes[name];
            return PartialView(type);
        }

        /// <summary>
        /// Create new content list by specified list template name
        /// </summary>
        /// <param name="name">Specified the new list name</param>
        /// <param name="title">Specified the list display title text</param>
        /// <param name="locale"></param>
        /// <returns></returns>
        [HttpPost, Loc]
        public ActionResult Create(string @base, string name, string title, string locale, string website = "home", bool showInMenu = false)
        {
            var app = App.Get();
            var list = app.ContentTypes.InstantiateIn(@base, title, name, website, locale, showInMenu);
            return Redirect(list.SettingUrl);
            //return RedirectToAction("Settings", new { id = list.ID });
        }

        [HttpGet, ContentFormPage(FormType = ContentFormTypes.New)]
        public ActionResult New(string list_name)
        {
            var form = App.Get().Context.Form;
            if (form == null)
                return HttpNotFound();

            if (Request.IsAjaxRequest())
                return PartialView(form);
            else
                return View(form);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult New(string list_name, FormCollection forms,
            string categories = "",
            string tags = "",
            bool allowComments = false,
            bool isPublished = false,
            string returnUrl = "",
            string parentID = "",
            int pos = 0)
        {
            var list = App.Get().CurrentWeb.Lists[list_name];
            var cats = categories;
            if (!string.IsNullOrEmpty(cats) && (cats.StartsWith("0")))
                cats = cats.Replace("0,", "");

            var item = list.NewItem(forms, User.Identity.Name, allowComments, isPublished, parentID, pos, categories: cats, tags: tags);

            if (Request.Files.Count > 0)
                item.AttachFiles(Request.Files, App.Get().NetDrive);

            if (list.IsOwner(HttpContext))
            {
                item.Moderated = DateTime.Now;
                item.Auditor = User.Identity.Name;
                item.ModerateState = (int)ModerateStates.Approved;
                item.Save();
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            if (list.DetailForm != null)
                return Redirect(Url.Content(item.Url));

            return Redirect(Url.Content(list.DefaultUrl));
        }

        [HttpGet, ContentFormPage(FormType = ContentFormTypes.Edit)]
        public ActionResult Edit(string list_name, string item_slug)
        {
            if (string.IsNullOrEmpty(list_name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(item_slug))
                throw new ArgumentNullException("slug");

            //var list = App.Get().CurrentWeb.Lists[name];
            var ctx = App.Get().Context;

            var form = ctx.Form;//list.EditForm;

            if (form == null)
                return HttpNotFound();

            var item = ctx.DataItem;//list.GetItem(slug);

            if (item == null)
                return HttpNotFound();

            ViewBag.DataItem = item;

            if (Request.IsAjaxRequest())
                return PartialView(item);
            else
                return View(item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(string list_name,
            string item_slug,
            string removedAttachs,
            string categories,
            string tags,
            string reslug,
            bool? isPublished, bool? allowComments, FormCollection forms, string returnUrl)
        {
            if (string.IsNullOrEmpty(list_name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(item_slug))
                throw new ArgumentNullException("slug");

            var item = App.Get().CurrentWeb.Lists[list_name].GetItem(item_slug);
            var orgUrl = item.UrlComponent;
            var newUrl = "";

            if (isPublished.HasValue)
            {
                item.IsPublished = isPublished.Value;
                item.Published = DateTime.Now;
            }
            else
            {
                item.IsPublished = false;
            }

            var cats = categories;
            if (!string.IsNullOrEmpty(cats) && (cats.StartsWith("0")))
                cats = cats.Replace("0,", "");

            if (allowComments.HasValue)
                item.EnableComments = allowComments.Value;
            else
                item.EnableComments = false;

            item.SetTags(tags);

            item.Modifier = User.Identity.Name;
            item.Modified = DateTime.Now;
            if (!reslug.Equals(item_slug, StringComparison.OrdinalIgnoreCase))
                item.Slug = DNA.Utility.TextUtility.Slug(reslug);

            item.Save(forms);

            item.SetCategories(cats);

            newUrl = item.UrlComponent;
            if (orgUrl.Equals(newUrl, StringComparison.OrdinalIgnoreCase))
                App.Get().Urls.Rename(orgUrl, newUrl);

            var netDrive = App.Get().NetDrive;

            if (!string.IsNullOrEmpty(removedAttachs))
            {
                var attachArgs = removedAttachs.Split(',').Select(r => Convert.ToInt32(r)).ToArray();
                item.DetachFiles(attachArgs, netDrive);
            }

            item.AttachFiles(Request.Files, netDrive);
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return Redirect(item.Url);
        }

        [ContentViewPage]
        public ActionResult Tags(string list_name, string tag, ContentQuery query)
        {
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentNullException("tag");

            return _InternalView(list_name);
        }

        [ContentViewPage]
        public ActionResult Archives(string list_name, int year, int month, ContentQuery query)
        {
            return _InternalView(list_name);
        }

        private ActionResult _InternalView(string list_name)
        {
            if (string.IsNullOrEmpty(list_name))
                throw new ArgumentNullException("name");

            var view = App.Get().Context.View;
            if (view == null)
            {
                var list = App.Get().CurrentWeb.Lists[list_name];

                if (list == null)
                    return HttpNotFound();

                App.Get().Context.List = list;
                view = list.DefaultView;

                if (view == null)
                    throw new ContentViewNotFoundException();

                App.Get().Context.View = view;
            }

            return View("~/views/dynamicui/index.cshtml");
        }

        [ContentViewPage]//, OutputCache(Duration = 60, VaryByParam = "*")
        public ActionResult Views(string list_name, string slug, ContentQuery query)
        {
            if (string.IsNullOrEmpty(list_name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(slug))
                throw new ArgumentNullException("slug");

            var view = App.Get().Context.View;
            if (view == null)
            {
                var list = App.Get().CurrentWeb.Lists[list_name];

                if (list == null)
                    return HttpNotFound();

                App.Get().Context.List = list;
                view = list.Views[slug];

                if (view == null)
                    throw new ContentViewNotFoundException();

                App.Get().Context.View = view;
            }

            return View("~/views/dynamicui/index.cshtml");
        }

        [OutputCache(Duration = 60, VaryByParam = "*")]
        public ActionResult Feed(string list_name, string slug, ContentQuery query, string format = "rss")
        {
            if (string.IsNullOrEmpty(list_name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(slug))
                throw new ArgumentNullException("slug");

            var list = App.Get().CurrentWeb.Lists[list_name];
            if (list == null)
                return HttpNotFound();

            var view = list.Views[slug];

            if (view == null)
                throw new ContentViewNotFoundException();

            if (format == "rss")
                return Content(view.ToRss(), "text/xml", Encoding.UTF8);
            else
                return Content(view.ToAtom(), "text/xml", Encoding.UTF8);
        }

        [ContentFormPage]
        public ActionResult Detail(string list_name, string item_slug, int ver = 0)
        {
            if (string.IsNullOrEmpty(list_name))
                throw new ArgumentNullException("list_name");

            if (string.IsNullOrEmpty(item_slug))
                throw new ArgumentNullException("slug");

            var ctx = App.Get().Context;
            var list = ctx.List;
            //App.Get().CurrentWeb.Lists[name];

            if (list.DetailForm == null)
                return HttpNotFound();

            var item = ctx.DataItem;
            //list.GetItem(slug, ver);
            ViewBag.DataItem = item;

            if (Request.IsAuthenticated && item != null)
            {
                if (!User.Identity.Name.Equals(item.Owner))
                    item.Read();
            }
            else
            {
                if (item != null)
                    item.Read();
            }

            if (item == null)
                return HttpNotFound();

            if (Request.IsAjaxRequest())
                return PartialView(item);
            else
                return View(item);
        }

        [HttpPost]
        public ActionResult Delete(string name, Guid id, string returnUrl)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (id == null || id == Guid.Empty)
                throw new ArgumentNullException("id");

            var list = App.Get().CurrentWeb.Lists[name];
            list.DeleteItem(id);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return Redirect(list.DefaultUrl);
        }

        [Authorize, Loc]
        public ActionResult Reshare(Guid id)
        {
            var item = App.Get().DataContext.Find<ContentDataItem>(id);
            var wrapper = App.Get().Wrap(item);
            return PartialView(wrapper);
        }

        [Authorize, Loc, HttpPost]
        public ActionResult Reshare(Guid id, string annotation)
        {
            var item = App.Get().DataContext.Find<ContentDataItem>(id);
            var wrapper = App.Get().Wrap(item);
            //wrapper.ReshareTo(
            return PartialView();
        }

        [HttpPost]
        public ActionResult Rollback(string name, Guid id)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (id == null || id == Guid.Empty)
                throw new ArgumentNullException("id");

            var list = App.Get().CurrentWeb.Lists[name];

            if (list.EditForm.IsAuthorized(HttpContext))
                return new HttpUnauthorizedResult();

            var item = list.GetItem(id);
            var url = item.Url;

            if (item == null)
                return HttpNotFound();

            item.Rollback();

            return Redirect(url);
        }

        public ActionResult Download(int id)
        {
            var ctx = App.GetService<IDataContext>();
            var attch = ctx.Find<ContentAttachment>(id);
            if (attch == null)
                return HttpNotFound();
            attch.Downloads++;
            ctx.Update(attch);
            ctx.SaveChanges();
            return Redirect(attch.Uri);
        }

        [Loc, SiteDashboard]
        public ActionResult Import()
        {
            return View();
        }

        [Loc,HttpPost]
        public ActionResult DataFileSchema(string file, string type)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");

            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("type");

            var pkg = AppModel.Get().ContentTypes[type];
            ViewBag.FileUri =new Uri(file);
            return PartialView(pkg);
        }

        [SecurityAction("Backup data",
            PermssionSet = "Contents",
            Description = "Allow users to backup list schema and data to file.",
            TitleResName = "SA_ContentBackup",
            DescResName = "SA_ContentBackupDesc",
            PermssionSetResName = "SA_Contents")]
        public ActionResult Export(string name)
        {
            var list = App.Get().CurrentWeb.Lists[name];

            if (list == null)
                return HttpNotFound();
            var backupUri = new Uri(App.Get().Context.AppUrl + "webshared/" + App.Get().CurrentWeb.Name + "/backup/");
            var backupPath = App.Get().NetDrive.MapPath(backupUri);

            if (!System.IO.Directory.Exists(backupPath))
                System.IO.Directory.CreateDirectory(backupPath);

            var fileUri = new Uri(backupUri + list.Name + list.ID.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "." + list.Locale + ".xml");

            var fileName = App.Get().NetDrive.MapPath(fileUri);
            list.ElementWithBase64Data().Save(fileName);
            return Json(fileUri.ToString(), JsonRequestBehavior.AllowGet);
            //var stream = new System.IO.MemoryStream();
            //list.ElementWithBase64Data().Save(stream);
            //stream.Position = 0;
            //return File(stream, "text/xml", list.Name + ".xml");
            //return File(stream, "text/xml");//, list.Name + ".xml");
            //return Content(list.ToXml(), "text/xml", System.Text.Encoding.UTF8);
        }

        [SiteDashboard]
        [SecurityAction("Restore data",
            PermssionSet = "Contents",
            Description = "Allows users to recover views, forms, and data items from backup file.",
            TitleResName = "SA_ContentRestore",
            DescResName = "SA_ContentRestoreDesc",
            PermssionSetResName = "SA_Contents"
            )]
        public ActionResult Restore()
        {
            return View();
        }

        [Loc]
        public ActionResult Locate(string list_name, string item_slug)
        {
            var list = App.Get().CurrentWeb.Lists[list_name];
            if (list != null)
            {
                var item = list.GetItem(item_slug);
                if (item != null)
                    return Redirect(item.Url);
            }
            return HttpNotFound();
        }

        [SiteDashboard, HttpPost]
        public void Restore(string file)
        {
            //App.Get().
            var backupUri = new Uri(App.Get().Context.AppUrl + "webshared/" + App.Get().CurrentWeb.Name + "/backup/" + file);
            var backupFile = App.Get().NetDrive.MapPath(backupUri);
            var list = App.Get().CurrentWeb.CreateList(backupUri.ToString());
        }

        [Loc]
        public ActionResult ValidateSlug(string name, Guid id, string slug)
        {
            var list = App.Get().CurrentWeb.Lists[name];
            var exists = App.Get().DataContext.Where<ContentDataItem>(i => i.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase) && !i.ID.Equals(id)).Count() > 0;
            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        [Loc]
        public ActionResult Versions(string name, Guid id)
        {
            var list = App.Get().CurrentWeb.Lists[name];
            var item = list.GetItem(id);
            return PartialView(item.Versions());
        }

        [Loc]
        public ActionResult Catalog(string name, Guid id)
        {
            var list = App.Get().CurrentWeb.Lists[name];
            ViewData.Model = list.GetItem(id);
            return PartialView();
        }

        [Loc]
        public ActionResult Links() { return PartialView(); }
    }
}
