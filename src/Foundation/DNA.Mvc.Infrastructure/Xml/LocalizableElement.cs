//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml
{
    public class LocalizableElement : ICloneable
    {
        [XmlText]
        public string Text;

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        [XmlAttribute("dir")]
        public string Direction = "ltr";

        public LocalizableElement Clone()
        {
            return new LocalizableElement()
            {
                Text = this.Text,
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
