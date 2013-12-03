///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNA.Web
{
    /// <summary>
    /// Defines the template delegate or template text that makes the component temlpatable.
    /// </summary>
    public class TemplateViewComponent<TModel> : HtmlComponent<TModel>
        where TModel : class
    {
        /// <summary>
        /// Gets/Sets a delegate method allows developers inject the template code at runtime.
        /// </summary>
       //// [ScriptIgnore]
        public virtual HtmlTemplate<TModel> Template { get; set; }

        public override void RenderContent(System.Web.UI.HtmlTextWriter writer)
        {
            if (!Template.IsEmpty)
                Template.WriteTo(writer);
        }
    }

    public class TemplateViewComponent : HtmlComponent
    {
        private IHtmlTemplate _template = new HtmlTemplate();
        
        public IHtmlTemplate Template
        {
            get { return _template; }
            set { _template = value; }
        }

        public override void RenderContent(System.Web.UI.HtmlTextWriter writer)
        {
            if (!Template.IsEmpty)
                Template.WriteTo(writer);
        }
    }
}
