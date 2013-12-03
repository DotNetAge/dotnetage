//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Widgets
{
    [Serializable]
    public class NameElement
    {
        [XmlAttribute("short")]
        public string ShortName = "";

        [XmlText]
        public string FullName { get; set; }

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        [XmlAttribute("dir")]
        public string Direction = "ltr";

        [XmlAttribute("resKey")]
        public string ResKey = "";

    }
}
