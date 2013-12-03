//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Security;

namespace DNA.Web.Data.Entity
{
    public class PermissionRepository : EntityRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository() : base() { }

        public PermissionRepository(CoreDbContext dbContext) : base(dbContext) { }

        public Permission GetPermission(Type controllerType, string action)
        {
            if (controllerType == null)
                throw new ArgumentNullException("controllerType");

            if (string.IsNullOrEmpty(action))
                throw new ArgumentNullException("action");

            string asm = controllerType.Assembly.GetName().Name;
            return DbSet.FirstOrDefault(p => p.Action.Equals(action, StringComparison.OrdinalIgnoreCase) && p.Assembly.Equals(asm, StringComparison.OrdinalIgnoreCase) && p.Controller.Equals(controllerType.FullName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Permission> GetPermissions(int[] ids)
        {
            if (ids == null) throw new ArgumentNullException("ids");
            if (ids.Length == 0) throw new ArgumentOutOfRangeException("ids");
            string _cmd = "SELECT * FROM dna_permissions WHERE ID in (" + string.Join(",", ids) + ")";
            return DbSet.SqlQuery(_cmd).ToList();
        }

        public void AddPermissionsToRole(IEnumerable<Permission> perms, string roleName)
        {
            if (perms == null) throw new ArgumentNullException("perms");

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            var role = Context.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (role == null)
                throw new Exception(string.Format("{0} role not found", roleName));

            foreach (var perm in perms)
                //{
                role.Permissions.Add(perm);

            //    Context.PermissionRoles.Add(new PermissionRole()
            //    {
            //        Permission = perm,
            //        RoleName = roleName
            //    });
            //}

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public bool IsAuthorized(Permission perm, string roleName)
        {
            if (perm == null) throw new ArgumentNullException("perm");

            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException("roleName");

            if (roleName.ToLower() == "administrators")
                return true;

            if (perm.Roles != null)
                return perm.Roles.Count(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)) > 0;
            else
            {
                var role = context.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
                if (role != null)
                    return role.Permissions.Count(p => p.ID.Equals(perm.ID)) > 0;
                //return context.Roles.Count(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)) > 0;
            }
            //return Context.PermissionRoles.Count(p => p.PermID == perm.ID && p.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase)) > 0;
            return false;
        }

        public bool IsAuthorized(Permission perm, string[] roles)
        {
            if (perm == null) throw new ArgumentNullException("perm");
            if (roles == null || roles.Length == 0) throw new ArgumentNullException("roles");
            if (roles.Contains("administrators")) return true;

            string[] permRoles=null;

            if (perm.Roles != null)
                permRoles = perm.Roles.Select(r => r.Name).ToArray();
            else
                permRoles=context.Roles.Where(r=>roles.Contains(r.Name,StringComparer.OrdinalIgnoreCase)).Select(r=>r.Name).ToArray();
            
            if (permRoles!=null && permRoles.Count()>0)
            {
                foreach (var pr in permRoles)
                {
                    if (roles.Contains(pr))
                        return true;
                }
            }
            return false;
            //IList<string> sqlRoleStrs = new List<string>();
            //for (int i = 0; i < roles.Length; i++)
            //    sqlRoleStrs.Add(string.Format("N'{0}'", roles[i]));
            //string cmd = string.Format("SELECT * FROM dna_PermsInRoles WHERE PermID={0} AND RoleName in ({1})", perm.ID, string.Join(",", sqlRoleStrs.ToArray()));
            //return Context.PermissionRoles.SqlQuery(cmd).Count() > 0;
        }

        public IEnumerable<Permission> GetRolePermissions(string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException("roleName");
            string cmd = string.Format("SELECT * FROM dna_permissions WHERE ID in (SELECT PermID FROM dna_PermsInRoles WHERE RoleName=N'{0}' )", roleName);

            //if (IsNoTracking)
            //    return DbSet.SqlQuery(cmd).AsNoTracking().ToList();
            //else
            return DbSet.SqlQuery(cmd).ToList();
        }

        public IEnumerable<Permission> GetUserPermissions(string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");

            string[] roles = Roles.GetRolesForUser(userName);

            if (roles.Length == 0) return null;

            var formatArgs = new List<string>();
            foreach (var role in roles)
                formatArgs.Add(string.Format("N'{0}'", role));

            string cmd = string.Format(@"SELECT * FROM dna_permissions 
WHERE ID in (SELECT DISTINCT PermID 
FROM dna_PermsInRoles 
WHERE RoleName in ({0}))"
                , string.Join(",", formatArgs));

            //if (IsNoTracking)
            //    return DbSet.SqlQuery(cmd).AsNoTracking().ToList();
            //else
            return DbSet.SqlQuery(cmd).ToList();
        }

        public void ClearFromRole(string roleName)
        {
            Context.Database.ExecuteSqlCommand(string.Format("DELETE dna_PermsInRoles WHERE RoleName=N'{0}'", roleName));
            if (IsOwnContext)
                Context.SaveChanges();
        }

        public void AddPermissionToRole(Permission perm, string roleName)
        {
            if (perm == null)
                throw new ArgumentNullException("perm");

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            var role = Context.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (role == null)
                throw new Exception(string.Format("{0} role not found.", roleName));

            role.Permissions.Add(perm);

            //Context.PermissionRoles.Add(new PermissionRole()
            //{
            //    Permission = perm,
            //    RoleName = roleName
            //});

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public void RemovePermissionFromRole(Permission perm, string roleName)
        {
            //var pr = context.PermissionRoles.FirstOrDefault(p => p.RoleName.Equals(roleName) && p.PermID.Equals(perm.ID));
            //if (pr != null)
            //{
            //    Context.PermissionRoles.Remove(pr);

            //}
            if (perm == null) throw new ArgumentNullException("perm");

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            var role = Context.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (role == null)
                throw new Exception(string.Format("{0} role not found", roleName));

            role.Permissions.Remove(perm);

            if (IsOwnContext)
                Context.SaveChanges();

        }
    }
}
