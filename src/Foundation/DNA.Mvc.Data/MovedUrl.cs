//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Represent a renamed or moved url.
    /// </summary>
    public class MovedUrl
    {
        /// <summary>
        /// Gets/Sets ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the web ID.
        /// </summary>
        public int WebID { get; set; }

        /// <summary>
        /// Gets/Sets the original url.
        /// </summary>
        public string OriginalUrl { get; set; }

        /// <summary>
        /// Gets/Sets the new url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets/Sets the creation date.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets/Sets parent web object.
        /// </summary>
        public virtual Web Web { get; set; }
    }
}
