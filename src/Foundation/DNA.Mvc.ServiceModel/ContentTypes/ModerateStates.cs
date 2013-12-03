//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// This enumeration has a moderate flag.
    /// </summary>
    public enum ModerateStates
    {
        /// <summary>
        /// Pending for review.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Reject and now allow publish.
        /// </summary>
        Rejected = 1,

        /// <summary>
        /// Approved and allow publish.
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Not set flag.
        /// </summary>
        Notset = -1
    }
}
