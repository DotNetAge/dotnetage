//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Globalization;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent an integer field.
    /// </summary>
    public class IntegerField : ContentField
    {
        /// <summary>
        /// Initializes a new instance of IntegerField class.
        /// </summary>
        public IntegerField() { FieldType = 4; }

        /// <summary>
        /// Gets/Sets the minimum value.
        /// </summary>
        public int MinimumValue { get; set; }

        /// <summary>
        /// Gets/Sets the maximum value.
        /// </summary>
        public int MaximumValue { get; set; }

        /// <summary>
        /// Gets the system type of the IntegerField.
        /// </summary>
        public override Type SystemType
        {
            get
            {
                return typeof(Int32);
            }
        }

        /// <summary>
        /// Format the integer value to string.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The formatted integer string.</returns>
        public override string Format(object value)
        {

            var val = (!(value is DBNull) && value != null) ? value : DefaultValue;
            if (val == null || val is DBNull)
                val = 0;

            return (Convert.ToInt32(val)).ToString("G", CultureInfo.CurrentUICulture.NumberFormat);
        }

        /// <summary>
        /// Convert the integer value to xml string.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The xml formatted string.</returns>
        public override string ToXmlValue(object value)
        {
            if (value != null)
                return Convert.ToInt32(value).ToString();
            return "0";
        }

        protected override void SaveTo(XElement element)
        {
            element.Add(new XAttribute("type", "Integer"));
            if (this.DefaultValue != null)
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));
            if (MinimumValue > 0)
                element.Add(new XAttribute(MINIMUM, this.MinimumValue));
            if (MaximumValue > 0)
                element.Add(new XAttribute(MAXIMUM, this.MaximumValue));
        }

        /// <summary>
        /// Load IntegerField properties from xml.
        /// </summary>
        /// <param name="element">The XElement object.</param>
        /// <param name="locale">The locale name.</param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = 4;
            this.DefaultValue = element.IntAttr(DEFAULT);
            this.MinimumValue = element.IntAttr(MINIMUM);
            this.MaximumValue = element.IntAttr(MAXIMUM);
        }
    }
}
