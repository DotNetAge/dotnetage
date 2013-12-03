//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;

namespace DNA.Web.UI
{
    /// <summary>
    /// Represent a component render the widgets on it.
    /// </summary>
    public class WidgetZone : ContainerViewComponent<Widget>
    {
        /// <summary>
        /// Gets/Sets the zone title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets widget intances contains in this zone.
        /// </summary>
        public IList<Widget> Widgets
        {
            get { return InnerItems; }
            set { InnerItems = value; }
        }

        /// <summary>
        /// Gets the inline content template
        /// </summary>
        public Func<object, object> InlineContent { get; set; }

        /// <summary>
        /// Gets the content template
        /// </summary>
        public Action Content { get; set; }

        public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(CssClass))
                this.CssClass = "d-widget-zone " + this.CssClass;
            else
                this.CssClass = "d-widget-zone";
            this.DataAttributes.Add("role", "widgetzone");
            if (!string.IsNullOrEmpty(Title))
                this.DataAttributes.Add("label", Title);
            base.RenderBeginTag(writer);
        }

        public override void RenderContent(System.Web.UI.HtmlTextWriter writer)
        {
            if (Content != null)
                Content.Invoke();
            else
            {
                if (InlineContent != null)
                    writer.Write(InlineContent(null).ToString());
            }

            foreach (var widget in Widgets)
            {
                try
                {
                    if (widget.Model.IsExpanded)
                    {
                        widget.Render(writer);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}