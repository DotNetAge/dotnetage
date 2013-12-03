//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;

namespace DNA.Web.UI
{
    /// <summary>
    /// Define helper methods to render widget components.
    /// </summary>
    public static class WidgetExtensions
    {
        /// <summary>
        /// Create a WidgetViewBuilder
        /// </summary>
        /// <param name="html">The html helper.</param>
        /// <returns>The WidgetViewBuilder instance.</returns>
        public static WidgetViewBuilder Widget(this HtmlHelper html)
        {
            var httpContext = html.ViewContext.HttpContext;
            return new WidgetViewBuilder(new WidgetView()
            {
                Context = httpContext,
                Html = html
            }, httpContext);
        }

        /// <summary>
        /// Create a WidgetZoneBuilder 
        /// </summary>
        /// <param name="html">The thml helper.</param>
        /// <param name="name">The WidgetZone name (html element id).</param>
        /// <param name="title">The WidgetZone title.</param>
        /// <returns>The WidgetZoneBuilder instance.</returns>
        public static WidgetZoneBuilder Zone(this HtmlHelper html, string name, string title = "")
        {
            return new WidgetZoneBuilder(new WidgetZone()
            {
                Title = title,
                Name = name
            }, html.ViewContext.HttpContext) { Html = html }.GenerateId();
        }
    }

}