///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.Globalization;
using System.Text;
using System.IO;

namespace DNA.Web
{
    /// <summary>
    /// Represents a MVC component base class that responsible for render the html to ViewContent
    /// </summary>
    /// <remarks>A MVC Component maybe composed by many html elements,it define properties, data and render the html elements to ViewContent.</remarks>
    public abstract class HtmlComponent
    {
        private string _id;
        private Dictionary<string, object> htmlAttributes = new Dictionary<string, object>();
        private Dictionary<string, object> dataAttributes = new Dictionary<string, object>();

        public string Name { get; set; }

        public string CssClass { get; set; }

        public string Id
        {
            get
            {
                if ((string.IsNullOrEmpty(_id)) && (!string.IsNullOrEmpty(Name)))
                {
                    var _tag = new TagBuilder(TagName);
                    _tag.GenerateId(Name);
                    if (_tag.Attributes.ContainsKey("id"))
                        _id = _tag.Attributes["id"];
                    else
                        _id = Name;
                }
                return _id;
            }
            private set { _id = value; }
        }

        public virtual string TagName { get { return "div"; } }

        public Dictionary<string, object> DataAttributes
        {
            get { return dataAttributes; }
            set { dataAttributes = value; }
        }

        public Dictionary<string, object> HtmlAttributes
        {
            get
            {
                if (htmlAttributes == null)
                    htmlAttributes = new Dictionary<string, object>();
                return htmlAttributes;
            }
        }

        public HttpContextBase HttpContext { get; set; }

        public event ViewComponentRenderDelegate BeforeComponentRender;
        public event ViewComponentRenderDelegate BeforeContentRender;
        public event ViewComponentRenderDelegate AfterContentRender;
        public event ViewComponentRenderDelegate AfterComponentRender;

        public virtual void Render(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(TagName))
                throw new ArgumentNullException("TagName");

            if (BeforeComponentRender != null) BeforeComponentRender(writer);
            RenderBeginTag(writer);

            if (BeforeContentRender != null) BeforeContentRender(writer);
            RenderContent(writer);
            if (AfterContentRender != null) AfterContentRender(writer);

            RenderEndTag(writer);
            if (AfterComponentRender != null) AfterComponentRender(writer);
        }

        public virtual void RenderContent(HtmlTextWriter writer) { }

        public virtual void RenderBeginTag(HtmlTextWriter writer)
        {
            TagBuilder _tag = new TagBuilder(TagName);

            if (!string.IsNullOrEmpty(Name))
            {
                _tag.GenerateId(Name);
                if (!_tag.Attributes.ContainsKey("id"))
                    _tag.MergeAttribute("id", Name);

                Id = _tag.Attributes["id"];

                if (TagName.Equals("input", StringComparison.OrdinalIgnoreCase) || TagName.Equals("textarea", StringComparison.OrdinalIgnoreCase))
                    _tag.MergeAttribute("name", Name);
            }

            if (!string.IsNullOrEmpty(CssClass))
                _tag.AddCssClass(CssClass);

            if (this.HtmlAttributes != null)
            {
                foreach (var key in this.HtmlAttributes.Keys)
                {
                    if (this.HtmlAttributes[key] != null)
                    {
                        if (key.Contains("_"))
                            _tag.MergeAttribute(key.Replace("_", "-"), this.HtmlAttributes[key].ToString());
                        else
                            _tag.MergeAttribute(key, this.HtmlAttributes[key].ToString());
                    }
                }
            }

            if (this.DataAttributes != null)
            {
                foreach (var key in this.DataAttributes.Keys)
                {
                    if (this.DataAttributes[key] != null)
                        _tag.MergeAttribute("data-" + key, this.DataAttributes[key].ToString());
                }
            }

            writer.Write(_tag.ToString(TagRenderMode.StartTag));
        }

        public virtual void RenderEndTag(HtmlTextWriter writer)
        {
            writer.WriteEndTag(TagName);
        }
    }

    public abstract class HtmlComponent<TModel> : HtmlComponent
        where TModel : class
    {
        public TModel Model { get; set; }
    }

    public delegate void ViewComponentRenderDelegate(HtmlTextWriter writer);
}