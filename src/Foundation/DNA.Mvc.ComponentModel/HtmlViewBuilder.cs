///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;

namespace DNA.Web
{
    public abstract class HtmlViewBuilder<TComponent, TBuilder> 
        where TComponent : HtmlComponent
        where TBuilder : HtmlViewBuilder<TComponent, TBuilder>
    {
        private TComponent component;

        public TComponent Component
        {
            get { return component; }
            private set { component = value; }
        }

        public HttpContextBase Context { get; private set; }

        public HttpResponseBase Response { get { return Context.Response; } }

        public HttpRequestBase Request { get { return Context.Request; } }

        public HtmlViewBuilder(TComponent component,HttpContextBase context)
        {
            if (component == null)
                throw new ArgumentNullException("component");
            Component = component;
            this.Context = context;
        }

        public TBuilder GenerateId()
        {
            if (string.IsNullOrEmpty(Component.Name))
            {
                string prefix = Component.GetType().Name;
                string key = "_ID_SEQ_" + prefix;
                int seq = 1;
                if (Context.Items.Contains(key))
                {
                    seq = (int)Context.Items[key] + 1;
                    Context.Items[key] = seq;
                }
                else
                    Context.Items.Add(key, seq);
                Component.Name = prefix + seq.ToString();
            }

            return this as TBuilder;
        }

        public virtual TBuilder Name(string name)
        {
            Component.Name = name;
            return this as TBuilder;
        }

        /// <summary>
        /// Set the html element class name
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public virtual TBuilder AddClass(string className)
        {
            Component.CssClass = className;
            return this as TBuilder;
        }

        /// <summary>
        /// Overwrite the html style attribute value
        /// </summary>
        /// <param name="cssText"></param>
        /// <returns></returns>
        public virtual TBuilder StyleText(string cssText)
        {
            if (string.IsNullOrEmpty(cssText) && Component.HtmlAttributes.ContainsKey("style"))
                Component.HtmlAttributes.Remove("style");

            if (Component.HtmlAttributes.ContainsKey("style"))
                Component.HtmlAttributes["style"] = cssText;
            else
                Component.HtmlAttributes.Add("style", cssText);

            return this as TBuilder;
        }

        /// <summary>
        /// Write the data-role attribute to component
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual TBuilder Role(string name)
        {
            Data("role", name);
            return this as TBuilder;
        }

        public virtual TBuilder Data(string key, object value)
        {
            if (Component.DataAttributes.ContainsKey(key))
                Component.DataAttributes.Remove(key);
            Component.DataAttributes.Add(key, value == null ? "" : value.ToString());
            return this as TBuilder;
        }

        public virtual TBuilder Attrs(object htmlAttributes)
        {
            var attributes = ObjectHelper.ConvertObjectToDictionary(htmlAttributes);
            foreach (var key in attributes.Keys)
            {
                var val = attributes[key];
                if (Component.HtmlAttributes.ContainsKey(key))
                    Component.HtmlAttributes.Remove(key);
                Component.HtmlAttributes.Add(key, val == null ? "" : val.ToString());
            }

            return this as TBuilder;
        }

        public virtual TBuilder Attr(string key, object value)
        {
            if (Component.HtmlAttributes.ContainsKey(key))
                Component.HtmlAttributes.Remove(key);
            Component.HtmlAttributes.Add(key, value == null ? "" : value.ToString());
            return this as TBuilder;
        }

        public virtual TBuilder ToolTip(string title)
        {
            Attr("title", title);
            return this as TBuilder;
        }

        /// <summary>
        /// For ASP.NET FORM
        /// </summary>
        public virtual void Render()
        {
            using (var writer = new HtmlTextWriter(this.Context.Response.Output))
            {
                Component.Render(writer);
            }
        }

        public virtual HelperResult GetHtml()
        {
            return new HelperResult(writer => Component.Render(new HtmlTextWriter(writer)));
        }

    }
}
