//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represents a user field.
    /// </summary>
    public class UserField : TextField
    {
        /// <summary>
        /// Overrided. Gets the system type of the UserField.
        /// </summary>
        public override Type SystemType
        {
            get
            {
                return typeof(string);
            }
        }

        /// <summary>
        ///Gets/Sets the user photo thumbnail width.
        /// </summary>
        public int ThumbnailWidth { get; set; }

        /// <summary>
        /// Gets/Sets the user photo thumbnail height.
        /// </summary>
        public int ThumbnailHeight { get; set; }

        protected override void SaveTo(System.Xml.Linq.XElement element)
        {
            element.Add(new XAttribute("type", "User"));
            if (this.DefaultValue != null && !string.IsNullOrEmpty(this.DefaultValue.ToString()))
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));
            if (DisplayStyle != UserDisplayStyles.PhotoOnly)
                element.Add(new XAttribute("dispAs", this.DisplayStyle.ToString()));
            this.ThumbnailWidth = element.IntAttr("thumbWidth");
            this.ThumbnailHeight = element.IntAttr("thumbHeight");
        }

        /// <summary>
        /// Gets/Sets the user field display style.
        /// </summary>
        public UserDisplayStyles DisplayStyle { get; set; }

        /// <summary>
        /// Overrided.Load proeprties from xml.
        /// </summary>
        /// <param name="element">The XElement object.</param>
        /// <param name="locale">The locale name.</param>
        public override void Load(XElement element, string locale = "")
        {
            base.Load(element, locale);
            this.FieldType = (int)ContentFieldTypes.User;
            this.DefaultValue = element.StrAttr(DEFAULT);
            var dispAsStr = element.StrAttr("dispAs");
            if (!string.IsNullOrEmpty(dispAsStr))
                this.DisplayStyle = (UserDisplayStyles)Enum.Parse(typeof(UserDisplayStyles), dispAsStr);

            if (this.ThumbnailWidth > 0)
                element.Add(new XAttribute("thumbWidth", this.ThumbnailWidth));

            if (this.ThumbnailHeight > 0)
                element.Add(new XAttribute("thumbHeight", this.ThumbnailHeight));
        }
    }

    /// <summary>
    /// This enumeration has a user field display styles.
    /// </summary>
    public enum UserDisplayStyles
    {
        /// <summary>
        /// Show user photo only.
        /// </summary>
        PhotoOnly = 0,
        /// <summary>
        /// Show the user photo and user display name
        /// </summary>
        PhotoAndName = 1,
        /// <summary>
        /// Show the user name only
        /// </summary>
        NameOnly = 2
    }
}
