//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using DNA.Web.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    //[HandlePartialError]
    public class WidgetController : Controller
    {

        #region Private
        private const string DESCRIPTOR_PATH = "~/content/widgets/";
        private IDataContext dataContext;

        public WidgetController(IDataContext context)
        {
            dataContext = context;
        }

        private object _GetDefault(Type value)
        {
            if (value.Equals(typeof(int))) return 0;
            if (value.Equals(typeof(bool))) return false;
            if (value.Equals(typeof(decimal))) return 0;
            if (value.Equals(typeof(double))) return 0;
            if (value.Equals(typeof(float))) return 0;
            return null;
        }

        #endregion

        [SecurityAction("Widgets", "Design page", "Allows users could switch the page design mode.",
            TitleResName = "SA_DesignPage",
            DescResName = "SA_DesignPageDesc",
            PermssionSetResName = "SA_Widgets"
            ), Loc, OutputCache(VaryByParam = "*", Duration = 300)]
        public ActionResult Explorer()
        {
            return PartialView();
        }

        [HostDashboard("Widgets",
            Sequence = 1,
            Group = "Extensions",
            GroupResKey = "Extensions",
            RouteName = "dna_host_ext_widgets",
            Icon = "d-icon-grid",
            ResKey = "Widgets")]
        public ActionResult Manager()
        {
            string targetPath = HttpContext.Server.MapPath("~/bin");
            App.Get().Widgets.Detect(targetPath);
            return View();
        }

        public ActionResult Generic(int id, string website)
        {
            var contentUrl = "";
            //Guid gid = new Guid(id);
            WidgetInstance _widget = null;
            try
            {
                _widget = dataContext.Widgets.Find(id);
            }
            catch
            {
                System.Threading.Thread.CurrentThread.Join(100);
                _widget = dataContext.Widgets.Find(id);
            }

            if (_widget == null)
                return HttpNotFound();

            ViewBag.WidgetInstance = _widget;

            var descroptor = _widget.WidgetDescriptor;
            contentUrl = descroptor.ResolveUri(descroptor.ContentUrl, Consts.WIDGET_PKG_PATH);

            //if (Request.IsAjaxRequest())
            // {
            //1.Load values from default
            if (!string.IsNullOrEmpty(descroptor.Defaults))
            {
                Dictionary<string, PropertyDescriptor> preferenceStore = new Dictionary<string, PropertyDescriptor>();
                var properties = descroptor.Properties;
                foreach (var pro in properties)
                {
                    preferenceStore.Add((string)pro["name"], new PropertyDescriptor
                    {
                        Name = (string)pro["name"],
                        IsReadonly = (bool)pro["readonly"],
                        Value = pro["value"]
                    });
                }

                var _pros = _widget.ReadUserPreferences();
                foreach (var p in _pros)
                {
                    var key = (string)p["name"];
                    if (preferenceStore.ContainsKey(key))
                    {
                        preferenceStore[key].IsReadonly = (bool)p["readonly"];
                        preferenceStore[key].Value = p["value"];
                    }
                }
                ViewBag.PropertyDescriptors = preferenceStore;
            }
            //}

            if (string.IsNullOrEmpty(contentUrl)) //View only 
                return PartialView();

            if (descroptor.ContentType == "application/x-ms-aspnet")
                return PartialView(contentUrl);
            else
            {
                //W3C Widget and Google gadget
                ViewBag.ContentUrl = contentUrl;
                return PartialView();
            }
        }

        public ActionResult _W3C(int id, string category, string name, string locale)
        {
            var basePath = Server.MapPath(Consts.WIDGET_PKG_PATH);

            var widgetData = dataContext.Widgets.Find(id);
            ViewData.Model = widgetData;
            var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            var descriptor = widgetData.WidgetDescriptor;
            var locContent = descriptor.LocaleContent(locale);

            var content = locContent.Text;

            var _data = descriptor.Defaults;

            if (!string.IsNullOrEmpty(widgetData.Data))
                _data = widgetData.Data;

            ViewBag.Data = _data;
            ViewBag.ContentType = locContent.Type;
            ViewBag.Encoding = locContent.Encoding;
            ViewBag.Dir = widgetData.WebPage.Dir;// locContent.Dir;
            var contentSrc = locContent.Url;

            #region Init client widget API
            var locName = descriptor.LocaleName(locale);

            var widgetObject = new
            {
                author = descriptor.Author,
                authorEmail = descriptor.AuthorEmail,
                authorHref = descriptor.AuthorHomePage,
                description = descriptor.LocaleDesc(locale),
                name = locName.FullName,
                shortName = locName.ShortName,
                version = descriptor.Version,
                id = descriptor.UID,
                _width = descriptor.Width,
                _height = descriptor.Height
            };

            ViewBag.WidgetJson = ser.Serialize(widgetObject);

            #endregion

            if (!string.IsNullOrEmpty(contentSrc))
            {
                var contentHtml = System.IO.File.ReadAllText(Server.MapPath(contentSrc), System.Text.Encoding.GetEncoding(locContent.Encoding));
                var headHtml = "";
                var bodyHtml = "";

                if (HtmlHeadRegex.IsMatch(contentHtml))
                {
                    var hm = HtmlHeadRegex.Match(contentHtml);
                    headHtml = hm.Result("$1");
                    ViewBag.ContentHeader = headHtml;
                }

                if (HtmlBodyRegex.IsMatch(contentHtml))
                {
                    var bm = HtmlBodyRegex.Match(contentHtml);
                    var attributes = bm.Result("$1");
                    bodyHtml = bm.Result("$2");

                    if (!string.IsNullOrEmpty(attributes))
                    {
                        if (HtmlOnLoadRegex.IsMatch(attributes))
                        {
                            var result = HtmlOnLoadRegex.Match(attributes).Result("$0");
                            if (result.StartsWith("onload"))
                                result = HtmlOnLoadRegex.Match(attributes).Result("$1");
                            if (!string.IsNullOrEmpty(result))
                            {
                                if (result.StartsWith("\""))
                                    result = result.Substring(1, result.Length - 2);
                                ViewBag.StartScripts = result;
                            }
                        }
                    }

                    ViewBag.ContentBody = bodyHtml;
                }
                //Render html file
                return View();
            }
            else
            {
                ViewBag.ContentBody = locContent.Text;
                return View();
            }
        }

        private static Regex HtmlOnLoadRegex = new Regex("onload='(.+?)'|\"(.+?)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static Regex HtmlHeadRegex = new Regex(@"<head[^>]*?>(.+?)</head>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static Regex HtmlBodyRegex = new Regex(@"\<body(?<attrs>.*?)\>(?<text>.+?)\<\/body\>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //<body[^>](.+?)>(.+?)</body>

        [Authorize, Loc]
        public ActionResult Settings(int id)
        {
            var widget = dataContext.Widgets.Find(id);
            ViewBag.Roles = dataContext.Widgets.GetRoles(id);
            return PartialView(widget);
        }

        [Authorize, Loc]
        public ActionResult Designer(int id)
        {
            var widget = dataContext.Widgets.Find(id);
            var widgetHelper = new WidgetHelper(widget);
            var designerView = widget.WidgetDescriptor.ResolveFileName(Server.MapPath("~/content/widgets"), "_designer.cshtml");
            if (System.IO.File.Exists(designerView))
                designerView = widget.WidgetDescriptor.ResolveUri("_designer.cshtml");
            else
                designerView = "Designer";

            return PartialView(designerView, widgetHelper);
        }

        [HostOnly, Loc]
        public ActionResult Edit(string installPath)
        {
            //App.Get().SetCulture(locale);
            var descriptor = App.Get().Descriptors[Server.UrlDecode(installPath)];
            return PartialView(descriptor);
        }

        [HostOnly, Loc]
        public ActionResult Package(string name)
        {
            var pkg = App.Get().Widgets.Find(name);
            return PartialView(pkg);
        }

        //[HostOnly, Loc]
        //public ActionResult Editor(string installPath)
        //{
        //    return View();
        //}
    }
}
