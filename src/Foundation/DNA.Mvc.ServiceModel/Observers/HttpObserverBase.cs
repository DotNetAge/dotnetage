//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Web;

namespace DNA.Web.Events
{
    /// <summary>
    /// Represent a base observer base class for web events.
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public abstract class HttpObserverBase<TEventArgs> : ObserverBase<TEventArgs>
           where TEventArgs : class
    {
        /// <summary>
        /// Gets current application context object.
        /// </summary>
        public virtual App AppContext { get { return App.Get(); } }

        /// <summary>
        /// Gets current web object.
        /// </summary>
        public virtual WebDecorator Web { get { return AppContext.CurrentWeb; } }

        /// <summary>
        /// Gets current user object.
        /// </summary>
        public virtual UserDecorator User { get { return AppContext.User; } }

        /// <summary>
        /// Gets current http context.
        /// </summary>
        public virtual HttpContextBase Context { get { return AppContext.HttpContext; } }
        
        /// <summary>
        /// Gets current http request.
        /// </summary>
        public virtual HttpRequestBase Request { get { return AppContext.HttpRequest; } }
    }

    public abstract class HttpObserverBase : HttpObserverBase<object> { }
}
