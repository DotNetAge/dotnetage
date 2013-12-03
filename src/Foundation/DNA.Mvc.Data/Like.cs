//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Represents a like class.
    /// </summary>
    public class Like
    {
        /// <summary>
        /// Gets/Sets ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the refer uri.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets/Sets the owner user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets/Sets the creation date of the like.
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
