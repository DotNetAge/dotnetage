//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web
{
    /// <summary>
    /// The Url helper extension class 
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Resovle the url with current theme path
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="path">The relative path.</param>
        /// <returns>A string contains absolute url for relative path.</returns>
        public static string ThemeContent(this UrlHelper helper, string path)
        {
            return helper.Content("~/Content/Themes/" + App.Get().CurrentWeb.Theme + "/" + path);
        }

        public static string HostAction(this UrlHelper helper, string module, string action, string controller, object routeValues = null)
        {
            var vals = routeValues == null ? new RouteValueDictionary() : new RouteValueDictionary(routeValues);
            vals.Add("action", action);
            vals.Add("controller", controller);
            vals.Add("solution", module);
            var routeName = "host_module_" + module + "_" + controller + "_" + action;
            return helper.RouteUrl(routeName, vals);
        }

        public static string MyAction(this UrlHelper helper, string solution, string action, string controller,object routeValues=null)
        {
            var vals = routeValues == null ? new RouteValueDictionary() : new RouteValueDictionary(routeValues);
            vals.Add("action", action);
            vals.Add("controller", controller);
            vals.Add("solution", solution);
            var routeName = "mysite_" + solution + "_" + controller + "_" + action;
            return helper.RouteUrl(routeName, vals);
        }

        public static string SolutionAction(this UrlHelper helper, string solution, string action, string controller, string culture, string website, object routeValues = null)
        {
            var vals = routeValues == null ? new RouteValueDictionary() : new RouteValueDictionary(routeValues);
            vals.Add("action", action);
            vals.Add("controller", controller);
            vals.Add("website", website);
            vals.Add("locale", culture);
            return helper.RouteUrl("solution_" + solution, vals);
        }

        
        /// <summary>
        /// Identity the specified url whether a local url
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="url">The url</param>
        /// <returns>If the specified url is local return true.</returns>
        public static bool IsLocal(this UrlHelper helper, string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.StartsWith("~/") || url.StartsWith("/")) { return true; }
                var appPath = App.Get().Context.AppUrl.ToString();
                return url.StartsWith(appPath);
            }

            return false;
        }

        /// <summary>
        /// Converts the content list defautl view virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="list">The content list object</param>
        /// <returns>A string that contains default view absolute url of the specified content list object.</returns>
        public static string Content(this UrlHelper helper, ContentListDecorator list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (!string.IsNullOrEmpty(list.DefaultUrl))
                return helper.Content(list.DefaultUrl);
            return "javascript:void(0);";
        }

        /// <summary>
        /// Converts the content list item display form virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="item">The content data item.</param>
        /// <returns>A string contains item display form absolute url. </returns>
        public static string Content(this UrlHelper helper, ContentDataItemDecorator item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            return helper.Content(item.Url);
        }

        /// <summary>
        ///  Converts the content list item display form virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="item">The content query result item.</param>
        /// <returns>A string contains item display form absolute url. </returns>
        public static string Content(this UrlHelper helper, ContentQueryResultItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            return helper.Content(item.Url);
        }

        /// <summary>
        ///  Converts the content view url virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="view">The view object</param>
        /// <returns>A absolute url of content view.</returns>
        public static string Content(this UrlHelper helper, ContentViewDecorator view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (!string.IsNullOrEmpty(view.Url))
                return helper.Content(view.Url);
            return "javascript:void(0);";
        }

        /// <summary>
        /// Converts the content form url virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="form">The form object.</param>
        /// <param name="urlparams"></param>
        /// <returns>A absolute url of content form.</returns>
        public static string Content(this UrlHelper helper, ContentFormDecorator form, params object[] urlparams)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            var url = form.Url(urlparams);
            if (!string.IsNullOrEmpty(url))
                return helper.Content(url);
            return "javascript:void(0);";
        }

        /// <summary>
        /// Get parameters from query string.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <returns></returns>
        public static QueryParams GetQueryParams(this UrlHelper helper)
        {
            var queryStr = helper.RequestContext.HttpContext.Request.QueryString;
            var query = new QueryParams();

            if (!string.IsNullOrEmpty(queryStr["index"]))
                query.Index = int.Parse(queryStr["index"]);

            if (!string.IsNullOrEmpty(queryStr["size"]))
                query.Size = int.Parse(queryStr["size"]);

            return query;
        }

        /// <summary>
        /// Get default absolute url for user.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="userName">The user name.</param>
        /// <returns>A absolute url of user.</returns>
        public static string User(this UrlHelper helper, string userName)
        {
            return helper.Content("~/profiles/" + userName);
        }

        public static string Resource(this UrlHelper helper, string solutionName, string resourceName)
        {
            return helper.Action("Resource", "Sys", new { solution = solutionName, name = resourceName });
        }
    }
}