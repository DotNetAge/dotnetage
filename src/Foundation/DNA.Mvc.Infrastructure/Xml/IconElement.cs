//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml
{
    public class IconElement : ICloneable
    {
        [XmlAttribute("src")]
        public string Source;

        [XmlAttribute("width")]
        public int Width;

        [XmlAttribute("height")]
        public int Height;

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        [XmlAttribute("dir")]
        public string Direction = "ltr";

        public IconElement Clone()
        {
           return new IconElement()
            {
                Width = this.Width,
                Height = this.Height,
                Source = this.Source,
                Language = this.Language,
                Direction = this.Direction
            };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
