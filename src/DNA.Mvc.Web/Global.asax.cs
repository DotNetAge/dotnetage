//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Data;
using DNA.Web.EmbeddedViews;
using DNA.Web.Scheduling;
using DNA.Web.ServiceModel;
using DNA.Web.ServiceModel.Tasks;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DNA.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public partial class WebApp : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute("dna_stores_api", "{api}/{webstore}/{action}", new { controller = "WebStoreAPI", Area = "", }, new { api = "api", webstore = "webstore" });
            Bootstrapper.RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new ErrorLogAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LanguageDirectorAttribute());
        }

        protected void Application_Start()
        {
            if (App.Settings.AutomaticMigrationsEnabled)
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<CoreDbContext, DNA.Web.Data.Entity.Migrations.Configuration>());
            else
                Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDbContext>());

            System.Web.Mvc.ViewEngines.Engines.Clear();
            System.Web.Mvc.ViewEngines.Engines.Add(new RazorViewEngine());

            ModuleRegistration.RegisterModules(RouteTable.Routes, new TypeContainer());

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            Bootstrapper.RegisterTypes();
            RegisterGlobalFilters(GlobalFilters.Filters);
            Bootstrapper.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var tasks = new TaskCollection();
            ModuleRegistration.RegisterTasks(tasks);
            Scheduler.AddTasks(tasks);

            if (App.Settings.AutoStartScheduler)
                Scheduler.Start();

            Bootstrapper.RegisterWidgets();

            //var embeddedViewResolver =System.Web.Mvc.DependencyResolver.Current.GetService<IEmbeddedViewResolver>();
            var embeddedProvider = new EmbeddedViewPathProvider(new EmbeddedViewResolver().GetEmbeddedViews());
            HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);

            //Bootstrapper.Init();
            ModuleRegistration.AppStart(this);
            //Logger.Info("Application start.");
        }

    }
}