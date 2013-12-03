//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Web.Mvc;

namespace DNA.Web
{
    /// <summary>
    /// The filter class use to generate the ContentList,ContentItem and ContentView instance for current context.
    /// </summary>
    /// <remarks>
    /// This filter will generate the instances to App.Context and auto create the form page when it's not exists.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ContentViewPageAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var app = App.Get();
            var ctx = app.Context;
            var routeData = filterContext.RouteData.Values;
            var name = (string)routeData["list_name"];
            var slug = (string)routeData["slug"];
            var dynamicPageTitle = "";
            var culture = ctx.Locale;

            if (!string.IsNullOrEmpty(name))
                ctx.List = app.CurrentWeb.Lists[name];

            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(slug))
                    ctx.View = ctx.List.Views[slug];

                if (routeData.ContainsKey("tags") || routeData.ContainsKey("archives"))
                {
                    if (ctx.List.DefaultView != null)
                    {
                        ctx.View = ctx.List.DefaultView;
                        slug = ctx.List.DefaultView.Name;
                        if (routeData.ContainsKey("tags"))
                            dynamicPageTitle = ctx.List.Title + " : " + routeData["tag"];
                        else
                            dynamicPageTitle = ctx.List.Title + " : " + routeData["year"] + "-" + routeData["month"];
                    }
                }

                if (ctx.View != null && !ctx.View.IsAuthorized(filterContext.RequestContext.HttpContext))
                {
                    filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
                    return;
                }

                if (ctx.View != null && !string.IsNullOrEmpty(ctx.View.Title) && string.IsNullOrEmpty(dynamicPageTitle))
                    dynamicPageTitle = ctx.View.Title;


                var view = ctx.View;
                var viewUrlFormat = "lists/{0}/views/{1}";
                var slugFormatted = string.Format(viewUrlFormat, name, slug);
                var page = app.CurrentWeb.FindPage(culture, slugFormatted);

                if (page == null)
                {
                    if (view != null && !view.NoPage)
                    {
                        var parentID = 0;
                        if (!view.IsDefault && ctx.List.DefaultView != null)
                        {
                            //Find parent page
                            var parentPage = app.CurrentWeb.FindPage(culture, string.Format(viewUrlFormat, name, ctx.List.DefaultView.Name));
                            if (parentPage != null)
                                parentID = parentPage.ID;
                        }

                        page = app.CurrentWeb.CreatePage(view, parentID);
                        ctx.Page = page.Model;
                    }
                    else
                    {
                        if (view != null)
                        {
                            if (ctx.List.DefaultView != null)
                            {
                                filterContext.Result = new RedirectResult(ctx.List.DefaultUrl);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    ctx.Page = page;
                }

                if (page != null)
                {
                    if (!string.IsNullOrEmpty(dynamicPageTitle))
                    {
                        filterContext.Controller.ViewBag.Title = dynamicPageTitle;
                        page.Title = dynamicPageTitle;
                    }

                    filterContext.Controller.ViewBag.Page = app.Wrap(page);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
