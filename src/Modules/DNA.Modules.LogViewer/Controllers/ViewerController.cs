//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data.Documents;
using DNA.Web;
using DNA.Web.Logging;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DNA.LogViewer.Controllers
{
    public class ViewerController : Controller
    {
        [HostDashboard(Text = "Log", Sequence = 5, Icon = "d-icon-file-2")]
        public ActionResult All(string id, int index = 0, int size = 50)
        {
            IEnumerable<LogEntity> logData = null;
            //  ViewBag.Type = "";
            var total = 0;
            using (var blobs = new DocumentStorage(Server.MapPath("~/app_data/logs")))
            {
                if (string.IsNullOrEmpty(id))
                    logData = blobs.All<LogEntity>(out total, index, size);
                else
                {
                    var logType = (LogEntityTypes)Enum.Parse(typeof(LogEntityTypes), id);
                    ViewBag.Type = logType;
                    logData = blobs.Where<LogEntity, DateTime>(l => l.Logged, f => f.LogEntityType == logType, out total, index: index, size: size);
                }
            }
            ViewBag.Total = total;

            return View(logData);
        }

        [HostDashboard, HttpPost]
        public void Clear()
        {
            using (var blobs = new DocumentStorage(Server.MapPath("~/app_data/logs")))
            {
                blobs.Clear<LogEntity>();
            }
        }

    }
}
