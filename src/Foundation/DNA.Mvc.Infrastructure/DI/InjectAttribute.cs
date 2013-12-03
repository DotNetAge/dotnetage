//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Represents an Ioc attribute class contains the register info use to extract self register to unity.config file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute() { }

        public InjectAttribute(string mapTo) { this.MapTo = mapTo; }

        public InjectAttribute(string mapTo, string alias) { this.Alias = alias; this.MapTo = mapTo; }

        /// <summary>
        /// Gets /Sets the alias name for the object mapping.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets/Sets the maping base type of the object. 
        /// </summary>
        /// <remarks>
        /// If this property set to null it will not add the config to register session.
        /// </remarks>
        public string MapTo { get; set; }
    }
}
