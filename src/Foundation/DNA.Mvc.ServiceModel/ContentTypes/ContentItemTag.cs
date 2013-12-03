//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the tagged object
    /// </summary>
    public class ContentItemTag
    {
        private ContentListDecorator _parent;

        /// <summary>
        /// The parent content list object.
        /// </summary>
        public ContentListDecorator Parent
        {
            get { return _parent; }
            private set { _parent = value; }
        }

        /// <summary>
        /// Initializes a new instance of the ContentItemTag class with content list.
        /// </summary>
        /// <param name="parent">The parent content list.</param>
        public ContentItemTag(ContentListDecorator parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the url of this tag.
        /// </summary>
        public string Url
        {
            get
            {
                return string.Format("~/{0}/{1}/{2}/tags/{3}.html", Parent.Web.Name, Parent.Locale, Parent.Name,this.Name);
            }
        }

        /// <summary>
        /// Gets / Sets the tag name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the data item count of this tag.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets the total tagged items count.
        /// </summary>
        public int TotalTags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Ratio
        {
            get
            {
                if (TotalTags > 0)
                    return ((decimal)Count / (decimal)TotalTags)*100;
                return 0;
            }
        }
    }
}
