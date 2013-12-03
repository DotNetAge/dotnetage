//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Globalization;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a archive for content item.
    /// </summary>
    public class ContentItemArchive
    {
        private ContentListDecorator _parent;
        private string displayText;

        /// <summary>
        /// Gets the parent list
        /// </summary>
        public ContentListDecorator Parent
        {
            get { return _parent; }
            private set { _parent = value; }
        }

        /// <summary>
        /// Initializes a new instance of the ContentItemArchive class with content list object.
        /// </summary>
        /// <param name="parent"></param>
        public ContentItemArchive(ContentListDecorator parent)
        {
            this.Parent = parent;
        }
        
        /// <summary>
        /// Gets the content items count of this archive.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets the url of this archive.
        /// </summary>
        public string Url 
        {
            get
            {
                var monthStr = Month.ToString();
                if (monthStr.Length == 1)
                    monthStr = "0" + monthStr;

                return string.Format("~/{0}/{1}/{2}/archives/{3}-{4}.html", Parent.Web.Name, Parent.Locale, Parent.Name, this.Year, monthStr);
            }
        }

        /// <summary>
        /// Gets / Sets the display text of the archive.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (string.IsNullOrEmpty(displayText))
                    return Year.ToString() + "-" + Month.ToString();
                return displayText;
            }
            set { displayText = value; }
        }

        /// <summary>
        /// Gets/Sets the year.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets/Sets the month.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Gets the month display name.
        /// </summary>
        public string MonthName
        {
            get
            {
                return DateTimeFormatInfo.CurrentInfo.GetMonthName(Month);
            }
        }
    }
}
