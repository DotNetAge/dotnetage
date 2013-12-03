//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web
{
    /// <summary>
    /// Represents a Vote class.
    /// </summary>
    public class Vote
    {
        /// <summary>
        /// Gets/Sets ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the object id.
        /// </summary>
        public string ObjectID { get; set; }

        /// <summary>
        /// Gets/Sets the owner user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets/Sets the vote value.
        /// </summary>
        public int Value { get; set; }
    }
}
