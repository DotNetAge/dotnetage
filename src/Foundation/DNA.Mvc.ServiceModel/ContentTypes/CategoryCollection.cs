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
    /// Represents a collection of CategoryDecorator objects
    /// </summary>
    public class CategoryCollection : IEnumerable<CategoryDecorator>
    {
        internal IDataContext DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of CategoryCollection class with datacontext, web object and parent category object.
        /// </summary>
        /// <param name="context">The data context object.</param>
        /// <param name="parentWeb">The web contains categories.</param>
        /// <param name="parent">The parent category.</param>
        public CategoryCollection(IDataContext context, WebDecorator parentWeb, CategoryDecorator parent = null)
        {
            this.DataContext = context;
            this.Parent = parent;
            this.ParentWeb = parentWeb;
        }

        /// <summary>
        /// Gets the parent category object of this collection.
        /// </summary>
        public CategoryDecorator Parent { get; private set; }

        /// <summary>
        /// Gets the parent web object
        /// </summary>
        public WebDecorator ParentWeb { get; private set; }

        /// <summary>
        /// Gets the category by specified name.
        /// </summary>
        /// <param name="name">The category name.</param>
        /// <returns>A category decorator object</returns>
        public CategoryDecorator this[string name]
        {
            get
            {
                var parentID = Parent == null ? 0 : Parent.ID;

                var query = DataContext.Find<Category>(c => c.WebID == ParentWeb.Id && c.ParentID == parentID &&
                    c.Locale.Equals(c.Locale.Equals(ParentWeb.Culture, StringComparison.OrdinalIgnoreCase))
                    && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (query != null)
                    return new CategoryDecorator(DataContext, query);
                return null;
            }
        }

        /// <summary>
        /// Gets the category by specifed id.
        /// </summary>
        /// <param name="id">The category id.</param>
        /// <returns>A category decorator object.</returns>
        public CategoryDecorator this[int id]
        {
            get
            {
                var query = DataContext.Find<Category>(c => c.WebID == ParentWeb.Id && c.ID == id);
                if (query != null)
                    return new CategoryDecorator(DataContext, query);
                return null;
            }
        }

        /// <summary>
        /// Remove the category from collection by specified id
        /// </summary>
        /// <param name="id">The category id.</param>
        /// <remarks>
        /// After remove the category from collection then the collection will auto save changes to database.
        /// </remarks>
        public void Remove(int id)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException("id");
            DataContext.Delete<Category>(c => c.WebID == ParentWeb.Id && c.ID == id);
            DataContext.SaveChanges();
        }

        /// <summary>
        /// Add category object to collection.
        /// </summary>
        /// <param name="category">The category object to add.</param>
        /// <remarks>
        /// After the category added the collection will save changes to database.
        /// </remarks>
        public void Add(Category category)
        {
            var wrapper = category as CategoryDecorator;
            if (wrapper == null)
            {
                category.WebID = this.ParentWeb.Id;
                DataContext.Add(wrapper);
            }
            else
            {
                var m = wrapper.Model;
                m.WebID = this.ParentWeb.Id;
                DataContext.Add(m);
            }
            DataContext.SaveChanges();
        }

        /// <summary>
        /// Create a new category and save to database.
        /// </summary>
        /// <param name="name">The new category name.</param>
        /// <param name="description">The new category description.</param>
        /// <returns>A new category decorator object.</returns>
        public CategoryDecorator New(string name, string description = "")
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            var cat = new Category()
            {
                WebID = this.ParentWeb.Id,
                ParentID = this.Parent == null ? 0 : this.Parent.ID,
                Locale = this.ParentWeb.Culture,
                Name = name,
                Description = description
            };
            DataContext.Add(cat);
            DataContext.SaveChanges();
            return new CategoryDecorator(DataContext, cat);
        }

        /// <summary>
        ///   Returns a category enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CategoryDecorator> GetEnumerator()
        {
            var query = DataContext.Where<Category>(c => c.WebID == ParentWeb.Id && c.Locale.Equals(ParentWeb.Culture, StringComparison.OrdinalIgnoreCase));

            if (Parent != null)
                query = query.Where(c => c.ParentID == Parent.ID);
            else
                query = query.Where(c => c.ParentID == 0);

            var results = new List<Category>();

            if (query.Count() > 0)
                results = query.ToList();

            var conversion = results.Select(c => new CategoryDecorator(DataContext, c)).ToList();
            return conversion.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Convert the collection to HTML string
        /// </summary>
        /// <param name="htmlAttributes">he html attributes of ul element.</param>
        /// <returns>A string contains ul element html.</returns>
        public string ToHtmlString(object htmlAttributes = null)
        {
            return this.HtmlElement(htmlAttributes).OuterXml();
        }

        /// <summary>
        /// Generate the collection to ul XElement.
        /// </summary>
        /// <param name="htmlAttributes">The html attributes of ul element.</param>
        /// <returns>A XElement instance for ul element.</returns>
        public XElement HtmlElement(object htmlAttributes = null)
        {
            var element = new XElement("ul");
            if (htmlAttributes != null)
                element.AddHtmlAttributes(htmlAttributes);
            var allchilren = this.ToList();
            foreach (var cat in allchilren)
                element.Add(cat.HtmlElement());
            return element;
        }
    }
}
