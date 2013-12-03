//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.UI
{
    /// <summary>
    /// Define a user preference for widget.
    /// </summary>
    /// <example>
    ///  <para>In Action define the method parameter that has the name same as the definition.The following example shows how to access the UserPreference value in Action Method.</para>
    /// <code language="cs">
    ///  [Widget("NotePad")]
    ///  [Property("Text")]
    ///  public ActionResult NotePad(string text)
    ///  {
    ///      if (string.IsNullOrEmpty(text))
    ///         ViewData["Message"]="Please set the text first.";
    ///      return View();
    ///  }
    /// </code>
    /// <para>
    ///   The View file needs inhert from <c> DNA.Web.DynamicUI.WidgetViewUserControl</c> to access the user preference in <c>UserData</c> property
    /// </para>
    /// <code language="html">
    /// &lt;%:@ Control Language="C#" Inherits="DNA.Web.DynamicUI.WidgetViewUserControl" %&gt;
    /// &lt;%: UserData["Text"] %&gt;
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PropertyAttribute : Attribute
    {
        private string dispName;
        private Type valueType = typeof(string);
        private ControlTypes propertyControl = ControlTypes.TextBox;
        private bool isreadonly = false;

        /// <summary>
        /// Gets/Sets the user preference is readonly
        /// </summary>
        public bool IsReadonly
        {
            get { return isreadonly; }
            set { isreadonly = value; }
        }

        /// <summary>
        /// Initializes a new instance of the PropertyAttribute class by specified property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        public PropertyAttribute(string name) { Name = name; }

        /// <summary>
        ///  Initializes a new instance of  the PropertyAttribute class by specified property name and displayName.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="displayName">The property display name.</param>
        public PropertyAttribute(string name, string displayName) { Name = name; DisplayName = displayName; }

        /// <summary>
        /// Gets/Sets the control type when the property render by <c>Ajax.RenderAutoSettingForm</c> method.
        /// </summary>
        public ControlTypes PropertyControl
        {
            get { return propertyControl; }
            set { propertyControl = value; }
        }

        /// <summary>
        /// Gets/Sets the type of the property value.
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
        public object DefaultValue { get; set; }


    }
}
