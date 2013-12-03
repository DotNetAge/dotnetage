//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.Routing
{
    public class NetDriveRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            routes.MapRoute("dna_webfiles_rename", "{service}/{website}/{*path}",
        new { website = "home", controller = "WebFiles", action = "Update" },
        new { service = "webshared", httpMethod = new HttpMethodConstraint("PUT") });

            routes.MapRoute("dna_webfiles_get", "{service}/{website}/{*path}",
                new { website = "home", controller = "WebFiles", action = "GetPath" },
                new { service = "webshared", httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("dna_webfiles_post", "{service}/{website}/{*path}",
                new { website = "home", controller = "WebFiles", action = "UploadOrCreate" },
                new { service = "webshared", httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("dna_webfiles_delete", "{service}/{website}/{*path}",
                new { website = "home", controller = "WebFiles", action = "Delete" },
                new { service = "webshared", httpMethod = new HttpMethodConstraint("DELETE") });

            #region The old NetDrive api
            //routes.MapRoute("dna_webfiles_put", "{service}/{website}/{*path}",
            //    new { website = "home", controller = "WebFiles", action = "Replace" },
            //    new { service = "webshared", httpMethod = new HttpMethodConstraint("PUT") });
            //routes.MapRoute("dna_webfiles_copy", "{service}/{website}/{*path}",
            //    new { website = "home", controller = "WebFiles", action = "Copy" },
            //    new { service = "webshared", httpMethod = new HttpMethodConstraint("COPY") });

            //routes.MapRoute("dna_webfiles_move", "{service}/{website}/{*path}",
            //    new { website = "home", controller = "WebFiles", action = "Move" },
            //    new { service = "webshared", httpMethod = new HttpMethodConstraint("MOVE") });

            //routes.MapRoute("dna_webfiles_mkcol", "{service}/{website}/{*path}",
            //    new { website = "home", controller = "WebFiles", action = "CreatePath" },
            //    new { service = "webshared", httpMethod = new HttpMethodConstraint("MKCOL") });
            //        routes.MapRoute("dna_webfiles_list", "{service}/{website}/{*path}",
            //new { website = "home", controller = "WebFiles", action = "List" },
            //new { service = "webshared", httpMethod = new HttpMethodConstraint("LIST") });

            //        routes.MapRoute("dna_webfiles_paths", "{service}/{website}/{*path}",
            //            new { website = "home", controller = "WebFiles", action = "PathList" },
            //            new { service = "webshared", httpMethod = new HttpMethodConstraint("PLIST") });

            //        routes.MapRoute("dna_webfiles_files", "{service}/{website}/{*path}",
            //new { website = "home", controller = "WebFiles", action = "FileList" },
            //new { service = "webshared", httpMethod = new HttpMethodConstraint("FLIST") });
            #endregion
        }
    }
}