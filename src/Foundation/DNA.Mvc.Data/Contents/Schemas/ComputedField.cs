//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Linq;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a computed filed.
    /// </summary>
    public class ComputedField : ContentField
    {
        public const string FIELDREF = "fieldRef";
        public const string PATTERN = "dispPattern";
        
        /// <summary>
        /// Initializes a new instance of the ComputedField.
        /// </summary>
        public ComputedField() { FieldType = 6; }

        /// <summary>
        /// Indicates whether the ComputedField can apply filter. 
        /// </summary>
        public override bool IsFilterable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Idicates whether the ComputedField is readonly.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        /// <summary>
        /// Idicates whether no handle the field value on data item save.
        /// </summary>
        public override bool IsIngored
        {
            get
            {
                return true;
            }

        }

        /// <summary>
        /// Gets/Sets the reference fields.
        /// </summary>
        public string[] FieldRefs { get; set; }

        /// <summary>
        /// Gets/Sets the field display pattern.
        /// </summary>
        public string DispPattern { get; set; }

        protected override void SaveTo(XElement element)
        {
            XNamespace ns = ContentList.DefaultNamespace;
            element.Add(new XAttribute("type", "Computed"));
            if (this.DefaultValue != null)
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));

            if (FieldRefs != null && FieldRefs.Count() > 0)
            {
                foreach (var field in FieldRefs)
                    element.Add(new XElement(ns+FIELDREF, new XAttribute(NAME, field)));
            }

            if (!string.IsNullOrEmpty(DispPattern))
                element.Add(XElement.Parse(DispPattern));
        }

        public override void Load(XElement element, string locale = "")
        {
            var ns = element.GetDefaultNamespace();

            base.Load(element, locale);
            this.FieldType = 6;
            var fieldRefs = element.Elements(ns+FIELDREF);
            if (fieldRefs != null && fieldRefs.Count() > 0)
                this.FieldRefs = fieldRefs.Select(f => f.StrAttr(NAME)).ToArray();

            var dispPattern = element.Element(ns+PATTERN);
            if (dispPattern != null)
                DispPattern = dispPattern.OuterXml();

            this.DefaultValue = element.StrAttr(DEFAULT);
        }
        
    }

}
