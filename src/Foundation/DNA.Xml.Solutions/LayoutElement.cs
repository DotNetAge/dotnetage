//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    public class LayoutElement : ICloneable
    {
        [XmlText]
        public string Text;

        [XmlAttribute("name")]
        public string Name;

        public LayoutElement Clone()
        {
            return new LayoutElement()
            {
                Text = this.Text,
                Name = this.Name
            };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
