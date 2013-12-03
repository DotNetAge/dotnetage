//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Represents a category.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets / Sets ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets parent category id.
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// Gets/Sets the category name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the category description text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets the parent web id.
        /// </summary>
        public int WebID { get; set; }

        /// <summary>
        /// Gets/Sets the locale name.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets/Sets the category path.
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// Gets/Sets the category image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets / Sets content dataitems under this category.
        /// </summary>
        public virtual ICollection<ContentDataItem> ContentDataItems { get; set; }
    }
}
