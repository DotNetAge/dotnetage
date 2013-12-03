//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a field reference object that use to define the view fields.
    /// </summary>
    public class ContentFieldRef : ContentField
    {
        private string cssClass = "";

        /// <summary>
        /// Gets/Sets the view field template
        /// </summary>
        public ContentTemplate Template { get; set; }

        /// <summary>
        /// Gets the feed item element name.
        /// </summary>
        public string ToFeedItemField { get; set; }

        /// <summary>
        /// Gets the parent view object.
        /// </summary>
        public ContentViewDecorator ParentView { get; private set; }

        /// <summary>
        /// Gets the parent list object.
        /// </summary>
        public new ContentListDecorator Parent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ContentFieldRef class.
        /// </summary>
        /// <param name="parent">The parent view object.</param>
        /// <param name="field">The content field.</param>
        public ContentFieldRef(ContentViewDecorator parent, ContentField field)
        {
            this.ParentView = parent;
            this.Parent = parent.Parent;
            field.CopyTo(this, "Parent");
            isFilterable = field.IsFilterable;
            this.Field = field;
            Template = new ContentTemplate();
            if (field.HasViewTemplate)
                Template.Source = field.ViewTemplate;
        }

        /// <summary>
        /// Gets the orginial content field object.
        /// </summary>
        public ContentField Field { get; private set; }

        /// <summary>
        /// Gets the css class names that use in view output.
        /// </summary>
        public string CssClass
        {
            get
            {
                if (string.IsNullOrEmpty(cssClass))
                {
                    cssClass = "d-view-field d-" + this.FieldTypeString.ToLower() + "-field " + this.Name;
                    if (Field.IsLinkToItem)
                        cssClass = cssClass + " d-link-to-item";
                }

                return cssClass;
            }
        }

        /// <summary>
        /// Gets the css class name by specified content query result item. 
        /// </summary>
        /// <param name="dataItem">The content query result item.</param>
        /// <returns>A string contains css class names.</returns>
        public string GetCssClass(ContentQueryResultItem dataItem) 
        {
            var cls = CssClass;
            if (this.FieldType == (int)ContentFieldTypes.Number || FieldType == (int)ContentFieldTypes.Integer || FieldType == (int)ContentFieldTypes.Currency)
            {
                if (dataItem[Name]==DBNull.Value || Convert.ToInt32(dataItem[Name]) == 0)
                {
                    cls += " d-view-field-val-zero";
                }
            }
            return cls;
        }

        /// <summary>
        /// Gets/Sets whether the data view show the field title label
        /// </summary>
        public bool ShowLabel { get; set; }

        private bool isFilterable = true;

        /// <summary>
        /// Gets the field can be seach.
        /// </summary>
        public override bool IsFilterable
        {
            get
            {
                return isFilterable;
            }
        }

    }
}
