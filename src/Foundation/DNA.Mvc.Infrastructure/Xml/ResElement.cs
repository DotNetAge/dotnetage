//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml
{
    public class ResElement:ICloneable
    {
        [XmlAttribute("src")]
        public string Source { get; set; }

        [XmlText]
        public string Text { get; set; }

        public virtual ResElement Clone()
        {
            var copy = new ResElement()
            {
                Source = this.Source,
                Text = this.Text
            };
            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
