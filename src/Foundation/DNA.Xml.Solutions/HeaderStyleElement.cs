//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    public class HeaderStyleElement : ICloneable
    {
        [XmlText]
        public string Text;

        [XmlAttribute("hidden")]
        public bool Hidden;

        [XmlAttribute("class")]
        public string CssClass;

        public HeaderStyleElement Clone()
        {
            return new HeaderStyleElement
            {
                Text = this.Text,
                Hidden = this.Hidden,
                CssClass = this.CssClass
            };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
