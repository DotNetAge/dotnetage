//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a file field.
    /// </summary>
    public class FileField : ContentField
    {
        /// <summary>
        /// Initializes a new instance of the FieldField class.
        /// </summary>
        public FileField() { FieldType = 11; }

        /// <summary>
        /// Gets/Sets the file url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Save FileField properties to xml
        /// </summary>
        /// <param name="element"></param>
        protected override void SaveTo(XElement element)
        {
            element.Add(new XAttribute("type", "File"));
            if (this.DefaultValue != null)
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));
        }

        /// <summary>
        /// Load file field from xml
        /// </summary>
        /// <param name="element"></param>
        /// <param name="locale"></param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = 11;
            this.DefaultValue = element.StrAttr(DEFAULT);
        }
    }
}
