//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

namespace DNA.Web.Routing
{
    /// <summary>
    /// This class use to register the external route table
    /// </summary>
    public class ExternalRouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            var path = HttpRuntime.AppDomainAppPath + "routes.xml";
            if (System.IO.File.Exists(path))
            {
                var xDoc = XDocument.Load(path);
                foreach (var ele in xDoc.Root.Elements())
                {
                    var name = ele.StrAttr("name");
                    var pattern = ele.StrAttr("pattern");
                    var defaultsEle = ele.Element("defaults");
                    var constraintsEle = ele.Element("constraints");
                    var nsEles = ele.Elements("namespace");
                    string[] namespaces = null;
                    object defaults = null;
                    object constraints = null;

                    if (defaultsEle != null)
                    {
                        defaults = new ExpandoObject();
                        foreach (var addEle in defaultsEle.Elements("add"))
                            ((IDictionary<String, object>)defaults).Add(addEle.StrAttr("name"), addEle.StrAttr("value"));
                    }

                    if (constraintsEle != null)
                    {
                        constraints = new ExpandoObject();
                        foreach (var addEle in constraintsEle.Elements("add"))
                            ((IDictionary<String, object>)constraints).Add(addEle.StrAttr("name"), addEle.StrAttr("value"));
                    }

                    if (nsEles != null)
                    {
                        var ns = new List<string>();
                        foreach (var nsEle in nsEles)
                            ns.Add(nsEle.StrAttr("value"));
                        namespaces = ns.ToArray();
                    }

                    if (routes[name] == null)
                        routes.MapRoute(name, pattern, defaults, constraints, namespaces);
                }
            }
        }
    }
}
