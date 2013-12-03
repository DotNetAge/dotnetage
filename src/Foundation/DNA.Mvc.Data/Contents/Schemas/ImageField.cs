//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent an image field.
    /// </summary>
    public class ImageField : ContentField
    {
        /// <summary>
        /// Initializes a new instance of the ImageField class.
        /// </summary>
        public ImageField() { FieldType = 10; }

        public const string WIDTH = "width";

        public const string HEIGHT = "height";

        /// <summary>
        /// Indicates whether the ImageField can apply to filter.
        /// </summary>
        public override bool IsFilterable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets / Sets the wdith of the image
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets/Sets the height of the image
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets/Sets the thumbnail image width.
        /// </summary>
        public int ThumbnailWidth { get; set; }

        /// <summary>
        /// Gets/Sets the thumbnail image height.
        /// </summary>
        public int ThumbnailHeight { get; set; }

        public override Type SystemType
        {
            get
            {
                return typeof(string);
            }
        }

        protected override void SaveTo(XElement element)
        {
            element.Add(new XAttribute("type", "Image"));

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

            if (this.Width > 0)
                element.Add(new XAttribute(WIDTH, this.Width));

            if (this.Height > 0)
                element.Add(new XAttribute(HEIGHT, this.Height));

            if (this.ThumbnailWidth > 0)
                element.Add(new XAttribute("thumbWidth", this.ThumbnailWidth));

            if (this.ThumbnailHeight > 0)
                element.Add(new XAttribute("thumbHeight", this.ThumbnailHeight));


        }

        /// <summary>
        /// Load ImageField properties from xml.
        /// </summary>
        /// <param name="element">The XElement object</param>
        /// <param name="locale">The locale name.</param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = 10;
            this.DefaultValue = element.StrAttr(DEFAULT);
            this.Width = element.IntAttr(WIDTH);
            this.Height = element.IntAttr(HEIGHT);
            this.ThumbnailWidth = element.IntAttr("thumbWidth");
            this.ThumbnailHeight = element.IntAttr("thumbHeight");
        }

        /// <summary>
        /// Conver the field value to xml string.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The xml formatted string.</returns>
        public override string ToXmlValue(object value)
        {
            if (value != null && !(value is DBNull) && !string.IsNullOrEmpty((string)value))
                return string.Format("<![CDATA[{0}]]>", value);

            return (string)value;
        }
    }
}
