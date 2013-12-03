//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the current website context.
    /// </summary>
    public class WebContext
    {
        private RouteData routeData = null;
        private int[] permHashValues = null;
        private Web web = null;
        private WebPage page = null;

        private IDataContext DataContext
        {
            get
            {
                return DependencyResolver.Current.GetService<IDataContext>();
            }
        }

        internal WebContext()
        {
            this.HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
        }

        internal WebContext(HttpContextBase httpContext)
        {
            this.HttpContext = httpContext;
        }

        /// <summary>
        /// Gets current http context.
        /// </summary>
        public HttpContextBase HttpContext { get; private set; }

        /// <summary>
        /// Gets current route data object.
        /// </summary>
        public RouteData RouteData
        {
            get
            {
                if (routeData == null)
                    return this.HttpContext.Request.RequestContext.RouteData;
                else
                    return routeData;
            }
        }

        /// <summary>
        /// Gets curren request
        /// </summary>
        public RequestContext RequestContext
        {
            get
            {
                return this.HttpContext.Request.RequestContext;
            }
        }

        /// <summary>
        /// Gets the current website name
        /// </summary>
        public string Website
        {
            get
            {
                var webName = "home";
                if (this.RouteData != null && this.RouteData.Values["website"] != null)
                    webName = this.RouteData.Values["website"].ToString();
                else
                {
                    if (!string.IsNullOrEmpty(this.HttpContext.Request.QueryString["website"]))
                        webName = this.HttpContext.Request.QueryString["website"];
                }
                return webName;
            }
        }

        /// <summary>
        /// Gets the current locale
        /// </summary>
        public string Locale
        {
            get
            {
                var locale = "";

                if (this.RouteData != null && this.RouteData.Values["locale"] != null)
                    locale = this.RouteData.Values["locale"].ToString();

                if (string.IsNullOrEmpty(locale) && !string.IsNullOrEmpty(this.RequestContext.HttpContext.Request.QueryString["locale"]))
                    locale = this.RequestContext.HttpContext.Request.QueryString["locale"];
                return locale;
            }

        }

        public string Slug
        {
            get
            {
                var slug = "default";
                if (this.RouteData != null && this.RouteData.Values["slug"] != null)
                    slug = this.RouteData.Values["slug"].ToString();
                return slug;

            }
        }

        /// <summary>
        /// Gets the full url of current application
        /// </summary>
        public Uri AppUrl
        {
            get
            {
                string baseUrl = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + HttpContext.Request.ApplicationPath;
                return new Uri(baseUrl);
            }
        }

        /// <summary>
        /// Gets the current request culture info object
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                var culture = new CultureInfo(this.Locale);
                return culture;
            }
        }

        ///// <summary>
        ///// Gets the curent requrest UI culture.
        ///// </summary>
        //public CultureInfo UICulture
        //{
        //    get
        //    {
        //    }
        //}

        public const string UserLocaleCookieName = "dna_user_sel_language";

        /// <summary>
        /// Gets default user language locale name.
        /// </summary>
        public string UserLocale
        {
            get
            {
                //Get user prefer language
                var lang = "";
                var cookie = HttpContext.Request.Cookies[UserLocaleCookieName];
                if (cookie != null)
                    lang = cookie.Value;
                else
                {
                    if (HttpContext.Request.UserLanguages != null && HttpContext.Request.UserLanguages.Length > 0)
                        lang = HttpContext.Request.UserLanguages[0];
                    else
                        lang = this.Locale;
                }
                return lang;
            }
        }

        //internal RouteData GetRouteData(Uri uri)
        //{
        //    var innerhttpContext = new HttpContext(new System.Web.Hosting.SimpleWorkerRequest(VirtualPathUtility.ToAppRelative(uri.LocalPath).Replace("~/", ""), "", null));
        //    var wrapper = new HttpContextWrapper(innerhttpContext);

        //    foreach (var route in RouteTable.Routes)
        //    {
        //        routeData = route.GetRouteData(wrapper);
        //        if (routeData != null)
        //        {
        //            var vpath = route.GetVirtualPath(wrapper.Request.RequestContext, routeData.Values);
        //            if (vpath != null)
        //            {
        //                return routeData;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //internal string GetRouteDefaultUrl(string[] ignoreRouteDataKeys, RouteData routeData)
        //{
        //    string defaultUrl = "";
        //    var routeDataValues = new RouteValueDictionary(routeData.Values);
        //    var route = (Route)routeData.Route;

        //    if (route != null)
        //    {
        //        if (ignoreRouteDataKeys != null)
        //        {
        //            //Using default value
        //            foreach (string key in ignoreRouteDataKeys)
        //            {
        //                if (route.Defaults.ContainsKey(key) && (routeDataValues.ContainsKey(key)))
        //                    routeDataValues[key] = route.Defaults[key];
        //            }
        //        }

        //        var virtualPathData = route.GetVirtualPath(this.HttpContext.Request.RequestContext, routeDataValues);
        //        if (virtualPathData != null)
        //            defaultUrl = virtualPathData.VirtualPath;
        //    }

        //    if (!string.IsNullOrEmpty(defaultUrl))
        //        defaultUrl = defaultUrl.StartsWith("/") ? ("~" + defaultUrl) : ("~/" + defaultUrl);

        //    return defaultUrl;
        //}

        //internal bool IsDefaultPageExists(string[] ignoreRouteDataKeys)
        //{
        //    return DataContext.WebPages.Find(GetRouteDefaultUrl(ignoreRouteDataKeys, RouteData)) != null;
        //}

        //public WebPage FindWebPage(Uri uri, RouteData routeData = null)
        //{
        //    if (Web == null) return null;

        //    if (routeData == null)
        //        routeData = RouteData;
        //    //routeData.Route
        //    if (DataContext == null) return null;
        //    var vPath = UrlUtility.ParseVirtualPath(uri, HttpContext.Request.ApplicationPath);
        //    var _tmp = DataContext.WebPages.Find(vPath);

        //    if (_tmp == null)
        //    {
        //        var sharingPages = DataContext.WebPages.Filter(p => p.IsStatic && p.IsShared);
        //        foreach (var sp in sharingPages)
        //        {
        //            if (sp.Path.Equals(GetRouteDefaultUrl(sp.IgnoreRouteKeys, routeData), StringComparison.OrdinalIgnoreCase))
        //            {
        //                _tmp = sp;
        //                break;
        //            }
        //        }
        //    }
        //    return _tmp;
        //}

        #region The data methods

        /// <summary>
        /// Gets current web instance from request url.
        /// </summary>
        public Web Web
        {
            get
            {
                if (web == null)
                    web = DataContext.Find<Web>(w => w.Name.Equals(Website, StringComparison.OrdinalIgnoreCase));
                return web;
            }
        }

        /// <summary>
        /// Gets current web page from request url
        /// </summary>
        public WebPage Page
        {
            get
            {
                if (page == null)
                    page = DataContext.Find<WebPage>(p => p.Slug.Equals(Slug, StringComparison.OrdinalIgnoreCase) && p.Locale.Equals(Locale, StringComparison.OrdinalIgnoreCase));
                return page;
            }
            internal set { page = value; }
        }

        /// <summary>
        /// Gets current content list from route
        /// </summary>
        public ContentListDecorator List { get; set; }

        /// <summary>
        ///  Gets current content list view.
        /// </summary>
        public ContentViewDecorator View { get; set; }

        /// <summary>
        /// Gets current content list form.
        /// </summary>
        public ContentFormDecorator Form { get; set; }

        /// <summary>
        /// Gets current data item.
        /// </summary>
        public ContentDataItemDecorator DataItem { get; internal set; }

        /// <summary>
        /// Indicates whether current user is a member of specified role.
        /// </summary>
        /// <param name="role">The role name.</param>
        /// <returns>true if a member otherwrise return false.</returns>
        public bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException("role");

            var roleCacheKey = "_roles";
            string[] cacheRoles = HttpContext.Items[roleCacheKey] != null ? (string[])HttpContext.Items[roleCacheKey] : null;
            if (cacheRoles != null)
            {
                return cacheRoles.Contains(role);
            }
            else
            {
                if (HttpContext.Request.IsAuthenticated)
                {
                    var roles = App.Get().Roles.GetUserRoles(HttpContext.User.Identity.Name);
                    HttpContext.Items[roleCacheKey] = roles;
                    return roles.Contains(role);
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates whether current usere has access permission to execute the specified action.
        /// </summary>
        /// <typeparam name="TController">The controller type.</typeparam>
        /// <param name="action">The action name that defined in controller.</param>
        /// <returns>true if has access permission otherwrise return false.</returns>
        public bool HasPermisson<TController>(string action)
        {
            return HasPermisson(typeof(TController), action);
        }

        /// <summary>
        /// Indicates whether current usere has access permission to execute the specified action.
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="controller">The type of controller.</param>
        /// <param name="action">The action name that defined in controller.</param>
        /// <returns>true if has access permission otherwrise return false.</returns>
        public bool HasPermisson<TController>(TController controller, string action)
        {
            return HasPermisson<TController>(action);
        }

        /// <summary>
        /// Indicates whether current usere has access permission to execute the specified action.
        /// </summary>
        /// <param name="controllerType">The type of controller.</param>
        /// <param name="action">The action name that defined in controller.</param>
        /// <returns>true if has access permission otherwrise return false.</returns>
        public bool HasPermisson(Type controllerType, string action)
        {
            if (!HttpContext.Request.IsAuthenticated) return false;

            if (controllerType == null) return false;

            if (string.IsNullOrEmpty(action)) return false;

            if (IsInRole("administrators")) return true;

            if ((Permissions != null) && (permHashValues != null))
            {
                var code = (controllerType.FullName + "." + action).ToLower().GetHashCode();
                return permHashValues.Contains(code);
            }
            return false;
        }

        /// <summary>
        /// Gets permissions for current user's roles.
        /// </summary>
        public IEnumerable<Permission> Permissions
        {
            get
            {
                var permKey = "_userperms";
                if (HttpContext.Items[permKey] != null)
                {
                    return (IEnumerable<Permission>)HttpContext.Items[permKey];
                }
                else
                {
                    if (HttpContext.Request.IsAuthenticated)
                    {
                        var rolesPermissions = App.Get().DataContext.Permissions.GetUserPermissions(HttpContext.User.Identity.Name);
                        if ((rolesPermissions != null) && (rolesPermissions.Count() > 0))
                            permHashValues = rolesPermissions.Select(p => (p.Controller + "." + p.Action).ToLower().GetHashCode()).ToArray();
                        HttpContext.Items[permKey] = rolesPermissions;
                        return rolesPermissions;
                    }
                }
                return null;
            }
        }

        #endregion
    }
}
