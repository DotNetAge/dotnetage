//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Data.Schema
{
    /// <summary>
    /// Represents a MicoData property the use to generate micro data attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MicroDataAttribute : Attribute
    {
        public MicroDataAttribute(string itemProp) { this.ItemProp = itemProp; }

        public MicroDataAttribute(string itemProp, string itemType) : this(itemProp) { this.ItemType = itemType; }

        /// <summary>
        /// Gets/Sets the MicroData itemprop property of the field.
        /// </summary>
        public virtual string ItemProp { get; set; }

        /// <summary>
        /// Gets/Sets the MicroData itemtype property of the field.
        /// </summary>
        public virtual string ItemType { get; set; }
    }
}
