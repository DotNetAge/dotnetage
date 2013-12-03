//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class WebsAPIController : Controller
    {
        /// <summary>
        /// Validate the web name is exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Validate(string name)
        {
            return Json((App.Get().Webs[name] == null), JsonRequestBehavior.AllowGet);
        }

        [HttpPost,
        ValidateAntiForgeryToken,
        SecurityAction("Webs", "Create new web", "Allows user to create new web.",
            ThrowOnDeny=true,
            TitleResName = "SA_CreateWeb",
            DescResName = "SA_CreateWebDesc",
            PermssionSetResName = "SA_Webs"
            )]
        public ActionResult Create(string name, string lang, string title = "", string desc = "", string solution = "", string theme = "")
        {
            var app = App.Get();
            if (app.Webs[name] != null)
                throw new HttpException("There is a web named \"" + name + "\".");

            try
            {
                WebDecorator web = null;
                if (!string.IsNullOrEmpty(solution))
                    web = app.Solutions.Install(solution, name, User.Identity.Name, title, desc, theme, lang);
                else
                {
                    web = app.CreateWeb(name, title, desc, lang, theme);
                    if (!string.IsNullOrEmpty(theme))
                        web.Theme = theme;

                    var defaultPage = web.CreatePage("Default");
                }

                string webJson = JsonConvert.SerializeObject(web.ToObject());
                return Content(webJson, "application/json", Encoding.UTF8);
            }
            catch (Exception e)
            {
                //var msg = e.Message;
                Exception innerExpt = e.InnerException;
                var errors = new StringBuilder();
                errors.AppendLine(e.Message);

                if (app.Webs[name] != null)
                {
                    App.Get().DataContext.Delete<Web>(w => w.Name.Equals(name,StringComparison.OrdinalIgnoreCase));
                    App.Get().DataContext.SaveChanges();
                }

                while (innerExpt != null)
                {
                    //msg = innerExpt.Message;
                    errors.AppendLine(innerExpt.Message);
                    innerExpt = innerExpt.InnerException;
                }

                throw new HttpException(errors.ToString());
            }

        }

        [HttpPost,HostOnly]
        public ActionResult Delete(string id) 
        {
            var web = App.Get().Webs[id];
            App.Get().DataContext.Delete(web.Model);
            App.Get().DataContext.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
