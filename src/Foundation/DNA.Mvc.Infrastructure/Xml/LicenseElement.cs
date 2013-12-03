//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml
{
    public class LicenseElement : ICloneable
    {
        [XmlText]
        public string Text;

        [XmlAttribute("href")]
        public string Source;

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        [XmlAttribute("dir")]
        public string Direction = "ltr";

        public LicenseElement Clone()
        {
            return (LicenseElement)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            return new LicenseElement()
            {
                Text = this.Text,
                Source = this.Source,
                Language = this.Language,
                Direction = this.Direction
            };
        }
    }
}
