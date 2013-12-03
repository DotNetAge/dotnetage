//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    public class WidgetStyleElement:ICloneable
    {
        [XmlElement("box")]
        public StyleElement Box;

        [XmlElement("header")]
        public HeaderStyleElement Header;

        [XmlElement("body")]
        public StyleElement Body;

        public WidgetStyleElement Clone()
        {
            var copy = new WidgetStyleElement();
            if (Box != null)
                copy.Box = Box.Clone();

            if (Header != null)
                copy.Header = this.Header.Clone();

            if (Body != null)
                copy.Body = this.Body.Clone();

            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }

    
}
