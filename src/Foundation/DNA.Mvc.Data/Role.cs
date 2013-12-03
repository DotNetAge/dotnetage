//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Represents a role
    /// </summary>
    public class Role
    {
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets role name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets users under this role.
        /// </summary>
        public virtual ICollection<User> Users { get; set; }

        /// <summary>
        /// Gets/Sets lists under this role.
        /// </summary>
        public virtual ICollection<ContentList> Lists { get; set; }

        /// <summary>
        /// Gets/Sets descriptors under this role.
        /// </summary>
        public virtual ICollection<WidgetDescriptor> Descriptors { get; set; }

        /// <summary>
        /// Gets/Sets widgets under this role.
        /// </summary>
        public virtual ICollection<WidgetInstance> Widgets { get; set; }

        /// <summary>
        /// Gets/Sets pages under this page.
        /// </summary>
        public virtual ICollection<WebPage> Pages { get; set; }

        /// <summary>
        /// Gets/Sets permission unser this role.
        /// </summary>
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
