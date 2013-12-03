//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Web
{
    /// <summary>
    /// Represents the solution registerion base class use to register the solution project to DotNetAge
    /// </summary>
    public sealed class ModuleRegistration
    {
        /// <summary>
        /// Register all modules
        /// </summary>
        public static void RegisterModules(RouteCollection routes, ITypeContainer container)
        {
            var typeSearcher = TypeSearcher.Instance();
            var moduleTypes = typeSearcher.SearchTypesByBaseType(typeof(IModule));
            Modules = new Dictionary<string, ModuleDescriptor>();

            foreach (var moduleType in moduleTypes)
            {
                if (moduleType.IsAbstract)
                    continue;

                var module = (IModule)Activator.CreateInstance(moduleType);

                if (module != null && string.IsNullOrEmpty(module.Name))
                    continue;

                if (Modules.ContainsKey(module.Name))
                    continue;

                var _asmName = moduleType.Assembly.GetName();
                var moduleDescriptor = new ModuleDescriptor()
                {
                    Name = module.Name,
                    AssemblyQualifiedName = moduleType.AssemblyQualifiedName,
                    AssemblyFullName = _asmName.FullName,
                    AssemblyName = _asmName.Name
                };
                moduleDescriptor.LoadAssemblyInfo();

                ServiceModule serviceModule = module as ServiceModule;

                if (serviceModule != null)
                {
                    serviceModule.RegisterTypes(container);
                    serviceModule.RegisterRoutes(RouteTable.Routes);
                }

                SolutionModule solutionModule = module as SolutionModule;

                if (solutionModule != null && !solutionModule.DisableAutoRoutesRegistration)
                {
                    #region Auto route generation

                    var routeName = string.Format("solution_{0}", module.Name);
                    var additional_ns = solutionModule.GetNamespaces();
                    var ns = new List<string>();
                    ns.Add(moduleType.Namespace);
                    if (additional_ns != null && additional_ns.Length > 0)
                        ns.AddRange(additional_ns);

                    if (routes[routeName] == null)
                    {
                        moduleDescriptor.RouteName = routeName;
                        var ctrls = moduleType.Assembly.GetTypes().Where(t => t.BaseType != null && t.BaseType.Equals(typeof(Controller))).ToList();

                        if (ctrls.Count > 0)
                        {
                            var hostDashboardType = Type.GetType("DNA.Web.HostDashboardAttribute,DNA.Web.ServiceModel");
                            var myDashboardType = Type.GetType("DNA.Web.MyDashboardAttribute,DNA.Web.ServiceModel");

                            foreach (var ctrl in ctrls)
                            {
                                var dashboardMethods = ctrl.GetMethods().Where(m => m.IsDefined(hostDashboardType, false));
                                var myDashboards = ctrl.GetMethods().Where(m => m.IsDefined(myDashboardType, false));

                                foreach (var m in dashboardMethods)
                                {
                                    var ctrlName = ctrl.Name.Replace("Controller", "");
                                    var dashboardRouteName = "host_module_" + module.Name + "_" + ctrlName + "_" + m.Name;
                                    if (RouteTable.Routes[dashboardRouteName] == null)
                                        routes.MapRoute(dashboardRouteName, "{host}/{solution}/{controller}/{action}/{id}",
                                            new { id = UrlParameter.Optional },
                                            new { host = "host", solution = module.Name, controller = ctrlName, action = m.Name });
                                }

                                foreach (var m in myDashboards)
                                {
                                    var ctrlName = ctrl.Name.Replace("Controller", "");
                                    var myRouteName = "mysite_" + module.Name + "_" + ctrlName + "_" + m.Name;
                                    if (RouteTable.Routes[myRouteName] == null)
                                        routes.MapRoute(myRouteName, "{mysite}/{solution}/{controller}/{action}/{id}",
                                            new { id = UrlParameter.Optional },
                                            new { mysite = "mysite", solution = module.Name, controller = ctrlName, action = m.Name });
                                }
                            }

                            if (RouteTable.Routes[routeName] == null)
                                routes.MapRoute(routeName, "{website}/{locale}/{solution}/{controller}/{action}/{id}",
                                    new { action = "index", id = UrlParameter.Optional },
                                    new { solution = module.Name, locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });
                        }
                    }

                    #endregion
                }

                Modules.Add(module.Name, moduleDescriptor);
            }

            container.Apply();
        }

        /// <summary>
        /// Register all embedded widgets
        /// </summary>
        /// <param name="widgets"></param>
        public static void RegisterWidgets(IWidgetCollection widgets)
        {
            foreach (var key in Modules.Keys)
            {
                try
                {
                    var moduleDescriptor = Modules[key];
                    var module = (IModule)Activator.CreateInstance(Type.GetType(moduleDescriptor.AssemblyQualifiedName));
                    module.RegisterWidgets(widgets);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Register all tasks
        /// </summary>
        /// <param name="tasks"></param>
        public static void RegisterTasks(TaskCollection tasks)
        {
            foreach (var key in Modules.Keys)
            {
                try
                {
                    var moduleDescriptor = Modules[key];
                    var module = (IModule)Activator.CreateInstance(Type.GetType(moduleDescriptor.AssemblyQualifiedName));
                    module.RegisterTasks(tasks);
                }
                catch
                {
                    continue;
                }
            }
        }

        public static void AppStart(HttpApplication app)
        {
            foreach (var key in Modules.Keys)
            {
                try
                {
                    var moduleDescriptor = Modules[key];
                    var module = (IModule)Activator.CreateInstance(Type.GetType(moduleDescriptor.AssemblyQualifiedName));
                    module.OnAppStart(app);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Gets the register modules.
        /// </summary>
        public static Dictionary<string, ModuleDescriptor> Modules { get; internal set; }
    }


}
