//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a note field for mulitlines text.
    /// </summary>
    public class NoteField : TextField
    {
        public NoteField() { FieldType = 2; LengthLimit = 255; }

        public const string HTML = "isHtml";
        public const string NUM_LINES = "numlines";
        public const string LENLITIM = "lenLimit";

        /// <summary>
        /// Gets/Sets the number of lines would be display in text editor.
        /// </summary>
        public int NumLines { get; set; }

        /// <summary>
        /// Gets/Sets the numbers of letter can display in view. 
        /// </summary>
        public int LengthLimit { get; set; }

        /// <summary>
        /// Gets/Sets whether the field value is html text format
        /// </summary>
        public bool IsHtml { get; set; }

        /// <summary>
        /// Convert the field value to xml string.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The xml formatted string.</returns>
        public override string ToXmlValue(object value)
        {
            if (value != null)
            {
                //if (IsHtml)
                return string.Format("<![CDATA[{0}]]>", value);
                //else
                // return System.Web.HttpUtility.HtmlEncode(value);
            }

            return base.ToXmlValue(value);
        }

        protected override void SaveTo(XElement element)
        {
            if (element.Attribute("type") != null)
                element.Attribute("type").SetValue("Note");
            else
                element.Add(new XAttribute("type", "Note"));

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

            element.Add(new XAttribute(HTML, IsHtml));

            if (this.NumLines > 0)
                element.Add(new XAttribute(NUM_LINES, this.NumLines));

            if (this.LengthLimit > 0 && this.LengthLimit != 255)
                element.Add(new XAttribute(LENLITIM, this.LengthLimit));
        }

        /// <summary>
        /// Load properties from xml.
        /// </summary>
        /// <param name="element">The XElement object.</param>
        /// <param name="locale">The locale name.</param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = 2;
            this.DefaultValue = element.StrAttr(DEFAULT);
            this.IsHtml = element.BoolAttr(HTML);
            this.NumLines = element.IntAttr(NUM_LINES);
            this.LengthLimit = element.IntAttr(LENLITIM);
            if (this.LengthLimit == 0)
                this.LengthLimit = 255;
        }
    }
}
