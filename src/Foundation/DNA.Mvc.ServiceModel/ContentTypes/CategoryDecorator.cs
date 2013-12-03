//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Linq;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to category object model. 
    /// </summary>
    public class CategoryDecorator : Category
    {
        private CategoryCollection children = null;
        private CategoryDecorator parent = null;

        internal IDataContext DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the CategoryDecorator class with data context and category model.
        /// </summary>
        /// <param name="context">The data context </param>
        /// <param name="model">The category model.</param>
        public CategoryDecorator(IDataContext context, Category model)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (model == null)
                throw new ArgumentNullException("model");

            this.DataContext = context;
            this.Model = model;
            model.CopyTo(this, "Web");
        }

        /// <summary>
        /// Gets / Sets the cagegory model object.
        /// </summary>
        public Category Model { get; set; }

        /// <summary>
        /// Gets children categors.
        /// </summary>
        public CategoryCollection Children
        {
            get
            {
                if (children == null)
                {
                    //var parentWeb = this.Model.Web;
                    //if (parentWeb == null)
                   var     parentWeb = DataContext.Find<Web>(this.Model.WebID);
                    children = new CategoryCollection(DataContext,new WebDecorator(parentWeb,DataContext), this);
                }
                return children;
            }
        }

        /// <summary>
        /// Gets the parent category 
        /// </summary>
        public CategoryDecorator Parent
        {
            get
            {
                if (this.ParentID == 0)
                    return null;
                if (this.parent == null)
                    this.parent = new CategoryDecorator(DataContext, DataContext.Find<Category>(this.ParentID));
                return parent;
            }
        }
        
        /// <summary>
        /// Gets the category whether is top level category.
        /// </summary>
        public bool IsTop
        {
            get
            {
                return this.ParentID == 0;
            }
        }

        /// <summary>
        /// Add children category by specified name and description.
        /// </summary>
        /// <param name="name">The new category name.</param>
        /// <param name="description">The new category description.</param>
        /// <returns>A category descorator object wraps the new category model.</returns>
        public CategoryDecorator AddChildren(string name, string description = "")
        {
            return this.Children.New(name, description);
        }

        /// <summary>
        /// Save changes to database.
        /// </summary>
        /// <returns>If success returns true.</returns>
        public bool Save()
        {
            this.CopyTo(Model, "Web");
            DataContext.Update(Model);
            return DataContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Move the category to other parent category.
        /// </summary>
        /// <param name="parentID">The parent category id.</param>
        /// <remarks>
        /// Set the parentID to move the category to top level.
        /// </remarks>
        public void MoveTo(int parentID)
        {
            parent = null;
            Model.ParentID = parentID;
            this.ParentID = parentID;
            DataContext.Update(Model);
            DataContext.SaveChanges();
        }

        /// <summary>
        /// Convert the category to li XElement
        /// </summary>
        /// <returns>A XElement for li element </returns>
        public XElement HtmlElement()
        {
            var element = new XElement("li",
                new XAttribute("data-id",this.ID.ToString()),
                new XAttribute("data-name",this.Name),
                new XElement("span",this.Name));

            if (Children != null && Children.Count()>0)
                element.Add(Children.HtmlElement());

            return element;
        }

    }
}
