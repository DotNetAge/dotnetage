//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Linq;
using System.Web.Mvc;
//using DNA.Web.Webstore;

namespace DNA.Web.Controllers
{
    public class DescriptorAPIController : Controller
    {
        private IDataContext dataContext;

        public DescriptorAPIController(IDataContext context)
        {
            dataContext = context;
        }

        /// <summary>
        /// List the installed widget paths (Only avalidable in DNA2)
        /// </summary>
        /// <remarks>
        /// REST API: GET /api/widgets/descriptors
        /// </remarks>
        /// <param name="path"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index(string path = "Content")
        {
            return Json(App.Get().GetDescriptorsIn(path).Select(r => r.ToObject(HttpContext, this.CurrentWebName())), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Packages(string path = "Content")
        {
            var results = App.Get().Widgets.Packages.Where(p => p.Category.Equals(path)).Select(t => t.ToJSON(dataContext.WidgetDescriptors));
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost, HostOnly]
        public void Uninstall(string category, string name)
        {
            App.Get().Widgets.Uninstall(category, Server.UrlDecode(name));
            var locID = category + "\\" + name;
            //AppManifest.Widgets.RemoveByID(locID);
            this.Trigger(Events.EventNames.WidgetUninstalled, new { name = name, category = category, localID = locID });
        }

        [Authorize, HttpPost, HostOnly]
        public ActionResult Register(string category, string name)
        {
            var descriptor = App.Get().Widgets.Register(category, name);

            this.Trigger(Events.EventNames.WidgetRegistered, new { name = name, category = category, descriptor = descriptor });
            return Content(descriptor.ToJsonString(HttpContext), "application/json", System.Text.Encoding.UTF8);
        }

        [Authorize, HttpPost, HostOnly]
        public ActionResult Update(string id)
        {
            var wid = Server.UrlDecode(id);
            var descriptor = App.Get().Widgets.Update(wid);
            return Content(descriptor.ToJsonString(HttpContext), "application/json", System.Text.Encoding.UTF8);
        }

        [Authorize, HttpPost, HostOnly]
        public ActionResult Unregister(string id)
        {
            var wid = Server.UrlDecode(id);
            App.Get().Descriptors.Remove(wid);
            this.Trigger(Events.EventNames.WidgetUnregistered, new { uid = wid });

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
