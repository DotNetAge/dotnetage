//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Data.Schema
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LookupFieldAttribute:FieldAttribute 
    {      
        /// <summary>
        ///  Initializes a new instance of the FieldAttribute class with field name.
        /// </summary>
        /// <param name="name">The field name.</param>
        public LookupFieldAttribute(string name):base(name) { }

        public LookupFieldAttribute(string name, string title) : base(name, title) { }

        /// <summary>
        /// Gets/Sets the list name or id where the field lookup from
        /// </summary>
        public string List { get; set; }

        /// <summary>
        /// Gets/Sets which field value from lookup and display
        /// </summary>
        public string KeyField { get; set; }
    }
}
