//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represents a collection of WebDecorator.
    /// </summary>
    public class WebCollection : IEnumerable<WebDecorator>
    {
        /// <summary>
        /// Gets the parent web object.
        /// </summary>
        public virtual WebDecorator Parent { get; private set; }

        public virtual IDataContext DataContext { get; set; }

        public WebCollection() { }

        /// <summary>
        /// Initializes a new instance of  the WebCollection class.
        /// </summary>
        /// <param name="dbContext">The data context object.</param>
        /// <param name="parent">The parent web instance.</param>
        public WebCollection(IDataContext dbContext, WebDecorator parent = null)
        {
            this.DataContext = dbContext;
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the default root web "home"
        /// </summary>
        public WebDecorator Root
        {
            get
            {
                return this["home"];
            }
        }

        /// <summary>
        /// Gets web by specified name.
        /// </summary>
        /// <param name="name">The web name.</param>
        /// <returns>A web decorator that wraps the web model.</returns>
        public virtual WebDecorator this[string name]
        {
            get
            {
                var model = DataContext.Find<Web>(w => w.Name.Equals(name));
                if (model == null)
                    return null;

                return new WebDecorator(model, this.DataContext);
            }
        }

        public IEnumerator<WebDecorator> GetEnumerator()
        {
            return DataContext.All<Web>().ToList().Select(w => new WebDecorator(w, this.DataContext)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
