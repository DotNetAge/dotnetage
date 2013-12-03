//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.OpenSearch
{
    [Serializable]
    public struct OpenSearchImage
    {
        /// <summary>
        /// Contains the height, in pixels, of this image. 
        /// </summary>
        [XmlAttribute("height")]
        public int Height;

        /// <summary>
        /// Contains the width, in pixels, of this image. 
        /// </summary>
        [XmlAttribute("width")]
        public int Width;

        /// <summary>
        /// Contains the the MIME type of this image. 
        /// </summary>
        [XmlAttribute("type")]
        public string Type;

        [XmlText]
        public string Url;
    }
}
