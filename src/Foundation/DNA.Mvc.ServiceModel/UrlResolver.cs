//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web;

namespace DNA.Web
{
    public static class UrlResolver
    {
        public static string ApplicationPath
        {
            get
            {
                return Resolve(new HttpContextWrapper(HttpContext.Current));
            }
        }

        public static string Resolve(HttpContextBase httpContext)
        {
            var request = httpContext.Request;
            string url = request.Url.Scheme + "://" + request.Url.Authority;
            if (!request.ApplicationPath.Equals("/"))
                url += request.ApplicationPath;
            return url;
        }

        public static string Resolve(Web web)
        {
            return Resolve(new HttpContextWrapper(HttpContext.Current), web);
        }

        public static string Resolve(HttpContextBase httpContext, Web web)
        {
            if (web.IsRoot)
                return Resolve(httpContext);
            else
                return Resolve(httpContext) + "/sites/" + web.Name;
        }

        public static string Content(Web web)
        {
            if (web != null)
            {
                return string.Format("/webshared/{0}/", web.Name);
            }
            return null;
        }
    }

}
