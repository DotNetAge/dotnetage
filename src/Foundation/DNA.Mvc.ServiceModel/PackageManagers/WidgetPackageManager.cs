//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using DNA.Web.UI;
using DNA.Xml;
using DNA.Xml.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    public class WidgetPackageManager : PackageManager<WidgetPackage, WidgetPackageFactory, WidgetElement>
    {
        private const string PKG_PATH = "~/content/widgets/";
        private PackageCollection<WidgetPackage, WidgetElement> packages;

        private HttpContextBase httpContext { get; set; }

        /// <summary>
        /// Gets/Sets the current data context object.
        /// </summary>
        public IDataContext DataContext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WidgetPackageManager() : this(new HttpContextWrapper(HttpContext.Current)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="context"></param>
        public WidgetPackageManager(HttpContextBase httpContext, IDataContext context)
            : this(httpContext)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.DataContext = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        public WidgetPackageManager(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            this.httpContext = httpContext;
            Init(httpContext.Server.MapPath(PKG_PATH));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public WidgetPackageManager(string path)
        {
            Init(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public override void Init(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            InstalledPath = path;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Categories
        {
            get
            {
                var dir = new DirectoryInfo(InstalledPath);
                var subdirs = dir.GetDirectories();
                return subdirs.Select(s => s.Name).ToArray();
            }
        }

        /// <summary>
        /// Gets all widget packages
        /// </summary>
        public override PackageCollection<WidgetPackage, WidgetElement> Packages
        {
            get
            {
                if (this.packages == null)
                {
                    packages = new PackageCollection<WidgetPackage, WidgetElement>();
                    packages.AddRange(LoadPackages());
                }
                return this.packages;
            }
        }

        private IEnumerable<WidgetPackage> LoadPackages()
        {
            var pkgs = new List<WidgetPackage>();
            var cats = Categories;
            var factory = new WidgetPackageFactory();

            foreach (var cat in cats)
            {
                var catPath = InstalledPath + cat;
                var dir = new DirectoryInfo(catPath);
                var subDirs = dir.GetDirectories();
                foreach (var sub in subDirs)
                {
                    try
                    {
                        pkgs.Add(factory.Create(sub.FullName));
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return pkgs;
        }

        /// <summary>
        /// Find the widget package by specified widget uid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WidgetPackage Find(string id)
        {
            return this.Packages.Find(p => p.Model.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the widget packages with in specified category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IEnumerable<WidgetPackage> GetPackages(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentNullException("category");

            return Packages.Where(p => p.Category.Equals(category)).ToList();
        }

        /// <summary>
        /// Register the specified widget package to widget catalog
        /// </summary>
        /// <param name="category">Specified category name that contains the widget package</param>
        /// <param name="name">Sepecified the widget package name which to register.</param>
        /// <returns></returns>
        public WidgetDescriptorDecorator Register(string category, string name)
        {
            if (DataContext == null)
                throw new ArgumentNullException("DataContext could not be null");

            var package = Packages.FirstOrDefault(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && p.Name.Equals(name));
            if (package == null)
                throw new WidgetPackageNotFoundException();

            return Register(package);
        }

        /// <summary>
        /// Register all widget package to widget catalog.
        /// </summary>
        /// <returns></returns>
        public int RegisterAll()
        {
            if (DataContext == null)
                throw new ArgumentNullException("DataContext could not be null");

            foreach (var pkg in Packages)
            {
                Register(pkg);
            }
            return DataContext.SaveChanges();
        }

        private WidgetDescriptorDecorator Register(WidgetPackage package)
        {
            if (package != null)
            {
                #region For common properties

                var widget = package.Locale(package.Model.DefaultLocale);

                var descriptor = new WidgetDescriptor()
                {
                    UID = widget.ID,
                    InstalledPath = package.Category + "\\" + package.Name,
                    Version = widget.Version,
                    Name = widget.Name.ShortName,
                    Title = widget.Name.FullName,
                    Description = widget.Description,
                    ContentType = widget.Content != null ? widget.Content.ContentType : "application/x-ms-aspnet",
                    ContentUrl = widget.Content != null ? widget.Content.Source : "",
                    ContentDirection = widget.Content != null ? widget.Content.Direction : "ltr",
                    ContentText = widget.Content != null ? widget.Content.Text : "",
                    Width = widget.Width,
                    Height = widget.Height,
                    ViewModes = widget.ViewModes,
                    Author = widget.Author != null ? widget.Author.Name : "",
                    AuthorEmail = widget.Author != null ? widget.Author.Email : "",
                    AuthorHomePage = widget.Author != null ? widget.Author.Uri : "",
                    Encoding = widget.Content != null ? widget.Content.Encoding : "utf-8",
                    ConfigXml = package.ConfigXml,
                    DefaultLocale = widget.DefaultLocale,
                    Modified = DateTime.Now,
                    IconUrl = (widget.Icons != null && widget.Icons.Count > 0) ? package.ResolveUri(widget.Icons.First().Source) : ""
                };

                var langs = package.GetSupportLanguages();
                if (langs != null && langs.Count() > 0)
                    descriptor.Locales = string.Join(",", langs);

                if (widget.Preferences != null && widget.Preferences.Count > 0)
                {
                    var items = new List<string>();
                    foreach (DNA.Xml.Widgets.PreferenceElement p in widget.Preferences)
                    {
                        items.Add("{name:\"" + p.Name + "\",readonly:" + p.IsReadonly.ToString().ToLower() + ",value:" + (string.IsNullOrEmpty(p.Value) ? "\"\"" : p.Value) + "}");
                    }
                    var jsonStr = "[" + string.Join(",", items.ToArray()) + "]";
                    descriptor.Defaults = jsonStr;
                }

                #endregion

                //Get widget type
                var widgetType = WidgetTypes.W3CWidget;

                if (widget.Features != null && widget.Features.Count > 0)
                {
                    var extFeature = widget.Features.FirstOrDefault(f => f.IsRequried && f.Name.Equals("mvc"));
                    if (extFeature != null)
                    {
                        widgetType = WidgetTypes.ServerWidget;
                        var area = extFeature.Params != null ? (extFeature.Params.FirstOrDefault(f => f.Name.Equals("area", StringComparison.OrdinalIgnoreCase))) : null;
                        var ctrl = extFeature.Params != null ? (extFeature.Params.FirstOrDefault(f => f.Name.Equals("controller", StringComparison.OrdinalIgnoreCase))) : null;
                        var act = extFeature.Params != null ? (extFeature.Params.FirstOrDefault(f => f.Name.Equals("action", StringComparison.OrdinalIgnoreCase))) : null;

                        if (area != null)
                            descriptor.Area = area.Value;

                        if (ctrl != null)
                            descriptor.Controller = ctrl.Value;

                        if (act != null)
                            descriptor.Action = act.Value;
                    }
                    widgetType = WidgetTypes.ServerWidget;
                }

                if (widget.Content != null)
                {
                    if (widget.Content.ContentType == "application/x-ms-aspnet")
                        widgetType = WidgetTypes.ServerWidget;
                }

                descriptor.WidgetType = (int)widgetType;
                DataContext.WidgetDescriptors.Create(descriptor);
                DataContext.SaveChanges();
                return new WidgetDescriptorDecorator(descriptor, DataContext);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Uninstall(string category, string name)
        {
            if (DataContext == null)
                throw new ArgumentNullException("DataContext could not be null");

            var package = Packages.FirstOrDefault(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && p.Name.Equals(name));
            if (package == null)
                throw new WidgetPackageNotFoundException();
            var result = package.Delete();
            if (result) this.packages = null;
            return result;
        }

        /// <summary>
        /// Detect the Widgets in assemblies then create the widget template to widget catalog.
        /// </summary>
        public void Detect(string targetPath, string defaultCategory = "utilities")
        {
            if (string.IsNullOrEmpty(targetPath))
                throw new ArgumentNullException("targetPath");

            //Auto detect package info
            string[] files = Directory.GetFiles(targetPath, "*.dll");

            foreach (string file in files)
            {
                try
                {
                    //When using LoadFile will cause could not get CustomAttributes!
                    Assembly assembly = Assembly.LoadFrom(file);
                    var attrs = assembly.GetCustomAttributes(true);
                    var fileInfo = new FileInfo(file);
                    var asmname = assembly.GetName();

                    Type[] types = assembly.GetTypes();

                    var controllers = types.Where(c => (c.BaseType == typeof(Controller) || c.BaseType == typeof(AsyncController)));

                    foreach (Type controller in controllers)
                    {
                        var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                        var actions = methods.Where(m => (m.GetCustomAttributes(typeof(WidgetAttribute), true).Length > 0));
                        string controllerShortName = controller.Name.Replace("Controller", "");
                        List<string> actionNames = new List<string>();

                        foreach (MethodInfo action in actions)
                        {
                            WidgetAttribute widgetAttr = (WidgetAttribute)action.GetCustomAttributes(typeof(WidgetAttribute), true)[0];
                            string widgetCat = !string.IsNullOrEmpty(widgetAttr.Category) ? widgetAttr.Category : defaultCategory;
                            var actionName = action.Name;

                            //Supports the AsyncController
                            if (action.Name.EndsWith("async", StringComparison.OrdinalIgnoreCase))
                                actionName = action.Name.Replace("Async", "");

                            var tmplID = widgetCat + "\\" + actionName;

                            //When the Widget had already registered then continue.
                            var registeredDescriptor = DataContext.WidgetDescriptors.Find(tmplID);

                            if (registeredDescriptor != null)
                                continue;

                            actionNames.Add("\"" + actionName + "\"");

                            var properties = action.GetCustomAttributes(typeof(PropertyAttribute), true);
                            var defaults = new Dictionary<string, object>();
                            foreach (var pro in properties)
                            {
                                var _pro = ((PropertyAttribute)pro);
                                defaults.Add(_pro.Name, _pro.DefaultValue);
                            }

                            var tmpl = new WidgetElement()
                            {
                                ID = tmplID,
                                Name = new NameElement()
                                {
                                    ShortName = actionName,
                                    FullName = widgetAttr.Title
                                },
                                Description = widgetAttr.Description,
                                Version = assembly.GetName().Version.ToString(),
                                //Author = new AuthorElement()
                                //{
                                //    //Name = webOwner.UserName,
                                //    //Uri = appPath,
                                //    //Email = webOwner.Email
                                //},
                                //Icons = new List<IconElement>(){
                                //  new IconElement(){ Source= widgetAttr.IconUrl}
                                //},
                                Features = new List<FeatureElement>(){
                                new FeatureElement(){
                                 Name ="mvc",
                                  IsRequried =true,
                                   Params=new List<ParamElement>()
                                   {
                                       new ParamElement(){ Name="area",Value=string.IsNullOrEmpty(widgetAttr.Area) ? GetArea(controller) : widgetAttr.Area},
                                       new ParamElement(){Name="controller",Value=controllerShortName},
                                       new ParamElement(){Name="action",Value=actionName}
                                   }
                                }
                                }
                            };

                            if (!string.IsNullOrEmpty(widgetAttr.IconUrl))
                                tmpl.Icons = new List<IconElement>(){
                                  new IconElement(){ Source= widgetAttr.IconUrl}
                                };

                            if (defaults.Count > 0)
                            {
                                tmpl.Preferences = new List<PreferenceElement>();

                                foreach (var key in defaults.Keys)
                                {
                                    tmpl.Preferences.Add(new PreferenceElement()
                                    {
                                        IsReadonly = false,
                                        Name = key,
                                        //Type = defaults[key] != null ? defaults[key].GetType().ToString() : (typeof(string)).ToString(),
                                        Value = defaults[key] != null ? defaults[key].ToString() : ""
                                    });
                                }
                            }

                            var widgetPath = InstalledPath + widgetCat + "\\" + actionName;
                            if (!Directory.Exists(widgetPath))
                                Directory.CreateDirectory(widgetPath);

                            var fileName = widgetPath + "\\config.xml";
                            XmlSerializerUtility.SerializeToXmlFile(fileName, tmpl);
                        }
                    }
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    continue;
                }
            }
        }

        private static string GetArea(Type ctrlType)
        {
            var types = ctrlType.Assembly.GetTypes();
            var areaRegistrationType = types.FirstOrDefault(t => t.BaseType != null && t.BaseType.Equals(typeof(AreaRegistration)));
            if (areaRegistrationType == null)
                return "";
            else
            {
                var areaInstance = (AreaRegistration)Activator.CreateInstance(areaRegistrationType);
                return areaInstance.AreaName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="installedPath"></param>
        /// <returns></returns>
        public WidgetDescriptorDecorator Update(string installedPath)
        {
            if (string.IsNullOrEmpty(installedPath))
                throw new ArgumentNullException("installedPath");
            var args = installedPath.Split('\\');

            return Update(args[0], args[1]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public WidgetDescriptorDecorator Update(string category, string name)
        {
            if (DataContext == null)
                throw new ArgumentNullException("DataContext could not be null");

            var package = Packages.FirstOrDefault(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && p.Name.Equals(name));
            if (package == null)
                throw new WidgetPackageNotFoundException();

            if (package != null)
            {
                #region For common properties

                var widget = package.Locale(package.Model.DefaultLocale);
                var _installedPath = category + "\\" + name;
                var descriptor = DataContext.Find<WidgetDescriptor>(w => w.InstalledPath.Equals(_installedPath));

                descriptor.Version = widget.Version;
                descriptor.Name = widget.Name.ShortName;
                descriptor.Title = widget.Name.FullName;
                descriptor.Description = widget.Description;
                descriptor.ContentType = widget.Content != null ? widget.Content.ContentType : "application/x-ms-aspnet";
                descriptor.ContentUrl = widget.Content != null ? widget.Content.Source : "";
                descriptor.ContentDirection = widget.Content != null ? widget.Content.Direction : "ltr";
                descriptor.ContentText = widget.Content != null ? widget.Content.Text : "";
                descriptor.Width = widget.Width;
                descriptor.Height = widget.Height;
                descriptor.ViewModes = widget.ViewModes;
                descriptor.Encoding = widget.Content != null ? widget.Content.Encoding : "utf-8";
                descriptor.ConfigXml = package.ConfigXml;
                descriptor.Modified = DateTime.Now;
                descriptor.IconUrl = (widget.Icons != null && widget.Icons.Count > 0) ? package.ResolveUri(widget.Icons.First().Source) : "";

                var langs = package.GetSupportLanguages();
                if (langs != null && langs.Count() > 0)
                    descriptor.Locales = string.Join(",", langs);

                if (widget.Preferences != null && widget.Preferences.Count > 0)
                {
                    var items = new List<string>();
                    foreach (DNA.Xml.Widgets.PreferenceElement p in widget.Preferences)
                        items.Add("{name:\"" + p.Name + "\",readonly:" + p.IsReadonly.ToString().ToLower() + ",value:" + (string.IsNullOrEmpty(p.Value) ? "\"\"" : p.Value) + "}");
                    var jsonStr = "[" + string.Join(",", items.ToArray()) + "]";
                    descriptor.Defaults = jsonStr;
                }

                #endregion

                //Get widget type
                var widgetType = WidgetTypes.W3CWidget;

                if (widget.Features != null && widget.Features.Count > 0)
                {
                    var extFeature = widget.Features.FirstOrDefault(f => f.IsRequried && f.Name.Equals("mvc"));
                    if (extFeature != null)
                    {
                        widgetType = WidgetTypes.ServerWidget;
                        var area = extFeature.Params != null ? (extFeature.Params.FirstOrDefault(f => f.Name.Equals("area", StringComparison.OrdinalIgnoreCase))) : null;
                        var ctrl = extFeature.Params != null ? (extFeature.Params.FirstOrDefault(f => f.Name.Equals("controller", StringComparison.OrdinalIgnoreCase))) : null;
                        var act = extFeature.Params != null ? (extFeature.Params.FirstOrDefault(f => f.Name.Equals("action", StringComparison.OrdinalIgnoreCase))) : null;

                        if (area != null)
                            descriptor.Area = area.Value;

                        if (ctrl != null)
                            descriptor.Controller = ctrl.Value;

                        if (act != null)
                            descriptor.Action = act.Value;
                    }
                    widgetType = WidgetTypes.ServerWidget;
                }

                if (widget.Content != null)
                {
                    if (widget.Content.ContentType == "application/x-ms-aspnet")
                        widgetType = WidgetTypes.ServerWidget;
                }

                descriptor.WidgetType = (int)widgetType;

                var descriptorWrapper = new WidgetDescriptorDecorator(descriptor, DataContext);

                #region update the in using widgets

                foreach (var wi in descriptor.Widgets)
                {
                    var prefs = wi.ReadUserPreferences();
                    var defaults = descriptorWrapper.GetDefaultValues();

                    foreach (var def in defaults)
                    {
                        var pref = prefs.FirstOrDefault(p => p["name"].Equals(def["name"]));
                        if (pref != null)
                        {
                            def["name"] = pref["name"];
                            def["value"] = pref["value"];
                            def["readonly"] = pref["readonly"];
                        }
                    }

                    wi.SaveUserPreferences(defaults);
                }

                #endregion
                DataContext.SaveChanges();
                return descriptorWrapper;
            }
            return null;
        }

        public void Reload()
        {
            this.packages = null;
        }
    }

}
