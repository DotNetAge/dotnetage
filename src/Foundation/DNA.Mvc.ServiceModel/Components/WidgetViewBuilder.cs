//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.UI
{
    /// <summary>
    /// Represents a component builder to build a WidgetViewComponent.
    /// </summary>
    public class WidgetViewBuilder:HtmlViewBuilder<WidgetView,WidgetViewBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the WidgetViewBuilder class with WidgetView object and http context.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="context"></param>
        public WidgetViewBuilder(WidgetView view, HttpContextBase context) : base(view, context)
        {
            view.Context = context;
        }

        /// <summary>
        /// Set Widget UserPreferences form template
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public WidgetViewBuilder UserPreferences(Action<WidgetHelper> content)
        {
            Component.UserPreferencesTemplate.Content = content;
            return this;
        }
        
        /// <summary>
        /// Set Widget UserPreferences form template
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public WidgetViewBuilder UserPreferences(Func<WidgetHelper,object> content)
        {
            Component.UserPreferencesTemplate.InlineContent = content;
            return this;
        }

        /// <summary>
        /// Define the Widget Content template
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public WidgetViewBuilder Content(Action<WidgetHelper> content)
        {
            Component.ContentTemplate.Content = content;
            return this;
        }

        /// <summary>
        /// Define the widget header template
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public WidgetViewBuilder Header(Action<WidgetHelper> content)
        {
            Component.HeaderTemplate.Content = content;
            return this;
        }

        /// <summary>
        /// Define the widget header template.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public WidgetViewBuilder Header(Func<WidgetHelper, object> content)
        {
            Component.HeaderTemplate.InlineContent = content;
            return this;
        }

        /// <summary>
        /// Define the Widget Content template
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public WidgetViewBuilder Content(Func<WidgetHelper,object> content)
        {
            Component.ContentTemplate.InlineContent = content;
            return this;
        }

        /// <summary>
        /// Define the Widget Content show in design time.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public WidgetViewBuilder Design(Func<WidgetHelper, object> content)
        {
            Component.DesignTemplate.InlineContent = content;
            return this;
        }

        /// <summary>
        /// Set Widget Content show in design time.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public WidgetViewBuilder Design(Action<WidgetHelper> content)
        {
            Component.DesignTemplate.Content = content;
            return this;
        }

        /// <summary>
        /// Set the WidgetViewComponent to auto save user preference changes.
        /// </summary>
        /// <returns></returns>
        public WidgetViewBuilder AutoSave()
        {
            this.Component.AutoSave = true;
            return this;
        }

        /// <summary>
        /// Hide the user preferences 
        /// </summary>
        /// <returns></returns>
        public WidgetViewBuilder HideUserPreferences() 
        {
            this.Component.HideUserPreferences = true;
            return this;
        }
    }
}