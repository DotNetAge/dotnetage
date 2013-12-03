using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Mvc
{
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
