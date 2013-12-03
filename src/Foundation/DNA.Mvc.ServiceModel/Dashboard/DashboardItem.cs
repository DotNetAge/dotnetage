//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Web.Routing;

namespace DNA.Web
{
    /// <summary>
    /// Define the Dashboard Item object.
    /// </summary>
    public class DashboardItem
    {
        /// <summary>
        /// Gets/Sets the dashboard item title text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the dashboard item navigation url.
        /// </summary>
        public string NavigationUrl { get; set; }
        
        /// <summary>
        /// Gets/Sets the dashboard item icon class name.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets/Sets the dashboard item image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets/Sets the dashboard item controller name.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets/Sets the dashboard item action name.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets/Sets the dashboard item area name.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Gets/Sets the dashboard item's sorting order of the group.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Identity this dashboard is current 
        /// </summary>
        /// <param name="context">The current request context.</param>
        /// <returns></returns>
        public bool IsCurrent(RequestContext context)
        {
            var values = context.RouteData.Values;
            string ctrl = (string)values["controller"];
            string act = (string)values["action"];
            string area = values["area"] == null ? "" : values["area"].ToString();
            return this.Area.Equals(area, StringComparison.OrdinalIgnoreCase) && this.Controller.Equals(ctrl, StringComparison.OrdinalIgnoreCase) && this.Action.Equals(act, StringComparison.OrdinalIgnoreCase);
        }
    }
}
