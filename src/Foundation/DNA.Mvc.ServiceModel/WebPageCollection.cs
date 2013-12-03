//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    ///  Represents a collection of WebPageDecorators.
    /// </summary>
    public class WebPageCollection : IEnumerable<WebPageDecorator>
    {
        private Web Parent { get; set; }

        private WebPage ParentPage { get; set; }

        private IDataContext DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the WebPageCollection class with given data context object and parent web model.
        /// </summary>
        /// <param name="dbContext">The data context.</param>
        /// <param name="parent">The web model.</param>
        public WebPageCollection(IDataContext dbContext, Web parent)
        {
            this.DataContext = dbContext;
            this.Parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the WebPageCollection class.
        /// </summary>
        /// <param name="dbContext">The data context.</param>
        /// <param name="parent">The web model.</param>
        public WebPageCollection(IDataContext dbContext, WebPage parent)
        {
            this.DataContext = dbContext;
            this.ParentPage = parent;
        }

        /// <summary>
        /// Gets web page by specified id.
        /// </summary>
        /// <param name="id">The web page id.</param>
        /// <returns>A web page instance.</returns>
        public WebPageDecorator this[int id]
        {
            get
            {
                var result = DataContext.WebPages.Find(id);
                if (result != null)
                    return new WebPageDecorator(result, DataContext);
                return null;
            }
        }

        //public IQueryable<Role> GetAccessRoles()
        //{ 
        //      if (Parent != null)
        //    {
        //        return DataContext.WebPages.Filter(w => w.WebID == Parent.Id && !w.AllowAnonymous)
        //    }
        //    else
        //    {
        //        var pID = ParentPage.ID;
        //        return DataContext.WebPages.Filter(w => w.ParentID == ParentPage.ID).ToList().Select(w => );
        //    }
        //    Where(p =>)
        //}

        public IEnumerator<WebPageDecorator> GetEnumerator()
        {
            if (Parent != null)
            {
                return DataContext.WebPages.Filter(w => w.WebID == Parent.Id).ToList().Select(w => new WebPageDecorator(w, this.DataContext)).GetEnumerator();
            }
            else
            {
                var pID = ParentPage.ID;
                return DataContext.WebPages.Filter(w => w.ParentID == ParentPage.ID).ToList().Select(w => new WebPageDecorator(w, this.DataContext)).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
