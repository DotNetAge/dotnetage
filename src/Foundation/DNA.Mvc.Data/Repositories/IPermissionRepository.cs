//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using System;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Define the methods to manipulate Permission data model.
    /// </summary>
    public interface IPermissionRepository :IRepository<Permission>
    {
        /// <summary>
        /// Get the permission by specified controller type and action name.
        /// </summary>
        /// <param name="controllerType">The specified controller type.</param>
        /// <param name="action">The action name of the specified controller.</param>
        /// <returns>Returns the permission object instance</returns>
        Permission GetPermission(Type controllerType, string action);

        /// <summary>
        /// Get the permissions by specified ids.
        /// </summary>
        /// <param name="ids">the specified array of permission id.</param>
        /// <returns>Returns permission object collection that match ids.</returns>
        IEnumerable<Permission> GetPermissions(int[] ids);

        /// <summary>
        /// Get the permissions for the specified role.
        /// </summary>
        /// <param name="roleName">The specified role name</param>
        /// <returns>Returns permission objects</returns>
        IEnumerable<Permission> GetRolePermissions(string roleName);

        /// <summary>
        /// Get permissons by specified user name.
        /// </summary>
        /// <param name="userName">Specified the userName.</param>
        /// <returns>Returns permission objects</returns>
        IEnumerable<Permission> GetUserPermissions(string userName);

        /// <summary>
        /// Add the permissions to the specified role name.
        /// </summary>
        /// <param name="perms">The permissions to add.</param>
        /// <param name="roleName">The specified role name</param>
        void AddPermissionsToRole(IEnumerable<Permission> perms, string roleName);

        /// <summary>
        /// Add permission object to specified role name.
        /// </summary>
        /// <param name="perm">The permission object.</param>
        /// <param name="roleName">The role name.</param>
        void AddPermissionToRole(Permission perm, string roleName);

        /// <summary>
        /// Remove the permission from specified role name.
        /// </summary>
        /// <param name="perm">The permission object.</param>
        /// <param name="roleName">The role name.</param>
        void RemovePermissionFromRole(Permission perm, string roleName);

        /// <summary>
        /// Clear the permissions from role
        /// </summary>
        /// <param name="roleName">The role name.</param>
        void ClearFromRole(string roleName);

        /// <summary>
        ///  Check the permission whether in the specified role name.
        /// </summary>
        /// <param name="perm">The permission to be checked</param>
        /// <param name="roleName">The specified role name.</param>
        /// <returns>Returns true when successful.</returns>
        bool IsAuthorized(Permission perm, string roleName);

        /// <summary>
        /// Check the permission whether in the specified roles.
        /// </summary>
        /// <param name="perm">The permission to be checked</param>
        /// <param name="roles">The role names.</param>
        /// <returns>Returns true when successful</returns>
        bool IsAuthorized(Permission perm, string[] roles);
    }
}
