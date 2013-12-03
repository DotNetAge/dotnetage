//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    public class Theme
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Version { get; set; }

        public string PrimaryImage { get; set; }

        public string Roles { get; set; }

        public DateTime Lastupdated { get; set; }

        public string Locale { get; set; }

        public static Theme Parse(string xml, string locale = "")
        {
            var element = XElement.Parse(xml);
            var theme = new Theme()
            {
                Name = element.StrAttr("name"),
                Url = element.StrAttr("url"),
                PrimaryImage = element.StrAttr("preview"),
                Version = element.StrAttr("version"),
                Roles = element.StrAttr("roles"),
                Locale = element.StrAttr("locale")
            };

            var title = element.ElementWithLocale("title", locale);
            var desc = element.ElementWithLocale("description", locale);

            if (title != null)
                theme.Title = title.Value;

            if (desc != null)
                theme.Description = desc.Value;

            return theme;
        }

        public XElement Element()
        {
            if (string.IsNullOrEmpty(Name))
                throw new Exception("The theme name can not be null");

            var theme = new XElement("theme",
                new XAttribute("name", this.Name),
                new XAttribute("updated", this.Lastupdated)
                );

            if (!string.IsNullOrEmpty(Url))
                theme.Add(new XAttribute("url", this.Url));

            if (!string.IsNullOrEmpty(Version))
                theme.Add(new XAttribute("version", this.Version));

            if (!string.IsNullOrEmpty(Roles))
                theme.Add(new XAttribute("roles", this.Roles));

            if (!string.IsNullOrEmpty(PrimaryImage))
                theme.Add(new XAttribute("preview", this.PrimaryImage));

            if (!string.IsNullOrEmpty(Url))
                theme.Add(new XAttribute("url", this.Url));

            if (!string.IsNullOrEmpty(Title))
                theme.Add(new XElement("title", this.Url));

            if (!string.IsNullOrEmpty(Description))
                theme.Add(new XElement("description", this.Url));
            return theme;
        }

    }
}
