//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the value object to handle the field value.
    /// </summary>
    public class ContentFieldValue
    {
        /// <summary>
        /// Gets / Sets the field raw value object.
        /// </summary>
        public object Raw { get; set; }

        /// <summary>
        /// Gets / Sets the parent field object.
        /// </summary>
        public ContentField Field { get; set; }

        /// <summary>
        /// Gets / Sets the parent list object.
        /// </summary>
        public ContentListDecorator ParentList { get; set; }

        /// <summary>
        /// Gets / Sets the data item object.
        /// </summary>
        public ContentDataItemDecorator Item { get; set; }

        /// <summary>
        /// Gets the data item default link url
        /// </summary>
        public string Link { get { return Item.Url; } }

        /// <summary>
        /// Gets whether the field value is undfined in data item raw value.
        /// </summary>
        public bool Undefined
        {
            get { return Field == null; }
        }

        /// <summary>
        /// Gets whether the field value is null.
        /// </summary>
        public bool IsNull
        {
            get
            {
                if (Undefined)
                    return true;

                return Raw == null || (Raw != null && string.IsNullOrEmpty(Raw.ToString()));
            }
        }

        /// <summary>
        /// Gets the field default value.
        /// </summary>
        public object Default
        {
            get {
                return this.Field.DefaultValue;
            }
        }

        /// <summary>
        /// Gets the field value formatted string.
        /// </summary>
        public string Formatted
        {
            get
            {
                return Field.Format(Raw);
            }
        }
    }
}
