//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DNA.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class LanguageDirectorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;

            //Start default language locate
            if (request.Path.Equals("/") && request.UserLanguages != null && request.UserLanguages.Length > 0)
            {
                var userLang = request.UserLanguages[0].ToLower();
                var langs = App.Get().SupportedCultures.Select(n => n.Name.ToLower()).ToArray();
                if (langs.Contains(userLang))
                {
                    var routeData = filterContext.RequestContext.RouteData;
                    if (routeData.Values["website"] != null && !string.IsNullOrEmpty((string)routeData.Values["website"]))
                    {
                        var webname = routeData.Values["website"].ToString();
                        var web = App.Get().Webs[webname];
                        if (web!=null && web.InstalledLocales.Contains(userLang))
                        {
                            string url = string.Format("~/{0}/{1}", routeData.Values["website"], userLang);
                            filterContext.Result = new RedirectResult(url);
                            return;
                        }
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
