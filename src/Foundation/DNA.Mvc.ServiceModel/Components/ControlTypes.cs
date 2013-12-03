//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.UI
{
    /// <summary>
    /// Specifies the function of a Property in a Widget.
    /// </summary>
    /// <remarks>
    ///  You can use the ControlTypes to specifies the function of a Property in a Widget definition use by auto render setting form.
    /// </remarks>
    /// <example>
    /// <para>The ControlTypes use by the Ajax.RenderAutoSettingForm to render the user preferences setting form.</para>
    /// <code language="cs"> 
    ///public class SampleWidgetController:Controller
    ///{
    ///   [Widget("SayHello","The widget just say hello.")]
    ///   [Property("Word", PropertyControl=ConrolTypes.TextArea)]
    ///   public ActionResult Hello()
    ///   {
    ///        return View();
    ///   }
    ///}
    ///</code>
    ///<para>In view file</para>
    ///<code language="aspx">
    ///&lt;%:@ Control Language="C#" Inherits="DNA.Web.DynamicUI.WidgetViewUserControl"  %&gt;
    ///&lt;%:Ajax.RenderAutoSettingForm(PropertyDescriptors, IDPrefix, IsDesignMode)%&gt;
    ///&lt;%:UserData["Word"] %&gt;
    ///&lt;%:Html.StartupScripts() %&gt;
    /// </code>
    /// </example>
    public enum ControlTypes
    {
        /// <summary>
        /// A textbox control of the Widget property.
        /// </summary>
        TextBox,
        /// <summary>
        /// A TextArea control of the Widget property
        /// </summary>
        TextArea,
        /// <summary>
        /// A Number control of the Widget property
        /// </summary>
        Number,
        /// <summary>
        /// A Richtext control of the Widget property
        /// </summary>
        Richtext,
        /// <summary>
        /// A Checkbox control of the Widget property
        /// </summary>
        Checkbox,
        /// <summary>
        /// A Radiobox control of the Widget property
        /// </summary>
        Radiobox,
        /// <summary>
        /// A FileSelector control of the Widget property
        /// </summary>
        FileSelector,
        /// <summary>
        /// A Slider control of the Widget property
        /// </summary>
        Slider,
        /// <summary>
        /// A DateTimePicker control of the Widget property
        /// </summary>
        DateTimePicker,
        /// <summary>
        /// A Dropdown control of the Widget property
        /// </summary>
        Dropdown
    }
}
