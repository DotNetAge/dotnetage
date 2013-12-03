using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DNA.Web
{
    public class ContentTemplate
    {
        private string contentType = "application/x-ms-aspnet";

        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        public string Source { get; set; }

        public string Text { get; set; }

        public ContentTemplate() { }

        public ContentTemplate(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                var element = XElement.Parse(xml);
                this.Text = element.Value;
                this.Source = element.StrAttr("src");
                var ct = element.StrAttr("contentType");

                if (!string.IsNullOrEmpty(ct))
                    this.contentType = ct;
                //Style = element.StrAttr("style");
            }
        }

        public bool IsClientTemplate
        {
            get
            {
                return !string.IsNullOrEmpty(ContentType) && ContentType.Equals("text/x-jquery-tmpl", StringComparison.OrdinalIgnoreCase);
            }
        }

        //public string Style { get; set; }

        public XElement Element(string tag = "body")
        {
             XNamespace ns = "http://www.dotnetage.com/XML/Schema/contents";
            var element = new XElement(ns+tag);

            if (!string.IsNullOrEmpty(ContentType) && !ContentType.Equals("application/x-ms-aspnet", StringComparison.OrdinalIgnoreCase))
                element.Add(new XAttribute("contentType", contentType));

            if (!string.IsNullOrEmpty(Text))
                element.Add(new XCData(Text));

            if (!string.IsNullOrEmpty(Source))
                element.Add(new XAttribute("src", Source));

            //if (!string.IsNullOrEmpty(Style))
            //    element.Add(new XAttribute("style", Style));
            return element;
        }

        public string ToXml(string tag = "body")
        {
            return this.Element(tag).OuterXml();
        }

        /// <summary>
        /// Gets/Sets whether the template is empty
        /// </summary>
        public bool IsEmpty
        {
            get {
                return string.IsNullOrEmpty(Source) && string.IsNullOrEmpty(Text);
            }
        }
    }
}
