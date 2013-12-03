//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Represent the widget data class use to widget registration.
    /// </summary>
    public class WidgetData
    {
        /// <summary>
        /// Gets/Sets the widget name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the widge title text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the widget description text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets the widget category.
        /// </summary>
        public string Category { get; set; }

        public Type ModuleType { get; set; }

        public string ViewName { get; set; }

        /// <summary>
        /// Gets/Sets the widget user preference definitions.
        /// </summary>
        public IDictionary<string, object> Preferences { get; set; }
    }
}
