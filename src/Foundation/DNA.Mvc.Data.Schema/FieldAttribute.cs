//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Data.Schema
{
    /// <summary>
    /// Represents a content field definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FieldAttribute:Attribute
    {
        /// <summary>
        ///  Initializes a new instance of the FieldAttribute class with field name.
        /// </summary>
        /// <param name="name">The field name.</param>
        public FieldAttribute(string name) { this.Name = name; }

        /// <summary>
        /// Initializes a new instance of the FieldAttribute class with field name and title.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="title">The field title.</param>
        public FieldAttribute(string name, string title) : this(name) { this.Title = title; }

        /// <summary>
        /// Initializes a new instance of the FieldAttribute class with field name,title and isrequired.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="title">The field title.</param>
        /// <param name="isRquired">Whether the field is requried.</param>
        public FieldAttribute(string name, string title, bool isRquired) : this(name, title) { this.IsRequired = isRquired; }

        /// <summary>
        /// Gets/Sets the field name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the property group name. 
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets/Sets the field title display text
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the field description text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets the placeholder text
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Identity this field could not be edit.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets/Sets whether this field is hidden.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in display form.
        /// </summary>
        public bool HideInDisplayForm { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in new form.
        /// </summary>
        public bool HideInNewForm { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in activity form.
        /// </summary>
        public virtual bool HideInActForm { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in edit form.
        /// </summary>
        public bool HideInEditForm { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in all view.
        /// </summary>
        public bool HideInView { get; set; }

        /// <summary>
        /// Indicates whether the field value is requried
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Indicates whether the field is the slug seed. 
        /// </summary>
        /// <remarks>
        /// This property only availdable for : TextField, IntegerField and DateField
        /// </remarks>
        public bool IsSlug { get; set; }

        public int Privacy { get; set; }

        /// <summary>
        /// Returns the field's default value
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Specified this field render as a link and refer to the current item
        /// </summary>
        public bool IsLinkToItem { get; set; }

        /// <summary>
        /// Gets/Sets the field type value.
        /// </summary>
        public int FieldType { get; set; }

    }
}
