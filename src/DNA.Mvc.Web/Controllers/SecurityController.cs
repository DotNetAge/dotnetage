//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Security;
using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    /// <summary>
    /// The controller manages user membership in roles for authorization checking in an ASP.NET application. 
    /// </summary>
    public class SecurityController : Controller
    {
        public ActionResult Index()
        {
            return View("_RoleForm");
        }

        //[HostDashboard("Network accounts",
        //    Group = "Security",
        //    RouteName = "dna_host_oauth",
        //    Icon = "d-icon-sharable",
        //    ResBaseName = "Managements",
        //    GroupResKey = "Security",
        //    ResKey = "Networkaccounts")]
        //public ActionResult OAuthManager()
        //{
        //    return View();
        //}

        /// <summary>
        /// Get the role list view of the Portal
        /// </summary>
        /// <returns></returns>
        [HostDashboard("Users",
            Group = "Security",
            Sequence = 2,
            RouteName = "dna_host_users",
            Icon = "d-icon-user-3",
            ResBaseName = "Managements",
            GroupResKey = "Security",
            ResKey = "Users")]
        public ActionResult ManageUsers()
        {
            return View();
        }

        [HostDashboard("Roles",
            Sequence = 1,
            Group = "Security",
            RouteName = "dna_host_roles",
            Icon = "d-icon-group",
            ResBaseName = "Managements",
            GroupResKey = "Security",
            ResKey = "Roles")]
        public ActionResult ManageRoles()
        {
            //ViewData.Model = Roles.GetAllRoles();
            return PartialView();
        }

        /// <summary>
        /// Adds a new role to the data source.
        /// </summary>
        /// <param name="NewRoleName">The name of the role to create.</param>
        /// <returns>if create role successful</returns>
        [HttpPost, HostOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string roleName, string description)
        {
            if (!string.IsNullOrEmpty(roleName))
                try
                {
                    App.Get().Roles.Create(roleName);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("roleName", e);
                    return Redirect("~/host/roles");
                }
            else
                ModelState.AddModelError("NewRoleName", String.Format(Resources.Validations.Required_Format, Resources.Commons.Name));

            return Redirect("~/host/roles?n=" + roleName);
        }

        /// <summary>
        /// Removes a role from the data source.
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <returns>The ActionResult.</returns>
        [HttpPost, HostOnly]
        public ActionResult Delete(string roleName)
        {
            App.Get().Roles.Delete(Server.UrlDecode(roleName));
            return RedirectToAction("ManageRoles");
        }

        public ActionResult ValidateRole(string roleName)
        {
            var roles = App.Get().Roles.All;
            if (roles.Contains(roleName))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds the specified user to the specified role.
        /// </summary>
        /// <param name="user"> The user name to add to the specified role.</param>
        /// <param name="roleName"> The role to add the specified user name to.</param>
        /// <returns>If successful return true.</returns>
        [HttpPost, HostOnly]
        public bool AddUser(string user, string roleName)
        {
            App.Get().Roles.AddUserToRole(user, roleName);
            return true;
        }

        /// <summary>
        /// Removes the specified user from the specified role.
        /// </summary>
        /// <param name="user">The user to remove from the specified role.</param>
        /// <param name="roleName">The role to remove the specified user from.</param>
        /// <returns>If successful return true.</returns>
        [HttpPost, HostOnly]
        public bool RemoveUser(string user, string roleName)
        {
            App.Get().Roles.RemoveUserFromRole(user, roleName);
            return true;
        }

        /// <summary>
        /// Get the permissions view for the specified role
        /// </summary>
        /// <param name="role">the name of the role to list</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult RoleDetail(string role)
        {
            ViewData["Role"] = role;
            return PartialView();
        }

        //[HttpPost]
        //[SecurityAction("Security", "Manage the permissions", "Allows users to manage the permissions of the role.",
        //    TitleResName = "SA_ManagePermissions",
        //    DescResName = "SA_ManagePermissions_Desc",
        //    PermssionSetResName = "SA_SECURITY"
        //    )]
        //[ValidateAntiForgeryToken]
        //public ActionResult Apply(string roleName, FormCollection forms)
        //{
        //    List<int> keys = new List<int>();
        //    foreach (string key in forms.Keys)
        //    {
        //        var selected = false;
        //        bool.TryParse(forms[key], out selected);
        //        if (!selected) continue;

        //        if (key.StartsWith("perm_"))
        //        {
        //            int k = int.MinValue;
        //            if (int.TryParse(key.Substring(5), out k))
        //                keys.Add(k);
        //        }
        //    }
        //    var perms = _context.DataContext.Permissions.GetPermissions(keys.ToArray());
        //    _context.DataContext.Permissions.ClearFromRole(roleName);
        //    _context.DataContext.Permissions.AddPermissionsToRole(perms, roleName);
        //    _context.DataContext.SaveChanges();
        //    return RedirectToAction("ManageRoles");
        //}

        //[Authorize]
        //public ActionResult UserDetail(string id)
        //{
        //    if (_context.IsAuthorized(this, "ManageUsers"))
        //    {
        //        return PartialView(Membership.GetUser(id));
        //    }
        //    else
        //        return Content(Resources.language.AccessDenied_MSG);
        //}

        [Authorize]
        public void UpdateUserRoles(string username, string roles)
        {
            if (App.Get().Context.HasPermisson(this, "ManageUsers"))
            {
                if (string.IsNullOrEmpty(username))
                    throw new ArgumentNullException("username");

                if (string.IsNullOrEmpty(roles))
                    throw new ArgumentNullException("roles");

                App.Get().Users[username].ClearRoles().AddToRoles(roles.Split(','));
            }
        }

        [HostOnly]
        public void Bend(string username)
        {
            //if (_context.IsAuthorized(this, "ManageUsers"))
            //{
            //    var user = Membership.GetUser(username);
            //    user.IsApproved = false;
            //    Membership.UpdateUser(user);
            //}
            throw new NotImplementedException();
        }

        [HostOnly]
        public void Unlock(string username)
        {
            throw new NotImplementedException();
            //if (_context.IsAuthorized(this, "ManageUsers"))
            //{
            //    var user = Membership.GetUser(username);
            //    user.IsApproved = true;
            //    Membership.UpdateUser(user);
            //    user.UnlockUser();
            //}
        }

        [HttpGet, HostOnly]
        public ActionResult UserRoles(string name, string locale)
        {
            App.Get().SetCulture(locale);
            var user = App.Get().Users[name];
            return PartialView(user);
        }

        [HttpPost, HostOnly]
        public ActionResult UserRoles(string name, string[] roles)
        {
            var user = App.Get().Users[name];
            user.ClearRoles();
            var _roles = new List<string>();
            if (!object.ReferenceEquals(roles, null))
                _roles.AddRange(roles);

            if (!_roles.Contains("guests"))
                _roles.Add("guests");

            user.AddToRoles(_roles.ToArray());
            return PartialView(user);
        }

        [HttpPost, HostOnly]
        public ActionResult RefreshPerms()
        {
            PermissionLoader.Load();
            return new HttpStatusCodeResult(200);
            //return Redirect("~/host/roles");
        }

        [HostOnly, HostDashboard]
        public ActionResult CreateUser() { return View(); }

        [HttpPost, HostOnly]
        public ActionResult CreateUser(RegisterModel model, string[] roles)
        {
            if (ModelState.IsValid)
            {
                string[] reserves = App.Settings.ReservedUserNames;
                if (reserves.Contains(model.UserName.ToLower()))
                {
                    ModelState.AddModelError("", string.Format(Resources.Validations.UserName_Reserved, "\"" + model.UserName + "\""));
                }
                else
                {
                    var context = DependencyResolver.Current.GetService<IDataContext>();
                    var status = context.Users.CreateUser(model.UserName, model.Password, model.Email);
                    if (status != UserCreateStatus.Success)
                        ModelState.AddModelError("", AccountValidation.ErrorCodeToString(status));
                    else
                    {
                        if (roles != null && roles.Length > 0)
                        {
                            var rl = roles.ToList();
                            rl.Remove("guests");
                            var user = App.Get().Users[model.UserName];
                            user.AddToRoles(roles);
                        }

                        return Redirect("~/host/users");
                    }
                }
            }
            return View();
        }
    }
}
