//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class AddressController : Controller
    {
        public App AppContext { get; set; }

        public AddressController() { }

        public AddressController(App appContext) { AppContext = appContext; }

        public ActionResult List(string userName)
        {
            var profile = new ProfileDecorator(AppContext.DataContext, AppContext.Users[userName].DefaultProfile);
            return PartialView(profile.Addresses);
        }

        public ActionResult Edit(int id)
        {
            return PartialView(GetAddress(id));
        }

        [Authorize, HttpPost]
        public ActionResult Edit(Address address, string format = "")
        {
            if (address.ID == 0)
            {
                address.CopyTo((IAddress)AppContext.Profile, "ID");
                AppContext.Profile.Address = address.Street;
                AppContext.Profile.Save();
            }
            else
            {
                AppContext.DataContext.Update(address);
                AppContext.DataContext.SaveChanges();
            }

            if (format == "json")
                return Json(address, JsonRequestBehavior.AllowGet);

            return PartialView(address);
        }

        [Authorize]
        public ActionResult New()
        {
            return PartialView("Edit");
        }

        [Authorize, HttpPost]
        public ActionResult New(Address address, string format = "")
        {
            address.Name = User.Identity.Name;
            AppContext.DataContext.Add(address);
            AppContext.DataContext.SaveChanges();

            if (format == "json")
                return Json(address, JsonRequestBehavior.AllowGet);

            return Detail(address.ID);

        }

        public ActionResult Detail(int id)
        {
            return PartialView(GetAddress(id));
        }

        public ActionResult GetStates(string id)
        {
            return Json(AppContext.GetStateProvinces(id), JsonRequestBehavior.AllowGet);
        }

        private Address GetAddress(int id)
        {

            if (id == 0)
            {
                var addr = (AppContext.Profile.DefaultAddress as IAddress).ConvertTo<Address>();
                addr.ID = 0;
                return addr;
            }
            else
                return AppContext.DataContext.Find<Address>(id);
        }
    }
}
