//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;

namespace DNA.Web.Mvc.ViewEngines.Razor
{
    /// <summary>
    /// Represents the properties and methods that are needed in order to render a view that uses ASP.NET Razor syntax.
    /// </summary>
    /// <typeparam name="TModel">The type of the view data model.</typeparam>
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private App _app;


        /// <summary>
        /// Gets current application object.
        /// </summary>
        public App Application
        {
            get
            {
                if (_app == null)
                    _app = AppModel.Get();
                return _app;
            }
        }

        /// <summary>
        /// Gets current application uri
        /// </summary>
        public Uri AppUri
        {
            get { return Application.Context.AppUrl; }
        }

        /// <summary>
        /// Gets current web site name.
        /// </summary>
        public string WebName
        {
            get { return Application.Context.Website; }
        }

        /// <summary>
        /// Gets current web object.
        /// </summary>
        public WebDecorator Web
        {
            get
            {
                return Application.CurrentWeb;
            }
        }

        /// <summary>
        /// Gets the current web page object.
        /// </summary>
        public WebPageDecorator WebPage
        {
            get
            {
                return Application.CurrentPage;
            }
        }

        /// <summary>
        /// Gets the content lists of currenct web
        /// </summary>
        public ContentListCollection Lists
        {
            get
            {
                return Application.Lists;
            }
        }

        /// <summary>
        /// Gets current theme name.
        /// </summary>
        public string Theme
        {
            get
            {
                return Web.Theme;
            }
        }

        /// <summary>
        /// Gets current user profile
        /// </summary>
        public new UserProfile Profile
        {
            get { return Application.Profile; }
        }

        /// <summary>
        ///  Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            return DNA.Web.ServiceModel.App.GetService<T>();
        }

        /// <summary>
        ///  Resolves multiply registered services.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetServices<T>()
        {
            return DNA.Web.ServiceModel.App.GetServices<T>();
        }

        /// <summary>
        /// Gets the current netdrive to access the web media files.
        /// </summary>
        public INetDriveService Files { get { return Application.NetDrive; } }

        /// <summary>
        /// Gets the document storage of current website
        /// </summary>
        public IUnitOfWorks Blobs
        {
            get
            {
                return Web.Storage;
            }
        }

        /// <summary>
        /// Gets the queue storage of  current website.
        /// </summary>
        public IQueues Queues
        {
            get
            {
                return Web.Queues;
            }
        }
    }

    /// <summary>
    /// Represents the properties and methods that are needed in order to render a view that uses ASP.NET Razor syntax.
    /// </summary>
    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}
