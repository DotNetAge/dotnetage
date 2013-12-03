//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Xml.Serialization;

namespace DNA.OpenSearch
{
    public enum OpenSearchSyndicationRights:short
    {
        [XmlEnum("open")]
        Open=1,
        [XmlEnum("limited")]
        Limited=2,
        [XmlEnum("private")]
        Private=3,
        [XmlEnum("closed")]
        Closed=4
    }
}
