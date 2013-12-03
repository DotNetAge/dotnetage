//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Serialization;

namespace DNA.Xml.Widgets
{
    /// <summary>
    /// Represents people or an organization attributed with the creation of the widget.
    /// </summary>
    public class AuthorElement : ICloneable
    {
        /// <summary>
        /// Represent the user name.
        /// </summary>
        [XmlText]
        public string Name;

        /// <summary>
        /// Its value represents an IRI that the author associates with himself or herself (e.g., a homepage, a profile on a social network, etc.). Usage is optional.
        /// </summary>
        [XmlAttribute("href")]
        public string Uri;

        /// <summary>
        /// A string that represents an email address associated with the author. Usage is optional.
        /// </summary>
        [XmlAttribute("email")]
        public string Email;

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        [XmlAttribute("dir")]
        public string Direction = "ltr";

        public AuthorElement Clone()
        {
            return (AuthorElement)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            var copy = new AuthorElement()
            {
                Name = this.Name,
                Direction = this.Direction,
                Email = this.Email,
                Uri = this.Uri
            };
            return copy;
        }
    }
}
