//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Represents a permission set class use to grouping the permissions.
    /// </summary>
    public class PermissionSet
    {
        /// <summary>
        /// Initializes a new instance of the PermissionSet class.
        /// </summary>
        public PermissionSet() { }

        /// <summary>
        /// Initializes a new instance of the PermissionSet class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        public PermissionSet(string name, string title="")
        {
            Name = name;
            Title = title;
            if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(title)) Title = name;
        }

        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the permissionset name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the display text
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the description (Not use now.)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets permissions
        /// </summary>
        public virtual ICollection<Permission> Permissions { get; set; }

        /// <summary>
        /// Gets/Setst the resource class name that use to globalize the permission title. 
        /// </summary>
        public string ResbaseName { get; set; }
        
        /// <summary>
        /// Gets/Sets the title resource key name. If the resource not found in the resource file DotNetAge will use title instead it.
        /// </summary>
        public string TitleResName { get; set; }
    }
}
