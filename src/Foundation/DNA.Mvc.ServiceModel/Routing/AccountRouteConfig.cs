//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class AccountRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            routes.MapRoute("dna_login", "{login}",
                new { controller = "Account", action = "Login", Area = "" },
                new { login = "login" });
            
            routes.MapRoute("dna_signup", "{signup}",
                new { controller = "Account", action = "Signup", Area = "" },
                new { signup = "signup" });

            routes.MapRoute("dna_reg", "{register}",
                new { controller = "Account", action = "Register", Area = "" },
                new { register = "register" });

            routes.MapRoute("dna_logout", "{logout}",
                new { controller = "Account", action = "LogOff", Area = "" },
                new { logout = "logout" });

            routes.MapRoute("dna_profile", "{profile}",
                new { controller = "Account", action = "EditProfile", Area = "" },
                new { profile = "profile" });

            //routes.MapRoute("dna_view_profile", "{profiles}/{user}",
            //    new { controller = "Social", action = "Activity" },
            //    new { profiles = "profiles" });
        }
    }
}