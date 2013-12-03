//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class FormAPIController : Controller
    {
        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.FormController,DNA.Web")]
        public ActionResult Save(int id, string title, string desc)
        {
            var form = App.Get().DataContext.Find<ContentForm>(id);
            if (title != null)
                form.Title = title;
            if (desc != null)
                form.Description = desc;
            App.Get().DataContext.SaveChanges();

            return new HttpStatusCodeResult(200);
        }

        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.FormController,DNA.Web")]
        public ActionResult SetRoles(int id, string[] roles, bool? allowAnonymous)
        {
            var form = App.Get().DataContext.Find<ContentForm>(id);
            var list = App.Get().FindList(form.ParentID);

            if (!list.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            if (allowAnonymous.HasValue)
                form.AllowAnonymous = allowAnonymous.Value;
            else
                form.AllowAnonymous = false;

            form.Roles = roles != null && roles.Length > 0 ? string.Join(",", roles) : "";
            App.Get().DataContext.SaveChanges();

            return new HttpStatusCodeResult(200);
        }
    }
}
