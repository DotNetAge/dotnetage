//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Globalization;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a currency field.
    /// </summary>
    public class CurrencyField : ContentField
    {
        /// <summary>
        /// Initializes a new instance of the CurrencyField class.
        /// </summary>
        public CurrencyField() { FieldType = 12; }

        /// <summary>
        /// Gets/Sets the minimum value
        /// </summary>
        public decimal MinimumValue { get; set; }

        /// <summary>
        /// Gets/Sets the maximum value.
        /// </summary>
        public decimal MaximumValue { get; set; }

        /// <summary>
        /// Gets or sets the format pattern for positive currency values.
        /// </summary>
        /// <remarks>
        /// This property has one of the values in the following table. The symbol "$" is the CurrencySymbol and n is a number.
        /// <list type=">">
        /// <item>0:$n</item> 
        /// <item>1:n$</item>
        /// <item>2:$ n</item>
        /// <item>3:n $</item>
        /// </list>
        /// </remarks>
        public int PositivePattern { get; set; }

        /// <summary>
        /// Gets or sets the string that separates groups of digits to the left of the decimal in currency values.
        /// </summary>
        public string GroupSeparator { get; set; }

        /// <summary>
        /// Gets or sets the string to use as the decimal separator in currency values.
        /// </summary>
        public string DecimalSeparator { get; set; }

        /// <summary>
        /// Gets or sets the number of decimal places to use in currency values.
        /// </summary>
        public int DecimalDigits { get; set; }

        /// <summary>
        /// Gets/Setst the default disply symbol on the front of the value.
        /// </summary>
        public string Symbol { get; set; }

        public override Type SystemType
        {
            get
            {
                return typeof(decimal);
            }
        }

        public override string Format(object value)
        {
            // var currentNumberFormat = CultureInfo.CurrentUICulture.NumberFormat;

            var val = (!(value is DBNull) && value != null) ? value : DefaultValue;
            //var numFormat = CultureInfo.CurrentUICulture.NumberFormat;
            var numFormat = new CultureInfo(CultureInfo.CurrentCulture.Name, false).NumberFormat;

            //var numFormat = new NumberFormatInfo();
            if (PositivePattern > -1)
                numFormat.CurrencyPositivePattern = PositivePattern;

            if (DecimalDigits > -1)
                numFormat.CurrencyDecimalDigits = DecimalDigits;

            if (!string.IsNullOrEmpty(this.DecimalSeparator))
                numFormat.CurrencyDecimalSeparator = DecimalSeparator;

            if (!string.IsNullOrEmpty(GroupSeparator))
                numFormat.CurrencyGroupSeparator = GroupSeparator;

            if (!string.IsNullOrEmpty(Symbol))
                numFormat.CurrencySymbol = Symbol;

            if (val != null && !(val is DBNull))
            {
                var dVal = Convert.ToDecimal(val);
                return dVal.ToString("C", numFormat);
            }

            decimal emptyVal = 0;
            return emptyVal.ToString("C", numFormat);
        }

        public override string ToXmlValue(object value)
        {
            if (!(value is DBNull) && value != null)
                return Convert.ToDecimal(value).ToString();
            return "0.00";
        }

        protected override void SaveTo(XElement element)
        {
            XNamespace ns = ContentList.DefaultNamespace;
            element.Add(new XAttribute("type", "Currency"));
            if (this.DefaultValue != null)
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));

            var numberElement = new XElement(ns + NUMBER_FROMAT);

            if (MinimumValue > 0)
                numberElement.Add(new XAttribute(MINIMUM, this.MinimumValue));
            if (MaximumValue > 0)
                numberElement.Add(new XAttribute(MAXIMUM, this.MaximumValue));

            if (!string.IsNullOrEmpty(this.Symbol))
                numberElement.Add(new XAttribute(SYMBOL, Symbol));

            if (this.DecimalDigits > 0)
                numberElement.Add(new XAttribute(DIGITS, this.DecimalDigits));

            if (!string.IsNullOrEmpty(DecimalSeparator))
                numberElement.Add(new XAttribute(DECIMAL_SPARATOR, DecimalSeparator));

            if (!string.IsNullOrEmpty(GroupSeparator))
                numberElement.Add(new XAttribute(GROUP_SEPARATOR, GroupSeparator));

            numberElement.Add(new XAttribute(POSITIVE_PATTERN, this.PositivePattern));

            element.Add(numberElement);
        }

        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = 12;
            this.DefaultValue = element.DecimalAttr(DEFAULT);
            this.MaximumValue = element.DecimalAttr(MAXIMUM);
            this.MinimumValue = element.DecimalAttr(MINIMUM);
            var numberFormat = element.ElementWithLocale(element.GetDefaultNamespace() + NUMBER_FROMAT, locale);
            if (numberFormat != null)
            {
                this.Symbol = numberFormat.StrAttr(SYMBOL);
                this.PositivePattern = numberFormat.IntAttr(POSITIVE_PATTERN);
                this.GroupSeparator = numberFormat.StrAttr(GROUP_SEPARATOR);
                this.DecimalSeparator = numberFormat.StrAttr(DECIMAL_SPARATOR);
                this.DecimalDigits = numberFormat.IntAttr(DIGITS);
            }
        }
    }

}
