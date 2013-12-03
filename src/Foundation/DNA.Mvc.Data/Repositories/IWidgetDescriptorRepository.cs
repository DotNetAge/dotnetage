//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Defines the methods to mainpluate WidgetDescriptor data objects.
    /// </summary>
    public interface IWidgetDescriptorRepository : IRepository<WidgetDescriptor>
    {
        /// <summary>
        /// Delete the specified descriptor by id.
        /// </summary>
        /// <param name="id">The specified descriptor id.</param>
        void Delete(int id);

        /// <summary>
        /// Delete the WidgetDescriptor by specified installedPath.
        /// </summary>
        /// <param name="installedPath">The widget installed path.</param>
        void Delete(string installedPath);

        /// <summary>
        /// Gets the WidgetDescriptor by specified controller name and action name.
        /// </summary>
        /// <param name="controllerName">The controller that contains the widget definition.</param>
        /// <param name="action">The widget Action name.</param>
        /// <returns>The WidgetDescriptor instance.</returns>
        WidgetDescriptor Find(string controllerName, string action);

        /// <summary>
        /// Gets the WidgetDescriptor by specified installation path.
        /// </summary>
        /// <param name="installedPath">The widget installed path.</param>
        /// <returns>A WidgetDescriptor object.</returns>
        WidgetDescriptor Find(string installedPath);

        /// <summary>
        /// Get widget descriptors by specified path.
        /// </summary>
        /// <param name="path">The widget installed path.</param>
        /// <returns>A widget descriptor collection.</returns>
        IEnumerable<WidgetDescriptor> WithInPath(string path);

        /// <summary>
        /// Get widget inusing count.
        /// </summary>
        /// <param name="installedPath">The widget installed path.</param>
        /// <returns>returns an integer value</returns>
        int InusingWidgetsCount(string installedPath);

        /// <summary>
        /// Add roles to the specified WidgetDescriptor
        /// </summary>
        /// <param name="id">The widget descriptor id.</param>
        /// <param name="roles">The role string array.</param>
        void AddRoles(int id, string[] roles);

        /// <summary>
        /// Get roles by specified widget descriptor id.
        /// </summary>
        /// <param name="id">The widget descritpor id.</param>
        /// <returns>A role name string array.</returns>
        string[] GetRoles(int id);

    }
}
