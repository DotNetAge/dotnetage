//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class ThemeAPIController : Controller
    {
        [Authorize]
        public void Install(string id)
        {
            App.Get().Themes.Download(id);
        }

        [Authorize, HttpPost]
        public void Uninstall(string id)
        {
            App.Get().Themes.Delete(id);
            this.Trigger(Events.EventNames.ThemeUninstalled, new { theme = id });
        }

        [Authorize, ValidateInput(false), HttpPost]
        public void Save(string name, string css, string media)
        {
            var path = Server.MapPath(string.Format(Url.Content("~/content/themes/{0}"), name));
            var cssfile = (media.Equals("pc-full", StringComparison.OrdinalIgnoreCase) || media.Equals("pc-center", StringComparison.OrdinalIgnoreCase)) ?
                Server.MapPath(string.Format(Url.Content("~/content/themes/{0}/site.theme.css"), name)) :
                Server.MapPath(string.Format(Url.Content("~/content/themes/{0}/media-{1}.css"), name, media));
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            System.IO.File.WriteAllText(cssfile, css);
        }

        [Authorize, HttpPost, Loc]
        public void Set(string name, string locale)
        {
            var app = App.Get();
            var web = app.CurrentWeb;
            web.Theme = name;
            web.Save();
        }

        [Authorize, HttpPost]
        public void UserSet(string id)
        {
            var profile = App.Get().Profile;
            profile.Theme = id;
            profile.Save();
        }
    }
}
