//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)


namespace DNA.Web
{
    /// <summary>
    /// Represent a Tag
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Gets/Sets ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the tag name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the tag locale name.
        /// </summary>
        public string Locale { get; set; }
    }
}
