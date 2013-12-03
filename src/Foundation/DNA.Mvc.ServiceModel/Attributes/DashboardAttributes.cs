//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Web.Mvc;

namespace DNA.Web
{
    /// <summary>
    /// Represents a base Dashboard attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class DashboardBaseAttribute : ActionFilterAttribute
    {
        private string _text;
        private string _resKey;
        private string _resBaseName = "Commons";
        private int order = 0;
        private string group = "";

        /// <summary>
        /// Gets/Sets the dashboard item group title resource key
        /// </summary>
        public string GroupResKey { get; set; }

        /// <summary>
        /// Gets/Sets the dashboard item group title
        /// </summary>
        /// <remarks>
        /// If set the Group property to null the Dashboard item will be created to "Common" group.
        /// </remarks>
        public string Group
        {
            get { return group; }
            set { group = value; }
        }

        /// <summary>
        /// Gets/Sets user route name of the Action.
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// Gets/sets the  dashboard menu item order
        /// </summary>
        public int Sequence
        {
            get { return order; }
            set { order = value; }
        }

        /// <summary>
        /// Gets/Sets the dashboard menu title resource key
        /// </summary>
        public string ResKey
        {
            get { return _resKey; }
            set { _resKey = value; }
        }

        /// <summary>
        /// Gets/Sets the dashboard menu resource file base name.
        /// </summary>
        public string ResBaseName
        {
            get { return _resBaseName; }
            set { _resBaseName = value; }
        }

        /// <summary>
        /// Gets/Sets the  dashboard menu item display title text.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets/Sets the icon file that in ("~/content/images")
        /// </summary>
        public string Icon { get; set; }
    }

    /// <summary>
    /// Represents an attribute that is used to set the Action to Website Dashboard menu
    /// </summary>
    /// <remarks>
    /// <para>When the SiteDashboardAttribute specified on the Action it will check the current user whether has permission access the Action.</para>
    /// <para>After specified the SiteDashboardAttribute also need to specified the Layout property to "~/Views/Shared/_Dashboard.cshtml" </para>
    /// </remarks>
    /// <example>
    /// <para>The Controller</para>
    /// <code language="cs">
    /// [SiteDashboard("My store settings",Group="Store")]
    /// public ActionResult MyStoreSettings()
    /// {
    ///     return View();
    /// }
    /// </code>
    /// <para>The MyStoreSettings.cshtml view file</para>
    /// <code language="aspx">
    /// @{
    ///    Layout = "~/Views/Shared/_Dashboard.cshtml";
    /// }
    /// @*Your view code here.*@
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SiteDashboardAttribute : DashboardBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of  SiteDashboardAttribute class.
        /// </summary>
        public SiteDashboardAttribute() { Order = 0; }

        /// <summary>
        /// Initialize the SiteDashboardAttribute instance with the Dashboard menu item text.
        /// </summary>
        /// <param name="text">The Dashboard menu item text.</param>
        public SiteDashboardAttribute(string text)
            : this()
        {
            Text = text;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.RequestContext.HttpContext;
            var request = context.Request;
            if (request.IsAuthenticated)
            {
                var app = App.Get();
                //var locale = app.Context.Web.DefaultLocale;
                app.SetCulture();
                filterContext.Controller.ViewBag.ControlPanel = "Site";

                if (!app.Context.Web.Owner.Equals(context.User.Identity.Name))
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }

                base.OnActionExecuting(filterContext);
            }
            else
                filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
        }

    }

    /// <summary>
    /// Represents an attribute that is used to set the Action to Administrator Dashboard menu. The Host dashboard page use to handle some global website settings.
    /// </summary>
    /// <remarks>
    /// <para>When the HostDashboardAttribute specified on the Action it will check the current user whether has permission access the Action.</para>
    /// <para>After specified the HostDashboardAttribute also need to specified the Layout property to "~/Views/Shared/_Dashboard.cshtml" </para>
    /// </remarks>
    /// <example>
    /// <para>The Controller</para>
    /// <code language="cs">
    /// [SiteDashboard("File type settings",)]
    /// public ActionResult FileTypeSettings()
    /// {
    ///     return View();
    /// }
    /// </code>
    /// <para>The FileTypeSettings.cshtml view file</para>
    /// <code language="aspx">
    /// @{
    ///    Layout = "~/Views/Shared/_Dashboard.cshtml";
    /// }
    /// @*Your view code here.*@
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HostDashboardAttribute : DashboardBaseAttribute
    {
        /// <summary>
        /// Initialize the HostDashboardAttribute instance.
        /// </summary>
        public HostDashboardAttribute() { Order = 0; }

        /// <summary>
        /// Initializes a new instance of HostDashboardAttribute class with the Dashboard menu item text.
        /// </summary>
        /// <param name="text">The Dashboard menu item text.</param>
        public HostDashboardAttribute(string text)
            : this()
        {
            Text = text;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            App.Get().SetCulture(App.Settings.DefaultLocale);
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var context = filterContext.RequestContext.HttpContext;
            var request = context.Request;
            if (request.IsAuthenticated)
            {
                if (context.User.Identity.Name.Equals(App.Settings.Administrator))
                {
                    filterContext.Controller.ViewBag.ControlPanel = "Host";
                    base.OnActionExecuted(filterContext);
                    return;
                }
            }

            filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
        }
    }

    /// <summary>
    /// Represents an attribute that is used to set the Action to MyDashboard menu
    /// </summary>
    /// <remarks>
    /// <para>When the MyDashboardAttribute specified on the Action only the login user can access.</para>
    /// <para>After specified the MyDashboardAttribute also need to specified the Layout property to "~/Views/Shared/_MyDashboard.cshtml" </para>
    /// </remarks>
    /// <example>
    /// <para>This example create a Action page to show user orders</para>
    /// <para>The Controller</para>
    /// <code language="cs">
    /// [MyDashboardAttribute("My orders",Group="Shopping")]
    /// public ActionResult MyOrders()
    /// {
    ///     return View();
    /// }
    /// </code>
    /// <para>The MyOrders.cshtml view file</para>
    /// <code language="aspx">
    /// @{
    ///    Layout = "~/Views/Shared/_MyDashboard.cshtml";
    /// }
    /// @*Your view code here.*@
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MyDashboardAttribute : DashboardBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the MyDashboardAttribute class.
        /// </summary>
        public MyDashboardAttribute() { Order = 0; }

        /// <summary>
        /// Initializes a new instance of  MyDashboardAttribute class with the MyDashboard menu item text.
        /// </summary>
        /// <param name="text">The MyDashboard menu item text.</param>
        public MyDashboardAttribute(string text)
            : this()
        {
            Text = text;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ctx = filterContext.RequestContext.HttpContext;
            var app = App.Get();

            if (!ctx.Request.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }

            var profile = app.User.DefaultProfile;
            var lang = string.IsNullOrEmpty(profile.Language) ? App.Settings.DefaultLocale : profile.Language;
            app.SetCulture(lang);

            base.OnActionExecuting(filterContext);
        }
    }
}
