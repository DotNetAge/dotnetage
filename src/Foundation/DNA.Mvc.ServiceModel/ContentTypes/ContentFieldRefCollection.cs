//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a collection of ContentFieldRef.
    /// </summary>
    public class ContentFieldRefCollection : List<ContentFieldRef>
    {
        private IDataContext DataContext { get; set; }

        private ContentViewDecorator Parent { get; set; }

        /// <summary>
        /// Initializes a new instance of the ContentFieldRefCollection class.
        /// </summary>
        /// <param name="context">The data context object.</param>
        /// <param name="parent">The content view object.</param>
        public ContentFieldRefCollection(IDataContext context, ContentViewDecorator parent)
        {
            this.DataContext = context;
            this.Parent = parent;

            if (string.IsNullOrEmpty(this.Parent.Model.FieldRefsXml))
            {
                var fields = parent.Parent.Fields;
                foreach (var f in fields)
                {
                    this.Add(new ContentFieldRef(parent, f));
                }
            }
            else
            {
                var element = XElement.Parse(parent.Model.FieldRefsXml);
                var ns = element.GetDefaultNamespace();
                var needUpdate = false;
                foreach (var fieldElement in element.Elements())
                {
                    var _ref = parent.Parent.Fields[fieldElement.StrAttr("name")];
                    if (_ref == null)
                    {
                        needUpdate = true;
                        continue;
                    }

                    var _refIns = new ContentFieldRef(parent, _ref);

                    if (!string.IsNullOrEmpty(fieldElement.StrAttr("toFeed")))
                        _refIns.ToFeedItemField = fieldElement.Attribute("toFeed").Value;

                    if (fieldElement.HasAttributes && fieldElement.Attribute("hidden") != null)
                        _refIns.IsHidden = fieldElement.BoolAttr("hidden");

                    _refIns.ShowLabel = fieldElement.BoolAttr("showLabel");

                    if (fieldElement.HasElements)
                        _refIns.Template = new ContentTemplate(fieldElement.Element(ns + "tmpl").OuterXml());
                    else
                    {
                        ///Apply default view template
                        if (_refIns.HasViewTemplate)
                            _refIns.Template = new ContentTemplate() { Source = _refIns.ViewTemplate };
                    }

                    this.Add(_refIns);
                }

                if (needUpdate)
                    Save();
            }
        }

        /// <summary>
        /// Gets the field reference object by specified field name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>A content field reference object.</returns>
        public ContentFieldRef this[string name]
        {
            get
            {
                return this.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Convert the collection to view field schema xml element.
        /// </summary>
        /// <returns></returns>
        public XElement Element()
        {
            XNamespace ns = ContentList.DefaultNamespace;

            var element = new XElement(ns + "fields");
            foreach (var f in this)
            {
                var fieldEle = new XElement(ns + "fieldRef", new XAttribute("name", f.Name));
                if (!string.IsNullOrEmpty(f.ToFeedItemField))
                    fieldEle.Add(new XAttribute("toFeed", f.ToFeedItemField));

                if (f.IsHidden)
                    fieldEle.Add(new XAttribute("hidden", true));

                if (f.ShowLabel)
                    fieldEle.Add(new XAttribute("showLabel", true));

                if (!f.Template.IsEmpty)
                    fieldEle.Add(f.Template.Element("tmpl"));

                element.Add(fieldEle);
            }
            return element;
        }

        public override string ToString()
        {
            return this.Element().OuterXml();
        }

        /// <summary>
        /// Save the content field reference collection to database.
        /// </summary>
        public void Save()
        {
            Parent.FieldRefsXml = this.Element().OuterXml();
            Parent.Save();
        }
    }
}
