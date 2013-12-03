//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.EmbeddedViews
{
    /// <summary>
    /// Represents meta data object for embeded view.
    /// </summary>
    [Serializable]
    public class EmbeddedViewMetadata
    {
        /// <summary>
        /// Gets/Sets the embedded view name.
        /// </summary>
        /// <remarks>
        /// eg:DNA.Store.Views.Seller.Order.cshtml
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the embeded view assembly full name
        /// </summary>
        public string AssemblyFullName { get; set; }
    }
}
