//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web
{
    /// <summary>
    /// The Dashboard class gets the global dashboad groups.
    /// </summary>
    public static class Dashboard
    {
        /// <summary>
        /// Gets the Website dashboard group objects
        /// </summary>
        public static IEnumerable<DashboardGroup> SiteGroups
        {
            get
            {
                return GetItemGroups<SiteDashboardAttribute>(new WebContext());
            }
        }

        /// <summary>
        /// Gets the Administrator dashboard group objects.
        /// </summary>
        public static IEnumerable<DashboardGroup> HostGroups
        {
            get
            {
                return GetItemGroups<HostDashboardAttribute>(new WebContext());
            }
        }

        /// <summary>
        /// Gets Mydashboard group objects.
        /// </summary>
        public static IEnumerable<DashboardGroup> MyGroups
        {
            get
            {
                return GetItemGroups<MyDashboardAttribute>(new WebContext());
            }
        }

        private static IEnumerable<DashboardGroup> GetItemGroups<T>(WebContext context)
            where T : DashboardBaseAttribute
        {
            var groups = new List<DashboardGroup>();
            var items = new List<DashboardItem>();
            var url = UrlUtility.CreateUrlHelper();

            var typeSearcher = TypeSearcher.Instance();
            var controllers = typeSearcher.SearchTypesByBaseType(typeof(Controller));

            foreach (Type controller in controllers)
            {
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                var actions = from MethodInfo method in methods
                              where (method.GetCustomAttributes(typeof(T), true).Length > 0)
                              select method;
                var sol = ModuleRegistration.Modules.Select(s => s.Value).FirstOrDefault(s => !string.IsNullOrEmpty(s.RouteName) && s.AssemblyFullName.Equals(controller.Assembly.FullName));

                foreach (MethodInfo action in actions)
                {
                    T attr = (T)Attribute.GetCustomAttribute(action, typeof(T));
                    string text = attr.Text;
                    var securityAttr = Attribute.GetCustomAttribute(action, typeof(SecurityActionAttribute));

                    if (securityAttr != null)
                    {
                        if (!context.HasPermisson(controller, action.Name))
                            continue;
                    }

                    var httpPostAttr = Attribute.GetCustomAttribute(action, typeof(HttpPostAttribute));
                    if (httpPostAttr != null)
                        continue;

                    if (string.IsNullOrEmpty(attr.Text) && string.IsNullOrEmpty(attr.ResKey))
                        continue;

                    if (!string.IsNullOrEmpty(attr.ResKey))
                    {
                        if (!string.IsNullOrEmpty(attr.ResBaseName))
                            text = App.GetResourceString(attr.ResBaseName, attr.ResKey);
                        else
                            text = App.GetResourceString(attr.ResKey);
                    }

                    string _controller = controller.Name.Replace("Controller", "");
                    var curGroup = groups.FirstOrDefault(g => g.Name.Equals(attr.Group, StringComparison.OrdinalIgnoreCase));

                    if (curGroup == null)
                    {
                        curGroup = new DashboardGroup() { Name = attr.Group };
                        groups.Add(curGroup);
                    }

                    if (!string.IsNullOrEmpty(attr.GroupResKey) && !string.IsNullOrEmpty(attr.ResBaseName) && string.IsNullOrEmpty(curGroup.Title))
                        curGroup.Title = App.GetResourceString(attr.ResBaseName, attr.GroupResKey);

                    var area = GetArea(controller);

                    var _item = new DashboardItem()
                    {
                        Order = attr.Sequence,
                        Title = text,
                        Icon = attr.Icon,
                        Action = action.Name,
                        Controller = _controller,
                        Area = area,
                        ImageUrl = string.IsNullOrEmpty(attr.Icon) ? url.Content("~/content/images/icon_process_16.png") : url.Content("~/content/images/" + attr.Icon),
                        NavigationUrl = url.Action(action.Name, _controller, new { Area = area, website = context.Website, locale = context.Locale })
                    };

                    var _routeName = attr.RouteName;

                    if (sol != null && string.IsNullOrEmpty(_routeName))
                    {
                        if (typeof(T) == typeof(HostDashboardAttribute))
                        {
                            _routeName = "host_module_" + sol.Name + "_" + _controller + "_" + action.Name;
                        }
                        else
                        {
                            if (typeof(T) == typeof(MyDashboardAttribute))
                                _routeName = "mysite_" + sol.Name + "_" + _controller + "_" + action.Name;
                            else
                                _routeName = sol.RouteName;
                        }
                    }

                    if (!string.IsNullOrEmpty(_routeName))
                    {
                        var route = RouteTable.Routes[_routeName] as Route;
                        var routeValues = new RouteValueDictionary(new
                        {
                            Area = area,
                            website = context.Website,
                            action = action.Name,
                            controller = _controller,
                            locale = context.Locale
                        });

                        var path = url.RouteUrl(_routeName, routeValues);

                        if (string.IsNullOrEmpty(path))
                        {
                            path = route.Url;
                            foreach (string key in route.Defaults.Keys)
                            {
                                path = path.Replace("{" + key + "}", routeValues.Keys.Contains(key) ? routeValues[key].ToString() : route.Defaults[key].ToString());
                            }

                            foreach (string key in route.Constraints.Keys)
                            {
                                var constr = route.Constraints[key].ToString();
                                if (!constr.StartsWith("("))
                                    path = path.Replace("{" + key + "}", constr);
                            }

                            foreach (string key in routeValues.Keys)
                            {
                                path = path.Replace("{" + key + "}", routeValues[key].ToString());

                            }
                        }
                        _item.NavigationUrl = url.Content("~/" + path);
                    }

                    if (curGroup.Items.Count(i => i.NavigationUrl.Equals(_item.NavigationUrl, StringComparison.OrdinalIgnoreCase)) == 0)
                        curGroup.Items.Add(_item);
                }
            }
            return groups.OrderBy(g => g.Name).ToList();
        }

        private static string GetArea(Type ctrlType)
        {
            return "";
            //var types = ctrlType.Assembly.GetTypes();
            //var areaRegistrationType = types.FirstOrDefault(t => t.BaseType != null && t.BaseType.Equals(typeof(AreaRegistration)));
            //if (areaRegistrationType == null)
            //    return "";
            //else
            //{
            //    var areaInstance = (AreaRegistration)Activator.CreateInstance(areaRegistrationType);
            //    return areaInstance.AreaName;
            //}
        }
    }

}