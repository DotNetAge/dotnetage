//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Linq;
using System.Web.Mvc;
using DNA.Utility;

namespace DNA.Web.UI
{
    /// <summary>
    /// Represents an attribute that is used to handle the return url.
    /// </summary>
    /// <remarks>
    /// The current request must have "returnUrl" routeData or has "returnUrl" in query string .
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ReturnAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var returnUrl = "";
            if (filterContext.RequestContext.RouteData.Values.ContainsKey("returnUrl"))
                returnUrl = (string)filterContext.RequestContext.RouteData.Values["returnUrl"];

            if (string.IsNullOrEmpty(returnUrl) && filterContext.HttpContext.Request.QueryString.AllKeys.Contains("returnUrl", StringComparer.OrdinalIgnoreCase))
                returnUrl = filterContext.HttpContext.Request.QueryString["returnUrl"];

            if (!string.IsNullOrEmpty(returnUrl) && UrlUtility.CreateUrlHelper().IsLocalUrl(returnUrl))
                filterContext.Result = new RedirectResult(returnUrl);
            else
                base.OnActionExecuted(filterContext);
        }
    }
}
