//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a collection of ContentFormDecorator objects
    /// </summary>
    public class ContentFormCollection : IEnumerable<ContentFormDecorator>
    {
        private ContentList Parent { get; set; }

        private IDataContext DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of  the ContentFieldCollection class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="parent">The parent content list object.</param>
        public ContentFormCollection(IDataContext dataContext, ContentList parent)
        {
            this.Parent = parent;
            this.DataContext = dataContext;
        }

        public IEnumerator<ContentFormDecorator> GetEnumerator()
        {
            var forms = DataContext.Where<ContentForm>(f => f.ParentID == Parent.ID);
            var formList = new List<ContentFormDecorator>();
            if (forms.Count() == 0)
                return formList.GetEnumerator();
            return forms.ToList().Select(f => new ContentFormDecorator(DataContext, f)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Convert the collection to field schema xml element.
        /// </summary>
        /// <returns></returns>
        public XElement Element()
        {
            XNamespace ns = ContentList.DefaultNamespace;
            var formsElement = new XElement(ns+"forms");
            foreach (var f in this)
                formsElement.Add(f.Element());
            return formsElement;
        }
    }
}
