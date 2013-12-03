//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.UI
{
    /// <summary>
    /// Represents an class that defines the widget user preference
    /// </summary>
    public class PropertyDescriptor
    {
        private string dispName;
        private Type valueType = typeof(string);
        private ControlTypes propertyControl = ControlTypes.TextBox;
        private bool isReadonly = false;

        /// <summary>
        /// Gets/Sets the user preference whether is readonly
        /// </summary>
        public bool IsReadonly
        {
            get { return isReadonly; }
            set { isReadonly = value; }
        }

        /// <summary>
        /// Gets/Sets the user preference render control
        /// </summary>
        public ControlTypes PropertyControl
        {
            get { return propertyControl; }
            set { propertyControl = value; }
        }
        
        /// <summary>
        /// Gets/Sets the type of the user preference value
        /// </summary>
        public Type ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        }

        /// <summary>
        /// Gets/Sets the property display name.
        /// </summary>
        public string DisplayName 
        { 
            get
            {
                return string.IsNullOrEmpty(dispName) ? Name : dispName;
            }
            set 
            {
                dispName = value;
            } 
        }
        
        /// <summary>
        /// Gets/Sets the property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the property value.
        /// </summary>
        public object Value { get; set; }
    }
}
