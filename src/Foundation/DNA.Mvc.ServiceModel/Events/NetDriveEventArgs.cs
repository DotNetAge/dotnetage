//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.Events
{
    /// <summary>
    /// Represents a web file event argument class.
    /// </summary>
    public class WebFileEventArgs
    {
        public Uri Uri { get; set; }
    }

    /// <summary>
    /// Represents a web file rename event argument class.
    /// </summary>
    public class WebFileRenameEventArgs
    {
        /// <summary>
        /// Gets the original file uri object
        /// </summary>
        public Uri SourceUri { get; set; }

        /// <summary>
        /// Gets the destination file uri object.
        /// </summary>
        public Uri DestinationUri { get; set; }
    }
}
