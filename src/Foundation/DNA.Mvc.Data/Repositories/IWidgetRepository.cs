//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using DNA.Data;

namespace DNA.Web
{
    /// <summary>
    /// Defines the methods to mainpluate Widget data objects.
    /// </summary>
    public interface IWidgetRepository:IRepository<WidgetInstance>
    {
        /// <summary>
        /// Create the Widget Instance by specified widget descriptor ,the web page visual path ,the zone element id and the position in widgets sequence.
        /// </summary>
        /// <param name="id">The new widget id.</param>
        ///<param name="descriptor">The WidgetDescriptor instance.</param>
        /// <param name="webPagePath">Specified the web page path to added</param>
        /// <param name="zoneID">The zone element id</param>
        /// <param name="position"> The position in widgets sequence</param>
        /// <returns>A new Widget instance.</returns>
        WidgetInstance AddWidget(WidgetDescriptor descriptor, int pageID, string zoneID, int position);

        /// <summary>
        /// Create the Widget instance by specified widget template.
        /// </summary>
        /// <param name="id">The new widget id.</param>
        /// <param name="tmpl">The widget template object.</param>
        /// <param name="webPagePath">The virutal path of the web page which the widget add to.</param>
        /// <param name="zoneID">The widget zone id that contains the new widget.The Widget will be added to zone0 when this paramater set to null </param>
        /// <param name="position">The widget position.</param>
        /// <returns>Returns a new widget instance.</returns>
        WidgetInstance AddWidget(IWidget tmpl, int pageID, string zoneID=null,int position=0);

        /// <summary>
        /// Move the specified widget to a zone.
        /// </summary>
        /// <param name="id">The widget instance id.</param>
        /// <param name="zoneID">the zone element id</param>
        /// <param name="position">The position in widgets sequence</param>
        void MoveTo(int id, string zoneID, int position);

        /// <summary>
        /// Add roles to the specified widget 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roles"></param>
        void AddRoles(int id, string[] roles);

        /// <summary>
        /// Get access roles by specified widget id.
        /// </summary>
        /// <param name="id">The widget id.</param>
        /// <returns>A role name string array.</returns>
        string[] GetRoles(int id);
    }
}
