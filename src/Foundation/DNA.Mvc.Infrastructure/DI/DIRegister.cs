//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DNA.Web
{
    public class DIRegister
    {
        public static void RegisterTypes()
        {
            RegisterSpecifiedTypes();
            
            var container = new UnityContainer();
            UnityConfigurationSection configuration = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            configuration.Configure(container);

            //Enabling Dependency Injection for filters
            var oldProvider = FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider);
            FilterProviders.Providers.Remove(oldProvider);
            var provider = new UnityFilterAttributeFilterProvider(container);
            FilterProviders.Providers.Add(provider);

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static void RegisterSpecifiedTypes()
        {
            var typeSearcher = TypeSearcher.Instance();

            var unityConfigFile = HttpRuntime.AppDomainAppPath + "unity.config";
            var unity = XDocument.Load(unityConfigFile);
            var ns = unity.Root.GetDefaultNamespace();
            var isChanged = false;

            var types = typeSearcher.SearchTypesOfAttribute(typeof(InjectAttribute), false);
            foreach (var type in types)
            {
                var typeName = type.Name;
                var typeFullName = type.FullName + "," + type.Assembly.GetName().Name;
                var asmName = type.Assembly.GetName().Name;
                var typeNS = type.Namespace;
                var mapTo = "";

                var iocAttr = typeSearcher.ExtractCustomAttribute<InjectAttribute>(type);

                if (!string.IsNullOrEmpty(iocAttr.Alias))
                    typeName = iocAttr.Alias;


                if (!string.IsNullOrEmpty(iocAttr.MapTo))
                    mapTo = iocAttr.MapTo;

                //Register alias name

                var aliasNode = unity.Root.Elements(ns + "alias").FirstOrDefault(e => e.StrAttr("alias").Equals(typeName));

                if (aliasNode == null)
                {
                    unity.Root.Add(new XElement(ns + "alias",
                        new XAttribute("alias", typeName),
                        new XAttribute("type", typeFullName)));
                    isChanged = true;
                }

                //Register namespace

                if (unity.Root.Elements(ns + "namespace").FirstOrDefault(e => e.StrAttr("name").Equals(typeNS)) == null)
                {
                    unity.Root.Add(new XElement(ns + "namespace", new XAttribute("name", typeNS)));
                    isChanged = true;
                }

                //Register assembly

                if (unity.Root.Elements(ns + "assembly").FirstOrDefault(e => e.StrAttr("name").Equals(asmName)) == null)
                {
                    unity.Root.Add(new XElement(ns + "assembly", new XAttribute("name", asmName)));
                    isChanged = true;
                }


                //Register types

                if (!string.IsNullOrEmpty(mapTo))
                {
                    var aliasName = typeName[0].ToString().ToLower() + typeName.Substring(1);

                    if (unity.Root.Descendants(ns + "register").FirstOrDefault(e => e.StrAttr("type").Equals(mapTo) &&
                        e.StrAttr("mapTo").Equals(typeName)) == null)
                    {
                        unity.Root
                            .Element(ns + "container")
                            .Add(new XElement(ns + "register",
                                new XAttribute("type", mapTo),
                                new XAttribute("mapTo", typeName),
                                new XAttribute("name", aliasName)));
                        isChanged = true;
                    }

                }
            }

            if (isChanged)
                unity.Save(unityConfigFile);
        }
    }
}
