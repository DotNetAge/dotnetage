///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.Sitemap
{
    [XmlRoot("sitemap")]
    public struct Sitemap
    {
        [XmlElement("loc")]
        public string Location;

        [XmlElement("lastmod")]
        public string LastModified;
    }
}