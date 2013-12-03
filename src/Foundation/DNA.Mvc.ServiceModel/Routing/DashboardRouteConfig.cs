//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class DashboardRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            var defaultLocale = App.Settings.DefaultLocale;

            //routes.MapRoute("dna_dashboard_index", "{dashboard}/{website}/{start}",
            //    new { controller = "Sys", action = "Index", website = "home" },
            //    new { dashboard = "dashboard", start = "start" });

            routes.MapRoute("dna_sitesettings", "{dashboard}/{website}/{locale}/{settings}",
                new { controller = "Sys", action = "SiteSettings", website = "home", locale = defaultLocale },
                new { dashboard = "dashboard", settings = "settings", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_netdrive", "{dashboard}/{website}/{locale}/{netdrive}",
                new { controller = "WebFiles", action = "Explorer", website = "home", locale = defaultLocale },
                new { netdrive = "netdrive", dashboard = "dashboard", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_webcategories", "{dashboard}/{website}/{locale}/{categories}",
                new { controller = "Sys", action = "Categories", website = "home", locale = defaultLocale },
                new { dashboard = "dashboard", categories = "categories", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_pages_mgr", "{dashboard}/{website}/{locale}/{pages}",
                new { controller = "DynamicUI", action = "Sitemap", website = "home", locale = defaultLocale },
                new { pages = "pages", dashboard = "dashboard", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_contents_mgr", "{dashboard}/{website}/{locale}/{contents}",
                new { controller = "Contents", action = "Lists", website = "home", locale = defaultLocale },
                new { dashboard = "dashboard", contents = "contents", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            #region List
            routes.MapRoute("dna_edit_list", "{dashboard}/{website}/{locale}/{lists}/{name}",
                new { controller = "List", action = "Edit", website = "home", locale = defaultLocale },
                new { dashboard = "dashboard", lists = "lists" , locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_restore_list", "{dashboard}/{website}/{locale}/{restore}",
                new { controller = "Contents", action = "Restore", website = "home", locale = defaultLocale },
                new { dashboard = "dashboard", restore = "restore-list" , locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            //routes.MapRoute("dna_design_view", "{dashboard}/{website}/{locale}/{name}/{design}/{slug}",
            //    new { controller = "View", action = "Design", website = "home", locale = defaultLocale },
            //    new { dashboard = "dashboard", design = "design-view" , locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_edit_view", "{dashboard}/{website}/{locale}/{name}/{views}/{slug}",
                new { controller = "View", action = "Edit", website = "home", locale = defaultLocale },
                new { dashboard = "dashboard", views = "views", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            //routes.MapRoute("dna_design_form", "{dashboard}/{website}/{locale}/{name}/{design}/{type}",
            //    new { controller = "Form", action = "Design", website = "home", locale = defaultLocale },
            //    new { dashboard = "dashboard", design = "design-form", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_edit_form", "{dashboard}/{website}/{locale}/{name}/{forms}/{type}",
                new { controller = "Form", action = "Edit", website = "home", locale = defaultLocale },
                new { dashboard = "dashboard", forms = "forms", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_import_and_createlist", "{dashboard}/{website}/{locale}/{importer}",
                new { controller = "Contents", action = "Import", website = "home", locale = defaultLocale },
                new { dashboard = "dashboard", importer = "importer", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });
            #endregion

        }
    }
}
