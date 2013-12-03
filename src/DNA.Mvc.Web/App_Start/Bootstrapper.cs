//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using DNA.Web.Routing;
using DNA.Web.ServiceModel;
using DNA.Xml.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

[assembly: PreApplicationStartMethod(typeof(DNA.Web.Bootstrapper), "LoadModuleAssemblies")]

namespace DNA.Web
{
    public static class Bootstrapper
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            if (!DNA.Web.ServiceModel.App.Settings.Initialized)
            {
                routes.MapRoute("Default", "{controller}/{action}", new { controller = "Install", action = "Index" });
                APIRouteConfig.Register(routes);
            }
            else
            {
                routes.MapRoute("dna_sitemaproute", "{service}", new { controller = "Syndication", action = "Sitemap" }, new { service = "sitemap.xml" });
                ErrorRouteConfig.Register(routes);
                //SearchRouteConfig.Register(routes);
                MyDashboarRouteConfig.Register(routes);
                DashboardRouteConfig.Register(routes);
                HostRouteConfig.Register(routes);
                NetDriveRouteConfig.Register(routes);
                AccountRouteConfig.Register(routes);
                APIRouteConfig.Register(routes);
                ContentRouteConfig.Register(routes);
                DynamicUIRouteConfig.Register(routes);
                //OAuthRouteConfig.Register(routes);
                //ExternalRouteConfig.Register(routes);
                routes.MapRoute("Default", "{controller}/{action}/{id}", new { id = "" });
            }
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //if (DnaConfig.IsNeedInstallCheck)
            //    filters.Add(new InstallationCheckerAttribute());

