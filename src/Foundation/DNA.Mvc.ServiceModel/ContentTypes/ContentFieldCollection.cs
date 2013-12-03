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
    /// Represents a collection of the ContentField.
    /// </summary>
    public class ContentFieldCollection : List<ContentField>
    {
        private IDataContext DataContext { get; set; }

        private ContentList ParentList { get; set; }

        /// <summary>
        /// Initializes a new instance of the ContentFieldCollection class.
        /// </summary>
        /// <param name="context">The data context</param>
        /// <param name="parent">The parent content list object.</param>
        public ContentFieldCollection(IDataContext context, ContentList parent)
        {
            this.DataContext = context;
            this.ParentList = parent;

            if (!string.IsNullOrEmpty(parent.FieldsXml))
            {
                this.AddRange(parent.ReadFields());
                //var serializer = new XmlSerializer(typeof(ContentListElement));
                //using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(parent.ConfigXml)))
                //{
                //    var m = (ContentListElement)serializer.Deserialize(stream);

                //    var factory = new DefaultContentFieldFactory();
                //    foreach (var f in m.Fields)
                //    {
                //        var fieldInstance = factory.Create(f);
                //        fieldInstance.Parent = this.ParentList;
                //        this.Add(fieldInstance);
                //    }
                //}
            }
        }

        /// <summary>
        /// Gets ContentField by specified field name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>A content field </returns>
        public ContentField this[string name]
        {
            get
            {
                return this.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Remove the content field object by specified name.
        /// </summary>
        /// <param name="name">The field name.</param>
        public void Remove(string name)
        {
            var field = this.First(f => f.Name.Equals(name));
            this.Remove(field);
        }

        /// <summary>
        /// Convert the field collection to fields schema element.
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            var fieldsElement = new XElement("fields");
            foreach (var f in this)
                fieldsElement.Add(f.Element());
            return fieldsElement;
        }
    }
}
