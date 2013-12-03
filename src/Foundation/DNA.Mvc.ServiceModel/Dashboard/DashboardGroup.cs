//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Represents the group object that use to group the dashboard menu items.
    /// </summary>
    public class DashboardGroup
    {
        private List<DashboardItem> items = new List<DashboardItem>();

        /// <summary>
        /// Gets/Sets the dashboard name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the dashboard title text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the Dashboard menu items.
        /// </summary>
        public List<DashboardItem> Items
        {
            get { return items; }
        }

    }
}
