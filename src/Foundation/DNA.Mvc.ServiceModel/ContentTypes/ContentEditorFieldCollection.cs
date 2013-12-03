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
    /// Represents a collection of ContentEditorField.
    /// </summary>
    public class ContentEditorFieldCollection : List<ContentEditorField>
    {
        private IDataContext DataContext { get; set; }

        private ContentFormDecorator Parent { get; set; }

        /// <summary>
        /// Initializes a new instance of the ContentEditorFieldCollection class.
        /// </summary>
        /// <param name="context">The data context.</param>
        /// <param name="parent">The content form object.</param>
        public ContentEditorFieldCollection(IDataContext context, ContentFormDecorator parent)
        {
            this.DataContext = context;
            this.Parent = parent;

            if (string.IsNullOrEmpty(this.Parent.Model.FieldsXml))
            {
                var fields = parent.Parent.Fields;
                foreach (var f in fields)
                {
                    this.Add(new ContentEditorField(parent, f));
                }
            }
            else
            {
                var element = XElement.Parse(parent.Model.FieldsXml);

                foreach (var fieldElement in element.Elements())
                {
                    var ns = fieldElement.GetDefaultNamespace();
                    var _ref = parent.Parent.Fields[fieldElement.StrAttr("name")];
                    if (_ref == null)
                        continue;
                    var _editor = new ContentEditorField(parent, _ref);
                    _editor.IsCaption = fieldElement.BoolAttr("caption");
                    _editor.ShowLabel = fieldElement.BoolAttr("showLabel");

                    if (fieldElement.HasAttributes && fieldElement.Attribute("hidden") != null)
                        _editor.IsHidden = fieldElement.BoolAttr("hidden");

                    if (fieldElement.HasElements && fieldElement.Element(ns + "tmpl") != null)
                        _editor.Template = new ContentTemplate(fieldElement.Element(ns + "tmpl").OuterXml());
                    else
                    {
                        switch ((ContentFormTypes)this.Parent.FormType)
                        {
                            case ContentFormTypes.Display:
                                _editor.Template = new ContentTemplate() { Source = _editor.DisplayTemplate };
                                break;
                            case ContentFormTypes.Activity:
                                _editor.Template = new ContentTemplate() { Source = _editor.ActivityTemplate };
                                break;
                            case ContentFormTypes.Edit:
                                _editor.Template = new ContentTemplate() { Source = _editor.EditTemplate };
                                break;
                            case ContentFormTypes.New:
                                _editor.Template = new ContentTemplate() { Source = _editor.NewTemplate };
                                break;
                        }
                    }
                    this.Add(_editor);
                }
            }
        }

        /// <summary>
        /// Gets the editor field by specified field name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns></returns>
        public ContentEditorField this[string name]
        {
            get
            {
                return this.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Add field to collection by specified field name and save the change to database
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns></returns>
        public ContentEditorField Add(string name)
        {
            if (this[name] != null)
                throw new Exception(name + " file is already exists in this form.");

            var fieldInstance = Parent.Parent.Fields[name];
            if (fieldInstance == null)
                throw new NullReferenceException(name + " field is not exists in " + Parent.Parent.Title);

            var field = new ContentEditorField(Parent, fieldInstance);
            this.Add(field);
            Save();
            return field;
        }

        /// <summary>
        /// Save the fields to database.
        /// </summary>
        public void Save()
        {
            Parent.FieldsXml = this.Element().OuterXml();
            Parent.Save();
        }

        /// <summary>
        /// Convert the field editor collection to schema xml.
        /// </summary>
        /// <returns></returns>
        public XElement Element()
        {
            XNamespace ns = ContentList.DefaultNamespace;
            return new XElement(ns + "fields", this.Select(f => f.Element()));
        }
    }
}
