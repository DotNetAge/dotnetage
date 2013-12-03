///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;

namespace DNA.Web
{
    public class TemplateViewBuilder<TComponent> : TemplateViewBuilder<TComponent, TemplateViewBuilder<TComponent>>
        where TComponent : TemplateViewComponent
    {
        public TemplateViewBuilder(TComponent component, HttpContextBase context) : base(component, context) { }
    }

    public class TemplateViewBuilder<TComponent, TBuilder> : HtmlViewBuilder<TComponent, TBuilder>
        where TComponent : TemplateViewComponent
        where TBuilder : TemplateViewBuilder<TComponent, TBuilder>
    {
        public TemplateViewBuilder(TComponent component, HttpContextBase context) : base(component, context) { }

        public TBuilder Template(Action value)
        {
            Component.Template.Content = value;
            return this as TBuilder;
        }

        public TBuilder Template(Func<object, object> value)
        {
            this.Component.Template.InlineContent = value;
            return this as TBuilder;
        }
    }

    public class TemplateViewBuilder<TModel, TComponent, TBuilder> : HtmlViewBuilder<TComponent, TBuilder>
        where TModel : class
        where TComponent : TemplateViewComponent<TModel>
        where TBuilder : TemplateViewBuilder<TModel, TComponent, TBuilder>
    {
        public TemplateViewBuilder(TComponent component, HttpContextBase context) : base(component, context) { }

        public TBuilder Template(Func<TModel, object> value)
        {
            this.Component.Template.InlineContent = value;
            return this as TBuilder;
        }

        public virtual TBuilder Template(Action<TModel> value)
        {
            this.Component.Template.Content = value;
            return this as TBuilder;
        }
    }
}