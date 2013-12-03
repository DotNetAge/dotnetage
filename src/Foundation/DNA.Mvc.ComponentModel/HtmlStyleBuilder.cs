///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;
//using System.Drawing;

namespace DNA.Web
{
    /// <summary>
    /// Build html css style text.
    /// </summary>
    public class HtmlStyleBuilder : HtmlAttributeBuilder
    {
        public HtmlStyleBuilder() { }

        /// <summary>
        /// Add style attribute.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HtmlStyleBuilder Add(string name, string value)
        {
            MergeAttribute(name, value);
            return this;
        }

        /// <summary>
        /// Remove attribute by specified attribute name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HtmlStyleBuilder Remove(string name)
        {
            if (InnerAttributes.ContainsKey(name))
                InnerAttributes.Remove(name);
            return this;
        }

        /// <summary>
        /// Add width attribute
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public HtmlStyleBuilder Width(Unit width)
        {
            MergeAttribute("width", width.ToString(), true);
            return this;
        }

        /// <summary>
        /// Add height attribute
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public HtmlStyleBuilder Height(Unit height)
        {
            MergeAttribute("height", height.ToString(), true);
            return this;
        }


        /// <summary>
        /// Set vertical-align attribute of htm element
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HtmlStyleBuilder Valign(VerticalAlign value)
        {

            if (value == VerticalAlign.NotSet)
                Remove("vertical-align");
            MergeAttribute("vertical-align", value.ToString().ToLower(), true);
            return this;
        }

        /// <summary>
        /// Set text-align attribute of html element
        /// </summary>
        /// <remarks>
        /// Set NotSet to remove this attribute.
        /// </remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public HtmlStyleBuilder Align(HorizontalAlign value)
        {
            if (value == HorizontalAlign.NotSet)
                Remove("text-align");
            MergeAttribute("text-align", value.ToString().ToLower(), true);
            return this;
        }

        public HtmlStyleBuilder Background(string value)
        {
            if (!string.IsNullOrEmpty(value))
                MergeAttribute("background", value, true);
            return this;
        }

        public HtmlStyleBuilder BackgroundImage(string url)
        {
            if (!string.IsNullOrEmpty(url))
                MergeAttribute("background-image", "url(" + url + ")", true);
            return this;
        }

        public HtmlStyleBuilder BackgroundColor(string color)
        {
            if (!string.IsNullOrEmpty(color))
                MergeAttribute("background-color", color);
            return this;
        }

        public HtmlStyleBuilder ForeColor(string color)
        {
            if (!string.IsNullOrEmpty(color))
                MergeAttribute("color", color);
            return this;
        }
        

        /// <summary>
        /// Write the style raw string to stream.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTo(HtmlTextWriter writer)
        {
            if (InnerAttributes.Count > 0)
            {
                writer.WriteAttribute("style",GetCssText());
            }
        }

        public string GetCssText()
        { 
            var _attrs=new List<string>();
            foreach (var key in InnerAttributes.Keys)
                _attrs.Add(key + ":" + InnerAttributes[key].ToString());
            return string.Join(";", _attrs.ToArray());
        }
    }

}
