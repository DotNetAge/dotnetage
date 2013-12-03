//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represents a text field.
    /// </summary>
    public class TextField : ContentField
    {
        /// <summary>
        /// Initializes a new instances of the TextField class.
        /// </summary>
        public TextField() { FieldType = 1; }

        public const string MAX_LEN = "maxlength";
        public const string MIN_LEN = "minlength";

        /// <summary>
        /// Gets/Sets the max length of the field value.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets/Sets the min length of the field value.
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Overrided. Convert the field value to xml format.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The xml formatted string.</returns>
        public override string ToXmlValue(object value)
        {
            if (value != null)
                //return string.Format("<![CDATA[{0}]]>", value);
                return System.Web.HttpUtility.HtmlEncode(value);
            return base.ToXmlValue(value);
        }

        protected override void SaveTo(XElement element)
        {
            element.Add(new XAttribute("type", "Text"));

            try
            {
                if (this.DefaultValue != null)
                {
                    if (this.DefaultValue.GetType() == typeof(string[]))
                    {
                        var vals = (string[])this.DefaultValue;
                        if (vals.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(vals[0]))
                                element.Add(new XAttribute(DEFAULT, vals[0]));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(this.DefaultValue.ToString()))
                            element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));
                    }
                }
            }
            catch
            {
            }

            if (MinLength > 0)
                element.Add(new XAttribute(MIN_LEN, this.MinLength));

            if (MaxLength > 0)
                element.Add(new XAttribute(MAX_LEN, this.MaxLength));
        }

        /// <summary>
        /// Overrided. Load properties from xml.
        /// </summary>
        /// <param name="element">The XElement object.</param>
        /// <param name="locale">The locale name.</param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = 1;

            //if (this.DefaultValue != null && !string.IsNullOrEmpty(this.DefaultValue.ToString()))
            this.DefaultValue = element.StrAttr(DEFAULT);

            //if (MinLength > 0)
            this.MinLength = element.IntAttr(MIN_LEN);

            //if (MaxLength > 0)
            this.MaxLength = element.IntAttr(MAX_LEN);
        }
    }
}
