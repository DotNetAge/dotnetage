//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent the boolean type data field
    /// </summary>
    public class BooleanField : ContentField
    {
        public const string YESTEXT = "yesText";
        public const string NOTEXT = "noText";
        string yesText = "Yes";
        string noText = "No";

        public BooleanField() { FieldType = 8; }

        /// <summary>
        /// Gets / Sets the display text when the field value is set to "yes" (true)
        /// </summary>
        public string YesText
        {
            get { return yesText; }
            set { yesText = value; }
        }

        /// <summary>
        /// Gets / Sets the display text when the field value is set to "no" (false)
        /// </summary>
        public string NoText
        {
            get { return noText; }
            set { noText = value; }
        }

        public override Type SystemType
        {
            get
            {
                return typeof(bool);
            }
        }

        /// <summary>
        /// Format the field value to xml string value.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The xml formatted value.</returns>
        public override string ToXmlValue(object value)
        {
            if (value != null)
                return base.ToXmlValue(value).ToLower();
            return Boolean.FalseString.ToLower();
        }

        /// <summary>
        /// Format Boolean field value to string.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The formatted field valule.</returns>
        public override string Format(object value)
        {
            var val =( value != null && !(value is DBNull))? value : DefaultValue;

            if (val != null && !(val is DBNull))
            {
                var valStr = val.ToString();
                var valBool = false;
                if (Boolean.TryParse(valStr, out valBool))
                {
                    return valBool ? YesText : NoText;
                }
            }

            return NoText;
        }

        /// <summary>
        /// Save boolean field properties to xml element.
        /// </summary>
        /// <param name="element">The xml element object.</param>
        protected override void SaveTo(XElement element)
        {
            element.Add(new XAttribute("type", "Boolean"));
            element.Add(new XAttribute(YESTEXT, yesText));
            element.Add(new XAttribute(NOTEXT, noText));
            if (this.DefaultValue != null)
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString().ToLower()));
        }

        /// <summary>
        /// Load properties from xelement object.
        /// </summary>
        /// <param name="element">The XElement object.</param>
        /// <param name="locale">The locale name.</param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);

            this.FieldType = 8;

            this.DefaultValue = element.BoolAttr(DEFAULT);
            
            if (!string.IsNullOrEmpty(element.StrAttr(YESTEXT)))
                this.YesText = element.StrAttr(YESTEXT);
            
            if (!string.IsNullOrEmpty(element.StrAttr(NOTEXT)))
                this.NoText = element.StrAttr(NOTEXT);

        }
    }
}
