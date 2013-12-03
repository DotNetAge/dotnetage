//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Data.Schema
{
    /// <summary>
    /// Represents a content type definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ListAttribute : Attribute
    {
        /// <summary>
        ///  Initializes a new instance of the ListAttribute class with field name.
        /// </summary>
        /// <param name="name">The list name.</param>
        public ListAttribute(string name) { this.Name = name; }

        /// <summary>
        /// Initializes a new instance of the ListAttribute class with field name and title.
        /// </summary>
        /// <param name="name">The list name.</param>
        /// <param name="title">The list title.</param>
        public ListAttribute(string name, string title) : this(name) { this.Title = title; }

        /// <summary>
        /// Gets / Sets the base content type name.
        /// </summary>
        public string BaseName { get; set; }

        /// <summary>
        /// Gets/Sets the list name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the list title display text
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the list description text.
        /// </summary>
        public string Description { get; set; }
    }
}
