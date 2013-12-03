//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.Events
{
    /// <summary>
    /// Represent a event name binder attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BindToAttribute:Attribute
    {
        public BindToAttribute(string eventName) { this.EventName = eventName; }

        /// <summary>
        /// Gets/Sets the binding event name.
        /// </summary>
        public string EventName { get; set; }
    }
}
