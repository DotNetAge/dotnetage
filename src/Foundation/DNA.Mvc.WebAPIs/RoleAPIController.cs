//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class RoleAPIController : Controller
    {
        //public ActionResult Create(string name) { }

        //public ActionResult Delete(string name) { }

        //private WebSiteContext _context;
        //public RoleAPIController(WebSiteContext context) { _context = context; }
        private IDataContext dataContext;

        public RoleAPIController(IDataContext context)
        {
            dataContext = context;
        }

        [HostOnly,HttpPost]
        public ActionResult Perm(int id, string role, bool grant = true)
        {
            var perm = dataContext.Permissions.Find(id);
            var prole = perm.Roles.FirstOrDefault(r => r.Name.Equals(role,StringComparison.OrdinalIgnoreCase));
            if (grant)
            {
                if (prole == null)
                {
                    dataContext.Permissions.AddPermissionToRole(perm, role);
                    dataContext.SaveChanges();
                }
            }
            else
            {
                if (prole != null)
                {
                    dataContext.Permissions.RemovePermissionFromRole(perm, role);
                    dataContext.SaveChanges();
                }
            }

            return new HttpStatusCodeResult(200);
        }

    }
}
