//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace DNA.Web.UI
{
    /// <summary>
    /// The helper class that provide UI helper methods
    /// </summary>
    public static class HtmlUIExtensions
    {
        /// <summary>
        /// Render the Theme elements for the portal.
        /// </summary>
        /// <param name="helper">The HTML Helper</param>
        /// <param name="theme">The Theme name which to render.</param>
        /// <returns></returns>
        [Obsolete]
        public static string Theme(this HtmlHelper helper, string theme)
        {
            return Theme(theme);
        }

        [Obsolete]
        public static string Theme(string theme)
        {
            StringBuilder links = new StringBuilder();
            links.AppendLine(CssLinks("~/Content/Themes/" + theme));
            return links.ToString();
        }

        /// <summary>
        /// Render the stylesheet links tag.
        /// </summary>
        /// <remarks>
        /// This helper will find all the *.css file in the specified path and resovle the client url of the files.
        /// </remarks>
        /// <param name="helper">The HTML Helper</param>
        /// <param name="path">The path where contains the stylesheets</param>
        /// <returns></returns>
        [Obsolete]
        public static string CssLinks(this HtmlHelper helper, string path)
        {
            return CssLinks(path);
        }

        [Obsolete]
        public static string CssLinks(string path)
        {
            StringBuilder links = new StringBuilder();
            string filePath = HttpContext.Current.Server.MapPath(path);
            if (Directory.Exists(filePath))
            {
                string[] cssFiles = System.IO.Directory.GetFiles(filePath, "*.css");
                for (int i = 0; i < cssFiles.Length; i++)
                {
                    string cssFile = System.IO.Path.GetFileName(cssFiles[i]);
                    links.AppendLine(CssLink(path + "/" + cssFile));
                }
            }
            return links.ToString();
        }

        /// <summary>
        /// Render the meta tag 
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="name">The meta name</param>
        /// <param name="content">The meta content</param>
        /// <returns></returns>
        public static string Meta(this HtmlHelper helper, string name, string content)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<meta");
            builder.Append(" name=\"" + name + "\"");
            builder.Append(" content=\"" + content + "\" />");
            return builder.ToString();
        }

        /// <summary>
        /// Generate google analytics track script 
        /// </summary>
        /// <remarks>
        /// if WebSite.GAAccount is empty this method will not invoke
        /// </remarks>
        /// <returns></returns>
        public static MvcHtmlString GoogleAnalyticsTrackingScript(this HtmlHelper helper)
        {
            //Microsoft.Web.Helpers.Analytics.GetGoogleAsyncHtml
            // var web = WebSite.Open();
            var _account = WebConfigurationManager.AppSettings["GAAccount"];

            if (!string.IsNullOrEmpty(_account))
            {
                StringBuilder scripts = new StringBuilder();

                scripts.Append("<script type=\"text/javascript\">")
                    .Append("var _gaq = _gaq || [];")
                .Append("_gaq.push(['_setAccount', '")
                .Append(_account)
                .Append("']);")
                .Append("_gaq.push(['_trackPageview']); ")
                .Append("(function(){")
                .Append("var ga = document.createElement('script');")
                .Append("ga.type = 'text/javascript';")
                .Append("ga.async = true; ")
                .Append("ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';")
                .Append("var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);")
                .Append("})();")
                .Append("</script>");
                return MvcHtmlString.Create(scripts.ToString());
            }
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Renders meta element for Google site verifcation .
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="key">The google verify key</param>
        /// <returns>A meta element for google site verification.</returns>
        public static MvcHtmlString GoogleSiteVerificationMetaTag(this HtmlHelper helper, string key = "")
        {
            if (string.IsNullOrEmpty(key))
                key = WebConfigurationManager.AppSettings["GoogleKey"];
            if (!string.IsNullOrEmpty(key))
            {
                var html = new TagBuilder("meta");
                html.MergeAttribute("name", "google-site-verification");
                html.MergeAttribute("content", key);
                return MvcHtmlString.Create(html.ToString(TagRenderMode.SelfClosing));
            }
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Renders meta element for bing verification.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="key">The bing app id</param>
        /// <returns>A meta element for bing site verification.</returns>
        public static MvcHtmlString BingSiteVerificationMetaTag(this HtmlHelper helper, string key = "")
        {
            if (string.IsNullOrEmpty(key))
                key = WebConfigurationManager.AppSettings["BingAppID"];
            if (!string.IsNullOrEmpty(key))
            {
                var html = new TagBuilder("meta");
                html.MergeAttribute("name", "msvalidate.01");
                html.MergeAttribute("content", key);
                return MvcHtmlString.Create(html.ToString(TagRenderMode.SelfClosing));
            }
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Render META HTML elements for apple-icon
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <returns></returns>
        public static MvcHtmlString AppleIcons(this HtmlHelper helper)
        {
            var defaultIcon = helper.ViewContext.HttpContext.Server.MapPath("~/apple-touch-icon.png");
            //var iphone_icon = helper.ViewContext.HttpContext.Server.MapPath("~/apple-touch-icon-57x57.png");
            var ipad_icon = helper.ViewContext.HttpContext.Server.MapPath("~/apple-touch-icon-72x72.png");
            var iphone4_icon = helper.ViewContext.HttpContext.Server.MapPath("~/apple-touch-icon-114x114.png");
            var sb = new StringBuilder();
            //var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;

            if (System.IO.File.Exists(defaultIcon))
                sb.AppendFormat("<link rel=\"apple-touch-icon\" href=\"{0}\" />", "apple-touch-icon.png");

            if (System.IO.File.Exists(ipad_icon))
                sb.AppendFormat("<link rel=\"apple-touch-icon\" sizes=\"72x72\" href=\"{0}\" />", "apple-touch-icon-72x72.png");

            if (System.IO.File.Exists(iphone4_icon))
                sb.AppendFormat("<link rel=\"apple-touch-icon\" sizes=\"114x114\" href=\"{0}\" />", "apple-touch-icon-114x114.png");

            var _result = sb.ToString();

            if (!string.IsNullOrEmpty(_result))
                return MvcHtmlString.Create(_result);

            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Render the link tag for css stylesheet
        /// </summary>
        /// <param name="helper">The Html Helper</param>
        /// <param name="url">the url of the stylesheet</param>
        /// <returns></returns>
        public static MvcHtmlString CssLink(this HtmlHelper helper, string url)
        {
            return MvcHtmlString.Create(CssLink(url));
        }

        public static MvcHtmlString UserLink(this HtmlHelper helper, string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = AppModel.Get().Users[userName];
                var url = new UrlHelper(helper.ViewContext.RequestContext);
                var dispName = user.DefaultProfile.DisplayName;
                if (string.IsNullOrEmpty(dispName))
                    dispName = user.UserName;
                return MvcHtmlString.Create(string.Format("<a href=\"{0}\" role=\"link\" data-icon-left=\"d-icon-user\">{1}</a>", url.Content("~/profiles/" + userName), dispName));
            }
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Get html attribute for Html preload feature.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <returns></returns>
        public static string Preload(this HtmlHelper helper)
        {
            if (!string.IsNullOrEmpty(helper.ViewContext.RequestContext.HttpContext.Request.Browser.Browser) && helper.ViewContext.RequestContext.HttpContext.Request.Browser.Browser.Equals("chrome", StringComparison.OrdinalIgnoreCase))
                return "rel=\"preload\"";
            else
                return "rel=\"prefetch\"";
        }

        public static string Link(string rel, string type, string href)
        {
            return Link("", rel, type, href);
        }

        public static string Link(string title, string rel, string type, string href)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<link")
                      .Append(" rel=\"" + rel + "\"")
                      .Append(" type=\"" + type + "\"");
            if (!string.IsNullOrEmpty(title))
                builder.Append(" title=\"" + title + "\"");
            string _url = href;

            if (VirtualPathUtility.IsAppRelative(href))
                _url = VirtualPathUtility.ToAbsolute(href);
            builder.Append(" href=\"" + _url + "\" />");
            return builder.ToString();
        }

        public static string CssLink(string url)
        {
            return Link("stylesheet", "text/css", url);
        }

        /// <summary>
        /// Build the script reference link element 
        /// </summary>
        /// <remarks>
        ///  This method will append the "~/Scripts/" to the script link url.
        /// </remarks>
        /// <param name="urls">the script url reference params list</param>
        /// <returns></returns>
        public static MvcHtmlString ScriptRefs(this HtmlHelper helper, bool isInternal, params string[] urls)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < urls.Length; i++)
                builder.AppendLine(ScriptRef(helper, urls[i], isInternal));
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString ScriptRefs(this HtmlHelper helper, params string[] urls)
        {
            return helper.ScriptRefs(true, urls);
        }

        /// <summary>
        /// Build the script reference link element 
        /// </summary>
        /// <remarks>This method will append the "~/Scripts/" to the script link url.</remarks>
        /// <param name="helper">The html helper object to extends.</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ScriptRef(this HtmlHelper helper, string url, bool isInternal = true)
        {
            string _url = url;
            TagBuilder builder = new TagBuilder("script");
            builder.Attributes.Add("type", "text/javascript");

            if (isInternal)
            {
                if (!_url.StartsWith("~/Scripts/"))
                    _url = "~/Scripts/" + url;
            }

            if (VirtualPathUtility.IsAppRelative(_url))
                _url = VirtualPathUtility.ToAbsolute(_url);
            builder.Attributes.Add("src", _url);

            //Check if the script is already register.
            if (ScriptIsRegistered(helper, _url))
                return "";

            return builder.ToString();
        }

        public static bool ScriptIsRegistered(HtmlHelper helper, string _url)
        {
            List<string> srl = null;
            if (helper.ViewContext.TempData["ScriptRegisterList"] == null)
            {
                srl = new List<string>();
                helper.ViewContext.TempData["ScriptRegisterList"] = srl;
                return false;
            }

            srl = (List<string>)helper.ViewContext.TempData["ScriptRegisterList"];
            return srl.Contains(_url);
        }

        /// <summary>
        /// Enable the client validation for jQuery
        /// </summary>
        /// <param name="helper"></param>
        [Obsolete("This method is not use anymore", true)]
        public static void EnableClientjQueryValidation(this HtmlHelper helper)
        {
            helper.EnableClientValidation();
            helper.RegisterStartupScript("EnableClientValidation(mvcClientValidationMetadata[0]);");
        }

        public static MvcHtmlString Logo(this HtmlHelper helper)
        {
            return Logo(helper, null);
        }

        /// <summary>
        /// Render the logo image link of the portal
        /// </summary>
        /// <param name="helper">The Html Helper</param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString Logo(this HtmlHelper helper, object htmlAttributes)
        {
            //var web = Context.Web;
            // WebSite.Open(helper.ViewContext.RouteData);
            //if (web == null)
            //web = Context.RootWeb;
            // WebSite.Open();
            var web = App.Get().CurrentWeb;
            if (!string.IsNullOrEmpty(web.LogoImageUrl))
            {
                TagBuilder builder = new TagBuilder("img");

                if (htmlAttributes != null)
                    builder.MergeAttributes<string, object>(ObjectHelper.ConvertObjectToDictionary(htmlAttributes));
                builder.Attributes.Add("alt", web.Title);
                builder.Attributes.Add("src", (new UrlHelper(helper.ViewContext.RequestContext)).Content(web.LogoImageUrl));
                builder.AddCssClass("dna-logo");
                return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
            }
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Render the copyright information of the portal.
        /// </summary>
        /// <param name="helper">The Html helper</param>
        /// <returns></returns>
        public static MvcHtmlString Copyright(this HtmlHelper helper)
        {
            return Copyright(helper, null);
        }

        public static MvcHtmlString Copyright(this HtmlHelper helper, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("div");
            if (htmlAttributes != null)
                builder.MergeAttributes<string, object>(ObjectHelper.ConvertObjectToDictionary(htmlAttributes));
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            builder.InnerHtml += "<small itemprop=\"copyrightHolder\" style=\"text-align:center;padding:10px;\">Proudly powered by <a href=\"http://www.dotnetage.com\" title=\"Click to visit DotNetAge offical site.\" class=\"d-link\" target=\"_blank\">DotNetAge v " + version.ToString() + "</a></small>";
            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// Cut the input text
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="text"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Cut(this HtmlHelper helper, string text, int len)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if ((len > 0) && (text.Length > len))
                return text.Substring(0, len) + "...";
            else
                return text;
        }

        public static string ResolveUrl(string url)
        {
            string _url = url;
            if (VirtualPathUtility.IsAppRelative(_url))
                _url = VirtualPathUtility.ToAbsolute(_url);
            return _url;
        }

        public static string ScriptBlock(this HtmlHelper helper, string script)
        {
            StringBuilder scripts = new StringBuilder();
            scripts.AppendLine("<script type='text/javascript'>");
            scripts.Append(script);
            //scripts.AppendLine("});");
            scripts.AppendLine("</script>");
            return scripts.ToString();
        }

        public static MvcHtmlString StartupScripts(this HtmlHelper helper)
        {
            StringBuilder resScripts = new StringBuilder();
            if (helper.ViewContext.TempData["ClientRes"] != null)
            {
                IDictionary<string, string> resources = helper.ViewContext.TempData["ClientRes"] as IDictionary<string, string>;
                if (resources != null)
                {

                    List<string> options = new List<string>();

                    foreach (string key in resources.Keys)
                        options.Add(key + ":'" + resources[key] + "'");

                    resScripts.Append("$.extend(portal.res,")
                                .Append("{" + string.Join(",", options.ToArray()) + "}")
                                .Append(");");
                    helper.ViewContext.TempData.Remove("ClientRes");
                }
            }

            if (helper.ViewContext.TempData["StartupScripts"] != null)
            {
                StringBuilder scripts = new StringBuilder();
                scripts.AppendLine("<script type='text/javascript'>");
                scripts.AppendLine("$(function(){");
                if (resScripts.Length > 0)
                    scripts.Append(resScripts.ToString());
                scripts.Append(helper.ViewContext.TempData["StartupScripts"].ToString());
                scripts.AppendLine("});");
                scripts.AppendLine("</script>");
                helper.ViewContext.TempData.Remove("StartupScripts");
                return MvcHtmlString.Create(scripts.ToString());
            }
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Register the startup script to the context then using StartupScripts method the render all registed scripts.
        /// </summary>
        /// <param name="helper">The helper object</param>
        /// <param name="script">The script to register</param>
        /// <remarks>
        /// This method will wrapp the try{}catch(){} block of the specified script.
        /// </remarks>
        public static void RegisterStartupScript(this HtmlHelper helper, string script)
        {
            //string securityScript = "try{" + script + "}catch(e){uiHelper.showMsg('Client script error',e);}";
            string securityScript = script;// "try{" + script + "}catch(e){alert(e.description );}";
            if (helper.ViewContext.TempData["StartupScripts"] != null)
                helper.ViewContext.TempData["StartupScripts"] += "\r\n" + securityScript;
            else
                helper.ViewContext.TempData["StartupScripts"] = securityScript;
        }

        ///// <summary>
        ///// Render OAuth login buttons 
        ///// </summary>
        ///// <param name="helper">The html helper object.</param>
        ///// <returns></returns>
        //public static HelperResult OAuthLogin(this HtmlHelper helper)
        //{
        //    return new HelperResult((writer) =>
        //    {
        //        var providers = App.Get().OAuthProviders;
        //        var list = new XElement("ul", new XAttribute("class", "d-items"), new XAttribute("style", "display:inline-block"));
        //        var _format = "Login with {0}";

        //        var _tmp = App.GetResourceString("Security", "LoginWith_Format");
        //        if (!string.IsNullOrEmpty(_tmp))
        //            _format = _tmp;

        //        foreach (OAuth.OAuthProviderBase p in providers)
        //        {
        //            if (p.IsAvailabled)
        //            {
        //                var item = new XElement("li", new XAttribute("class", "d-item d-float-left"));
        //                var link = new XElement("a",
        //                    new XAttribute("title", string.Format(_format, p.Name)),
        //                    new XAttribute("data-role", "oauthlogin"),
        //                    new XAttribute("style", "display:inline-block;cursor:pointer;"),
        //                    new XAttribute("data-provider", p.Name),
        //                    new XAttribute("data-profile-endpoint", p.UserInfoEndPoint),
        //                    new XAttribute("data-version", p.Version));

        //                if (p.Version.Equals("2.0", StringComparison.OrdinalIgnoreCase))
        //                    link.Add(new XAttribute("data-scope", (((DNA.OAuth.OAuth2Provider)p).Scopes).ToString()));

        //                if (!string.IsNullOrEmpty(p.Icon))
        //                {
        //                    if (p.Icon.StartsWith("d-icon", StringComparison.OrdinalIgnoreCase))
        //                        link.Add(new XElement("span", new XAttribute("data-icon", p.Icon), new XAttribute("data-icon-size", "large")));
        //                    else
        //                        link.Add(new XElement("img", new XAttribute("alt", p.Name), new XAttribute("src", p.Icon)));
        //                }

        //                item.Add(link);
        //                list.Add(item);
        //            }
        //        }

        //        if (list.HasElements)
        //            writer.Write(list.OuterXml());
        //    });
        //}

        public static MvcHtmlString Pager(this HtmlHelper helper, int totalRecords, string baseUrl, int index = 1, int size = 50)
        {

            var pagecount = 0;
            pagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords) / Convert.ToDecimal(50)));
            var request = helper.ViewContext.RequestContext.HttpContext.Request;
            if (index <= 1)
            {
                if (request.QueryString["index"] != null)
                {
                    if (Int32.TryParse(request.QueryString["index"], out index))
                    {
                        index++;
                    }
                }
            }

            var s = size;
            if (request.QueryString["size"] != null)
            {
                if (!Int32.TryParse(request.QueryString["size"], out s))
                {
                    s = size;
                }
            }

            var script = string.Format("location='{0}?index='+(ui-1)+'&size={1}';", baseUrl, s);

            if (pagecount > 1)
            {
                var tag = new TagBuilder("div");
                tag.MergeAttribute("data-role", "pager");
                tag.MergeAttribute("data-pages", pagecount.ToString());
                tag.MergeAttribute("data-index", index.ToString());
                tag.MergeAttribute("data-size", s.ToString());
                tag.MergeAttribute("data-change", script);
                return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
            }

            return MvcHtmlString.Empty;
        }
    }
}
