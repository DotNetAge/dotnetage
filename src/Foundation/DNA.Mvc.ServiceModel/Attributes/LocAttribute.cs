//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Web.Mvc;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents an attribute that is used to handle content and UI culture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class LocAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var app = App.Get();
            var locale = app.Context.Locale;
            var userLocale = app.Context.UserLocale;

            if (string.IsNullOrEmpty(locale))
                locale = App.Settings.DefaultLocale;

            if (string.IsNullOrEmpty(userLocale))
                userLocale = locale;

            app.SetCulture(locale, userLocale);

            base.OnActionExecuting(filterContext);
        }
    }
}
