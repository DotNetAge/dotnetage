//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class MyDashboarRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            routes.MapRoute("dna_my_home", "{mysite}",
                new { controller = "Account", action = "Index" },
                new { mysite = "mysite" });

            routes.MapRoute("dna_my_profile", "{mysite}/{profile}",
                new { controller = "Account", action = "Settings" },
                new { mysite = "mysite", profile = "profile" });
        }
    }
}
