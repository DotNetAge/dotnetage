//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Widgets
{
    public class ParamElement : ICloneable
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("value")]
        public string Value;

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        //[XmlAttribute("dir")]
        //public string Direction = "ltr";

        public ParamElement Clone()
        {
            return (ParamElement)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            return new ParamElement()
            {
                Name = this.Name,
                Value = this.Value,
                Language = this.Language
                //,Direction = this.Direction
            };
        }
    }
}
