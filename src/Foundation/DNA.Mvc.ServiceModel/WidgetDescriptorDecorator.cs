//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to WidgetDescriptor object model. 
    /// </summary>
    public class WidgetDescriptorDecorator : WidgetDescriptor
    {
        private IDataContext Context { get; set; }

        private IWidgetDescriptorRepository Repository { get { return Context.WidgetDescriptors; } }

        /// <summary>
        /// Initializes a new instance of the WidgetDescriptorDecorator class with given widget descriptor and data context object.
        /// </summary>
        /// <param name="descriptor">The widget descriptor</param>
        /// <param name="context">The data context.</param>
        public WidgetDescriptorDecorator(WidgetDescriptor descriptor, IDataContext context)
        {
            Model = descriptor;
            descriptor.CopyTo(this, new string[] { "Widgets", "Roles" });
            Context = context;
        }

        /// <summary>
        /// Gets the WidgetDescriptor object model.
        /// </summary>
        public WidgetDescriptor Model { get; set; }

        /// <summary>
        /// Gets access roles of this widget descriptor.
        /// </summary>
        public new string[] Roles
        {
            get { return Repository.GetRoles(this.ID); }
        }

        /// <summary>
        /// Add access roles to widget descriptor.
        /// </summary>
        /// <param name="roles">The access role names.</param>
        public void AddRoles(params string[] roles)
        {
            Repository.AddRoles(ID, roles);
            Context.SaveChanges();
        }

        /// <summary>
        /// Save and submit changes to database.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            this.CopyTo(Model, new string[] { "Widgets", "Roles" });
            Context.Update(Model);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Create widget instance by specified page id ,zone id and position.
        /// </summary>
        /// <param name="pageID">The page id that contains the new widget.</param>
        /// <param name="zoneID">The widget zone id.</param>
        /// <param name="pos">The position index of the widget in the zone.</param>
        /// <returns>A widget instance </returns>
        public WidgetInstanceDecorator InstantiateIn(int pageID, string zoneID, int pos = 0)
        {
            return InstantiateIn(Context.WebPages.Find(pageID), zoneID, pos);
        }

        /// <summary>
        /// Create widget instance to specified web page.
        /// </summary>
        /// <param name="page">The page that contains the new widget.</param>
        /// <param name="zoneID">The widget zone id.</param>
        /// <param name="pos">The position index of the widget in the zone.</param>
        /// <returns>A widget instance </returns>
        public WidgetInstanceDecorator InstantiateIn(WebPage page, string zoneID, int pos = 0)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (string.IsNullOrEmpty(zoneID))
                throw new ArgumentNullException("zoneID");

            var pageWrapper = new WebPageDecorator(page, Context);
            return pageWrapper.Widgets.Add(this.ID, zoneID, pos);
        }

        /// <summary>
        /// Gets widget render url 
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        /// <param name="website">The website name</param>
        /// <param name="locale">The locale name.</param>
        /// <returns></returns>
        public string GetContentUrl(HttpContextBase httpContext, string website = "home", string locale = "en-US")
        {
            var contentUrl = "";
            var Url = new System.Web.Mvc.UrlHelper(httpContext.Request.RequestContext);
            if (!string.IsNullOrEmpty(Controller) && !string.IsNullOrEmpty(Action))
                contentUrl = !string.IsNullOrEmpty(website) ? Url.Action(Action, ControllerShortName, new { Area = string.IsNullOrEmpty(this.Area) ? "" : this.Area, website = website, id = this.ID, preview = true }) : Url.Action(this.Action, this.ControllerShortName, new { Area = string.IsNullOrEmpty(this.Area) ? "" : this.Area, id = this.ID, preview = true });
            else
                contentUrl = !string.IsNullOrEmpty(website) ? Url.Action("Generic", "Widget", new { Area = "", website = website, id = this.ID, preview = true }) : Url.Action("Generic", "Widget", new { Area = "", id = this.ID, preview = true });
            return contentUrl;
        }

        /// <summary>
        /// Get widget default values.
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetDefaultValues()
        {
            if (!string.IsNullOrEmpty(this.Defaults))
                return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(this.Defaults);
            return null;
        }

        /// <summary>
        /// Convert default values to expando object.
        /// </summary>
        /// <returns></returns>
        public dynamic GetDeafultObject()
        {
            if (!string.IsNullOrEmpty(this.Defaults))
            {
                dynamic _defs = new System.Dynamic.ExpandoObject();
                var _defObjs = GetDefaultValues();
                foreach (var obj in _defObjs)
                {
                    var name = (string)obj["name"];
                    ((IDictionary<String, Object>)_defs).Add(name, obj["value"]);
                }
                return _defs;
            }
            return null;
        }

        /// <summary>
        /// Gets total reference count.
        /// </summary>
        public int ReferenceCount
        {
            get
            {
                return this.Repository.InusingWidgetsCount(this.InstalledPath);
            }
        }

        /// <summary>
        /// Convert widget descriptor to dynamic object.
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        /// <param name="website">The website name.</param>
        /// <param name="locale">The locale name.</param>
        /// <returns></returns>
        public dynamic ToObject(HttpContextBase httpContext, string website = "home", string locale = "en-US")
        {
            var Url = new System.Web.Mvc.UrlHelper(httpContext.Request.RequestContext);
            return new
            {
                id = this.ID,
                title = this.Title,
                description = this.Description,
                icon = string.IsNullOrEmpty(this.IconUrl) ? null : Url.Content(this.IconUrl),
                contentUrl = GetContentUrl(httpContext, website, locale),
                contentType = this.ContentType,
                version = this.Version,
                defaults = this.Defaults
            };
        }

        /// <summary>
        /// Convert the widget descriptor to json string.
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        /// <param name="website">The website name.</param>
        /// <param name="locale">The locale name.</param>
        /// <returns>A string that contains widget descriptor object in json format.</returns>
        public string ToJsonString(HttpContextBase httpContext, string website = "home", string locale = "en-US")
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(ToObject(httpContext, website, locale));
        }

    }
}
