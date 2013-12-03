//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace DNA.Web.Controllers
{
    public class WebPageAPIController : Controller
    {
        //private WebSiteContext _context;
        //private const string DESCRIPTOR_PATH = "~/content/widgets/";
        ///public WebPageAPIController(WebSiteContext context) { _context = context; }
        private IDataContext dataContext;

        public WebPageAPIController(IDataContext context)
        {
            dataContext = context;
        }
        /// <summary>
        /// Get a web page object by specified uri or page id
        /// </summary>
        /// <remarks>
        /// REST API: GET /api/pages?url=url
        /// </remarks>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Index(Uri url, int id = 0)
        {
            return View();
        }

        /// <summary>
        /// Get the web page templates
        /// </summary>
        /// <remarks>
        /// REST API : GET /api/pages/templates
        /// </remarks>
        /// <returns></returns>
        [Authorize]
        public ActionResult Templates()
        {
            var tmpls = App.Get().Pages.Packages.Select(m => m.Model).ToList();
            return Json(tmpls.Select(t => t.ToJSON()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost,
        SecurityAction("Pages", "Add page", "Allows users can be create new pages.",
            TitleResName = "SA_CreateNewPage",
            DescResName = "SA_CreateNewPageDesc",
            PermssionSetResName = "SA_PAGES"
            )]
        public ActionResult Add(string website, string title, string desc, string keywords, int parentId = 0, string layout = "", string tmpl = "blank", string locale = "")
        {
            var web = App.Get().Webs[string.IsNullOrEmpty(website) ? "home" : website];

            if (web == null)
                throw new HttpException("The " + website + " not found");

            try
            {
                var page = App.Get().Pages.InstantiateIn(tmpl, web.Model, title, desc, keywords, parentId, string.IsNullOrEmpty(locale) ? web.DefaultLocale : locale);

                if (!string.IsNullOrEmpty(layout))
                {
                    page.ViewData = "";
                    page.ViewName = "~/views/dynamicui/layouts/layout_" + layout + ".cshtml";
                    page.Save();
                }
                web.ClearCache();
                return Content(page.ToJsonString(), "application/json", System.Text.Encoding.UTF8);
            }
            catch (Exception e)
            {
                var sb = new StringBuilder();
                sb.AppendLine(e.Message);
                var etp = e.InnerException;
                while (etp != null)
                {
                    sb.AppendLine(etp.Message);
                    etp = etp.InnerException;
                }
                throw new HttpException(sb.ToString());
                //return new HttpStatusCodeResult(500, sb.ToString());
            }
        }

        [HttpPost, Loc, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.DynamicUIController,DNA.Web")]
        public ActionResult Edit(int id)
        {
            var page = App.Get().DataContext.Find<WebPage>(id);
            var orgUrl = App.Get().Wrap(page).FullUrl;
            TryUpdateModel(page);
            App.Get().DataContext.SaveChanges();
            var newUrl = App.Get().Wrap(page).FullUrl;
            if (!orgUrl.Equals(newUrl, StringComparison.OrdinalIgnoreCase))
                App.Get().Urls.Rename(orgUrl, newUrl);

            App.Get().CurrentWeb.ClearCache();

            return new HttpStatusCodeResult(200);
        }

        //[HttpPost]
        //public ActionResult Install(int id)
        //{
        //    throw new NotImplementedException();
        //}

        [HttpPost]
        public ActionResult Uninstall(string name)
        {
            if (App.Get().Context.HasPermisson(this, "Delete"))
                return new HttpUnauthorizedResult();

            App.Get().Pages.Delete(name);
            return new HttpStatusCodeResult(200);
        }

        ///// <summary>
        ///// Upload a web page template to create new page instance.
        ///// </summary>
        ///// <remarks>
        ///// REST API: POST /api/pages/import
        ///// </remarks>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Import()
        //{
        //    if (Request.Files.Count == 0)
        //        return Json(new { error = "No page template upload file found." }, JsonRequestBehavior.AllowGet);

        //    if (Request.Files[0].ContentLength == 0)
        //        return Json(new { error = "The uploaded page template is incorrect." }, JsonRequestBehavior.AllowGet);

        //    var postedFile = Request.Files[0];
        //    //var template = TemplateHelper.LoadWebPageTemplate(postedFile.InputStream);
        //    //var page = CreatePageFromTemplate(template);
        //    ///TODO:Import page from template
        //    //return Json(page.ToJSON(), JsonRequestBehavior.AllowGet);
        //    return Json(new { });
        //}

        /// <summary>
        /// Delete the page by specified page id.
        /// </summary>
        ///<param name="id">The page id.</param>
        [
        HttpPost,
        Loc,
        SecurityAction("Pages", "Delete page", "Allows the users can delete the page.",
            TitleResName = "SA_DeletePage",
            DescResName = "SA_DeletePageDesc",
            PermssionSetResName = "SA_PAGES"
        )]
        public void Delete(int id)
        {
            dataContext.WebPages.Delete(id);
            dataContext.SaveChanges();
            App.Get().CurrentWeb.ClearCache();
        }

        /// <summary>
        /// Export specified page instance to the json data file.
        /// </summary>
        /// <param name="id">Specified the page id</param>
        /// <returns>Json data file</returns>
        [SecurityAction("Pages", "Export page data", "Allows users can export and download the exists page data.",
            TitleResName = "SA_ExportPageData",
            DescResName = "SA_ExportPageDataDesc",
            PermssionSetResName = "SA_PAGES"
        )]
        public ActionResult Export(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException("id");

            var page = dataContext.WebPages.Find(id);

            if (page == null)
                return HttpNotFound();

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            try
            {
                ///TODO: Export page data to xml
                //  writer.Write(page.ToXml(Server.MapPath(DESCRIPTOR_PATH)));
            }
            catch
            {
                return Content("Could not export the page data.Mybe the xml document no correct.");
            }
            writer.Flush();
            stream.Position = 0;

            return File(stream, "text/xml", TextUtility.Slug(page.Title) + "-" + DateTime.Now.ToString("yy-MM-dd") + ".xml");
        }

        /// <summary>
        /// Get the page objects by specified web site
        /// </summary>
        /// <remarks>
        /// REST API: GET /api/pages/list
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        public ActionResult List(string website, int parentID = -1)
        {
            var web = App.Get().Webs[website];
            if (web == null)
                return Json(new { error = "There is no website in DNA." }, JsonRequestBehavior.AllowGet);

            if (parentID > -1)
            {
                var parentPage = web.FindPage(parentID);
                if (parentPage == null)
                    return Json(new { error = "Parent page not found." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(parentPage.Children.OrderBy(p => p.Pos).ToList().Select(p => p.ToObject()), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(web.Pages.OrderBy(p => p.Pos).ToList().Select(p => p.ToObject()), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Move the page position
        /// </summary>
        /// <param name="id">Specified the page id</param>
        /// <param name="parentID">Specified the new parent id of the page</param>
        /// <param name="pos">Specified the page new pos</param>
        [HttpPost, Loc]
        [SecurityAction("Pages", "Change position", "Allows users can move the page to a new position",
            TitleResName = "SA_MoveChange",
            DescResName = "SA_MoveChangeDesc",
            PermssionSetResName = "SA_PAGES"
        )]
        public ActionResult Move(int id, int parentID, int pos = 0, int webID = 0)
        {
            if (parentID != id)
            {
                var page = App.Get().CurrentWeb.FindPage(id);

                if (!page.IsOwner(HttpContext))
                    return new HttpUnauthorizedResult();

                if (page != null)
                {
                    if (webID > 0)
                    {
                        var parentWeb = App.Get().Webs.FirstOrDefault(w => w.Id.Equals(webID));
                        if (parentWeb != null)
                            parentWeb.ClearCache();
                        page.MoveTo(parentWeb);
                    }
                    else
                        page.MoveTo(parentID, pos);
                }

                App.Get().CurrentWeb.ClearCache();
            }

            return new HttpStatusCodeResult(200);
        }

        /// <summary>
        ///  Set the specified page to default page
        /// </summary>
        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.DynamicUIController,DNA.Web")]
        public void SetDefault(int id)
        {
            var page = App.Get().CurrentWeb.FindPage(id);
            if (page != null)
                page.SetAsDefault();

            App.Get().CurrentWeb.ClearCache();
        }

        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.DynamicUIController,DNA.Web")]
        public void SetVisible(int id, bool value)
        {
            var page = App.Get().CurrentWeb.FindPage(id);
            if (page != null)
                page.SetVisible(value);

            App.Get().CurrentWeb.ClearCache();
        }

        [HttpPost, PermissionRequired("Add"), Loc]
        public ActionResult Copy(int id)
        {
            var page = App.Get().CurrentWeb.FindPage(id);
            if (page == null)
                return Json(new { error = "Web page not found." }, JsonRequestBehavior.AllowGet);
            var copy = page.Clone();
            App.Get().CurrentWeb.ClearCache();
            return Content(copy.ToJsonString(), "application/json", System.Text.Encoding.UTF8);
        }

        //[Authorize, HttpPost]
        //public ActionResult Publish(int id, string remarks = "")
        //{
        //    var page = App.Get().CurrentWeb.FindPage(id);

        //    if (page == null)
        //        return Json(new { error = "Web page not found" }, JsonRequestBehavior.AllowGet);

        //    page.Publish(remarks);

        //    return Json(new { version = page.Version }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult Rollback(int id, int version)
        //{
        //    var page = App.Get().CurrentWeb.FindPage(id);

        //    if (page == null)
        //        return Json(new { error = "Web page not found" }, JsonRequestBehavior.AllowGet);

        //    page.Rollback(version);

        //    return Content(page.ToJsonString(), "application/json");
        //}

        //public ActionResult Versions(int id)
        //{
        //    var p = _context.DataContext.WebPages.Find(id);
        //    var vers = p.Versions;
        //    if (vers != null)
        //    {
        //        return Json(vers.OrderByDescending(v => v.Version).ToList().Select(v => new
        //        {
        //            version = v.Version,
        //            published = v.Published,
        //            remarks = v.Remarks
        //        }), JsonRequestBehavior.AllowGet);
        //    }
        //    return Content("[]", "application/json");
        //}

        [ValidateInput(false), HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.DynamicUIController,DNA.Web")]
        public ActionResult ApplyLayout(int id, string name, string data = "", string dir = "", string viewMode = "")
        {
            var page = dataContext.WebPages.Find(id);
            if (page == null)
                return HttpNotFound();


            if (!string.IsNullOrEmpty(data))
            {
                var viewData = Server.UrlDecode(data);

                using (var input = new MemoryStream())
                {
                    var buffer = Encoding.UTF8.GetBytes(viewData);
                    input.Write(buffer, 0, buffer.Length);
                    input.Position = 0;
                    var html = new StringBuilder();

                    #region formatted the html to razor
                    using (var reader = XmlReader.Create(input))
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (reader.IsEmptyElement)
                                        {
                                            html.AppendFormat("<{0}/>", reader.Name);
                                        }
                                        else
                                        {
                                            //if (reader.Name.Equals("table", StringComparison.OrdinalIgnoreCase) ||
                                            //    reader.Name.Equals("tbody", StringComparison.OrdinalIgnoreCase))
                                            //    continue;
                                            //if (reader.Name.Equals("tr", StringComparison.OrdinalIgnoreCase))
                                            //{
                                            //    html.AppendFormat("<{0} class=\"d-hbox\">", "div");
                                            //    continue;
                                            //}

                                            if (reader.HasAttributes)
                                            {
                                                var dataRoleAttr = reader.GetAttribute("data-role");
                                                if (!string.IsNullOrEmpty(dataRoleAttr) && dataRoleAttr.Equals("widgetzone"))
                                                {
                                                    var _id = reader.GetAttribute("id");
                                                    var _title = reader.GetAttribute("title");
                                                    var attrs = new Dictionary<string, string>();
                                                    if (string.IsNullOrEmpty(_title))
                                                        _title = _id;

                                                    while (reader.MoveToNextAttribute())
                                                    {
                                                        if (!reader.Name.Equals("id", StringComparison.OrdinalIgnoreCase)
                                                            && !reader.Name.Equals("title", StringComparison.OrdinalIgnoreCase)
                                                            && !reader.Name.Equals("data-role", StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            var keyName = reader.Name.Replace("-", "_");

                                                            if (!attrs.ContainsKey(keyName))
                                                            {
                                                                if (keyName.Equals("class"))
                                                                    attrs.Add("@" + keyName, reader.Value);
                                                                else
                                                                    attrs.Add(keyName, reader.Value);
                                                            }
                                                        }
                                                    }


                                                    if (attrs.Count == 0)
                                                    {
                                                        html.AppendFormat("@Html.Zone(\"{0}\", \"{1}\").GetHtml()", _id, _title);
                                                    }
                                                    else
                                                    {
                                                        var attrList = new List<string>();
                                                        foreach (var ak in attrs.Keys)
                                                        {
                                                            attrList.Add(string.Format("{0}=\"{1}\"", ak, attrs[ak]));
                                                        }
                                                        var attrStr = "{" + string.Join(",", attrList.ToArray()) + "}";
                                                        html.AppendFormat("@Html.Zone(\"{0}\", \"{1}\").Attrs(new {2}).GetHtml()", _id, _title, attrStr);
                                                    }
                                                    //Move to the </div>
                                                    reader.Read();
                                                }
                                                else
                                                {
                                                    //if (reader.Name.Equals("td", StringComparison.OrdinalIgnoreCase))
                                                    //{
                                                    //    html.AppendFormat("<{0} class=\"d-box1\" ", "div");
                                                    //    while (reader.MoveToNextAttribute())
                                                    //    {
                                                    //        if (reader.Name.Equals("class", StringComparison.OrdinalIgnoreCase))
                                                    //            continue;
                                                    //        html.AppendFormat(" {0}=\"{1}\"", reader.Name, reader.Value);
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    html.AppendFormat("<{0}", reader.Name);

                                                    while (reader.MoveToNextAttribute())
                                                        html.AppendFormat(" {0}=\"{1}\"", reader.Name, reader.Value);
                                                    //  }
                                                    html.AppendFormat(">", reader.Name);
                                                }
                                            }
                                            else
                                            {
                                                html.AppendFormat("<{0}>", reader.Name);
                                            }
                                        }
                                        break;
                                    case XmlNodeType.Text:
                                        html.AppendLine(reader.Value);
                                        break;
                                    case XmlNodeType.Comment:
                                        html.AppendFormat("<!--{0}-->", reader.Value);
                                        break;
                                    case XmlNodeType.EndElement:
                                        //if (reader.Name.Equals("table", StringComparison.OrdinalIgnoreCase) ||
                                        //        reader.Name.Equals("tbody", StringComparison.OrdinalIgnoreCase))
                                        //    continue;
                                        //if (reader.Name.Equals("tr", StringComparison.OrdinalIgnoreCase) ||
                                        //    reader.Name.Equals("td", StringComparison.OrdinalIgnoreCase))
                                        //{
                                        //    html.AppendFormat("</{0}>", "div");
                                        //    continue;
                                        //}
                                        html.AppendFormat("</{0}>", reader.Name);
                                        break;
                                }
                            }
                        }
                        catch (XmlException ex)
                        {
                            // Console.WriteLine(ex.Message);
                        }
                    }
                    #endregion

                    page.ViewData = html.ToString();
                }

                #region Replace the widget-zone

                //var regex = new Regex("<div\\s*class=\"d-widget-zone\"\\s*data-role=\"widgetzone\"\\s*id=\"(.+?)\"\\s*title=\"(.+?)\"\\s*\\>\\<\\/div\\>", RegexOptions.Compiled | RegexOptions.Singleline);
                //var regex1 = new Regex("<div\\s*class=\"d-layout\\s*d-widget-zone\"\\s*data-role=\"widgetzone\"\\s*id=\"(.+?)\"\\s*\\>\\<\\/div\\>", RegexOptions.Compiled | RegexOptions.Singleline);
                //var regex2 = new Regex("<div\\s*class=\"d-widget-zone\"\\s*data-role=\"widgetzone\"\\s*id=\"(.+?)\"\\s*\\>\\<\\/div\\>", RegexOptions.Compiled | RegexOptions.Singleline);

                //var replace = "@Html.Zone(\"$1\", \"$2\").GetHtml()";
                //var replace1 = "@Html.Zone(\"$1\", \"$1\").GetHtml()";
                //var replace2 = "@Html.Zone(\"$1\", \"$1\").GetHtml()";

                //var formatter = new ReplacementTextFormatter(regex, replace);
                //var formatter1 = new ReplacementTextFormatter(regex1, replace1);
                //var formatter2 = new ReplacementTextFormatter(regex2, replace2);

                //var viewData = formatter.Format(rawHtml);
                //viewData = formatter1.Format(viewData);
                //viewData = formatter2.Format(viewData);
                //page.ViewData = viewData;//viewData;

                #endregion

                var webName = this.CurrentWebName();
                var path = string.Format("~/content/layouts/{0}/{1}.cshtml", webName, page.ID.ToString());
                var _dir = Server.MapPath(string.Format("~/content/layouts/{0}", webName));

                if (!Directory.Exists(_dir))
                    Directory.CreateDirectory(_dir);

                System.IO.File.WriteAllText(Server.MapPath(path), page.ViewData, Encoding.UTF8);
                page.ViewName = path;
            }
            else
            {
                if (!string.IsNullOrEmpty(name))
                {
                    page.ViewData = "";
                    page.ViewName = "~/views/dynamicui/layouts/" + name + ".cshtml";
                }
            }

            if (!string.IsNullOrEmpty(dir) && !page.Dir.Equals(dir, StringComparison.OrdinalIgnoreCase))
            {
                page.Dir = dir;
            }

            if (!string.IsNullOrEmpty(viewMode) && !page.ViewMode.ToString().Equals(viewMode, StringComparison.OrdinalIgnoreCase))
            {
                var vm = 0;
                if (int.TryParse(viewMode, out vm))
                {
                    page.ViewMode = vm;
                }
            }

            dataContext.WebPages.Update(page);
            dataContext.SaveChanges();
            App.Get().CurrentWeb.ClearCache();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.DynamicUIController,DNA.Web")]
        public ActionResult ApplyFor(int id, int scope)
        {
            var page = dataContext.WebPages.Find(id);
            if (page == null)
                return HttpNotFound();

            var path = page.Path + "/";
            var pages = scope == 0 ? dataContext.WebPages.Filter(p => p.Path.StartsWith(path)) : dataContext.WebPages.Filter(p => p.WebID == page.WebID);
            pages.AsParallel().ForAll(p => { p.MasterID = id; });

            //if (scope == 0) //children
            //{
            //    RecursUpdate(page, page);
            //}
            //else
            //{
            //    var pages = dataContext.WebPages.Filter(p => p.WebID == page.WebID);
            //    foreach (var p in pages)
            //    {
            //        p.MasterID = id;
            //        //p.ViewData = page.ViewData;
            //        //p.ViewMode = page.ViewMode;
            //        //p.CssText = page.CssText;
            //        //p.Dir = page.Dir;
            //        //p.ViewName = page.ViewName;
            //    }
            //}

            dataContext.SaveChanges();
            App.Get().CurrentWeb.ClearCache();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.DynamicUIController,DNA.Web")]
        public ActionResult SaveStyle(int id, string text)
        {
            var page = dataContext.WebPages.Find(id);
            if (page == null)
                return HttpNotFound();
            page.CssText = text;
            dataContext.SaveChanges();
            App.Get().CurrentWeb.ClearCache();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.DynamicUIController,DNA.Web")]
        public void CopyStyle(int srcId, int destId)
        {
            var page = dataContext.WebPages.Find(srcId);
            var destPage = dataContext.WebPages.Find(destId);
            destPage.ViewData = page.ViewData;
            destPage.ViewName = page.ViewName;
            destPage.ViewMode = page.ViewMode;
            destPage.CssText = page.CssText;
            destPage.StyleSheets = page.StyleSheets;
            dataContext.SaveChanges();
            App.Get().CurrentWeb.ClearCache();
        }

        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.DynamicUIController,DNA.Web")]
        public void Reset(int id, bool includeLayout = false)
        {
            var page = dataContext.WebPages.Find(id);
            if (includeLayout)
            {
                page.ViewData = "";
                page.ViewName = "";
            }
            page.CssText = "";
            page.StyleSheets = "";
            dataContext.SaveChanges();
            App.Get().CurrentWeb.ClearCache();
        }

        //private WebPage CreatePageFromTemplate(PageElement template)
        //{
        //    var page = dataContext.WebPages.Create(App.Get().Context.Web, 0, template);
        //    dataContext.SaveChanges();

        //    if (template.Widgets.Count > 0)
        //    {
        //        var orderedWidgets = template.Widgets.GroupBy(w => w.Zone);
        //        foreach (var orderedTmpl in orderedWidgets)
        //        {
        //            var widgetsInZone = orderedTmpl.OrderBy(o => o.Sequence);

        //            foreach (var widgetTmpl in widgetsInZone)
        //                dataContext.Widgets.AddWidget(widgetTmpl, page.ID, widgetTmpl.Zone, widgetTmpl.Sequence);
        //        }

        //        var updateCount = dataContext.SaveChanges();
        //    }
        //    return page;
        //}


    }
}
