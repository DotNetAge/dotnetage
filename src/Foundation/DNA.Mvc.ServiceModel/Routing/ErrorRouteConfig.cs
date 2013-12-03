//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class ErrorRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            routes.MapRoute("dna_notfound", "{index}.html", new { index = "404", controller = "Error", action = "NotFound" }, new { index = "404" });
            routes.MapRoute("dna_srverror", "{index}.html", new { controller = "Error", action = "ServerError" }, new { index = "500" });
        }

    }
}
