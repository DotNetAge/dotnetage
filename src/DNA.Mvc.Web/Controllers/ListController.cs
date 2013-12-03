//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class ListController : Controller
    {
        [SiteDashboard,
        SecurityAction("Edit list settings", 
            PermssionSet = "Contents", 
            Description = "Allows user can edit list settings and custom fields,views,and forms. ",
            TitleResName = "SA_EditList",
            DescResName = "SA_EditListDesc",
            PermssionSetResName = "SA_Contents"
            )]
        public ActionResult Edit(string name)
        {
            return View(App.Get().CurrentWeb.Lists[name]);
        }

        [SecurityAction("Edit list field", 
            PermssionSet = "Contents", 
            Description = "Allows user can be edit the field settings. ",
            TitleResName = "SA_EditField",
            DescResName = "SA_EditFieldDesc",
            PermssionSetResName = "SA_Contents")]
        public ActionResult EditField(int id, string name)
        {
            var list = App.Get().CurrentWeb.Lists[id];
            ViewBag.List = list;
            return PartialView("_Field", list.Fields[name]);
        }

        [SecurityAction("Create new list field", 
            PermssionSet = "Contents", 
            Description = "Allows user can be create new fields for list. ",
            TitleResName = "SA_CreateList",
            DescResName = "SA_CreateListDesc",
            PermssionSetResName = "SA_Contents")]
        public ActionResult NewField(int id)
        {
            ViewBag.List = App.Get().CurrentWeb.Lists[id];
            return PartialView("_Field", new TextField());
        }

        [Authorize, HttpPost]
        public void SortFields(int id, string orders)
        {
            var list = App.Get().CurrentWeb.Lists[id];
            if (User.IsAdministrator() || list.Owner.Equals(User.Identity.Name))
                list.SetFieldOrders(orders.Split(','));
        }

    }
}
