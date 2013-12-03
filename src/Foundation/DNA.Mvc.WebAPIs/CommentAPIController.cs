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
    /// <summary>
    /// The comment API controller
    /// </summary>
    public class CommentAPIController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// GET /api/comments/list?url=[url]&index=[pageIndex]&size=[pageSize]
        /// </example>
        /// <param name="url"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet, Compress]
        public ActionResult List(Uri url, int? index, int? size, int replyTo = 0)
        {
            if (url == null)
                return HttpNotFound();

            var formattedUrl = url.ToString().ToLower();
            var total = 0;

            if (index.HasValue && size.HasValue)
            {
                var modelResult = App.Get().FindComments(url.ToString(), out total, index.Value - 1, size.Value, replyTo);
                var userNames = modelResult.Select(u => u.UserName).ToArray();
                var profiles = App.Get().DataContext.Where<UserProfile>(u => userNames.Contains(u.UserName)).ToList();

                var result = new
                {
                    Total = total,
                    Model = modelResult.Select(m => m.ToObject(profiles, Request.ApplicationPath))
                };

                return Content(JsonConvert.SerializeObject(result, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat }), "application/json", System.Text.Encoding.UTF8);
            }
            else
            {
                var results = App.Get().FindComments(url.ToString(), replyTo);
                var userNames = results.Select(u => u.UserName).ToArray();
                var profiles = App.Get().DataContext.Where<UserProfile>(u => userNames.Contains(u.UserName)).ToList();
                return Content(JsonConvert.SerializeObject(results.Select(m => m.ToObject(profiles, Request.ApplicationPath)), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat }), "application/json", System.Text.Encoding.UTF8);
            }
            //return Json(App.Get().FindComments(url.ToString()).Select(m => m.ToObject()), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="replyTo"></param>
        /// <returns></returns>
        [Authorize, HttpPost, ValidateInput(false)]
        public ActionResult Add(Uri url, string content, int replyTo = 0)
        {
            if (url == null)
                throw new HttpException("Url can not be null");
            var comment = App.Get().User.AddComment(url.ToString(), content, true, replyTo);
            string json = JsonConvert.SerializeObject(comment.ToObject(appPath: Request.ApplicationPath), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            return Content(json, "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize,HttpPost]
        public void Delete(int id)
        {
            App.Get().User.Comments.Delete(id);
        }

        [HttpGet]
        public ActionResult Recent(int maxReturns = 20)
        {
            var q = App.Get().DataContext.All<Comment>().OrderByDescending(c => c.Posted).Take(maxReturns).ToList();
            if (q.Count > 0)
            {
                var userNames = q.Select(u => u.UserName).ToArray();
                var profiles = App.Get().DataContext.Where<UserProfile>(u => userNames.Contains(u.UserName)).ToList();
                var results = q.ToList().Select(c => App.Get().Wrap(c).ToObject(profiles, Request.ApplicationPath)).ToList();
                return Content(JsonConvert.SerializeObject(results, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat }), "application/json", Encoding.UTF8);
            }
            return Json(new string[0], JsonRequestBehavior.AllowGet);
        }
    }
}
