//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    public class RefElement : ICloneable
    {
        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language="";

        [XmlAttribute("src")]
        public string Ref;

        public RefElement Clone()
        {
            var copy = new RefElement()
            {
                Language = this.Language,
                Ref = this.Ref
            };
            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
