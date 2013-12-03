//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class APIRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            routes.MapRoute("dna_accounts", "{api}/{accounts}/{action}", new { controller = "AccountAPI", Area = "", }, new { api = "api", accounts = "accounts" });

            routes.MapRoute("dna_widgets", "{api}/{website}/{widgets}/{action}/{id}", new { controller = "WidgetAPI", action = "Index", Area = "", website = "home", id = UrlParameter.Optional }, new { api = "api", widgets = "widgets" });
            routes.MapRoute("dna_descriptors", "{api}/{widgets}/{action}/{id}", new { controller = "DescriptorAPI", action = "Index", Area = "", id = UrlParameter.Optional }, new { api = "api", widgets = "widgets" });

            routes.MapRoute("dna_theme_set", "{api}/{website}/{themes}/{action}", new { controller = "ThemeAPI", action = "Set", Area = "", website = "home" }, new { api = "api",action="set", themes = "themes" });
            routes.MapRoute("dna_themes", "{api}/{themes}/{action}/{id}", new { controller = "ThemeAPI", action = "Index", Area = "", id = UrlParameter.Optional }, new { api = "api", themes = "themes" });
            routes.MapRoute("dna_webs", "{api}/{webs}/{action}/{id}", new { controller = "WebsAPI", action = "Index", Area = "", id = UrlParameter.Optional }, new { api = "api", webs = "webs" });
            routes.MapRoute("dna_webpages", "{api}/{website}/{pages}/{action}/{id}", new { controller = "WebPageAPI", action = "Index",Area = "", id = UrlParameter.Optional, website = "home" }, new { api = "api", pages = "pages" });
            //routes.MapRoute("dna_gallery", "{api}/{gallery}/{action}/{id}", new { controller = "GalleryAPI", action = "Index", id = UrlParameter.Optional }, new { api = "api", gallery = "gallery" });

            routes.MapRoute("dna_roles", "{api}/{roles}/{action}", new { controller = "RoleAPI", Area = "", }, new { api = "api", roles = "roles" });
            routes.MapRoute("dna_comments", "{api}/{comments}/{action}/{id}", new { controller = "CommentAPI", action = "List", Area = "", id = UrlParameter.Optional }, new { api = "api", comments = "comments" });
            
            routes.MapRoute("dna_contents", "{api}/{website}/{contents}/{action}/{id}", new { controller = "ContentsAPI", Area = "", id = UrlParameter.Optional,website="home" }, new { api = "api", contents = "contents" });
            routes.MapRoute("dna_lists", "{api}/{website}/{lists}/{action}/{id}", new { controller = "ListAPI", Area = "", id = UrlParameter.Optional, website = "home" }, new { api = "api", lists = "lists" });
            routes.MapRoute("dna_views", "{api}/{website}/{views}/{action}/{id}", new { controller = "ViewAPI", Area = "", id = UrlParameter.Optional, website = "home" }, new { api = "api", views = "views" });
            routes.MapRoute("dna_forms", "{api}/{website}/{forms}/{action}/{id}", new { controller = "FormAPI", Area = "", id = UrlParameter.Optional, website = "home" }, new { api = "api", forms = "forms" });

            routes.MapRoute("dna_categories", "{api}/{website}/{locale}/{cats}/{action}/{id}", new { controller = "CategoryAPI", Area = "", id = UrlParameter.Optional, website = "home" }, new { api = "api", cats = "cats", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_cloud", "{api}/{cloud}/{action}/{id}", new { controller = "CloudAPI", Area = "", id = UrlParameter.Optional }, new { api = "api", cloud = "cloud" });
            //routes.MapRoute("dna_notifies", "{api}/{notify}/{action}", new { controller = "NotifyAPI"}, new { api = "api", notify = "notify" });
            routes.MapRoute("dna_langs", "{api}/{website}/{langs}/{action}", new { controller = "LanguageAPI" ,website="home"}, new { api = "api", langs = "langs" });
            //routes.MapRoute("dna_activities", "{api}/{social}/{action}", new { controller = "SocialAPI"}, new { api = "api", activities = "social" });
        }
    }
}