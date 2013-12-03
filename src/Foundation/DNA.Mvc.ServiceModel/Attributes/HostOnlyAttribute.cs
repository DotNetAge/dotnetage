//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Web.Mvc;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents an attribute that is used to speicify the Action only allow Administrator access and only run under Dashboard area. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HostOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAuthenticated &&
                filterContext.RequestContext.HttpContext.User.Identity.Name.Equals(App.Settings.Administrator))
                base.OnActionExecuting(filterContext);
            else
                filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}
