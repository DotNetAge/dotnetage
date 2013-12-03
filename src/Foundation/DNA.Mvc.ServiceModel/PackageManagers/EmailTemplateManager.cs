//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace DNA.Web.ServiceModel
{
    public class EmailTemplateManager
    {
        public List<EmailTemplate> Templates { get; private set; }

        private string BasePath { get; set; }

        public EmailTemplateManager(string path)
        {
            //BasePath = path + (path.EndsWith("\\") ? "" : "\\");
            BasePath = path;
            var xdoc = XDocument.Load(path + "manifast.xml");
            Templates = xdoc.Root.Elements().Select(e => new EmailTemplate()
            {
                Name = e.StrAttr("name"),
                Path = BasePath + e.StrAttr("file"),
                Title = e.StrAttr("title")
            }).ToList();
        }

        public EmailTemplate this[string name]
        {
            get
            {
                return Templates.First(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

    }

    public class EmailTemplate
    {
        public string Title { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }

        public string Transform<T>(T instance, string appUrl = "")
        {
            var sb = new StringBuilder();
            using (var output = new StringWriter(sb))
            {
                var doc = new XDocument();
                var type = typeof(T);
                var rootElement = new XElement("modal");
                var properties = instance.ToDictionary();
                foreach (var key in properties.Keys)
                    rootElement.Add(new XElement(key, properties[key] != null ? properties[key] : ""));
                var xslt = new XslCompiledTransform();
                xslt.Load(XmlReader.Create(Path));
                var args = new XsltArgumentList();
                doc.Add(rootElement);
                args.AddParam("appUrl", "", appUrl);
                xslt.Transform(doc.CreateReader(), args, output);
            }
            return sb.ToString();
        }
    }

    public static class SystemMails
    {
        public static string PasswodChanged = "pwd_changed";
        public static string NewUser = "register";
        public static string EmailValidate = "pwd_valid";
        public static string ResetPasswod = "";
        public static string Contact = "contact";
    }

}
