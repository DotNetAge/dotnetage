//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.IO;
using System.Web;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    public class TemplateHelper
    {
        public static string SaveAsView(ContentListDecorator list, string template, string fileName,bool overwrite=false)
        {
            var netdrive = App.Get().NetDrive;
            var webName = list.Web.Name;
            var listPath = list.DefaultListPath.ToString();
            var dataPath = netdrive.MapPath(new Uri(listPath + "tmpls/"));

            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            if (!File.Exists(dataPath + "web.config"))
            {
                var xdoc = XElement.Load(HttpContext.Current.Server.MapPath("~/views/web.config"));
                xdoc.Save(dataPath + "web.config");
            }

            var fullName = netdrive.MapPath(new Uri(string.Format(listPath + "tmpls/{0}", fileName)));

            if (!File.Exists(fullName) || overwrite)
                File.WriteAllText(fullName, template);

            if (list.Web.Name.Equals("home"))
                return string.Format("~/app_data/files/public/lists/{0}/tmpls/{1}",list.ID, fileName);
            else
                return string.Format("~/app_data/files/personal/{0}/lists/{1}/tmpls/{2}", webName, list.ID, fileName);
        }
    }
}
