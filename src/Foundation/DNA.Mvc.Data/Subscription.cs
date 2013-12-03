//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// The Subscription data object.
    /// </summary>
    [Obsolete]
    public class Subscription
    {
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the token key in hash code format.
        /// </summary>
        public int HashKey { get; set; }

        /// <summary>
        /// Gets/Sets the token key of the subscription.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets/Sets the subscription user name.
        /// </summary>
        public string UserName { get; set; }
    }
}
