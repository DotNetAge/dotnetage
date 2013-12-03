//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class LanguageAPIController : Controller
    {
        //[Authorize, HttpPost, Loc]
        //public ActionResult Switch(string to = "en-US")
        //{
        //    var app = App.Get();
        //    var web = app.CurrentWeb;

        //    if (!web.DefaultLocale.Equals(to, StringComparison.OrdinalIgnoreCase))
        //        web.SwitchLocale(to);

        //    return new HttpStatusCodeResult(200);
        //}

        //[Loc]
        //public ActionResult SwitchUI(string lang="") 
        //{
        //    throw new NotImplementedException();
        //}

        [Authorize,
        HttpPost,
        SecurityAction("Management base", "Localization", "Allows users can localize the pages and contents.",
           TitleResName = "SA_Localization",
           DescResName = "SA_LocalizationDesc",
           PermssionSetResName = "SA_Managementbase"
            )]
        public void Copy(string locale, string to)
        {
            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException("to");

            var app = App.Get();
            var web = app.CurrentWeb;
            var solutions = web.InstalledSolutions.Split(',');

            var fromLocale = "." + locale;
            var toLocale = "." + to;
            var fromSolution = false;

            var cookieName = DNA.Web.ServiceModel.WebContext.UserLocaleCookieName;

            if (Response.Cookies[cookieName] == null)
                Response.Cookies.Add(new System.Web.HttpCookie(cookieName, to));
            else
                Response.Cookies[cookieName].Value = to;


            web.Culture = to;
            web.Save();

            foreach (var solution in solutions)
            {
                if (solution.EndsWith(fromLocale, StringComparison.OrdinalIgnoreCase))
                {
                    //Is target locale copy installed?
                    var solutionName = solution.Split('.')[0];
                    if (!solutions.Contains(solutionName + "." + to))
                    {
                        var pkg = app.Solutions[solutionName];
                        var supportLanguages = pkg.GetSupportLanguages();
                        if (supportLanguages != null && supportLanguages.Contains(to))
                        {
                            app.Solutions.Install(pkg.Name, web.Name, User.Identity.Name, "", lang: to);
                            fromSolution = true;
                        }
                    }
                }
            }

            if (!fromSolution)
            {
                var from = locale;

                if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Can not make the translation copy form itself.");

                if (web.InstalledLocales.Contains(to.ToLower()))
                {
                    //web.SwitchLocale(to);
                }
                else
                {
                    var pages = web.Pages.Where(p => p.Locale.Equals(from, StringComparison.OrdinalIgnoreCase))
                                                             .OrderBy(p => p.Pos)
                                                             .ToList();
                    RecurseClone(0, null, pages, to);
                }
            }

            //App.Get().CurrentWeb.loc
        }

        private void RecurseClone(int parentID, WebPageDecorator parentPage, IEnumerable<WebPageDecorator> sources, string locale)
        {
            var pages = sources.Where(p => p.ParentID.Equals(parentID)).OrderByDescending(p => p.Pos);
            if (pages.Count() > 0)
            {
                foreach (var p in pages)
                {
                    if (p.Slug.StartsWith("lists"))
                        continue;

                    var newPage = p.Clone(locale);

                    if (parentPage != null)
                        newPage.MoveTo(parentPage.ID);
                    else
                        newPage.MoveTo(0);

                    RecurseClone(p.ID, newPage, sources, locale);
                }
            }
        }

    }


}
