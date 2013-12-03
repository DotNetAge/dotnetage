//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class InstallationGuardAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var needLogin = true;

            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (filterContext.HttpContext.User.Identity.Name.Equals(App.Settings.Administrator))
                    needLogin = false;
            }

            if (filterContext.HttpContext.Request.Cookies["dna_administrator"] != null)
                needLogin = false;

            if (needLogin)
                filterContext.Result = new RedirectResult("~/install/login");

        }
    }
}