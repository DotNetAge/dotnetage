///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace DNA.Web
{
    /// <summary>
    /// Define the html view template for ASPX and Razor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHtmlTemplate<T>
    {
        Action<T> Content { get; set; }

        Func<T, object> InlineContent { get; set; }

        bool IsEmpty { get; }

        void WriteTo(HtmlTextWriter writer);
    }

    /// <summary>
    /// Define the html view template for ASPX and Razor
    /// </summary>
    public interface IHtmlTemplate
    {
        Action Content { get; set; }

        Func<object, object> InlineContent { get; set; }

        bool IsEmpty { get; }

        void WriteTo(HtmlTextWriter writer);
    }

    public class HtmlTemplate<T> : IHtmlTemplate<T>
    {
        public T DataItem { get; set; }

        /// <summary>
        /// Gets/Sets a delegate method allows developers inject the template code at runtime.
        /// </summary>
        public virtual Action<T> Content { get; set; }

        /// <summary>
        /// Gets/Sets the Razor inline template
        /// </summary>
        public Func<T, object> InlineContent { get; set; }

        public virtual void WriteTo(T dataItem, HtmlTextWriter writer)
        {
            this.DataItem = dataItem;
            this.WriteTo(writer);
        }

        public virtual void WriteTo(HtmlTextWriter writer)
        {
            if (Content != null)
            {
                Content.Invoke(DataItem);
            }
            else
            {
                if (InlineContent != null)
                {
                    var raw = InlineContent(DataItem).ToString();
                    writer.Write(raw);
                }
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return ((Content == null) && (InlineContent == null));
            }
        }
    }

    public class HtmlTemplate : IHtmlTemplate
    {
        public Action Content { get; set; }

        public Func<object, object> InlineContent { get; set; }

        public void WriteTo(System.Web.UI.HtmlTextWriter writer)
        {
            if (Content != null)
            {
                Content.Invoke();
            }
            else
            {
                if (InlineContent != null)
                    writer.Write(InlineContent(null).ToString());
            }
        }

        public bool IsEmpty
        {
            get
            {
                return ((Content == null) && (InlineContent == null));
            }
        }

    }
}
