//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class WidgetAPIController : Controller
    {
        //private WebSiteContext _context;

        //public WidgetAPIController(WebSiteContext context) { _context = context; }
        private IDataContext dataContext;

        public WidgetAPIController(IDataContext context)
        {
            dataContext = context;
        }

        /// <summary>
        /// Returns the widget instances of the specified page.
        /// </summary>
        /// <remarks>
        /// REST API: GET /api/widgets
        /// see : http://www.dotnetage.com/doc/dna/api-widget-list.html
        /// </remarks>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Index(Uri url, int id = 0)
        {
            WebPage page = null;
            if (id == 0)
            {
                ///TODO: Find the custom MVC WebPage
                //var routeData = _context.FindRoute(url);
                //if (routeData != null)
                //    page = _context.FindWebPage(url, routeData);
            }
            else
                page = dataContext.WebPages.Find(id);

            if (page == null)
                return Json(new { error = "Page not found" }, JsonRequestBehavior.AllowGet);

            //Auth
            if (page.Web.IsRoot)
            {
                if (!page.Owner.Equals(User.Identity.Name) && !App.Get().Context.HasPermisson(this, "Explorer"))
                    return Json(new { error = "You have not enough permission to get this page." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!page.Owner.Equals(User.Identity.Name))
                    return Json(new { error = "You have not enough permission to get this page." }, JsonRequestBehavior.AllowGet);
            }

            var p = new WebPageDecorator(page, dataContext);
            //var inPageWidgets=p.Widgets.ToList();
            //dataContext.Where<Widget>(f=>f.
            return Json(p.Widgets.Select(w => w.ToObject(HttpContext, page.Locale, this.CurrentWebName())), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Add new widget instance to the specified widget zone.
        /// </summary>
        /// <remarks>
        /// REST API : POST /api/widgets/add 
        /// </remarks>
        /// <param name="id">specified WidgetDescriptor ID</param>
        /// <param name="pid">Specified the Page ID which the widget add to</param>
        /// <param name="zoneID">Specified widget zone id</param>
        /// <param name="pos">the position of the widget in wiget zone</param>
        /// <returns></returns>
        [HttpPost, Loc]
        [SecurityAction("Widgets", "Add widget to page", "Allows user add a new widget instance to the exists page.",
            TitleResName = "SA_AddWidget",
            DescResName = "SA_AddWidgetDesc",
            PermssionSetResName = "SA_Widgets"
            )]
        public ActionResult Add(int id, int pageID, string zoneID, int pos)
        {
            //try
            //{
            var app = App.Get();
            var page = app.CurrentWeb.FindPage(pageID);
            if (!page.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            var widget = page.Widgets.Add(id, zoneID, pos);
            //app.AddWidgetTo(app.CurrentWeb.Name, pageID, id, zoneID, pos);
            ClearCache(widget);
            return Content(widget.ToJsonString(HttpContext, app.Context.Locale, this.CurrentWebName()), "application/json", System.Text.Encoding.UTF8);
            //}
            //catch (Exception e)
            //{
            //    return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            //}

        }

        [HttpPost, Loc, PermissionRequired("Add")]
        public ActionResult AddView(int id, int pageID, string zoneID, int pos)
        {
            try
            {
                var app = App.Get();
                var page = app.CurrentWeb.FindPage(pageID);
                if (!page.IsOwner(HttpContext))
                    return new HttpUnauthorizedResult();

                var viewDescriptor = app.Descriptors[@"content\dataview"];
                //var widget = app.AddWidgetTo(pageID, viewDescriptor.ID, zoneID, pos);
                var widget = page.Widgets.Add(viewDescriptor.Model, zoneID, pos);
                var view = new ContentViewDecorator(app.DataContext.Find<ContentView>(id), app.DataContext);

                widget.ShowHeader = true;
                widget.ShowBorder = false;
                widget.Title = view.Title;
                widget.Link = Url.Content(view.Url);
                widget.SetUserPreference("listName", view.Parent.Name);
                widget.SetUserPreference("viewName", view.Name);
                widget.SetUserPreference("allowFiltering", false);
                widget.SetUserPreference("allowSorting", false);
                widget.SetUserPreference("allowPaging", view.AllowPaging);
                widget.SetUserPreference("pagingInfo", false);
                widget.SetUserPreference("pageIndex", view.PageIndex);
                widget.SetUserPreference("pageSize", view.PageSize);
                widget.Save();
                ClearCache(widget);
                //var widget = app.AddWidgetTo(pageID, id, zoneID, pos);
                return Content(widget.ToJsonString(HttpContext, app.Context.Locale, this.CurrentWebName()), "application/json", System.Text.Encoding.UTF8);
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, Loc, PermissionRequired("Add")]
        public ActionResult AddList(string name, int pageID, string zoneID, int pos)
        {
            try
            {
                var app = App.Get();
                var page = app.CurrentWeb.FindPage(pageID);
                if (!page.IsOwner(HttpContext))
                    return new HttpUnauthorizedResult();

                var listDescriptor = app.Descriptors[@"content\listinfo"];
                var viewDescriptor = app.Descriptors[@"content\dataview"];
                var listWidget = page.Widgets.Add(listDescriptor.Model, zoneID, pos);
                //app.AddWidgetTo(pageID, listDescriptor.ID, zoneID, pos);
                var widget = page.Widgets.Add(viewDescriptor.Model, zoneID, pos + 1);
                //app.AddWidgetTo(pageID, viewDescriptor.ID, zoneID, pos + 1);
                var list = app.CurrentWeb.Lists[name];
                var view = list.DefaultView;
                //var view = new ContentViewDecorator(app.DataContext.Find<ContentView>(id), app.DataContext);
                listWidget.ShowHeader = false;
                listWidget.ShowBorder = false;
                listWidget.Title = list.Title;
                listWidget.SetUserPreference("listName", name);
                widget.SetUserPreference("showTools", true);
                listWidget.Save();

                widget.ShowHeader = false;
                widget.ShowBorder = false;
                widget.Title = view.Title;
                widget.Link = Url.Content(view.Url);
                widget.SetUserPreference("listName", view.Parent.Name);
                widget.SetUserPreference("viewName", view.Name);
                widget.SetUserPreference("allowFiltering", false);
                widget.SetUserPreference("allowSorting", false);
                widget.SetUserPreference("allowPaging", view.AllowPaging);
                widget.SetUserPreference("pagingInfo", true);
                widget.SetUserPreference("pageIndex", view.PageIndex);
                widget.SetUserPreference("pageSize", view.PageSize);
                widget.Save();

                //var widget = app.AddWidgetTo(pageID, id, zoneID, pos);
                //return Content(widget.ToJsonString(HttpContext, app.Context.Locale, this.CurrentWebName()), "application/json", System.Text.Encoding.UTF8);
                ClearCache(widget);
                return Content("[" + listWidget.ToJsonString(HttpContext, app.Context.Locale, this.CurrentWebName()) + "," +
                    widget.ToJsonString(HttpContext, app.Context.Locale, this.CurrentWebName())
                    + "]", "application/json", System.Text.Encoding.UTF8);
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, Loc, PermissionRequired("Add")]
        public ActionResult AddType(string name, int pageID, string zoneID, int pos, string locale)
        {
            var list = App.Get().ContentTypes.InstantiateIn(name, webName: App.Get().CurrentWeb.Name, culture: locale);
            return AddList(list.Name, pageID, zoneID, pos);
        }

        [HttpPost, Loc, PermissionRequired("Add")]
        public ActionResult AddFile(string file, int pageID, string zoneID, int pos)
        {
            try
            {
                var app = App.Get();
                var page = app.CurrentWeb.FindPage(pageID);
                if (!page.IsOwner(HttpContext))
                    return new HttpUnauthorizedResult();

                var webFile = new WebResourceInfo(file);
                WidgetDescriptorDecorator descriptor = null;

                if (webFile.IsFile)
                {

                    if (descriptor == null && webFile.ContentType.StartsWith("image"))
                        descriptor = app.Descriptors[@"multimedia\image"];

                    if (descriptor == null && webFile.ContentType.StartsWith("video"))
                        descriptor = app.Descriptors[@"multimedia\video"];

                    if (descriptor == null && webFile.ContentType.Equals("text/html"))
                        descriptor = app.Descriptors[@"text\htmlfile"];

                    //if (webFile.ContentType.Equals("text/xml"))
                    //    descriptor = app.Descriptors[@"text\code"];

                    if (webFile.ContentType.Equals("text/javascript") || webFile.ContentType.Equals("text/vbscript"))
                        descriptor = app.Descriptors[@"text\script"];

                    if (descriptor == null && webFile.Extension.Equals(".css", StringComparison.OrdinalIgnoreCase))
                        descriptor = app.Descriptors[@"text\css"];

                    var codeexts = new string[] { ".cs", ".vb", ".sql", ".c", ".asp", ".aspx" };
                    if (descriptor == null && (codeexts.Contains(webFile.Extension.ToLower()) || (webFile.ContentType.Equals("text/xml"))))
                        descriptor = app.Descriptors[@"text\codefile"];

                    //var isText = false;

                    if (descriptor == null && webFile.ContentType.StartsWith("text"))
                    {
                        //;isText = true;
                        descriptor = app.Descriptors[@"text\notes"];
                    }

                    if (descriptor == null)
                        descriptor = app.Descriptors[@"content\filedownload"];
                }
                else
                    descriptor = app.Descriptors[@"content\downloadlist"];

                var widget = page.Widgets.Add(descriptor.Model, zoneID, pos);
                //app.AddWidgetTo(pageID, descriptor.ID, zoneID, pos);

                widget.ShowHeader = false;
                widget.ShowBorder = false;
                widget.Title = webFile.Name;

                //if (isText)
                //{
                //    var _file = app.NetDrive.MapPath(new Uri(file));
                //    if (System.IO.File.Exists(_file))
                //    {
                //        var fileContent = System.IO.File.OpenText(_file);
                //        widget.SetUserPreference("content", fileContent);
                //    }
                //}
                //else
                widget.SetUserPreference("src", file);

                widget.Save();
                ClearCache(widget);
                return Content(widget.ToJsonString(HttpContext, app.Context.Locale, this.CurrentWebName()), "application/json", System.Text.Encoding.UTF8);
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost, Loc, PermissionRequired("Add")]
        public ActionResult AddSlideShow(int pageID, string zoneID, int pos, string json)
        {
            var app = App.Get();
            var page = app.CurrentWeb.FindPage(pageID);
            if (!page.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            WidgetDescriptorDecorator descriptor = app.Descriptors[@"multimedia\slideshow"];

            var widget = page.Widgets.Add(descriptor.Model, zoneID, pos);
            widget.ShowHeader = false;
            widget.ShowBorder = false;

            //widget.Title = webFile.Name;
            widget.SetUserPreference("data", json);
            widget.Save();
            ClearCache(widget);
            return Content(widget.ToJsonString(HttpContext, app.Context.Locale, this.CurrentWebName()), "application/json", System.Text.Encoding.UTF8);

        }

        /// <summary>
        /// Delete the widget by specified id. 
        /// </summary>
        /// <remarks>
        /// REST API: POST /api/widgets/remove
        /// </remarks>
        /// <param name="id">The id of the widget.</param>
        [HttpPost]
        [SecurityAction("Widgets", "Delete widget", "Allows users delete the existing widgets.",
            ThrowOnDeny = true,
            TitleResName = "SA_DeleteWidget",
            DescResName = "SA_DeleteWidgetDesc",
            PermssionSetResName = "SA_Widgets"
            )]
        public ActionResult Remove(int id)
        {
            var app = App.Get();
            var widget = app.CurrentWeb.Widgets.FirstOrDefault(w => w.ID == id);
            if (widget == null)
                return HttpNotFound("Widget not found.");

            var page = app.CurrentWeb.FindPage(widget.PageID);
            if (page == null)
                return HttpNotFound("Page not found.");

            if (!page.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            App.Get().CurrentWeb.Widgets.Remove(id);
            ClearCache(widget);
            return new HttpStatusCodeResult(200);
        }

        /// <summary>
        /// Toggle the widget's expanded state.
        /// </summary>
        /// <remarks>
        /// REST API : POST /api/widgets/toggle
        /// </remarks>
        /// <param name="id"> Specified the widget id to toggle. </param>
        [HttpPost]
        [SecurityAction("Widgets", "Toggle widget state", "Allows users collapse or expand widgets.",
            ThrowOnDeny = true,
            TitleResName = "SA_ToggleWidget",
            DescResName = "SA_ToggleWidgetDesc",
            PermssionSetResName = "SA_Widgets"
            )]
        public ActionResult Toggle(int id)
        {
            if (id > 0)
            {
                var app = App.Get();
                var widget = app.CurrentWeb.Widgets.FirstOrDefault(w => w.ID == id);
                if (widget == null)
                    return HttpNotFound("Widget not found.");

                var page = app.CurrentWeb.FindPage(widget.PageID);
                if (page == null)
                    return HttpNotFound("Page not found.");

                if (!page.IsOwner(HttpContext))
                    return new HttpUnauthorizedResult();

                App.Get().CurrentWeb.Widgets.Get(id).Toggle();
                ClearCache(widget);
            }

            return new HttpStatusCodeResult(200);
        }

        /// <summary>
        /// Move the widget to anthor widget zone or new position.
        /// </summary>
        /// <param name="id">The specified widget id.</param>
        /// <param name="zoneID">The target widget zone id.</param>
        /// <param name="pos">The new  position of the widget</param>
        /// <remarks>
        /// REST API: POST /api/widgets/moveto
        /// </remarks>
        [HttpPost]
        [SecurityAction("Widgets", "Drag and drop widgets", "Allows user drag and drop the widgets on the page.",
            ThrowOnDeny = true,
            TitleResName = "SA_MoveWidget",
            DescResName = "SA_MoveWidgetDesc",
            PermssionSetResName = "SA_Widgets"
            )]
        public ActionResult MoveTo(int id, string zoneID, int pos)
        {
            if (id == 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "id parameter is required");

            if (string.IsNullOrEmpty(zoneID))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "zoneID parameter is required");
            //throw new ArgumentNullException("zoneID");

            if (pos < 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "position parameter must be greater than zero.");
            //throw new ArgumentOutOfRangeException("position");

            var app = App.Get();
            var widget = app.CurrentWeb.Widgets.FirstOrDefault(w => w.ID == id);
            if (widget == null)
                return HttpNotFound("Widget not found.");

            var page = app.CurrentWeb.FindPage(widget.PageID);
            if (page == null)
                return HttpNotFound("Page not found.");

            if (!page.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();


            App.Get().CurrentWeb.Widgets.Get(id).MoveTo(zoneID, pos);
            ClearCache(widget);
            return new HttpStatusCodeResult(200);
        }

        ///// <summary>
        ///// Apply the common settings of the widget.
        ///// </summary>
        ///// <param name="id">The specified widget id</param>
        //[HttpPost]
        //[SecurityAction("Widgets", "Apply widget settings", "Allows users can apply the common widget settings changes",
        //    ThrowOnDeny = true,
        //    TitleResName = "SA_ApplyWidgetSettings",
        //    DescResName = "SA_ApplyWidgetSettingsDesc",
        //    PermssionSetResName = "SA_Widgets"
        //    )]
        //public ActionResult Update(Guid id, string title, string link = "", string icon = "", bool isclosable = true, bool showBorder = true, bool showHeader = true)
        //{
        //    if (id.Equals(Guid.Empty))
        //        return Json(new { error = "The widget id can not be empty." }, JsonRequestBehavior.AllowGet);

        //    using (_context)
        //    {
        //        var widget = App.Get().CurrentWeb.Widgets[id];
        //        if (widget != null)
        //        {
        //            widget.Title = title;
        //            widget.Link = link;
        //            widget.ShowHeader = showHeader;
        //            widget.IconUrl = icon;
        //            widget.Save();
        //            return Content(widget.ToJsonString(HttpContext, this.CurrentWebName()), "application/json", System.Text.Encoding.UTF8);
        //        }
        //        else
        //            return Json(new { error = "Widget not found." }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        /// <summary>
        /// Apply the user preferences change
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false),
            SecurityAction("Widgets", "Apply widget settings", "Allows users can apply the common widget settings changes",
                ThrowOnDeny = true,
                TitleResName = "SA_ApplyWidgetSettings",
                DescResName = "SA_ApplyWidgetSettingsDesc",
                PermssionSetResName = "SA_Widgets"
                )
        ]
        public ActionResult Apply(int id, FormCollection forms)
        {
            var app = App.Get();
            var widget = dataContext.Widgets.Find(id);

            if (widget == null)
                return HttpNotFound("Widget not found.");

            var page = app.CurrentWeb.FindPage(widget.PageID);
            if (page == null)
                return HttpNotFound("Page not found.");

            if (!page.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();


            //var descriptor = widget.WidgetDescriptor;

            var settings = widget.ReadUserPreferences();
            //descriptor.Properties;
            //widget.WidgetDescriptor.Defaults

            #region set properties
            foreach (var k in forms.AllKeys)
            {
                if (k == "id")
                    continue;

                var val = settings.FirstOrDefault(d => d["name"].ToString().Equals(k, StringComparison.OrdinalIgnoreCase));

                if (val != null)
                {
                    var rawVal = forms[k];
                    var intVal = 0;
                    double dbVal = 0.0;

                    #region set values
                    if (rawVal == "on")
                        val["value"] = true;
                    else
                    {
                        if (rawVal.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                            rawVal.Equals("false", StringComparison.OrdinalIgnoreCase))
                        {
                            val["value"] = bool.Parse(rawVal);
                        }
                        else
                        {
                            if (int.TryParse(rawVal, out intVal))
                            {
                                val["value"] = intVal;
                            }
                            else
                            {
                                if (double.TryParse(rawVal, out dbVal))
                                {
                                    val["value"] = dbVal;
                                }
                                else
                                {
                                    val["value"] = forms[k];
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion

            widget.SaveUserPreferences(settings);
            dataContext.Widgets.Update(widget);
            dataContext.SaveChanges();
            ClearCache(widget);
            ClearHtmlCache(widget);
            return new HttpStatusCodeResult(200);
        }

        ///// <summary>
        ///// Add a new widget instance from the import data file
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[SecurityAction("Widgets", "Import user preferences", "Allows users can upload the widget user preferences data file and import to the exists page.",
        //    ThrowOnDeny = true,
        //    TitleResName = "SA_ImportWidgetData",
        //    DescResName = "SA_ImportWidgetDataDesc",
        //    PermssionSetResName = "SA_Widgets"
        //    )]
        //public ActionResult Import(Uri url, string zoneID, int pos)
        //{
        //    //var postedFile = Request.Files["importedFile"];
        //    //var page = _context.FindWebPage(url);

        //    //if (page == null)
        //    //{
        //    //    var routeData = _context.FindRoute(url);
        //    //    if (routeData != null)
        //    //    {
        //    //        page = _context.FindWebPage(url, routeData);
        //    //    }
        //    //}

        //    //var _tmpl = TemplateHelper.LoadWidgetTemplate(postedFile.InputStream);
        //    //var widgetInstance = _context.DataContext.Widgets.AddWidget(Guid.NewGuid(), _tmpl, page.Path, zoneID, pos);
        //    //widgetInstance.ZoneID = zoneID;
        //    //widgetInstance.Pos = pos;
        //    //_context.DataContext.SaveChanges(false);
        //    //return Json(widgetInstance.ToJSON(page.Web.Name), JsonRequestBehavior.AllowGet);
        //    throw new NotImplementedException();
        //}

        [Authorize, ValidateInput(false), PermissionRequired("Apply")]
        public ActionResult ApplyStyle(int id, string box = "", string body = "", string header = "")
        {
            var app = App.Get();
            var widget = dataContext.Widgets.Find(id);

            if (widget == null)
                return HttpNotFound("Widget not found.");

            var page = app.CurrentWeb.FindPage(widget.PageID);
            if (page == null)
                return HttpNotFound("Page not found.");

            if (!page.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            widget.CssText = box;
            widget.HeaderCssText = header;
            widget.BodyCssText = body;
            dataContext.Widgets.Update(widget);
            //dataContext.SaveChanges(false);
            dataContext.SaveChanges();
            ClearCache(widget);
            ClearHtmlCache(widget);
            return new HttpStatusCodeResult(200);
        }

        [Authorize, HttpGet]
        public ActionResult Roles(string id)
        {
            // var gid = Guid.Empty;
            var gid = 0;
            //Guid.TryParse(id, out gid);
            int.TryParse(id, out gid);
            if (gid > 0)
            {
                var _roles = App.Get().CurrentWeb.Widgets.Get(gid).Roles;
                if (_roles != null)
                    return Json(_roles, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var installedPath = Server.UrlDecode(id);
                var descriptor = App.Get().Descriptors[installedPath];
                if (descriptor != null)
                    return Json(descriptor.Roles, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "No roles found" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost]
        public ActionResult Roles(string id, string[] roles)
        {
            var gid = 0;// Guid.Empty;
            //Guid.TryParse(id, out gid);
            int.TryParse(id, out gid);

            if (gid > 0)
            {
                var w = App.Get().CurrentWeb.Widgets.Get(gid);
                var p = App.Get().CurrentWeb.FindPage(w.PageID);

                if (!p.IsOwner(HttpContext))
                    return new HttpUnauthorizedResult();

                w.AddRoles(roles);
                return Json(roles, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!User.Identity.Equals(App.Settings.Administrator))
                    return new HttpUnauthorizedResult();

                var installedPath = Server.UrlDecode(id);
                var descriptor = App.Get().Descriptors[installedPath];
                if (descriptor != null)
                {
                    descriptor.AddRoles(roles);
                    return Json(roles, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { error = "No roles found" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost, PermissionRequired("Apply")]
        public ActionResult Save(int id, string title, string link, string iconUrl, string[] roles,
            bool showHeader = false,
            bool showBorder = false,
            bool transparent = false,
            int showMode = 0,
            string viewMode = "",
            string headerClass = "",
            string contentClass = "",
            string bodyCssText = "",
            string cssText = "",
            string headerCssText = "",
            bool noroles = false)
        {
            var widget = App.Get().CurrentWeb.Widgets.Get(id);
            var showModeChanged = (int)widget.ShowMode != showMode;
            widget.Title = title;
            widget.Link = link;
            widget.IconUrl = iconUrl;
            widget.ShowHeader = showHeader;
            widget.ShowBorder = showBorder;
            widget.ShowMode = (WidgetShowModes)showMode;
            widget.BodyClass = contentClass;
            widget.BodyCssText = bodyCssText;
            widget.HeaderClass = headerClass;
            widget.HeaderCssText = headerCssText;
            widget.CssText = cssText;
            widget.ViewMode = viewMode;
            widget.Transparent = transparent;

            widget.Save();
            if (noroles == false)
                widget.AddRoles(roles);

            ClearCache(widget);
            ClearHtmlCache(widget);

            if (WidgetShowModes.ParentPage != (WidgetShowModes)showMode)
            {
                var page = widget.WebPage;
                var locale = page.Locale;
                //Clear all widget cache
                var pages = App.Get().CurrentWeb.Pages.Where(p => p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var p in pages)
                {
                    var key = "page" + p.ID.ToString() + "_widgets";
                    if (HttpContext.Cache[key] != null)
                        HttpContext.Cache.Remove(key);
                }
            }

            return Content(widget.ToJsonString(HttpContext, this.CurrentWebName()), "application/json", System.Text.Encoding.UTF8);
        }

        private void ClearCache(WidgetInstance widget)
        {
            var key = "page" + widget.PageID.ToString() + "_widgets";
            if (HttpContext.Cache[key] != null)
                HttpContext.Cache.Remove(key);
        }

        private void ClearHtmlCache(WidgetInstance widget)
        {
            var key = "widget" + widget.ID.ToString() + "_caching_html";
            if (HttpContext.Cache[key] != null)
                HttpContext.Cache.Remove(key);
        }
    }
}
