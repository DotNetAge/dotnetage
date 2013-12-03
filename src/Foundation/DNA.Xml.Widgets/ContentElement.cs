//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Widgets
{
    [Serializable, XmlRoot("content")]
    public class ContentElement : ICloneable
    {
        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        [XmlAttribute("dir")]
        public string Direction = "ltr";

        [XmlAttribute("src")]
        public string Source="";

        [XmlAttribute("type")]
        public string ContentType="text/html";

        [XmlAttribute("encoding")]
        public string Encoding="utf-8";

        [XmlText]
        public string Text;

        public ContentElement Clone()
        {
            return (ContentElement)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            return new ContentElement()
            {
                Language = this.Language,
                Direction = this.Direction,
                Source = this.Source,
                ContentType = this.ContentType,
                Encoding = this.Encoding,
                Text = this.Text
            };
        }
    }
}
