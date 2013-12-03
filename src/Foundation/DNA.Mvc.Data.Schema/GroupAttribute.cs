//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Data.Schema
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GroupAttribute:Attribute
    {
        public GroupAttribute() { }

        public GroupAttribute(string name) { this.Name = name; }

        public string Name { get; set; }
    }
}
