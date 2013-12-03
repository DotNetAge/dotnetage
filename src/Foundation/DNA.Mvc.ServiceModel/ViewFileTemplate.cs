//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the view file template model class.
    /// </summary>
    public class ViewFileTemplate
    {
        /// <summary>
        /// Gets / Sets the template title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets / Sets the template icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets / Sets the template description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets / Sets the unique template name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets / Sets the title reousrce key.
        /// </summary>
        public string ResKey { get; set; }

        /// <summary>
        /// Gets / Sets the description resource key .
        /// </summary>
        public string DescResKey { get; set; }

        internal string BasePath { get; set; }

        /// <summary>
        /// Gets / Sets the relative template path.
        /// </summary>
        public string Path
        {
            get
            {
                return BasePath + this.Name + (this.Name.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? "" : ".cshtml");
            }
        }
        
        /// <summary>
        /// Initializes a new instance of  the ViewFileTemplate class.
        /// </summary>
        /// <param name="element">The template data xml file.</param>
        public ViewFileTemplate(XElement element)
        {
            this.Title = element.Element("title") != null ? element.Element("title").Value : element.Value;
            this.Description = element.Element("desc") != null ? element.Element("desc").Value : "";
            this.Name = element.StrAttr("name");
            this.Icon = element.StrAttr("icon");
            if (element.Element("title") != null)
                this.ResKey = element.Element("title").StrAttr("resKey");
            if (element.Element("desc") != null)
                this.DescResKey = element.Element("desc").StrAttr("resKey");
        }
    }
}
