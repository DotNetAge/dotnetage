//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)


namespace DNA.Web
{
    /// <summary>
    /// Represent the service module class
    /// </summary>
    public abstract class ServiceModule : IModule
    {
        /// <summary>
        /// Gets the solution name.
        /// </summary>
        /// <remarks>
        /// If this property returns Empty or null and solution route exists that the Solution will not be registered.
        /// </remarks>
        public abstract string Name { get; }

        /// <summary>
        /// If this property returns true the auto route registion will be disabled
        /// </summary>
        public virtual bool DisableAutoRoutesRegistration { get { return false; } }

        /// <summary>
        /// Register the types to DI container.
        /// </summary>
        /// <param name="container"></param>
        internal protected virtual void RegisterTypes(ITypeContainer container) { }

        /// <summary>
        /// Register custom routes.
        /// </summary>
        /// <param name="routes">The global route colleciton</param>
        public virtual void RegisterRoutes(System.Web.Routing.RouteCollection routes) { }

        /// <summary>
        /// Register widgets
        /// </summary>
        /// <param name="widgets"></param>
        public virtual void RegisterWidgets(IWidgetCollection widgets) { }

        /// <summary>
        /// Register tasks.
        /// </summary>
        /// <param name="tasks"></param>
        public virtual void RegisterTasks(Scheduling.TaskCollection tasks) { }

        /// <summary>
        /// Handle application 
        /// </summary>
        /// <param name="app"></param>
        public virtual void OnAppStart(System.Web.HttpApplication app) { }
    }
}
