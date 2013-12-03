//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DNA.Web
{
    /// <summary>
    /// Represents an attribute that is used to define the Action render as DotNetAge dynamic page.
    /// </summary>
    public class SiteMapActionAttribute : ActionFilterAttribute
    {
        private bool showInMenu = true;
        private bool isShared = true;
        private string[] ignoreRouteValues;

        /// <summary>
        /// Gets/Sets ignore the route data values of the request.
        /// </summary>
        public string[] IgnoreRouteDataKeys
        {
            get { return ignoreRouteValues; }
            set { ignoreRouteValues = value; }
        }

        /// <summary>
        /// Gets/Sets only create page for this action once time.
        /// </summary>
        /// <remarks>
        /// If this property sets the ExcludeValues will be enable
        /// </remarks>
        public bool IsShared
        {
            get { return isShared; }
            set { isShared = value; }
        }

        /// <summary>
        /// Gets/Sets the whether the SiteMapAction can shows in the main menu.
        /// </summary>
        public bool ShowInMenu
        {
            get { return showInMenu; }
            set { showInMenu = value; }
        }

        /// <summary>
        /// Gets / Sets the webpage auto generate into sitemap.xml
        /// </summary>
        public bool ShowInSiteMap { get; set; }

        /// <summary>
        /// Gets/Sets the title of the SiteMapAction
        /// </summary>
        /// <remarks>
        /// This title will shows in menu and html header's title tag.
        /// </remarks>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the description of the SiteMapAction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets the template file visual path that using when the action page is create.
        /// </summary>
        public string Template { get; set; }
        
        public static Regex ipExpr = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var app = App.Get();
            var routeData = filterContext.RequestContext.RouteData;
            var routeValues = routeData.Values;
            var controller = (string)routeValues["controller"];
            var action = (string)routeValues["action"];
            var locale = (routeValues.ContainsKey("locale") && routeValues["locale"] != null) ? routeValues["locale"].ToString() : App.Settings.DefaultLocale.ToLower();
            var context = app.Context;
            var request = filterContext.HttpContext.Request;
            WebPageDecorator page = null;
            var hostName = request.Url.Host;

            #region Resolve www

            if (App.Settings.WWWResolved && !hostName.StartsWith("localhost"))
            {
                if (!ipExpr.IsMatch(hostName))
                {
                    if (!hostName.StartsWith("www") && hostName.Split('.').Count() == 1)
                    {
                        var resolveUrl = request.Url.Scheme + "://www." + request.Url.Authority + request.RawUrl;
                        filterContext.Result = new RedirectResult(resolveUrl, true);
                        return;
                    }
                }
            }

            #endregion

            ///TODO:When the web not found redirect to web list page
            if (app.CurrentWeb == null)
            {
                if (app.Urls.Redirect(filterContext.HttpContext))
                    filterContext.Result = new HttpStatusCodeResult(301);
                else
                {
                    if ((string)routeValues["website"] == "home")
                        filterContext.Result = new RedirectResult("~/install/index", true);
                    else
                        filterContext.Result = new RedirectResult("~/", true);
                }
                return;
            }

            if (request.IsAuthenticated)
            {
                //Generate the user 
                filterContext.HttpContext.User = app.User;
            }

            if (controller.Equals("DynamicUI", StringComparison.OrdinalIgnoreCase) && action.Equals("Index", StringComparison.OrdinalIgnoreCase))
            {
                //This is a dynamic web page
                var slug = (string)routeValues["slug"];
                page = app.CurrentWeb.FindPage(locale, slug);
                if (page != null)
                {
                    context.Page = page.Model;
                    filterContext.Controller.ViewBag.Page = page;
                }
                else
                {
                    if (app.Urls.Redirect(filterContext.HttpContext))
                        filterContext.Result = new HttpStatusCodeResult(301);
                    else
                    {
                        if (app.CurrentWeb.DefaultPage != null)
                            filterContext.Result = new RedirectResult(app.CurrentWeb.DefaultPage.Url);
                        else
                            filterContext.Result = new HttpNotFoundResult();
                    }
                }
            }
            else
            {
                #region refill the parameters for action

                if (filterContext.ActionParameters.Count > 0)
                {
                    //If the action is custom action we must generate the locale , website
                    var _params = filterContext.ActionDescriptor.GetParameters();

                    foreach (var _param in _params)
                    {
                        if (_param.ParameterType.Equals(typeof(WebContext)))
                            filterContext.ActionParameters[_param.ParameterName] = context;
                        else
                        {
                            if (_param.ParameterType.Equals(typeof(string)) && _param.ParameterName.Equals("website", StringComparison.OrdinalIgnoreCase))
                                filterContext.ActionParameters[_param.ParameterName] = context.Website;

                            if (_param.ParameterType.Equals(typeof(string)) && _param.ParameterName.Equals("locale", StringComparison.OrdinalIgnoreCase))
                                filterContext.ActionParameters[_param.ParameterName] = context.Locale;
                        }
                    }
                }

                #endregion

                //Generate the slug from route
                if (routeData != null && routeData.Route != null)
                {
                    var route = (System.Web.Routing.Route)routeData.Route;
                    var routeUrl = route.Url;

                    #region format routeUrl as slug

                    foreach (var key in routeValues.Keys)
                    {
                        if ((IgnoreRouteDataKeys != null) && (IgnoreRouteDataKeys.Length > 0))
                        {
                            if (IgnoreRouteDataKeys.Contains(key))
                                continue; //ignore
                        }

                        if (routeValues[key] != null)
                        {
                            routeUrl = routeUrl.Replace("{" + key + "}", routeValues[key].ToString());
                        }
                        else
                        {
                            if (route.Defaults.ContainsKey(key) && route.Defaults[key] != null)
                                routeUrl = routeUrl.Replace("{" + key + "}", route.Defaults[key].ToString());
                        }
                    }

                    #endregion

                    page = app.CurrentWeb.FindPage(locale, routeUrl);

                    if (page == null)
                    {
                        var newpage = new WebPage()
                        {
                            Slug = routeUrl,
                            IsShared = this.IsShared,
                            IsStatic = true,
                            Title = string.IsNullOrEmpty(this.Title) ? filterContext.ActionDescriptor.ActionName : this.Title,
                            Description = this.Description,
                            ShowInMenu = this.ShowInMenu,
                            ShowInSitemap = this.ShowInSiteMap,
                            AllowAnonymous = true,
                            Created = DateTime.Now,
                            LastModified = DateTime.Now,
                            WebID = app.CurrentWeb.Id,
                            Owner = app.CurrentWeb.Owner,
                            Dir = app.CurrentWeb.Dir,
                            Locale = locale
                        };
                        app.DataContext.WebPages.Create(newpage);
                        app.DataContext.SaveChanges();
                        page = app.Wrap(newpage);
                        context.Page = newpage;
                    }
                    else
                        context.Page = page.Model;

                    filterContext.Controller.ViewBag.Page = page;
                }
            }

            if (page != null)
            {
                if (!page.IsAuthorized(filterContext.HttpContext))
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
                else
                {
                    if (!string.IsNullOrEmpty(page.Description))
                        filterContext.Controller.ViewBag.Description = page.Description;
                    if (!string.IsNullOrEmpty(page.Keywords))
                        filterContext.Controller.ViewBag.Keywords = page.Keywords;

                    if (!string.IsNullOrEmpty(page.LinkTo))
                        filterContext.Result = new RedirectResult(page.LinkTo);
                }
            }
        }

    }
}
