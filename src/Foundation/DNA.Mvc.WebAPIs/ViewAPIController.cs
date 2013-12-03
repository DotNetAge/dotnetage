//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class ViewAPIController : Controller
    {
        public ActionResult ValidateFilter(int id, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                var view = App.Get().DataContext.Find<ContentView>(id);
                var wrapper = new ContentViewDecorator(view, App.Get().DataContext);
                try
                {
                    wrapper.Items(filter);
                }
                catch (Exception e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateName(int id, string name)
        {
            return Json(App.Get().DataContext.Count<ContentView>(v => v.ParentID == id && v.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) == 0, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.ViewController,DNA.Web")]
        public ActionResult Save(int id, string title, string desc, string sort, string filter, bool? allowPaging, bool? isDefault, int size = 0)
        {
            var view = App.Get().DataContext.Find<ContentView>(id);
            var list = App.Get().FindList(view.ParentID);

            if (!list.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            if (title != null)
                view.Title = title;

            if (desc != null)
                view.Description = desc;

            if (filter != null)
            {
                view.Filter = filter;
                var wrapper = new ContentViewDecorator(view, App.Get().DataContext);
                wrapper.Refresh();
            }

            if (sort != null)
            {
                view.Sort = sort;
                var wrapper = new ContentViewDecorator(view, App.Get().DataContext);
                wrapper.Refresh();
            }

            if (allowPaging.HasValue)
                view.AllowPaging = allowPaging.Value;

            if (isDefault.HasValue)
            {
                if (isDefault.Value)
                {
                    var views = App.Get().DataContext.Where<ContentView>(v => v.ParentID == view.ParentID && v.ID != view.ID);
                    foreach (var v in views)
                        v.IsDefault = false;
                }

                view.IsDefault = isDefault.Value;
            }

            if (size > 0)
                view.PageSize = size;

            App.Get().DataContext.SaveChanges();
            return new HttpStatusCodeResult(200);
        }

        [HttpPost, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.ViewController,DNA.Web")]
        public ActionResult SetRoles(int id, string[] roles, bool? allowAnonymous)
        {
            var view = App.Get().DataContext.Find<ContentView>(id);
            var list = App.Get().FindList(view.ParentID);

            if (!list.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            if (allowAnonymous.HasValue)
                view.AllowAnonymous = allowAnonymous.Value;
            else
                view.AllowAnonymous = false;

            view.Roles = roles != null && roles.Length > 0 ? string.Join(",", roles) : "";
            App.Get().DataContext.SaveChanges();
            return new HttpStatusCodeResult(200);
        }

        [HttpPost, ValidateInput(false),
        PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.ViewController,DNA.Web")]
        public ActionResult SetTmpl(int id, string text, string contentType = "text/xslt", string type = "body")
        {
            var view = App.Get().DataContext.Find<ContentView>(id);
            var list = App.Get().FindList(view.ParentID);

            if (!list.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            var transoformer = TemplateTransformers.Get(contentType);
            transoformer.Transform(text, view);

            App.Get().DataContext.SaveChanges();
            return new HttpStatusCodeResult(200);
        }

        [HttpPost, ValidateInput(false),
        PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.ViewController,DNA.Web")]
        public ActionResult SetEmptyTmpl(int id, string text)
        {
            var view = App.Get().DataContext.Find<ContentView>(id);
            var list = App.Get().FindList(view.ParentID);

            if (!list.IsOwner(HttpContext))
                return new HttpUnauthorizedResult();

            view.EmptyTemplateXml = (new ContentTemplate() { Text = text }).ToXml("emptyPattern");
            App.Get().DataContext.SaveChanges();
            return new HttpStatusCodeResult(200);
        }
    }
}
