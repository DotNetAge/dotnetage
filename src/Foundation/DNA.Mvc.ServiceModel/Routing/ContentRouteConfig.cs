//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class ContentRouteConfig
    {
        public static void Register(RouteCollection routes)
        {

            #region List
            routes.MapRoute("dna_create_list", "{website}/{locale}/{lists}/{create}",
                new { controller = "Contents", action = "Create", Area = "" },
                new { lists = "lists", create = "create", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_list_actions", "{website}/{locale}/{list}/{action}/{id}",
                new { controller = "List", Area = "", website = "home", id = UrlParameter.Optional },
                new { list = "list", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_list_view_actions", "{website}/{locale}/{view}/{action}/{id}",
                new { controller = "View", Area = "", website = "home", id = UrlParameter.Optional },
                new { view = "view", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_list_form_actions", "{website}/{locale}/{form}/{action}/{id}",
                new { controller = "Form", Area = "", website = "home", id = UrlParameter.Optional },
                new { form = "form", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });
            #endregion

            routes.MapRoute("dna_list_tags", "{website}/{locale}/{list_name}/{tags}/{tag}.{ext}",
                new { controller = "Contents", action = "Tags", Area = "" },
                new { locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", tags = "tags", ext = "html" });
            
            routes.MapRoute("dna_list_archives", "{website}/{locale}/{list_name}/{archives}/{year}-{month}.{ext}",
                new { controller = "Contents", action = "Archives", Area = "", year = "2013", month = "01" },
                new { locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", archives = "archives", year = @"^\d{4}", month = @"^\d{2}", ext = "html" });

            routes.MapRoute("dna_item_locate", "{website}/{locale}/{list_name}/{locate}/{item_slug}",
                new { controller = "Contents", action = "Locate", Area = ""},
                new { locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", locate = "locate" });


            routes.MapRoute("dna_newitem", "{website}/{locale}/{lists}/{list_name}/{new}.{ext}",
                new { controller = "Contents", action = "New", Area = "" },
                new { lists = "lists", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", @new = "new", ext = "html" });

            routes.MapRoute("dna_edititem", "{website}/{locale}/{lists}/{list_name}/{edit}/{item_slug}.{ext}",
                new { controller = "Contents", action = "Edit", Area = "" },
                new { lists = "lists", edit = "edit", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", ext = "html" });

            routes.MapRoute("dna_item_link", "{website}/{locale}/{list_name}/{year}/{month}/{day}/{item_slug}.{ext}",
                new { controller = "Contents", action = "Detail", Area = "", year = "2013", month = "01", day = "01" },
                new { locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", year = @"^\d{4}", month = @"^\d{2}", day = @"^\d{2}", ext = "html" });

            routes.MapRoute("dna_list_categories", "{website}/{locale}/{list_name}/{categories}/{category}.{ext}",
                new { controller = "Contents", action = "Views", Area = "" },
                new { locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", categories = "categories", ext = "html" });

            routes.MapRoute("dna_view_link", "{website}/{locale}/{lists}/{list_name}/{views}/{slug}.{ext}",
                new { controller = "Contents", action = "Views", Area = "", locale = "en-us" },
                new { lists = "lists", views = "views", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", ext = "html" });

            routes.MapRoute("dna_view_rss", "{website}/{locale}/{lists}/{list_name}/{rss}/{slug}.{ext}",
                new { controller = "Contents", action = "Feed", Area = "", format = "rss", locale = "en-us" },
                new { lists = "lists", rss = "rss", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", ext = "xml" });

            routes.MapRoute("dna_view_atom", "{website}/{locale}/{lists}/{list_name}/{atom}/{slug}.{ext}",
                new { controller = "Contents", action = "Feed", Area = "", format = "atom", locale = "en-us" },
                new { lists = "lists", atom = "atom", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})", ext = "xml" });

            //routes.MapRoute("dna_rss_link", "{website}/{lists}/{name}/{rss}/{slug}.{ext}", new { controller = "Contents", action = "Rss", Area = "" }, new { lists = "lists", rss = "rss", ext = "xml" });
            //routes.MapRoute("dna_atom_link", "{website}/{lists}/{name}/{atom}/{slug}.{ext}", new { controller = "Contents", action = "Atom", Area = "" }, new { lists = "lists", atom = "atom", ext = "xml" });
        }
    }
}