//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Globalization;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a number field.
    /// </summary>
    public class NumberField : ContentField
    {
        /// <summary>
        /// Initializes a new instance of NumberField class.
        /// </summary>
        public NumberField() { FieldType = 5; }

        /// <summary>
        /// Gets/Sets the minimum value.
        /// </summary>
        public decimal MinimumValue { get; set; }

        /// <summary>
        /// Gets/Sets the maximum value.
        /// </summary>
        public decimal MaximumValue { get; set; }

        /// <summary>
        /// Gets/Sets decimal digits.
        /// </summary>
        public int DecimalDigits { get; set; }

        /// <summary>
        /// Gets or sets the string to use as the decimal separator in currency values.
        /// </summary>
        public string DecimalSeparator { get; set; }

        /// <summary>
        /// Gets or sets the string that separates groups of digits to the left of the decimal in currency values.
        /// </summary>
        public string GroupSeparator { get; set; }

        /// <summary>
        /// Gets or sets the string to use as the percent symbol.
        /// </summary>
        public string PercentSymbol { get; set; }

        /// <summary>
        /// Indicates whether the NumberField value show as percentage.
        /// </summary>
        public bool ShowAsPercentage { get; set; }

        /// <summary>
        /// Gets the system type of the NumberField.
        /// </summary>
        public override Type SystemType
        {
            get
            {
                return typeof(decimal);
            }
        }

        /// <summary>
        /// Format the number field value to string.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The formatted string.</returns>
        public override string Format(object value)
        {
            var val = (!(value is DBNull) && value != null) ? value : DefaultValue;

            //var numFormat = CultureInfo.CurrentUICulture.NumberFormat;
            //var numFormat = new NumberFormatInfo();
            var numFormat = new CultureInfo(CultureInfo.CurrentUICulture.Name, false).NumberFormat;
            if (ShowAsPercentage)
            {
                if (DecimalDigits > -1)
                    numFormat.PercentDecimalDigits = this.DecimalDigits;
                if (!string.IsNullOrEmpty(PercentSymbol))
                    numFormat.PercentSymbol = this.PercentSymbol;
                if (!string.IsNullOrEmpty(GroupSeparator))
                    numFormat.PercentGroupSeparator = this.GroupSeparator;
                if (!string.IsNullOrEmpty(DecimalSeparator))
                    numFormat.PercentDecimalSeparator = this.DecimalSeparator;
            }
            else
            {
                if (DecimalDigits > -1)
                    numFormat.NumberDecimalDigits = DecimalDigits;
                if (!string.IsNullOrEmpty(GroupSeparator))
                    numFormat.NumberGroupSeparator = this.GroupSeparator;
                if (!string.IsNullOrEmpty(DecimalSeparator))
                    numFormat.NumberDecimalSeparator = this.DecimalSeparator;
            }
            var _format = ShowAsPercentage ? "P" : "N";
            if (!(value is DBNull) && value != null)
            {
                var dVal = Convert.ToDecimal(val);
                return dVal.ToString(_format, numFormat);
            }

            decimal emptyVal = 0;
            return emptyVal.ToString(_format, numFormat);
        }

        /// <summary>
        /// Overrided. Convert to xml string value.
        /// </summary>
        /// <param name="value">The field value .</param>
        /// <returns>The xml formatted string.</returns>
        public override string ToXmlValue(object value)
        {
            if (value != null)
                return Convert.ToDecimal(value).ToString();
            return "0.00";
        }

        protected override void SaveTo(XElement element)
        {
            XNamespace ns = ContentList.DefaultNamespace;

            element.Add(new XAttribute("type", "Number"));
            if (this.DefaultValue != null)
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));

            var numberElement = new XElement(ns+NUMBER_FROMAT, new XAttribute(SHOW_AS_PERCENTAGE, ShowAsPercentage));

            if (MinimumValue > 0)
                numberElement.Add(new XAttribute(MINIMUM, this.MinimumValue));

            if (MaximumValue > 0)
                numberElement.Add(new XAttribute(MAXIMUM, this.MaximumValue));

            if (!string.IsNullOrEmpty(this.PercentSymbol))
                numberElement.Add(new XAttribute(SYMBOL, PercentSymbol));

            if (this.DecimalDigits > 0)
                numberElement.Add(new XAttribute(DIGITS, this.DecimalDigits));

            if (!string.IsNullOrEmpty(DecimalSeparator))
                numberElement.Add(new XAttribute(DECIMAL_SPARATOR, DecimalSeparator));

            if (!string.IsNullOrEmpty(GroupSeparator))
                numberElement.Add(new XAttribute(GROUP_SEPARATOR, GroupSeparator));

            element.Add(numberElement);
        }

        /// <summary>
        /// Load properties from xml.
        /// </summary>
        /// <param name="element">The XElement</param>
        /// <param name="locale">The locale name.</param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = 5;
            this.DefaultValue = element.DecimalAttr(DEFAULT);
            this.MaximumValue = element.DecimalAttr(MAXIMUM);
            this.MinimumValue = element.DecimalAttr(MINIMUM);
            var numberFormat = element.ElementWithLocale(element.GetDefaultNamespace() + NUMBER_FROMAT, locale);

            if (numberFormat != null)
            {
                this.PercentSymbol = numberFormat.StrAttr(SYMBOL);
                this.ShowAsPercentage = numberFormat.BoolAttr(SHOW_AS_PERCENTAGE);
                this.GroupSeparator = numberFormat.StrAttr(GROUP_SEPARATOR);
                this.DecimalSeparator = numberFormat.StrAttr(DECIMAL_SPARATOR);
                this.DecimalDigits = numberFormat.IntAttr(DIGITS);
            }
        }
    }
}
