//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represests the view field value object.
    /// </summary>
    public class ContentViewFieldValue
    {
        /// <summary>
        /// Initializes a new instance of the ContentViewFieldValue class with field and item.
        /// </summary>
        /// <param name="field">The view field object</param>
        /// <param name="item">The query result item.</param>
        public ContentViewFieldValue(ContentFieldRef field, ContentQueryResultItem item)
        {
            this.Item = item;
            Field = field;
        }

        /// <summary>
        /// Gets the parent query result item.
        /// </summary>
        public ContentQueryResultItem Item { get; private set; }

        /// <summary>
        /// Gets the parent view field object.
        /// </summary>
        public ContentFieldRef Field { get; private set; }

        /// <summary>
        /// Gets the field name.
        /// </summary>
        public string FieldName { get { return Field.Name; } }

        /// <summary>
        /// Gets the field raw value.
        /// </summary>
        public object Value
        {
            get
            {
                return Item[FieldName];
            }
        }

        /// <summary>
        /// Indicates whether the field value is null.
        /// </summary>
        public bool IsNull
        {
            get
            {
                if (Value is DBNull)
                    return true;
                else
                {
                    if (Value == null)
                        return true;
                    else
                        return string.IsNullOrEmpty(Value.ToString());
                }
            }
        }

        /// <summary>
        /// Gets the css class name of the view field.
        /// </summary>
        public string CssClass
        {
            get { return this.Field.FieldTypeString.ToLower() + " " + FieldName.ToLower(); }
        }
    }
}
