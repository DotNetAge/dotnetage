//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web;
using DNA.Web.ServiceModel;
using System;
using System.Web.Mvc;

namespace DNA.Modules.Utilities.Controllers
{
    public class UtilityController : Controller
    {
        [SecurityAction("Management base", "Edit robot.txt", "Allows users can edit the robot.txt file.",
            TitleResName = "SA_Robot",
            DescResName = "SA_RobotDesc",
            PermssionSetResName = "SA_Managementbase")]
        [HostDashboard(Text = "robots.txt", Group = "Tools",
            Icon = "d-icon-file-3",
            ResBaseName = "Managements",
            GroupResKey = "Tools",
            ResKey = "Robot")]
        public ActionResult Robot()
        {
            ViewBag.Rules = "";
            ViewBag.Error = "";
            var robot = Server.MapPath("~/robots.txt");
            try
            {
                if (!System.IO.File.Exists(robot))
                {
                    System.IO.File.CreateText(robot);
                }
                else
                {
                    using (var reader = System.IO.File.OpenText(robot))
                    {
                        ViewBag.Rules = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
            return View();
        }

        [Authorize, HttpPost, HostDashboard]
        public ActionResult Robot(string rules)
        {
            if (App.Get().Context.HasPermisson(this, "Robot"))
            {
                ViewBag.Rules = rules;
                ViewBag.Error = "";
                var robot = Server.MapPath("~/robots.txt");
                try
                {
                    using (var writer = new System.IO.StreamWriter(robot))
                    {
                        writer.Write(rules);
                        writer.Flush();
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                    return View();
                }
                return View();
            }
            else
            {
                return RedirectToAction("AccessDenied", "Security", new { Area = "" });
            }
        }

        [HostDashboard(ResKey = "MetaTags",
            ResBaseName = "Managements",
            Group = "Tools",
            Icon = "d-icon-embed")]
        public ActionResult Metas()
        {
            string fileName = Server.MapPath("~/Views/Shared/_CustomHead.cshtml");
            if (System.IO.File.Exists(fileName))
            {
                ViewBag.Metas = System.IO.File.ReadAllText(fileName);
            }
            else
                ViewBag.Metas = "";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false), Authorize, HostDashboard]
        public ActionResult Metas(string metas)
        {
            string fileName = Server.MapPath("~/Views/Shared/_CustomHead.cshtml");
            if (!string.IsNullOrEmpty(metas))
                System.IO.File.WriteAllText(fileName, metas);
            else
            {
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                    System.IO.File.WriteAllText(fileName, "");
                }
            }

            return RedirectToAction("Metas");
        }

        //[HostDashboard(
        //   ResBaseName = "Managements", 
        //    Group = "Tools",
        //    Text="Renamed resources"
        //    )]
        //public ActionResult Redirection() 
        //{
        //    var renames = App.Get().DataContext.All<MovedUrl>();
        //    return View(renames);
        //}

        //[HostDashboard( Text="Country codes",ResBaseName = "Managements", Group = "Tools")]
        //public ActionResult CountryCodes()
        //{
        //    return View();
        //}

        //[HostDashboard]
        //public ActionResult StateCodes(string id)
        //{
        //    ViewBag.Region = new RegionInfo(id);
        //    return View(App.Get().GetStateProvinces(id));
        //}
    }
}
