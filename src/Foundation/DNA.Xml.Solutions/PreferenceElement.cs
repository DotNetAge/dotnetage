//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    public class PreferenceElement : ICloneable
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("value")]
        public string Value;

        public PreferenceElement Clone()
        {
            return new PreferenceElement()
               {
                   Name = this.Name,
                   Value = this.Value
               };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
