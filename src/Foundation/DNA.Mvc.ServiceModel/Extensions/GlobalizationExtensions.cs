//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace DNA.Web.UI
{
    /// <summary>
    /// The helper class provides the methods for get globalization object and format globalization strings.
    /// </summary>
    public static class GlobalizationExtensions
    {
        /// <summary>
        /// Render the globalize resource text
        /// </summary>
        /// <param name="helper">The HTML helper</param>
        /// <param name="key">the reousece key</param>
        /// <returns></returns>
        public static string Global(this HtmlHelper helper, string key)
        {
            return Global(helper, null, key);
        }

        /// <summary>
        /// Render the globalize resource text
        /// </summary>
        /// <param name="helper">The HTML helper</param>
        /// <param name="classKey">the key specified the which resource shold load</param>
        /// <param name="key">the resource key</param>
        /// <returns></returns>
        public static string Global(this HtmlHelper helper, string classKey, string key)
        {
            string ck = "language";
            if (!string.IsNullOrEmpty(classKey))
                ck = classKey;
            try
            {
                return HttpContext.GetGlobalResourceObject(ck, key, CurrentCulture).ToString();
            }
            catch { return key; }
        }

        private static CultureInfo CurrentCulture
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
                // HtmlUIExtensions.GetCultureInfo(); 
            }
        }

        /// <summary>
        /// Render the value to current culture number format.
        /// </summary>
        /// <param name="helper">The html helper.</param>
        /// <param name="value">The value to be formated.</param>
        /// <returns></returns>
        public static string Global(this HtmlHelper helper, int value)
        {
            return value.ToString("D", CurrentCulture.NumberFormat);
        }

        /// <summary>
        /// Render the value to current culture number format.
        /// </summary>
        /// <param name="helper">The html helper.</param>
        /// <param name="value">The value to be formated.</param>
        /// <returns></returns>
        public static string Global(this HtmlHelper helper, float value)
        {
            return value.ToString("N", CurrentCulture.NumberFormat);
        }

        /// <summary>
        /// Render the value to current culture number format.
        /// </summary>
        /// <param name="helper">The html helper.</param>
        /// <param name="value">The value to be formated.</param>
        /// <returns></returns>
        public static string Global(this HtmlHelper helper, decimal value)
        {
            return value.ToString("N", CurrentCulture.NumberFormat);
        }

        /// <summary>
        /// Render the value to current culture number format.
        /// </summary>
        /// <param name="helper">The html helper.</param>
        /// <param name="value">The value to be formated.</param>
        /// <returns></returns>
        public static string Global(this HtmlHelper helper, double value)
        {
            return value.ToString("N", CurrentCulture.NumberFormat);
        }

        /// <summary>
        /// Render the value to current culture currency format.
        /// </summary>
        /// <param name="helper">The html helper.</param>
        /// <param name="value">The value to be formated.</param>
        /// <returns></returns>
        public static string Currency(this HtmlHelper helper, float value)
        {
            return value.ToString("C", CurrentCulture.NumberFormat);
        }

        /// <summary>
        /// Render the value to current culture currency format.
        /// </summary>
        /// <param name="helper">The html helper.</param>
        /// <param name="value">The value to be formated.</param>
        /// <returns></returns>       
        public static string Currency(this HtmlHelper helper, decimal value)
        {
            return value.ToString("C", CurrentCulture.NumberFormat);
        }

        /// <summary>
        /// Render the value to current culture currency format.
        /// </summary>
        /// <param name="helper">The html helper.</param>
        /// <param name="value">The value to be formated.</param>
        /// <returns></returns>
        public static string Currency(this HtmlHelper helper, double value)
        {
            return value.ToString("C", CurrentCulture.NumberFormat);
        }

        /// <summary>
        /// Convert the date to current user's time zone and format the datetime to the user culture.
        /// </summary>
        /// <param name="helper">The html helper.</param>
        /// <param name="date">the date to be formated.</param>
        /// <returns></returns>
        public static string Global(this HtmlHelper helper, DateTime date, bool isTime)
        {
            DateTime _date = date;
            if (helper.ViewContext.HttpContext.Request.IsAuthenticated)
            {
                //   ProfileBase profile = helper.ViewContext.HttpContext.Profile;
                // if (profile["TimeZone"] != null)
                //{
                //if (!string.IsNullOrEmpty(profile["TimeZone"].ToString()))
                var web = App.Get().CurrentWeb;
                if (!string.IsNullOrEmpty(web.TimeZone))
                {
                    try
                    {
                        // WebSite.Open(helper.ViewContext.RouteData);

                        TimeZoneInfo destTimeZone = web.TimeZoneInfo;

                        //TimeZoneInfo.FindSystemTimeZoneById(profile["TimeZone"].ToString());
                        if (destTimeZone != null)
                            _date = TimeZoneInfo.ConvertTime(date, web.TimeZoneInfo, destTimeZone);
                    }
                    catch (Exception e)
                    {
                        if (e is TimeZoneNotFoundException)
                        {
                            if (isTime)
                                return _date.ToString(CurrentCulture.DateTimeFormat.LongTimePattern);
                            else
                                return _date.ToString(CurrentCulture.DateTimeFormat);
                        }
                    }
                }
                //}
            }
            if (isTime)
                return _date.ToString(CurrentCulture.DateTimeFormat.LongTimePattern);
            else
                return _date.ToString(CurrentCulture.DateTimeFormat);
        }

        public static string Global(this HtmlHelper helper, DateTime date)
        {
            return helper.Global(date, false);
        }

        /// <summary>
        /// Format the input string to the globalize string text.if the input text start with # this method will the key 
        /// in the global resource file.
        /// </summary>
        /// <param name="helper">The Html helper</param>
        /// <param name="text">The input text to globalize</param>
        /// <returns></returns>
        public static string GlobalFormat(this HtmlHelper helper, string text)
        {
            if (text.StartsWith("#"))
            {
                string key = text.Substring(1);
                if (!string.IsNullOrEmpty(key))
                    return Global(helper, key);
            }
            return text;
        }

        /// <summary>
        /// Register the client resource to the client resource object.
        /// </summary>
        /// <remarks>
        /// After register you can get the resource value in javascript by using portal.res.[the resource name you register]
        /// </remarks>
        /// <param name="helper">The html helper.</param>
        /// <param name="name">The resource name.</param>
        /// <param name="value">The resource value.</param>
        public static void ClientRes(this HtmlHelper helper, string name, string value)
        {
            if (helper.ViewContext.TempData["ClientRes"] == null)
                helper.ViewContext.TempData["ClientRes"] = new Dictionary<string, string>();
            Dictionary<string, string> resources = helper.ViewContext.TempData["ClientRes"] as Dictionary<string, string>;
            if (!resources.Keys.Contains(name))
                resources.Add(name, value);
        }
    }

    /// <summary>
    /// Defines the globalization methods
    /// </summary>
    [Obsolete]
    public static class GE
    {
        ///// <summary>
        ///// Get current culture for current request.
        ///// </summary>
        ///// <returns></returns>
        //public static CultureInfo GetCurrentCulture(HttpContextBase httpContext)
        //{
        //    if (httpContext != null)
        //    {
        //        var lang = App.Get().Context.Locale;
        //        //"en-US";
        //        //var routeData=httpContext.Request.RequestContext.RouteData
        //        //if ((httpContext.Request != null) && (httpContext.Request.UserLanguages != null) && (httpContext.Request.UserLanguages.Count() > 0))
        //        //    lang = httpContext.Request.UserLanguages.First();
        //        //var cookieName = "userLanguage";
        //        //if (httpContext.Request.IsAuthenticated)
        //        //{
        //        //    var lanInAuthCookie = GetLanguageFromCookie(httpContext, cookieName);
        //        //    if (!string.IsNullOrEmpty(lanInAuthCookie))
        //        //    {
        //        //        lang = lanInAuthCookie;
        //        //    }
        //        //    else
        //        //    {
        //        //       ///TODO:Read language from profile
        //        //        lang = "en-US";
        //        //    }
        //        //    //else
        //        //    //{
        //        //    //    if ((httpContext.Profile["Language"] != null) && (!string.IsNullOrEmpty(httpContext.Profile["Language"] as string)))
        //        //    //        lang = httpContext.Profile["Language"].ToString();
        //        //    //}
        //        //}
        //        //else
        //        //{
        //        //    //Anonymous
        //        //    //cookieName = httpContext.Request.AnonymousID;
        //        //    var langInCookie = GetLanguageFromCookie(httpContext, cookieName);
        //        //    if (!string.IsNullOrEmpty(langInCookie))
        //        //        lang = langInCookie;
        //        //}
        //        var curCulture = CultureInfo.GetCultureInfo(lang);

        //        if (curCulture != null)
        //        {
        //            Thread.CurrentThread.CurrentUICulture = curCulture;
        //            Thread.CurrentThread.CurrentCulture = curCulture;
        //            return curCulture;
        //        }
        //    }

        //    return Thread.CurrentThread.CurrentUICulture;
        //}

        //public static void SetLanguageCookie(HttpContextBase httpContext, string cookieName, string lang)
        //{
        //    if (httpContext.Response.Cookies.AllKeys.Contains(cookieName))
        //        httpContext.Response.Cookies[cookieName].Value = lang;
        //    else
        //        httpContext.Response.Cookies.Add(new HttpCookie(cookieName, lang));
        //}

        //private static string GetLanguageFromCookie(HttpContextBase httpContext, string cookieName)
        //{
        //    string lang = "";

        //    if (httpContext.Request.Cookies[cookieName] != null)
        //    {
        //        if (!string.IsNullOrEmpty(httpContext.Request.Cookies[cookieName].Value))
        //            lang = httpContext.Request.Cookies[cookieName].Value;
        //    }
        //    return lang;
        //}

        public static void SetCulture()
        {
            try
            {
                var lang = App.Get().CurrentWeb.Culture;
                var curCulture = CultureInfo.GetCultureInfo(lang);

                if (curCulture != null)
                {
                    Thread.CurrentThread.CurrentUICulture = curCulture;
                    Thread.CurrentThread.CurrentCulture = curCulture;
                }
            }
            catch { }
        }

        public static string Global(string classKey, string key)
        {
            return GlobalizationExtensions.Global(null, classKey, key);
        }

        public static string Global(string key)
        {
            return GlobalizationExtensions.Global(null, key);
        }

        /// <summary>
        /// Load the globalization conent form user label
        /// </summary>
        /// <param name="label"></param>
        /// <remarks>
        /// Global conent string format : 
        /// format 1: [g=classkey]resKey[/g]
        /// format 2:[g]reskey[/g]
        /// </remarks>
        /// <returns></returns>
        public static string GetContent(string label)
        {
            if (!string.IsNullOrEmpty(label) && label.StartsWith("[g:", StringComparison.OrdinalIgnoreCase))
            {
                var match = GLabelRegex2.Match(label);
                var key = label;
                var classKey = "language";
                if (match.Success)
                {
                    classKey = match.Result("$1");
                    key = match.Result("$2");
                }
                else
                {
                    match = GLabelRegex.Match(label);
                    if (match.Success)
                    {
                        key = match.Result("$1");
                    }
                }
                var gresult = Global(classKey, key);
                //When the resource key not found in language file we will find the "labels" file again.
                if (gresult.Equals(key) && classKey.Equals("language"))
                    return Global("labels", key);
                return gresult;
            }
            return label;
        }

        public static string Global(this Controller controller, string label)
        {
            return GetContent(label);
        }

        private static readonly Regex GLabelRegex = new Regex(@"\[g\:(.+?)\]", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex GLabelRegex2 = new Regex(@"\[g\:(.+?)\:(.+?)\]", RegexOptions.Compiled | RegexOptions.Singleline);
    }
}
