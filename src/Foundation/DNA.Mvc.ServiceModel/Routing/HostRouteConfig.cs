//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class HostRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            routes.MapRoute("dna_global_settings", "{host}/{settings}", new { controller = "Sys", action = "GlobalSettings", Area = "" }, new { host = "host", settings = "settings" });
            routes.MapRoute("dna_host_smtp", "{host}/{smtp}", new { controller = "Sys", action = "Smtp", Area = "" }, new { host = "host", smtp = "smtp" });
            routes.MapRoute("dna_host_ext_contents", "{host}/{contents}", new { controller = "Contents", action = "Packages", Area = "" }, new { host = "host", contents = "contents" });
            routes.MapRoute("dna_host_ext_themes", "{host}/{themes}", new { controller = "Theme", action = "Manager", Area = "" }, new { host = "host", themes = "themes" });
            routes.MapRoute("dna_host_ext_widgets", "{host}/{widgets}", new { controller = "Widget", action = "Manager", Area = "" }, new { host = "host", widgets = "widgets" });
            routes.MapRoute("dna_host_users", "{host}/{users}", new { controller = "Security", action = "ManageUsers", Area = "" }, new { host = "host", users = "users" });
            routes.MapRoute("dna_host_roles", "{host}/{roles}", new { controller = "Security", action = "ManageRoles", Area = "" }, new { host = "host", roles = "roles" });
            routes.MapRoute("dna_host_add_user", "{host}/{adduser}", new { controller = "Security", action = "CreateUser", Area = "" }, new { host = "host", adduser = "create-user" });
            routes.MapRoute("dna_host_modules", "{host}/{modules}", new { controller = "Sys", action = "Modules", Area = "" }, new { host = "host", modules = "modules" });
            
        }
    }
}
