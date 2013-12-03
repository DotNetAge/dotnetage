//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to WidgetInstance object model. 
    /// </summary>
    public sealed class WidgetInstanceDecorator : WidgetInstance
    {
        private IWidgetRepository Repository { get; set; }
        internal WidgetInstance Model { get; set; }
        private string[] _roles = null;

        /// <summary>
        /// Initializes a new instance of the WidgetInstanceDecorator class.
        /// </summary>
        /// <param name="widget">The widget instance object.</param>
        /// <param name="repository">The widget repository</param>
        public WidgetInstanceDecorator(WidgetInstance widget, IWidgetRepository repository)
        {
            this.Repository = repository;
            this.Model = widget;
            widget.CopyTo(this, "WebPage", "WidgetDescriptor", "Roles");
            ReflectionHelper.Copy(widget, this);
        }

        /// <summary>
        /// Gets the widget access roles.
        /// </summary>
        public new string[] Roles
        {
            get
            {
                if (_roles == null)
                    _roles = Repository.GetRoles(ID);
                return _roles;
            }
        }

        /// <summary>
        /// Add access roles to widget.
        /// </summary>
        /// <param name="roles"></param>
        public void AddRoles(params string[] roles)
        {
            Repository.AddRoles(ID, roles);
            _roles = roles;
            Repository.Submit();
        }

        /// <summary>
        /// Toggle the widget expand state
        /// </summary>
        public void Toggle()
        {
            Model.IsExpanded = !Model.IsExpanded;
            this.IsExpanded = Model.IsExpanded;
            Repository.Update(Model);
            Repository.Submit();
        }

        /// <summary>
        /// Move the widget to specified zone and position.
        /// </summary>
        /// <param name="zoneID">The widget zone id.</param>
        /// <param name="pos">The widget position in the zone.</param>
        public void MoveTo(string zoneID, int pos)
        {
            if (this.ZoneID != zoneID || this.Pos != pos)
            {
                Repository.MoveTo(this.ID, zoneID, pos);
                Repository.Submit();
            }
        }

        /// <summary>
        /// Save widget properties to database.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            this.CopyTo(Model, "WebPage", "WidgetDescriptor", "Roles", "ShowMode");
            Repository.Update(Model);
            return Repository.Submit() > 0;
        }

        /// <summary>
        /// Convert widget instance object to json form string.
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        /// <param name="culture">The culture name.</param>
        /// <param name="website">The website name.</param>
        /// <returns>A string contains widget instance data in json format.</returns>
        public string ToJsonString(HttpContextBase httpContext, string culture, string website = "")
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(ToObject(httpContext, culture, website));
        }

        /// <summary>
        /// Convert widget instance object to dynamic object.
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        /// <param name="culture">The culture name.</param>
        /// <param name="website">The website name.</param>
        /// <returns></returns>
        public dynamic ToObject(HttpContextBase httpContext, string culture, string website = "")
        {
            var Url = new UrlHelper(httpContext.Request.RequestContext);
            var descroptor = this.WidgetDescriptor;
            var contentUrl = descroptor.ContentUrl;
            //var culture = httpContext.Request.RequestContext.RouteData.Values["locale"];
            var widget = this;

            if (!string.IsNullOrEmpty(descroptor.Controller) && !string.IsNullOrEmpty(descroptor.Action))
                contentUrl = !string.IsNullOrEmpty(website) ? Url.Action(descroptor.Action, descroptor.Controller, new { locale = culture, Area = string.IsNullOrEmpty(descroptor.Area) ? "" : descroptor.Area, website = website, id = 0 }) : Url.Action(descroptor.Action, descroptor.Controller, new { locale = culture, Area = string.IsNullOrEmpty(descroptor.Area) ? "" : descroptor.Area });
            else
                contentUrl = !string.IsNullOrEmpty(website) ? Url.Action("Generic", "Widget", new { locale = culture, Area = "", website = website, id = widget.ID }) : Url.Action("Generic", "Widget", new { locale = culture, Area = "", id = widget.ID });

            return new
            {
                id = widget.ID.ToString(),
                expanded = widget.IsExpanded,
                cssText = widget.CssText,
                headerCssText = widget.HeaderCssText,
                bodyCssText = widget.BodyCssText,
                pos = widget.Pos,
                showHeader = widget.ShowHeader,
                showBorder = widget.ShowBorder,
                transparent=widget.Transparent,
                isStatic = widget.IsStatic,
                mode = widget.ShowMode,
                viewMode=widget.ViewMode,
                title = widget.Title,
                link = !string.IsNullOrEmpty(widget.Link) ? Url.Content(widget.Link) : "",
                icon = !string.IsNullOrEmpty(widget.IconUrl) ? Url.Content(widget.IconUrl) : "",
                contentUrl = contentUrl,
                zone = widget.ZoneID
            };
        }

        /// <summary>
        /// Set user preference
        /// </summary>
        /// <param name="key">The user preference key.</param>
        /// <param name="value">The user preference value.</param>
        /// <param name="readonly">Indicates whether the value is readonly.</param>
        public void SetUserPreference(string key, object value, bool @readonly = false)
        {
            List<Dictionary<string, object>> jsonData = null;
            if (string.IsNullOrEmpty(this.Data))
            {
                if (!string.IsNullOrEmpty(this.WidgetDescriptor.Defaults))
                    jsonData = this.WidgetDescriptor.Properties;
            }
            else
            {
                jsonData = (new JavaScriptSerializer()).Deserialize<List<Dictionary<string, object>>>(this.Data);
            }

            if (jsonData != null && jsonData.Count > 0)
            {
                var pref = jsonData.FirstOrDefault(s => s["name"].Equals(key));
                if (pref != null)
                {
                    pref["name"] = key;
                    pref["value"] = value;
                    pref["readonly"] = @readonly;
                }
                else
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("name", key);
                    dic.Add("readonly", @readonly);
                    dic.Add("value", value);
                    jsonData.Add(dic);
                }
                SaveUserPreferences(jsonData.Select(j => (IDictionary<string, object>)j).ToList());
            }
            else
                this.Data = "";
        }

        /// <summary>
        /// Get user preference value by specified key.
        /// </summary>
        /// <param name="key">The user preference key.</param>
        /// <returns>The object value.</returns>
        public object GetUserPreference(string key)
        {
            List<Dictionary<string, object>> jsonData = null;
            if (string.IsNullOrEmpty(this.Data))
            {
                if (!string.IsNullOrEmpty(this.WidgetDescriptor.Defaults))
                    jsonData = this.WidgetDescriptor.Properties;
            }
            else
            {
                jsonData = (new JavaScriptSerializer()).Deserialize<List<Dictionary<string, object>>>(this.Data);
            }

            if (jsonData != null && jsonData.Count > 0)
            {
                var pref = jsonData.FirstOrDefault(s => s["name"].Equals(key));
                if (pref != null)
                {
                    return pref["value"];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets / Sets the widget show mode.
        /// </summary>
        public new WidgetShowModes ShowMode
        {
            get { return (WidgetShowModes)this.Model.ShowMode; }
            set { this.Model.ShowMode = (int)value; }
        }

        #region schema
        private const string _ID = "pid";
        private const string TITLE = "title";
        private const string ICON = "icon";
        private const string LINK = "link";
        private const string STYLE = "style";
        private const string PREFERENCE = "preference";
        private const string ZONE = "zone";
        private const string SEQ = "seq";
        private const string ROLES = "roles";
        private const string ACTION = "action";
        private const string CTRL = "controller";
        private const string SHOWIN = "showIn";
        private const string VIEWMODE = "viewmode";
        private const string AREA = "area";
        private const string WIDGET = "widget";
        private const string NAME = "name";
        private const string VALUE = "value";
        private const string SRC = "src";
        private const string CLS = "class";
        #endregion
        
        /// <summary>
        /// Convert the widget instance object to xml data element.
        /// </summary>
        /// <returns>The xml element that contains widget data.</returns>
        public XElement Element()
        {
            XNamespace ns = "http://www.dotnetage.com/XML/Schema/widget-data";

            var element = new XElement(ns + WIDGET,
                new XAttribute(_ID, this.WidgetDescriptor.UID),
                new XAttribute(ZONE, this.ZoneID),
                new XAttribute(SHOWIN, GetShowMode(this.ShowMode)));

            if (!string.IsNullOrEmpty(this.Title))
                element.Add(new XElement(ns + TITLE, new XCData(Title)));

            if (this.Pos > 0)
                element.Add(new XAttribute(SEQ, this.Pos));

            if (this.Roles != null && this.Roles.Length > 0)
                element.Add(new XAttribute(ROLES, string.Join(",", this.Roles)));

            if (!string.IsNullOrEmpty(this.ViewMode))
                element.Add(new XAttribute(VIEWMODE, this.ViewMode));

            if (!string.IsNullOrEmpty(this.Link))
                element.Add(new XElement(ns + LINK, new XAttribute(SRC, this.Link)));

            if (this.IsStatic)
                element.Add(new XAttribute("static", true));

            #region style elements
            var styleEl = new XElement(ns + STYLE);
            if (!string.IsNullOrEmpty(BodyCssText) || !string.IsNullOrEmpty(BodyClass))
            {
                var boxEl = new XElement(ns + "box");

                if (!string.IsNullOrEmpty(BodyClass))
                    boxEl.Add(new XAttribute(CLS, BodyClass));

                if (!string.IsNullOrEmpty(BodyCssText))
                    boxEl.Add(new XCData(BodyCssText));

                styleEl.Add(boxEl);
            }

            if (!string.IsNullOrEmpty(HeaderClass) || !string.IsNullOrEmpty(HeaderCssText))
            {
                var headerEl = new XElement(ns + "header");
                if (!string.IsNullOrEmpty(HeaderClass))
                    headerEl.Add(new XAttribute(CLS, HeaderClass));

                if (!string.IsNullOrEmpty(HeaderCssText))
                    headerEl.Add(new XCData(HeaderCssText));

                if (!this.ShowHeader)
                    headerEl.Add(new XAttribute("hidden", true));

                styleEl.Add(headerEl);
            }

            if (!string.IsNullOrEmpty(BodyClass) || !string.IsNullOrEmpty(BodyCssText))
            {
                var bodyEl = new XElement(ns + "body");
                if (!string.IsNullOrEmpty(BodyClass))
                    bodyEl.Add(new XAttribute(CLS, BodyClass));
                if (!string.IsNullOrEmpty(BodyCssText))
                    bodyEl.Add(new XCData(BodyCssText));

                styleEl.Add(bodyEl);
            }


            if (styleEl.HasElements)
                element.Add(styleEl);

            #endregion

            if (!this.ShowHeader)
            {
                element.Add(new XElement(ns + PREFERENCE,
                        new XAttribute(NAME, "showHeader"),
                        new XAttribute(VALUE, "false")));
            }

            if (!this.ShowBorder)
            {
                element.Add(new XElement(ns + PREFERENCE,
                        new XAttribute(NAME, "showBorder"),
                        new XAttribute(VALUE, "false")));
            }

            if (!string.IsNullOrEmpty(this.Data))
            {
                var data = ReadUserPreferences();
                foreach (var d in data)
                {
                    var pref = new XElement(ns + PREFERENCE,
                        new XAttribute(NAME, d[NAME]),
                        new XAttribute(VALUE, FormatVal(d[VALUE])));
                    element.Add(pref);
                }
            }

            return element;
        }

        private string GetShowMode(WidgetShowModes showIn)
        {
            if (showIn == WidgetShowModes.ParentPage)
                return "parent";

            if (showIn == WidgetShowModes.DescendantPages)
                return "descendant";

            return "all";
        }

        private object FormatVal(object val)
        {
            if (val != null)
            {
                if (val is string)
                {
                    var str = val.ToString();
                    if (!str.StartsWith("'") && !str.EndsWith("'"))
                        return "'" + str + "'";
                }

                if (val is bool)
                    return val.ToString().ToLower();
            }
            return val;
        }
    }
    
    /// <summary>
    /// This enumeration has a show mode flag.
    /// </summary>
    public enum WidgetShowModes
    {
        /// <summary>
        /// Show the widget in own page.
        /// </summary>
        ParentPage,
        /// <summary>
        /// Show the widget in all pages.
        /// </summary>
        AllPages,
        /// <summary>
        /// Show the widget in all descendant pages.
        /// </summary>
        DescendantPages
    }
}
