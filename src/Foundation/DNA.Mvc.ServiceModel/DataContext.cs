//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web;
using DNA.Data;
using DNA.Data.Entity;
using DNA.Web.Data.Entity;
using System;

namespace DNA.Web.Data
{

    /// <summary>
    /// Represents the data context object that implement the IUnitOfWork and IRepository patterns.
    /// </summary>
    public class DataContext : UnitOfWorksBase<CoreDbContext>, IDataContext
    {
        /// <summary>
        /// Initializes a new instance of DataContext class.
        /// </summary>
        public DataContext()
        {
            this.dbContext = new CoreDbContext();
            dbContext.Configuration.ValidateOnSaveEnabled = false;
        }

        /// <summary>
        /// Initializes a new instance of DataContext using given recipe.
        /// </summary>
        /// <param name="recipe">The recipe.</param>
        public DataContext(string recipe)
        {
            this.dbContext = new CoreDbContext() { Recipe = recipe };
            dbContext.Configuration.ValidateOnSaveEnabled = false;
            //if (string.IsNullOrEmpty(recipe) || recipe.Equals("mysql", StringComparison.OrdinalIgnoreCase))
            //{
            //    dbContext.Configuration.LazyLoadingEnabled = false;
            //    dbContext.Configuration.ProxyCreationEnabled = false;
            //}
        }

        /// <summary>
        /// Initializes a new instance of DataContext using given string as the recipe and name or connection.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="nameOrConnectionString"></param>
        public DataContext(string recipe, string nameOrConnectionString)
        {
            this.dbContext = new CoreDbContext(nameOrConnectionString) { Recipe = recipe };

            //if (string.IsNullOrEmpty(recipe) || recipe.Equals("mysql", StringComparison.OrdinalIgnoreCase))
            //{
            //    dbContext.Configuration.LazyLoadingEnabled = false;
            //    dbContext.Configuration.ProxyCreationEnabled = false;
            //}

            dbContext.Configuration.ValidateOnSaveEnabled = false;
        }

        private IWebPageRepository webPageRepository = null;
        private IWidgetRepository widgetRepository = null;
        private IWidgetDescriptorRepository widgetDescriptorRepository = null;
        private IPermissionRepository permissionRepository = null;
        private IContentDataItemRepository contentDataItemRepository = null;
        private IUserRepository userRepository = null;

        /// <summary>
        /// Gets the low level web page repository that use to manipulate web page model to database..
        /// </summary>
        public virtual IWebPageRepository WebPages
        {
            get
            {
                if (webPageRepository == null)
                    webPageRepository = new WebPageRepository(this.dbContext);
                return webPageRepository;
            }
        }

        /// <summary>
        /// Gets the low level widget repository that use to manipulate widget model to database.
        /// </summary>
        public virtual IWidgetRepository Widgets
        {
            get
            {
                if (widgetRepository == null)
                    widgetRepository = new WidgetRepository(this.dbContext);
                return widgetRepository;
            }
        }

        /// <summary>
        /// Gets the low level widget descriptor repository that use to manipulate widget descriptor model to database.
        /// </summary>
        public virtual IWidgetDescriptorRepository WidgetDescriptors
        {
            get
            {
                if (widgetDescriptorRepository == null)
                    widgetDescriptorRepository = new WidgetDescriptorRepository(this.dbContext);
                return widgetDescriptorRepository;
            }
        }

        /// <summary>
        /// Gets the low level permission repository that use to manipulate permission model to database.
        /// </summary>
        public virtual IPermissionRepository Permissions
        {
            get
            {
                if (permissionRepository == null)
                    permissionRepository = new PermissionRepository(this.dbContext);
                return permissionRepository;
            }
        }

        /// <summary>
        /// Gets the low level user repository that use to manipulate user model to database.
        /// </summary>
        public virtual IUserRepository Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(this.dbContext);
                return userRepository;
            }
        }

        /// <summary>
        /// Gets the low level content data item repository that use to manipulate content data item model to database.
        /// </summary>
        public IContentDataItemRepository ContentDataItems
        {
            get
            {
                if (contentDataItemRepository == null)
                    contentDataItemRepository = new ContentDataItemRepository(this.dbContext);
                return contentDataItemRepository;
            }
        }
    }
}
