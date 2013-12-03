//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.IO;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class ThemeController : Controller
    {
        [HostDashboard("Themes",
            Group = "Extensions",
            RouteName = "dna_host_ext_themes",
            Icon="d-icon-paint-format",
            GroupResKey = "Extensions",
            ResKey = "Themes")]
        public ActionResult Manager()
        {
            return View();
        }

        [Authorize, Loc]
        public ActionResult Explorer(int id)
        {
            return PartialView(App.Get().CurrentWeb.FindPage(id));
        }

        [Loc,HostOnly]
        public ActionResult Edit(string name)
        {
            var theme = App.Get().Themes[name];
            return PartialView(theme);
        }

        [Loc, HostOnly]
        public ActionResult Delete(string name)
        {
            var theme = App.Get().Themes[name];
            if (theme != null)
            {
                //DNA.Web.Webstore.AppManifest.Themes.RemoveByID(name);
                Directory.Delete(theme.InstalledPath, true);
            }
            return new HttpStatusCodeResult(200);
        }

        [Authorize]
        public ActionResult Personal()
        {
            return PartialView();
        }
    }
}