            var gfilters = DependencyResolver.Current.GetServices<IGlobalFilter>();
            foreach (var filter in gfilters)
                filters.Add(filter);
        }

        public static void RegisterTypes()
        {
            LoadModuleAssemblies();

            DIRegister.RegisterTypes();

            if (DnaConfig.GetCurrentTrustLevel().HasFlag(AspNetHostingPermissionLevel.Unrestricted))
                BuildResources();


        }

        /// <summary>
        /// Register widgets in modules.
        /// </summary>
        public static void RegisterWidgets()
        {
            var widgets = new WidgetCollection();
            ModuleRegistration.RegisterWidgets(widgets);
            var widgetMgr = App.Get().Widgets;
            var pendingRegister = new List<string>();

            foreach (var w in widgets)
            {
                var installedPath = w.Category + "\\" + w.Name;
                var widgetID = DNA.Utility.TextUtility.Slug(w.ModuleType.FullName + "." + w.Name);
                var viewName = string.Format("~/Widgets/{0}/{1}", w.Name, w.ViewName);

                if (widgetMgr.Find(widgetID) == null)
                {
                    XNamespace ns = "http://www.w3.org/ns/widgets";
                    var widgetEle = new XElement(ns + "widget",
                        new XAttribute("id", widgetID),
                        new XAttribute("version", w.ModuleType.Assembly.GetName().Version.ToString()),
                        new XElement(ns + "name", new XAttribute("short", w.Name), w.Title),
                        new XElement(ns + "description", w.Description)
                        );

                    var contentEle = new XElement(ns + "content", new XAttribute("type", "application/x-ms-aspnet"), new XAttribute("src", viewName));
                    widgetEle.Add(contentEle);
                    
                    var fullPath = Path.Combine(HostingEnvironment.MapPath("~/content/widgets/"), installedPath);
                    if (!Directory.Exists(fullPath))
                        Directory.CreateDirectory(fullPath);

                    if (w.Preferences != null && w.Preferences.Count > 0)
                    {
                        foreach (var key in w.Preferences.Keys)
                        {
                            var val = w.Preferences[key];
                            widgetEle.Add(new XElement(ns + "preference", new XAttribute("name", key), new XAttribute("value", FormPreferenceValue(val))));
                        }
                    }

                    var configXmlFile = Path.Combine(fullPath, "config.xml");
                    widgetEle.Save(configXmlFile);

                    //XmlSerializerUtility.SerializeToXmlFile(configXmlFile, widgetEle);
                    pendingRegister.Add(w.Category + "," + w.Name);
                }
                else
                {
                    // widgetMgr.Update(w);
                }


            }

            if (pendingRegister.Count > 0)
            {
                App.Get().Widgets.Reload();

                for (int i = 0; i < pendingRegister.Count; i++)
                {
                    try
                    {
                        var args = pendingRegister[i].Split(',');
                        App.Get().Widgets.Register(args[0], args[1]);
                    }
                    catch (Exception e)
                    {

                        Logger.Error(e);
                        continue;
                    }
                }
            }
        }

        private static string FormPreferenceValue(object val)
        {
            if (val == null)
                return "";

            if (val.GetType().Equals(typeof(string)))
                return string.Format("'{0}'", val);

            return val.ToString();
        }

        /// <summary>
        /// Load all installed solution assemblies to current app domain.
        /// </summary>
        public static void LoadModuleAssemblies()
        {
            string modulesPath = HostingEnvironment.MapPath("~/content/modules");
            string moduleRuntimePath = HostingEnvironment.MapPath("~/content/modules/bin");

            if (modulesPath == null || moduleRuntimePath == null)
                throw new DirectoryNotFoundException("modules");

            var ModulesFolder = new DirectoryInfo(modulesPath);
            var ModuleRuntimeFolder = new DirectoryInfo(moduleRuntimePath);

            Directory.CreateDirectory(ModuleRuntimeFolder.FullName);

            //clear out plugins
            foreach (var f in ModuleRuntimeFolder.GetFiles("*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    f.Delete();
                }
                catch (Exception)
                {
                }
            }

            ////copy files
            foreach (var plug in ModulesFolder.GetFiles("*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var di = Directory.CreateDirectory(ModuleRuntimeFolder.FullName);
                    File.Copy(plug.FullName, Path.Combine(di.FullName, plug.Name), true);
                }
                catch (Exception)
                {
                }
            }

            // * This will put the plugin assemblies in the 'Load' context
            // This works but requires a 'probing' folder be defined in the web.config
            // eg: <probing privatePath="content/solutions/temp" />
            var assemblies = ModuleRuntimeFolder.GetFiles("*.dll", SearchOption.AllDirectories)
                .Where(s => !s.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("EntityFramework.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("Newtonsoft.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("Antlr3.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("WebGrease.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("CSharpFormat.", StringComparison.OrdinalIgnoreCase)
                )
                    .Select(x => AssemblyName.GetAssemblyName(x.FullName))
                    .Select(x => Assembly.Load(x.FullName));

            foreach (var assembly in assemblies)
            {
                //Type type = assembly.GetTypes().FirstOrDefault(t => t.BaseType != null &&
                //    t.BaseType.Equals(typeof(IModule)));

                //if (type != null)
                //{
                try
                {
                    //Add the module as a reference to the application
                    BuildManager.AddReferencedAssembly(assembly);
                }
                catch (Exception) { continue; }
                //}
            }
        }

        public static void BuildResources()
        {
            #region  Check widgets resources
            var basePath = HttpRuntime.AppDomainAppPath + "Content\\widgets";
            var cats = System.IO.Directory.GetDirectories(basePath);
            foreach (var catBase in cats)
            {
                var widgetPaths = System.IO.Directory.GetDirectories(catBase);
                foreach (var wPath in widgetPaths)
                {
                    var resPath = wPath + "\\resources";
                    if (System.IO.Directory.Exists(resPath))
                    {
                        ResBuilder.Build(resPath);
                    }
                }
            }
            #endregion

            #region Check content type resources
            basePath = HttpRuntime.AppDomainAppPath + "Content\\types";
            var typePaths = System.IO.Directory.GetDirectories(basePath);
            foreach (var wPath in typePaths)
            {
                var resPath = wPath + "\\resources";
                if (System.IO.Directory.Exists(resPath))
                    ResBuilder.Build(resPath);
            }
            #endregion
        }


    }
}