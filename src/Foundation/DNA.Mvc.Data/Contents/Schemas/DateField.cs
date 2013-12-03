//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a date time field.
    /// </summary>
    public class DateField : ContentField
    {
        public const string SHOW_TIME="showTime";
        
        /// <summary>
        /// Initializes a new instance of the DateField class
        /// </summary>
        public DateField() { FieldType = 9; }

        /// <summary>
        /// Gets/Sets the formate datetime string
        /// </summary>
        public string FormatString { get; set; }

        public override Type SystemType
        {
            get
            {
                return typeof(DateTime);
            }
        }

        /// <summary>
        /// Gets/Sets whether the show time
        /// </summary>
        public bool ShowTime { get; set; }

        public DateTime Parse(object value)
        {
            var val = (!(value is DBNull) && value != null) ? value : null;

            if (!(value is DBNull) && value != null)
            {
                DateTime date = DateTime.MinValue;
                if (val is DateTime)
                    date = (DateTime)val;
                else
                {
                    if ((val as String) != null)
                        DateTime.TryParse((string)val, out date);
                }
                return date;
            }
            return DateTime.MinValue;
        }

        public override string Format(object value)
        {
            var val = (!(value is DBNull) && value != null) ? value : DefaultValue;

            if (!(value is DBNull) && value != null)
            {
                DateTime date = DateTime.MinValue;
                if (val is DateTime)
                    date = (DateTime)val;
                else
                {
                    if ((val as String) != null)
                        DateTime.TryParse((string)val, out date);
                }

                if (date != DateTime.MinValue)
                {
                    if (!string.IsNullOrEmpty(FormatString))
                        return date.ToString(FormatString);
                    else
                        return date.ToString();
                }

            }
            return "-";

        }

        public override string ToXmlValue(object value)
        {
            if (value != null)
            {
                DateTime dateVal = (DateTime)value;
                return dateVal.ToString("O");
            }
            return base.ToXmlValue(value);
        }

        protected override void SaveTo(XElement element)
        {
            element.Add(new XAttribute("type", "DateTime"));
            if (this.DefaultValue != null)
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));
            if (!string.IsNullOrEmpty(FormatString))
                element.Add(new XAttribute(FORMAT, FormatString));
            element.Add(new XAttribute(SHOW_TIME, this.ShowTime));
        }

        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            FieldType = 9;
            var defaultDateStr = element.StrAttr(DEFAULT);
            if (!string.IsNullOrEmpty(defaultDateStr))
            { 
                DateTime defaultDate=DateTime.MinValue;
                if (DateTime.TryParse(defaultDateStr, out defaultDate))
                    this.DefaultValue = defaultDate;
            }

            this.FormatString = element.StrAttr(FORMAT);
            this.ShowTime = element.BoolAttr(SHOW_TIME);
        }
    }
}
