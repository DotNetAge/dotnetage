//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a role manager that use to manage user roles.
    /// </summary>
    public class RoleManager
    {
        private string[] roles = null;

        private IDataContext DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the RoleManager with data context.
        /// </summary>
        /// <param name="context">The data context object.</param>
        public RoleManager(IDataContext context)
        {
            this.DataContext = context;
        }

        /// <summary>
        /// Gets all roles name
        /// </summary>
        public string[] All
        {
            get
            {
                if (roles == null)
                    roles = DataContext.All<Role>().Select(r => r.Name).ToArray();
                return roles;
            }
        }

        /// <summary>
        /// Find role by specified name.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <returns>A role object that match specified name.</returns>
        public Role Find(string name)
        {
            return DataContext.Find<Role>(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        /// <returns>A queryable role collection.</returns>
        public IQueryable<Role> GetAllRoles()
        {
            return DataContext.All<Role>();
        }

        /// <summary>
        /// Create a new role by specified role name and description.
        /// </summary>
        /// <param name="role">The role name.</param>
        /// <param name="desc">The role description.l</param>
        /// <returns></returns>
        public bool Create(string role, string desc = "")
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException("role");

            if (DataContext.Count<Role>(n => n.Name.Equals(role)) > 0)
                throw new Exception("The \"" + role + "\" role is exists.");

            DataContext.Add<Role>(new Role() { Name = role, Description = desc });
            return DataContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Removes a role from the data source.
        /// </summary>
        /// <param name="role">The name of the role to delete.</param>
        /// <returns>true if role was deleted from the data source; otherwise, false.</returns>
        public bool Delete(string role)
        {
            var undeleteable = new string[] { "administrators", "guests" };
            if (undeleteable.Contains(role))
                throw new Exception("Can not delete the system role : " + role);
            DataContext.Delete<Role>(r => r.Name.Equals(role));
            return DataContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>true if the role name already exists in the data source; otherwise, false.</returns>
        public bool Exists(string roleName)
        {
            return All.Contains(roleName);
        }

        /// <summary>
        /// Gets role names by specified user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>An array object that contains role names.</returns>
        public string[] GetUserRoles(string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");
            var user = DataContext.Find<User>(u => u.UserName.Equals(userName));
            return user.Roles.Select(r => r.Name).ToArray();
            //return DataContext.Where<UsersInRoles>(u => u.UserName.Equals(userName)).Select(r => r.RoleName).ToArray(); 
        }

        /// <summary>
        /// Adds the specified user to the specified roles.
        /// </summary>
        /// <param name="userName"> The user name to add to the specified roles.</param>
        /// <param name="roles">A string array of roles to add the specified user name to.</param>
        public void AddUserToRoles(string userName, string[] roles)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (roles == null)
                throw new ArgumentNullException("roles");

            var user = DataContext.Find<User>(u => u.UserName.Equals(userName));

            if (user != null)
                throw new UserNotFoundException();

            var _roles = DataContext.Where<Role>(r => roles.Contains(r.Name,StringComparer.OrdinalIgnoreCase)).ToList();
            var userRoles = user.Roles.Select(u=>u.Name).ToArray();
            foreach (var r in _roles)
            {
                if (!userRoles.Contains(r.Name))
                    user.Roles.Add(r);
            }

            //user.Roles.Clear();
            //foreach (var r in roles)
            //{
            //    if (!All.Contains(r))
            //        throw new Exception("The role \"" + r + "\" not found. Can not add user to unexists role!");

            //    if (DataContext.Find<UsersInRoles>(u => u.UserName.Equals(userName) && u.RoleName.Equals(r)) != null)
            //        throw new Exception("Can not add the user \"" + userName + "\" already in role \"" + r + "\"");

            //    DataContext.Add<UsersInRoles>(new UsersInRoles()
            //    {
            //        RoleName = r,
            //        UserName = userName
            //    });
            //}


            DataContext.SaveChanges();
        }

        /// <summary>
        /// Add user to specified role.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="role">The role name.</param>
        public void AddUserToRole(string userName, string role)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException("role");

            var user = DataContext.Find<User>(u => u.UserName.Equals(userName));

            if (user == null)
                throw new UserNotFoundException();

            //var namingRoles=new string[]{"administrators","guests"};

            if (user.Roles.FirstOrDefault(r => r.Name.Equals(role, StringComparison.OrdinalIgnoreCase)) != null)
                throw new Exception(string.Format("The user already in {0} role", role));


            var _role = DataContext.Find<Role>(r => r.Name.Equals(role, StringComparison.OrdinalIgnoreCase));

            if (_role == null)
                throw new Exception(string.Format("{0} role is not found.", role));
            user.Roles.Add(_role);

            //foreach (var r in roles)
            //user.Roles.Add(r);
            //context.SaveChanges();
            //var userInRole = DataContext.Find<UsersInRoles>(u => u.UserName.Equals(userName) && u.RoleName.Equals(role));
            //if (userInRole != null)
            //    throw new Exception("Can not add the user \"" + userName + "\" already in role \"" + role + "\"");
            //DataContext.Add<UsersInRoles>(new UsersInRoles()
            //{
            //    RoleName = role,
            //    UserName = userName
            //});
            DataContext.SaveChanges();
        }

        /// <summary>
        /// Removes the specified user from the specified role.
        /// </summary>
        /// <param name="userName">The user to remove from the specified role</param>
        /// <param name="role">The role to remove the specified user from</param>
        public void RemoveUserFromRole(string userName, string role)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException("role");

            var user = DataContext.Find<User>(u => u.UserName.Equals(userName));

            if (user == null)
                throw new UserNotFoundException();
            var _role = user.Roles.FirstOrDefault(r => r.Name.Equals(role));
            if (_role == null)
                throw new Exception(string.Format("{0} role not found", role));
            user.Roles.Remove(_role);
            DataContext.SaveChanges();

            //var userInRole = DataContext.Find<UsersInRoles>(u => u.UserName.Equals(userName) && u.RoleName.Equals(role));
            //if (userInRole == null)
            //    throw new Exception("Can not remove the user \"" + userName + "\" from an unexists role \"" + role + "\"");
            //throw new NotImplementedException();
            //DataContext.Delete(userInRole);
            //DataContext.SaveChanges();

        }

        /// <summary>
        /// Remove the specified user from specified roles.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="roles">The role name array.</param>
        public void RemoveUserFromRoles(string userName, string[] roles)
        {
            foreach (var r in roles)
                this.RemoveUserFromRole(userName, r);
        }

        /// <summary>
        /// Get users by specified role name.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>A user collection.</returns>
        public IEnumerable<UserDecorator> GetUsersInRole(string roleName)
        {
            if (!Exists(roleName))
                throw new Exception("Role \"" + roleName + "\" not exists.");

            var userNames = DataContext.Find<Role>(r => r.Name.Equals(roleName)).Users.Select(u => u.UserName);
            var users = new List<UserDecorator>();
            if (userNames != null && userNames.Count() > 0)
            {
                var _users = DataContext.Where<User>(u => userNames.Contains(u.UserName)).ToList();
                if (_users != null && _users.Count > 0)
                    users.AddRange(_users.Select(u => new UserDecorator(u, DataContext)).ToList());
            }

            return users;
        }

        /// <summary>
        /// Gets permissions by specified role name.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>A permission collection</returns>
        public IEnumerable<Permission> GetRolePermissons(string roleName)
        {
            return DataContext.Permissions.GetRolePermissions(roleName);
        }

        /// <summary>
        /// Indicates whether the specified role name that has specified permission.
        /// </summary>
        /// <param name="perm">The permission object.</param>
        /// <param name="roleName">The role name.</param>
        /// <returns>true if is authorized otherwrise false.</returns>
        public bool IsAuthorized(Permission perm, string roleName)
        {
            return DataContext.Permissions.IsAuthorized(perm, roleName);
        }

        /// <summary>
        /// Add the permissions to the specified role name.
        /// </summary>
        /// <param name="perms">The permissions to add.</param>
        /// <param name="roleName">The specified role name</param>
        public void AddPermissionsToRole(IEnumerable<Permission> perms, string roleName) { DataContext.Permissions.AddPermissionsToRole(perms, roleName); }

        /// <summary>
        /// Add permission to specified role name.
        /// </summary>
        /// <param name="perm">The permission object.</param>
        /// <param name="roleName">The role name.</param>
        public void AddPermissionToRole(Permission perm, string roleName) { DataContext.Permissions.AddPermissionToRole(perm, roleName); }

        /// <summary>
        /// Remove permission from specified role name.
        /// </summary>
        /// <param name="perm">The permission object.</param>
        /// <param name="roleName">The role name.</param>
        public void RemovePermissionFromRole(Permission perm, string roleName) { DataContext.Permissions.RemovePermissionFromRole(perm, roleName); }
    }
}
