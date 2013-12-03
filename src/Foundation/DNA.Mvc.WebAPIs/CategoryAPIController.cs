//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Linq;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class CategoryAPIController : Controller
    {
        [Loc]
        public ActionResult Items(int id = 0)
        {
            var web = App.Get().CurrentWeb;
            var cats = web.Categories
                                     .Where(p => p.ParentID == id)
                                     .ToList()
                                     .Select(c => new
                                     {
                                         id = c.ID,
                                         parentID = c.ParentID,
                                         name = c.Name,
                                         description = c.Description,
                                         image=c.ImageUrl
                                     });

            return Json(cats, JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost, Loc]
        public ActionResult Add(string name, int parentID = 0,string desc="",string imgUrl="")
        {
            var web = App.Get().CurrentWeb;
            if (!web.Owner.Equals(User.Identity.Name))
                return new HttpUnauthorizedResult();

            var cat = parentID > 0 ? web.Categories[parentID].AddChildren(name) :
                web.Categories.New(name);

            cat.Description = desc;
            cat.ImageUrl=imgUrl;
            cat.Save();

            return Json(new
            {
                id = cat.ID,
                name = cat.Name,
                parentID = cat.ParentID,
                desc = cat.Description,
                image = cat.ImageUrl
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost, Loc]
        public ActionResult Move(int id, int parentID)
        {
            var web = App.Get().CurrentWeb;
            if (!web.Owner.Equals(User.Identity.Name))
                return new HttpUnauthorizedResult();
            web.Categories[id].MoveTo(parentID);
            return new HttpStatusCodeResult(200);
        }

        [Authorize, HttpPost, Loc]
        public ActionResult Delete(int id)
        {
            var web = App.Get().CurrentWeb;
            if (!web.Owner.Equals(User.Identity.Name))
                return new HttpUnauthorizedResult();
            web.Categories.Remove(id);
            return new HttpStatusCodeResult(200);
        }

        [Authorize, HttpPost, Loc]
        public ActionResult Update(int id, string name, string desc, string imgUrl)
        {
            var web = App.Get().CurrentWeb;
            if (!web.Owner.Equals(User.Identity.Name))
                return new HttpUnauthorizedResult();
            var cat = web.Categories[id];
            if (!cat.Name.Equals(name))
            {
                cat.Name = name;
                cat.Description = desc;
                cat.ImageUrl = imgUrl;
                cat.Save();
            }
            return new HttpStatusCodeResult(200);
        }
    }
}
