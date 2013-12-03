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
    /// Represents a collection of ContentViews
    /// </summary>
    public class ContentViewCollection : IEnumerable<ContentViewDecorator>
    {
        private ContentList Parent { get; set; }

        private IDataContext DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the ContentViewCollection class.
        /// </summary>
        /// <param name="dataContext">The data context object.</param>
        /// <param name="parent">The content list object.</param>
        public ContentViewCollection(IDataContext dataContext, ContentList parent)
        {
            this.Parent = parent;
            this.DataContext = dataContext;
        }

        /// <summary>
        /// Gets the view by specified id.
        /// </summary>
        /// <param name="id">The view id.</param>
        /// <returns>A view decorator wraps the view model object.</returns>
        public ContentViewDecorator this[int id]
        {
            get
            {
                var view = DataContext.Find<ContentView>(id);
                if (view.ParentID == Parent.ID)
                    return new ContentViewDecorator(view, DataContext);
                return null;
            }
        }

        /// <summary>
        /// Gets the view by specified name.
        /// </summary>
        /// <param name="name">The view name.</param>
        /// <returns>A view decorator wraps the view model object.</returns>
        public ContentViewDecorator this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty("name"))
                    throw new ArgumentNullException("name");
                var view = DataContext.Find<ContentView>(c => c.ParentID == Parent.ID && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (view != null)
                    return new ContentViewDecorator(view, DataContext);
                return null;
            }
        }

        //public ContentViewDecorator Find(string slug)
        //{
        //    if (string.IsNullOrEmpty(slug))
        //        throw new ArgumentNullException("slug");
        //    var result = DataContext.Find<ContentView>(v => v.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase) && v.ParentID == Parent.ID);
        //    if (result != null)
        //        return new ContentViewDecorator(result, DataContext);
        //    return null;
        //}

        /// <summary>
        /// Gets the default view object
        /// </summary>
        public ContentViewDecorator Default
        {
            get
            {
                if (DataContext.Where<ContentView>(v => v.ParentID == Parent.ID).Count() == 0)
                    return null;

                var defaultView = DataContext.Find<ContentView>(v => v.ParentID.Equals(Parent.ID) && v.IsDefault);
                if (defaultView == null)
                    defaultView = DataContext.Where<ContentView>(v => v.ParentID.Equals(Parent.ID)).FirstOrDefault();

                if (defaultView != null)
                    return new ContentViewDecorator(defaultView, DataContext);

                return null;
            }
        }

        public IEnumerator<ContentViewDecorator> GetEnumerator()
        {
            var views = DataContext.Where<ContentView>(v => v.ParentID.Equals(Parent.ID));
            var resultViews = new List<ContentViewDecorator>();
            if (views.Count() > 0)
                resultViews.AddRange(views.ToList().Select(v => new ContentViewDecorator(v, DataContext)));

            return resultViews.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Convert the view collections to schema element.
        /// </summary>
        /// <returns>The xml element object that contains views definition.</returns>
        public XElement Element()
        {
            XNamespace ns = ContentList.DefaultNamespace;
            var viewsElement = new XElement(ns + "views");
            foreach (var f in this)
                viewsElement.Add(f.Element());
            return viewsElement;
        }
    }
}
