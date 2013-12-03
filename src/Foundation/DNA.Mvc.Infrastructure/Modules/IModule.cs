//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Scheduling;
using System.Web;
using System.Web.Routing;

namespace DNA.Web
{
    /// <summary>
    /// Define the plugable module
    /// </summary>
    /// <remarks>
    /// <para>All plugable module must be implement the IModule.</para>
    /// </remarks>
    public interface IModule
    {        
        /// <summary>
        /// Gets the module name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Register custom routes.
        /// </summary>
        /// <param name="routes">The global route colleciton.</param>
        void RegisterRoutes(RouteCollection routes);

        /// <summary>
        /// Register embeded widgets
        /// </summary>
        /// <param name="widgets"></param>
        void RegisterWidgets(IWidgetCollection widgets);

        /// <summary>
        /// Register tasks
        /// </summary>
        /// <param name="tasks"></param>
        void RegisterTasks(TaskCollection tasks);

        /// <summary>
        /// Handle application start.
        /// </summary>
        void OnAppStart(HttpApplication app);
    }
}
