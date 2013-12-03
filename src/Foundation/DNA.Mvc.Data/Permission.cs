//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Represents a permission.
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Gets/Sets the permission id.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the permission title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the Action name that controlling by Permission.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets/Sets the assembly name of the controller class 
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// Gets/Sets the controller name that contains the Action.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets/Sets the permission description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets the permisson set id.
        /// </summary>
        public int PermissionSetID { get; set; }

        /// <summary>
        /// Gets/Sets the permissionSet 
        /// </summary>
        public virtual PermissionSet PermissionSet { get; set; }

        /// <summary>
        /// Gets/Sets the Role names that has this Permission
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }
    }
}
