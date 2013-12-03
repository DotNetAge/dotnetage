//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class DynamicUIRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            var defaultLocale = App.Settings.DefaultLocale;

            routes.MapRoute("dna_editpage", "{editpage}/{id}",
                new { controller = "DynamicUI", action = "Edit", Area = "" },
                new { editpage = "editpage" });

///TODO:Here may be miss the website parameter
            routes.MapRoute("dna_widget_locale_page", "{content}/{widgets}/{category}/{name}/{locales}/{lang}/{id}.{ext}",
                new { controller = "Widget", action = "_W3C", website = "home" },
                new { content = "content", locales = "locales", ext = "html", widgets = "widgets" });

            routes.MapRoute("dna_widget_page", "{content}/{widgets}/{category}/{name}/{id}.{ext}",
                new { controller = "Widget", action = "_W3C", website = "home" },
                new { content = "content", ext = "html", widgets = "widgets" });

            //Example : http://domain-name/home/en-us/default.html
            routes.MapRoute("dna_dynamic_pages", "{website}/{locale}/{slug}.{ext}",
                new { controller = "DynamicUI", action = "index", website = "home", slug = "default", locale = defaultLocale, Area = "" },
                new { ext = "html", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            //Map to default , eg: http://www.domain/  or  http://www.domain/en-us/
            routes.MapRoute("dna_dynamic_pages_default", "{website}/{locale}",
                new
                {
                    controller = "DynamicUI",
                    action = "index",
                    website = "home",
                    slug = "default",
                    locale = defaultLocale,
                    Area = ""
                }, new { locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            //routes.MapRoute("dna_mypages", "{sites}/{website}/{layout}.{extension}", new { controller = "DynamicUI", action = "Index", website = "home" }, new { sites = "sites", extension = "html" });
            //routes.MapRoute("dna_mysite", "{website}-{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = "", Area = "" });
        }
    }
}