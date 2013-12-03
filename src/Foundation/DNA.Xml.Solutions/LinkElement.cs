//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    [Serializable]
    public class LinkElement : ICloneable
    {
        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        [XmlAttribute("target")]
        public string Target = "_self";

        [XmlAttribute("title")]
        public string Title = "";

        [XmlAttribute("src")]
        public string Source = "";

        [XmlAttribute("rel")]
        public string Rel = "nofollow";

        public LinkElement Clone()
        {
            return new LinkElement()
            {
                Language = this.Language,
                Target = this.Target,
                Title = this.Title,
                Source = this.Source,
                Rel = this.Rel
            };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
