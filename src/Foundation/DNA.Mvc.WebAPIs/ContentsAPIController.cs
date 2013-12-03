//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class ContentsAPIController : Controller
    {
        private IDataContext dataContext;
        public ContentsAPIController(IDataContext context)
        {
            this.dataContext = context;
        }

        /// <summary>
        /// Create new content list item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult NewItem(int id, FormCollection forms, string categories = "", string tags = "",
            bool allowComments = false, bool isPublished = false, string returnUrl = "", string parentID = "",
            int pos = 0, string locale = "en-US")
        {
            var list = App.Get().CurrentWeb.Lists[id];

            if (!list.NewForm.IsAuthorized(HttpContext))
                throw new HttpException("Unauthorized user");
                //return new HttpUnauthorizedResult();

            var item = list.NewItem(forms, User.Identity.Name, isPublished, allowComments, parentID, pos, locale, categories, tags);

            string jsonStr = JsonConvert.SerializeObject(item.ToObject(), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            return Content(jsonStr, "application/json", Encoding.UTF8);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateItem(int id, Guid itemID, FormCollection forms)
        {
            var list = App.Get().CurrentWeb.Lists[id];
            if (!list.EditForm.IsAuthorized(HttpContext))
                throw new HttpException("Unauthorized user");

            var item = list.GetItem(itemID);
            item.Save(forms);
            string jsonStr = JsonConvert.SerializeObject(item.ToObject(), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            return Content(jsonStr, "application/json", Encoding.UTF8);
        }

        [Authorize, HttpPost]
        public ActionResult DeleteItem(int id, Guid itemID)
        {
            var list = App.Get().CurrentWeb.Lists[id];
            if (!list.EditForm.IsAuthorized(HttpContext))
                return new HttpUnauthorizedResult();

            return Json(list.DeleteItem(itemID), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete the ContentList instance by specified list id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public ActionResult Delete(int id)
        {
            var list = App.Get().FindList(id);
            if (!User.IsAdministrator() && !User.Identity.Name.Equals(list.Owner))
                return new HttpUnauthorizedResult();

            var success = App.Get().CurrentWeb.Lists.Remove(id);
            //return Json(new { success = success }, JsonRequestBehavior.AllowGet);
            return new HttpStatusCodeResult(200);
        }

        public ActionResult Items(string name, string slug, ContentQuery query, string parentID = "", string format = "json")
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            //if (string.IsNullOrEmpty(slug))
            //  throw new ArgumentNullException("slug");

            var list = App.Get().CurrentWeb.Lists[name];
            //ViewBag.List = list;
            ContentViewDecorator view;

            if (string.IsNullOrEmpty(slug))
                view = list.Views.FirstOrDefault();
            else
                view = list.Views[slug];

            if (!string.IsNullOrEmpty(query.Filter))
                query.Filter = query.GetFilterExpression();

            if (!string.IsNullOrEmpty(parentID))
            {
                if (string.IsNullOrEmpty(query.Filter))
                    query.Filter = "parentId='" + parentID + "'";
                else
                {
                    query.Filter = " AND parentId='" + parentID + "'";
                }
            }

            if (view == null)
                throw new ContentViewNotFoundException();

            if (!view.IsAuthorized(HttpContext))
                return new HttpUnauthorizedResult();

            var results = view.Items(query);

            if (format == "xml")
                return Content(results.Element().OuterXml(), "text/xml", Encoding.UTF8);

            var model = results.Items.Select(i => list.GetItem(i.ID).ToObject()).ToList();
            var jsonModel = new
            {
                Model = model,
                Total = query.Total
            };

            var jstr = JsonConvert.SerializeObject(jsonModel, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            return Content(jstr, "application/json", Encoding.UTF8);
        }

        [Loc]
        public ActionResult Categories(string name, string locale)
        {
            //var web = App.Get().Webs[name];
            var cats = App.Get().CurrentWeb.Categories.ToList().Select(c => new
            {
                id = c.ID,
                name = c.Name,
                desc = c.Description
            });
            return Json(cats, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reshare(Guid id, string name, int toListID = 0)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("id");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var list = App.Get().CurrentWeb.Lists[name];
            if (list == null)
                throw new ContentListNotFoundException();

            var item = list.GetItem(id);
            if (item == null)
                throw new ContentDataItemNotFoundException();

            ContentDataItemDecorator reshare = null;
            if (toListID > 0)
            {
                var targetList = App.Get().DataContext.Find<ContentList>(toListID);
                reshare = item.ReshareTo(targetList);
            }
            else
                reshare = item.ReshareTo();
            string jsonStr = JsonConvert.SerializeObject(reshare.ToObject(), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            return Content(jsonStr, "application/json", Encoding.UTF8);
        }

        public ActionResult Shares(string name, Guid id, QueryParams query)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("id");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var list = App.Get().CurrentWeb.Lists[name];
            if (list == null)
                throw new ContentListNotFoundException();

            var item = list.GetItem(id);
            if (item == null)
                throw new ContentDataItemNotFoundException();

            var results = query.GetPageResult(item.Reshares()).ToList().Select(r => App.Get().Wrap(r).ToObject()).ToList();
            var modal = new
            {
                Modal = results,
                Total = item.Reshares().Count()
            };

            string jsonStr = JsonConvert.SerializeObject(modal, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            return Content(jsonStr, "application/json", Encoding.UTF8);
        }

        public void DisableComments(Guid id, bool value)
        {
            dataContext.ContentDataItems.DisableComments(id, value);
            dataContext.SaveChanges();
        }

        [Authorize,HttpPost]
        public void Publish(Guid id)
        {
            dataContext.ContentDataItems.Publish(id);
            dataContext.SaveChanges();
        }

        [Authorize,HttpPost]
        public ActionResult Audit(Guid id, int state, string remarks)
        {
            var item = dataContext.Find<ContentDataItem>(id);
            var wrapper = App.Get().FindList(item.ParentID);

            if (wrapper.IsModerator(User.Identity.Name) || User.IsAdministrator())
            {
                dataContext.ContentDataItems.Audit(id, User.Identity.Name, state);
                dataContext.SaveChanges();

                if (!string.IsNullOrEmpty(remarks))
                {
                    var itemWrapper = App.Get().Wrap(item);
                    App.Get().User.AddComment(itemWrapper.UrlComponent, remarks);
                }

                return new HttpStatusCodeResult(200);
            }

            return new HttpUnauthorizedResult();
        }

        public ActionResult Views(string id)
        {
            var list = App.Get().CurrentWeb.Lists[id];
            return Json(list.Views.Select(v => v.ToObject()).ToList(), JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost]
        public ActionResult Vote(string list, Guid id, int value)
        {
            var _list=App.Get().CurrentWeb.Lists[list];
            if (!_list.AllowVotes)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

            var item = _list.GetItem(id);
            return Json(item.Vote(User.Identity.Name, value), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewFields(string name, string slug)
        {
            var list = App.Get().CurrentWeb.Lists[name];
            var view = list.Views[slug];
            var fields = view.FieldRefs.Select(f => new
            {
                name = f.Name,
                title = f.Title
            });
            return Json(fields, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsFollowing(int id)
        {
            if (!Request.IsAuthenticated)
                return Json(false, JsonRequestBehavior.AllowGet);
            return Json(App.Get().CurrentWeb.Lists[id].IsFollowing(User.Identity.Name), JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost]
        public ActionResult Follow(int id)
        {
            if (!Request.IsAuthenticated)
                return Json(false, JsonRequestBehavior.AllowGet);
            return Json(App.Get().CurrentWeb.Lists[id].Follow(User.Identity.Name), JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost]
        public ActionResult Unfollow(int id)
        {
            if (!Request.IsAuthenticated)
                return Json(false, JsonRequestBehavior.AllowGet);
            return Json(App.Get().CurrentWeb.Lists[id].Unfollow(User.Identity.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Followers(int id, QueryParams query)
        {
            var followers = App.Get().CurrentWeb.Lists[id].Followers;
            var results = query.GetPageResult(followers).ToList();
            var users = results.Select(p => new
            {
                id = p.UserName,
                dispName = p.DisplayName,
                name = new
                {
                    firstName = p.FirstName,
                    lastName = p.LastName
                },
                email = p.Email,
                picture = p.Avatar,
                link = Url.Content("~/profiles/" + p.UserName)
            }).ToList();
            string jsonStr = JsonConvert.SerializeObject(users);
            return Content(jsonStr, "application/json", Encoding.UTF8);
        }

        public ActionResult Move(string name, Guid parentID, Guid id, int pos)
        {
            var list = App.Get().CurrentWeb.Lists[name];
            if (!list.EditForm.IsAuthorized(HttpContext))
                return new HttpUnauthorizedResult();

            var item = list.GetItem(id);
            if (item != null)
                item.MoveTo(parentID, pos);
            
            return new HttpStatusCodeResult(200);
        }

        //[HttpPost]
        //public void TypeRoles(string name)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
