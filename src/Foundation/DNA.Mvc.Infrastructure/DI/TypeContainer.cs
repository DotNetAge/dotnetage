//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DNA.Web
{
    public class TypeContainer : ITypeContainer
    {
        private bool isChanged = false;
        private XDocument unity;
        private XNamespace ns;
        private string unityConfigFile;

        public TypeContainer()
        {
            unityConfigFile = HttpRuntime.AppDomainAppPath + "unity.config";
            unity = XDocument.Load(unityConfigFile);
            ns = unity.Root.GetDefaultNamespace();
        }

        public void Register<TFrom, To>()
        {
            RegisterInterface<TFrom>();
            RegisterInterface<To>();

            var mapTo = typeof(TFrom);
            var type = typeof(To);
            var typeName = type.Name;
            var typeNS = type.Namespace;

            //Register types
            var aliasName = typeName[0].ToString().ToLower() + typeName.Substring(1);

            if (unity.Root.Descendants(ns + "register").FirstOrDefault(e => e.StrAttr("type").Equals(mapTo.Name) &&
                e.StrAttr("mapTo").Equals(typeName)) == null)
            {
                unity.Root
                    .Element(ns + "container")
                    .Add(new XElement(ns + "register",
                        new XAttribute("type", mapTo.Name),
                        new XAttribute("mapTo", typeName),
                        new XAttribute("name", aliasName)));
                isChanged = true;
            }
        }

        public void RegisterInterface<T>()
        {
            var type = typeof(T);
            var typeName = type.Name;
            var typeFullName = type.FullName + "," + type.Assembly.GetName().Name;
            var asmName = type.Assembly.GetName().Name;
            var typeNS = type.Namespace;

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

        }

        public void Apply()
        {
            if (isChanged)
                unity.Save(unityConfigFile);
        }
    }
}
