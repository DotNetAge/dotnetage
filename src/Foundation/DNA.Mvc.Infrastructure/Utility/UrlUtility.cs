//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Utility
{
    public static class UrlUtility
    {
        private static readonly Regex pingbackLinkRegex = new Regex("<link rel=\"pingback\" href=\"([^\"]+)\" ?/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex pingbackLinkRegex_2 = new Regex("<link href=\"([^\"]+)\" rel=\"pingback\"  ?/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The href regex.
        /// </summary>
        private static readonly Regex HrefRegex = new Regex("href=\"(.*)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        ///     Regex used to find the trackback link on a remote web page.
        /// </summary>
        private static readonly Regex TrackbackLinkRegex = new Regex(
            "trackback:ping=\"([^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        ///     Regex used to find all hyperlinks.
        /// </summary>
        private static readonly Regex UrlsRegex = new Regex(
            @"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Gets all the URLs from the specified string.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>A list of Uri</returns>
        public static IEnumerable<Uri> GetUrlsFromContent(string content)
        {
            var urlsList = new List<Uri>();
            foreach (var url in
                UrlsRegex.Matches(content).Cast<Match>().Select(myMatch => myMatch.Groups["url"].ToString().Trim()))
            {
                Uri uri;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    urlsList.Add(uri);
                }
            }

            return urlsList;
        }

        /// <summary>
        /// Examines the web page source code to retrieve the trackback link from the RDF.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The trackback Uri</returns>
        public static Uri GetTrackBackUrlFromContent(string input)
        {
            var url = TrackbackLinkRegex.Match(input).Groups[1].ToString().Trim();
            Uri uri;

            return Uri.TryCreate(url, UriKind.Absolute, out uri) ? uri : null;
        }

        public static Uri GetPingbackUrlFromContent(string input)
        {
            var match = pingbackLinkRegex.Match(input);
            var url = "";
            if (!match.Success)
                match = pingbackLinkRegex_2.Match(input);
            if (!match.Success) return null;

            url = match.Groups[1].ToString().Trim();
            Uri uri;
            return Uri.TryCreate(url, UriKind.Absolute, out uri) ? uri : null;
        }

        public static string ParseVirtualPath(RequestContext context, string[] ignoreKeys)
        {
            if ((ignoreKeys == null) || (ignoreKeys.Length == 0)) return ParseVirtualPath(context);

            var routeDataValues = new RouteValueDictionary(context.RouteData.Values);
            foreach (string key in ignoreKeys)
            {
                if (routeDataValues.ContainsKey(key))
                    routeDataValues[key] = "{" + key.ToLower() + "}";
                else
                    routeDataValues.Add(key, "{" + key.ToLower() + "}");
            }

            string path = context.RouteData.Route.GetVirtualPath(context, routeDataValues).VirtualPath;

            if (string.IsNullOrEmpty(path))
                path = context.RouteData.Values["controller"] + "/" + context.RouteData.Values["action"];
            path = HttpContext.Current.Server.UrlDecode(path);
            if (path.StartsWith("~"))
                return path;
            return "~/" + path;
        }

        public static string ParseVirtualPath(RequestContext context)
        {
            string path = context.RouteData.Route.GetVirtualPath(context, context.RouteData.Values).VirtualPath;
            if (string.IsNullOrEmpty(path))
                //Why RouteData.Values has values when the request url is "home" or "/" the path always returns "" ?
                path = context.RouteData.Values["controller"] + "/" + context.RouteData.Values["action"];

            if (path.StartsWith("~"))
                return path;
            return "~/" + path;
        }

        /// <summary>
        /// Create a UrlHelper instance from current http context
        /// </summary>
        /// <returns></returns>
        public static UrlHelper CreateUrlHelper()
        {
            return new UrlHelper(CreateRequestContext(HttpContext.Current.Request.Url));
        }

        public static RequestContext CreateRequestContext(Uri url)
        {
            var appVPath = VirtualPathUtility.ToAppRelative(url.IsAbsoluteUri ? url.LocalPath : url.ToString()).Replace("~/", "");
            var request = new System.Web.Hosting.SimpleWorkerRequest(appVPath, "", null);
            var httpContext = new HttpContextWrapper(new HttpContext(request));
            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            return new RequestContext(httpContext, routeData);
        }

        public static string ParseVirtualPath(Uri url, string applicationPath = "")
        {
            //string virtualPath = "~" + url.LocalPath.ToLower();
            string virtualPath = string.IsNullOrEmpty(applicationPath) ? VirtualPathUtility.ToAppRelative(url.LocalPath).ToLower() : VirtualPathUtility.ToAppRelative(url.LocalPath, applicationPath).ToLower();
            if ((virtualPath.Equals("~/", StringComparison.OrdinalIgnoreCase)) ||
                (virtualPath.Equals("~/default.aspx", StringComparison.OrdinalIgnoreCase)) ||
                (virtualPath.Equals("~/home", StringComparison.OrdinalIgnoreCase)))
                virtualPath = "~/home/index";
            return virtualPath;
        }
    }
}
