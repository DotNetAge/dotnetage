//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    internal class ViewFileTemplateManager
    {
//        private const string basePath = "~/content/types/base/views/";
        internal static List<ViewFileTemplate> GetTemplates(HttpContextBase context, string basePath)
        {
            var config = context.Server.MapPath(basePath + "config.xml");
            var xdoc = XDocument.Load(config);
            return xdoc.Root.Elements().Select(e => new ViewFileTemplate(e) { BasePath = basePath }).ToList();
        }
    }


}
