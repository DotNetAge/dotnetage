//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represents the field that lookup the value from other list
    /// </summary>
    public class LookupField:ContentField
    {
        /// <summary>
        /// Gets/Sets the list name or id where the field lookup from
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// Gets/Sets the view name where the field to lookup the data from
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Gets/Sets which field value from lookup and display
        /// </summary>
        public string FieldName { get; set; }

        protected override void SaveTo(XElement element)
        {
            base.SaveTo(element);
            if (element.Attribute("type") != null)
                element.Attribute("type").SetValue("Lookup");
            else
                element.Add(new XAttribute("type", "Lookup"));

            element.Add(new XAttribute("list", ListName),
                new XAttribute("view",this.ViewName),
                new XAttribute("rel",this.FieldName));
        }

        /// <summary>
        /// Load properties from xml
        /// </summary>
        /// <param name="element">The XElement object.</param>
        /// <param name="locale">The locale name.</param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = 7;

            //this.DefaultValue = element.StrAttr(DEFAULT);
            this.ListName = element.StrAttr("list");
            this.ViewName = element.StrAttr("view");
            this.FieldName = element.StrAttr("rel");
        }
    }
}
