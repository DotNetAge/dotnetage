//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DNA.Web.UI
{
    /// <summary>
    /// Represents an attribute that is used to handle the widget action.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///    When application start the DotNetAge will auto discovery all Action that has <c>WidgetAttribute</c> definition and create 
    ///    <c>WidgetDescriptor</c>s by widget assembly version.</para>
    /// <para>
    ///   If you want DotNetAge widget engine renew the WidgetDescriptor every time please set the <c>AssemblyVersionAttribute</c> to enable auto incase
    ///   build version number in  AssemblyInfo.cs in Widget project. i.e:<c>[assembly: AssemblyVersion("1.0.0.*")]</c>
    ///  </para>
    /// </remarks>
    /// <example>
    ///   This example define a simple widget that just render a word text.
    /// <code language="cs"> 
    ///public class SampleWidgetController:Controller
    ///{
    ///   [Widget("SayHello","The widget just say hello.")]
    ///   [Property("Word", PropertyControl=ConrolTypes.TextArea)]
    ///   public ActionResult Hello()
    ///   {
    ///       return View();
    ///    }
    ///}
    /// <code language="aspx"> 
    ///&lt;%:@ Control Language="C#" Inherits="DNA.Web.DynamicUI.WidgetViewUserControl"  %&gt;
    ///&lt;%:Ajax.RenderAutoSettingForm(PropertyDescriptors, IDPrefix, IsDesignMode)%&gt;
    ///&lt;%:UserData["Word"] %&gt;
    ///&lt;%:Html.StartupScripts() %&gt;
    /// </code>
    /// </example>
    public class WidgetAttribute : ActionFilterAttribute
    {
        //private Scopes scope = Scopes.Shared;
        private bool showHeader = true;
        //private bool isClosable = true;
        //private bool isDeletable = true;
        //private bool showBorder = true;
        private string category = "Shared";

        /// <summary>
        /// Initialize the WidgetAttribute
        /// </summary>
        public WidgetAttribute() { }

        /// <summary>
        /// Initialize the WidgetAttribute by specified the widget title.
        /// </summary>
        /// <param name="title">The widget title.</param>
        public WidgetAttribute(string title) { Title = title; }

        /// <summary>
        /// Initializes a new instance of the WidgetAttribute class by specified the widget title and widget description.
        /// </summary>
        /// <param name="title">The widget title.</param>
        /// <param name="description">The widget description.</param>
        public WidgetAttribute(string title, string description) { Title = title; Description = description; }

        /// <summary>
        /// Gets/Sets wheather the widget header shows
        /// </summary>
        public bool ShowHeader
        {
            get { return showHeader; }
            set { showHeader = value; }
        }

        ///// <summary>
        ///// Gets/Sets wheather the widget can close
        ///// </summary>
        //public bool IsClosable
        //{
        //    get { return isClosable; }
        //    set { isClosable = value; }
        //}

        ///// <summary>
        ///// Gets/Sets wheather the widget can be deleted
        ///// </summary>
        //public bool IsDeletable
        //{
        //    get { return isDeletable; }
        //    set { isDeletable = value; }
        //}

        /// <summary>
        /// Gets/Sets the title of the widget
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the widget category
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        /// <summary>
        /// Gets/Sets the description of the widget.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets the title link url of the widget.
        /// </summary>
        public string TitleLink { get; set; }

        /// <summary>
        /// Gets/Sets the icon url of the widget that display on the left side of the widget header.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets/Sets the image url which display in the widget explorer.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets/Sets the Area that widget action belongs to.
        /// </summary>
        /// <remarks>
        /// If your Widget is has not a view that have same name as Action,and the widget action is in Area,you
        /// must specified this property.
        /// </remarks>
        public string Area { get; set; }

      

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            
            string strWidgetID = "";
           //bool isPreview = false;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (!string.IsNullOrEmpty(request.QueryString["id"]))
                    strWidgetID = request.QueryString["id"];
                //if (request.QueryString["preview"] != null)
                //    bool.TryParse(request.QueryString["preview"], out isPreview);
            }
            else
            {
                if (filterContext.RouteData.Values.ContainsKey("id"))
                    strWidgetID = filterContext.RouteData.Values["id"] as string;
                //if (filterContext.RouteData.Values.ContainsKey("preview"))
                //    bool.TryParse(filterContext.RouteData.Values["preview"] as string,out isPreview);
            }

            if (!string.IsNullOrEmpty(strWidgetID))
            {
                var controller = filterContext.Controller;
                var ViewData = controller.ViewData;

                var attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(PropertyAttribute), true);
                //var cacheAttrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(OutputCacheAttribute), true);
                //if (cacheAttrs.Count() > 0)
                //    _ignoreGlobalCache = true;
                //controller.TempData["IsPreview"] = isPreview;
                //controller.ViewBag.IsPreview = isPreview;

                var descriptors = new Dictionary<string, PropertyDescriptor>();

                //Get default values
                foreach (var attr in attrs)
                {
                    PropertyAttribute _attr = (PropertyAttribute)attr;
                    descriptors.Add(_attr.Name, new PropertyDescriptor()
                    {
                        Name = _attr.Name,
                        Value = _attr.DefaultValue,
                        DisplayName = _attr.DisplayName,
                        ValueType = _attr.ValueType,
                        PropertyControl = _attr.PropertyControl
                    });
                }

                #region no use
                //bool isApplySettingRequest = false;
                //if (!string.IsNullOrEmpty(request.QueryString["apply"]))
                //    bool.TryParse(request.QueryString["apply"], out isApplySettingRequest);

                //if (!isPreview)
                //{
                //    //string wid = request.QueryString["wid"];
                //    Guid id = new Guid(strWidgetID);
                //    string idPrefix = strWidgetID.Substring(0, 5);

                //    WidgetInstance _widget = null;
                //    try
                //    {
                //        _widget = Context.DataContext.Widgets.Find(id);
                //    }
                //    catch (Exception e)
                //    {
                //        ///WorkAround: This execption is strange!
                //        System.Threading.Thread.CurrentThread.Join(100);
                //        _widget = Context.DataContext.Widgets.Find(id);
                //    }

                //    #region When the widget request the "apply" command to server

                //    ////if (isApplySettingRequest)
                //    //if (request.HttpMethod == "PUT")
                //    //{
                //    //    filterContext.HttpContext.Response.AddHeader("cache-control", "no-cache");
                //    //    Dictionary<string, object> settings = new Dictionary<string, object>();
                //    //    FormValueProvider valueProvider = new FormValueProvider(filterContext.Controller.ControllerContext);

                //    //    foreach (var k in descriptors.Keys)
                //    //    {
                //    //        string name = k + idPrefix;
                //    //        if (valueProvider.ContainsPrefix(name))
                //    //            settings.Add(k, valueProvider.GetValue(name).ConvertTo(descriptors[k].ValueType));
                //    //    }

                //    //    if (settings != null)
                //    //    {
                //    //        IDictionary<string, object> data = ObjectHelper.ConvertObjectToDictionary(settings);
                //    //        _widget.SaveUserPreferences(data);
                //    //        Context.DataContext.Widgets.Update(_widget);
                //    //        Context.DataContext.SaveChanges();
                //    //    }
                //    //    //Service.ApplySettings(id, settings);
                //    //    filterContext.Result = new ContentResult() { Content = "OK" };
                //    //    return;
                //    //}

                //    #endregion


                //    // Service.GetWidget(id);
                //    //controller.TempData["WidgetInstance"] = _widget;
                //    controller.ViewBag.WidgetInstance = _widget;

                //    var _pros = _widget.ReadUserPreferences();

                //    foreach (var _pro in _pros)
                //    {
                //        var key=(string)_pro["name"];
                //        if (descriptors.ContainsKey(key))
                //            descriptors[key].Value=_pro["value"];
                //        //if (descriptors.ContainsKey(key))
                //           // descriptors[key].Value = _pros[key];
                //    }
                //}

                //controller.TempData["PropertyDescriptors"] = descriptors;
                #endregion

                int gid = 0;// Guid.Empty;
                //Guid.TryParse(strWidgetID, out gid);
                int.TryParse(strWidgetID, out gid);

                var _widget =App.Get().DataContext.Widgets.Find(gid);
                
                controller.ViewBag.PropertyDescriptors = descriptors;

                if (filterContext.ActionParameters.Count > 0)
                {
                    //foreach (var p in filterContext.ActionParameters)
                    if (_widget != null)
                    {
                        var ps = filterContext.ActionDescriptor.GetParameters();
                        foreach (var p in ps)
                            if (p.ParameterType.Equals(typeof(WidgetInstance)))
                                filterContext.ActionParameters[p.ParameterName] = _widget;
                    }

                    foreach (var key in descriptors.Keys)
                    {
                        string formatKey = key[0].ToString().ToLower() + key.Substring(1);
                        if (filterContext.ActionParameters.ContainsKey(formatKey))
                            filterContext.ActionParameters[formatKey] = descriptors[key].Value;
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
