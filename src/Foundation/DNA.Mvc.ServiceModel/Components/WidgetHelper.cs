//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DNA.Web.UI
{
    /// <summary>
    /// The helper class provides use in Widget view.
    /// </summary>
    public class WidgetHelper
    {
        private Dictionary<string, object> userPreferences;
        private WidgetInstance instance;
        private Dictionary<string, PropertyDescriptor> propertyDescriptors = null;

        public Dictionary<string, PropertyDescriptor> PropertyDescriptors
        {
            get { return propertyDescriptors; }
            internal set { propertyDescriptors = value; }
        }

        /// <summary>
        /// Gets the id prefix for widget element
        /// </summary>
        public string IDPrefix
        {
            get
            {
                if (instance != null)
                {
                    return instance.ID.ToString();
                    //.Substring(0, 5);
                }
                return "";
            }
        }

        /// <summary>
        /// Initialize the WidgetHelper instance.
        /// </summary>
        public WidgetHelper() { }

        /// <summary>
        /// Initialize the WidgetHelper instance by specified WidgetInstance object.
        /// </summary>
        /// <param name="widget"></param>
        public WidgetHelper(WidgetInstance widget)
        {
            this.instance = widget;

        }

        ResourceManager resourceMan = null;

        internal ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var resourceDir = HttpContext.Current.Server.MapPath("~/Content/Widgets/") + Model.WidgetDescriptor.InstalledPath + "\\resources";
                    resourceMan = ResourceManager.CreateFileBasedResourceManager("language", resourceDir, null);
                }
                return resourceMan;
            }
        }

        /// <summary>
        /// Return the local resource value by specified key
        /// </summary>
        /// <param name="key">Specified the user preference key</param>
        /// <returns></returns>
        public string Loc(string key)
        {
            return this.ResourceManager.GetString(key, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        /// Gets the current widget model from database.
        /// </summary>
        public WidgetInstance Model { get { return instance; } internal set { instance = value; } }

        /// <summary>
        /// Return the integer value from UserPreferences by specified key.
        /// </summary>
        /// <param name="key">Specified the user preference key</param>
        /// <returns>The integer value</returns>
        public int GetInt(string key)
        {
            return GetData<int>(key);
        }

        /// <summary>
        /// Returns the boolean value from UserPreferences by specified key
        /// </summary>
        /// <param name="key">Specified the user preference key</param>
        /// <returns></returns>
        public bool GetBool(string key)
        {
            return GetData<bool>(key);
        }

        /// <summary>
        /// Returns the string value from UserPreferences by specified key
        /// </summary>
        /// <param name="key">Specified the user preference key</param>
        /// <returns></returns>
        public string GetString(string key)
        {
            return GetData<string>(key);
        }

        /// <summary>
        /// Identity wheather the specified user preference has value.
        /// </summary>
        /// <param name="key">Specified user preference name</param>
        /// <returns></returns>
        public bool IsNotEmpty(string key)
        {
            return !string.IsNullOrEmpty(GetString(key));
        }

        /// <summary>
        /// Returns the specified type value from UserPreferences by specified key
        /// </summary>
        /// <typeparam name="T">Specified the return value type</typeparam>
        /// <param name="key">Specified the user preference key</param>
        /// <returns></returns>
        public T GetData<T>(string key)
        {
            if (UserPreferences.ContainsKey(key))
            {
                if (UserPreferences[key] != null)
                    return (T)Convert.ChangeType(UserPreferences[key], typeof(T));
                else
                    return default(T);
                //return (T)UserPreferences[key];
            }

            return default(T);
        }

        /// <summary>
        /// Returns the decimal value forom UserPreferences by specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public decimal GetDecimal(string key)
        {
            if (UserPreferences.ContainsKey(key))
                return Convert.ToDecimal(UserPreferences[key]);
            else
                return 0;
        }

        /// <summary>
        /// Returns the user preferences as dynamic object
        /// </summary>
        /// <returns></returns>
        public dynamic GetUserPreferences()
        {
            dynamic obj = new ExpandoObject();

            foreach (var up in UserPreferences)
            {
                ((IDictionary<String, Object>)obj).Add(up.Key, up.Value != null ? up.Value : "");
            }
            return obj;
        }

        /// <summary>
        /// Get the user preferences
        /// </summary>
        public Dictionary<string, object> UserPreferences
        {
            get { return userPreferences; }
            internal set { userPreferences = value; }
        }

        /// <summary>
        /// Generate an element id with prefix by specified name
        /// </summary>
        /// <param name="name">The specified user preference name.</param>
        /// <returns></returns>
        public string GetID(string name)
        {
            return this.GenerateFieldID(name);
        }

        [Obsolete]
        public string GenerateFieldID(string fieldName)
        {
            return fieldName + "_" + IDPrefix;
        }

        /// <summary>
        /// Get the widget definition package path 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetWidgetPath(string path)
        {
            if (path.StartsWith("~") || path.StartsWith("/"))
                throw new Exception("The path could not be start with \"~\" or \"\"/");

            if (Model != null)
            {
                return UrlUtility.CreateUrlHelper().Content("~/content/widgets/" + Model.WidgetDescriptor.InstalledPath.Replace("\\", "/") + "/" + path);
            }

            return "";
        }

        /// <summary>
        /// Gets the widget client id
        /// </summary>
        public string ClientID
        {
            get
            {
                return "widget_" + Model.ID.ToString();
            }
        }

        /// <summary>
        /// Resolve the file url in widget path
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string ResovleUrl(string name)
        {
            return this.Model.WidgetDescriptor.ResolveUri(name, lang: System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToLower());
        }

        /// <summary>
        /// Render the label element for secpified user preference value
        /// </summary>
        /// <param name="name">Specified the user preference name.</param>
        /// <param name="label">Gets/Sets the specified label on the right of the checkbox</param>
        /// <param name="htmlAttributes">Gets/Sets the html attributes object for checkbox</param>
        /// <returns>The html string of the label element</returns>
        public MvcHtmlString Label(string name, string label = "", object htmlAttributes = null)
        {
            try
            {
                var element = new XElement("label", string.IsNullOrEmpty(label) ? name : label);
                element.Add(new XAttribute("for", GenerateFieldID(name)));
                AddAttributes(htmlAttributes, element);
                return new MvcHtmlString(element.OuterXml());
            }
            catch (Exception e)
            {
                return new MvcHtmlString(string.Format("<span class=\"d-state-error d-inline\">{0}</span>", e.Message));
            }
        }

        private static void AddAttributes(object htmlAttribute, XElement element)
        {
            if (htmlAttribute != null)
            {
                var attrs = htmlAttribute.ToDictionary();
                foreach (var key in attrs.Keys)
                {
                    element.Add(new XAttribute(key.Replace("_", "-"), attrs[key]));
                }
            }
        }

        /// <summary>
        /// Render the textbox element for secpified user preference value
        /// </summary>
        /// <param name="name">Specified the user preference name.</param>
        /// <param name="htmlAttributes">Gets/Sets the html attributes object for checkbox</param>
        /// <returns>The html string of the textbox element</returns>
        public MvcHtmlString TextBox(string name, object htmlAttributes = null)
        {
            try
            {
                var element = new XElement("input", new XAttribute("name", name), new XAttribute("type", "text"));
                AddAttributes(htmlAttributes, element);
                if (element.Attribute("id") == null)
                    element.Add(new XAttribute("id", GenerateFieldID(name)));

                var val = UserPreferences[name];
                element.Add(new XAttribute("value", val != null ? val : ""));
                return new MvcHtmlString(element.OuterXml());
            }
            catch (Exception e)
            {
                return new MvcHtmlString(string.Format("<span class=\"d-state-error d-inline\">{0}</span>", e.Message));
            }
        }

        /// <summary>
        /// Render the hidden element for secpified user preference value
        /// </summary>
        /// <param name="name">Gets/Sets the specified user preference name.</param>
        /// <param name="htmlAttributes">Gets/Sets the html attributes object for checkbox</param>
        /// <returns>The html string of the input hidden element</returns>
        public MvcHtmlString Hidden(string name, object htmlAttributes = null)
        {
            try
            {
                var element = new XElement("input", new XAttribute("name", name), new XAttribute("type", "hidden"));
                AddAttributes(htmlAttributes, element);
                if (element.Attribute("id") == null)
                    element.Add(new XAttribute("id", GenerateFieldID(name)));

                var val = UserPreferences[name];
                element.Add(new XAttribute("value", val != null ? val : ""));
                return new MvcHtmlString(element.OuterXml());
            }
            catch (Exception e)
            {
                return new MvcHtmlString(string.Format("<span class=\"d-state-error d-inline\">{0}</span>", e.Message));
            }
        }

        /// <summary>
        /// Render the textarea element for secpified user preference value
        /// </summary>
        /// <param name="name">Gets/Sets the specified user preference name.</param>
        /// <param name="htmlAttributes">Gets/Sets the html attributes object for checkbox</param>
        /// <returns>The html string of the text area element</returns>
        public MvcHtmlString TextArea(string name, object htmlAttributes = null)
        {
            try
            {
                var element = new XElement("textarea", new XAttribute("name", name));
                element.Add(new XAttribute("id", GenerateFieldID(name)));
                AddAttributes(htmlAttributes, element);
                var val = UserPreferences[name];
                element.Value = val != null ? val.ToString() : "";
                return new MvcHtmlString(element.OuterXml());
            }
            catch (Exception e)
            {
                return new MvcHtmlString(string.Format("<span class=\"d-state-error d-inline\">{0}</span>", e.Message));
            }
        }

        /// <summary>
        /// Render the checkbox element for secpified user preference value
        /// </summary>
        /// <param name="name">Gets/Sets the specified user preference name.</param>
        /// <param name="label">Gets/Sets the specified label on the right of the checkbox</param>
        /// <param name="htmlAttributes">Gets/Sets the html attributes for checkbox</param>
        /// <returns></returns>
        public MvcHtmlString Checkbox(string name, string label, object htmlAttributes = null)
        {
            try
            {
                var element = new XElement("input",
                    new XAttribute("name", name),
                    new XAttribute("type", "hidden"),
                    new XAttribute("data-role", "checkbox"),
                    new XAttribute("data-label", label));

                AddAttributes(htmlAttributes, element);
                if (element.Attribute("id") == null)
                    element.Add(new XAttribute("id", GenerateFieldID(name)));

                var val = GetBool(name);

                if (val)
                    element.Add(new XAttribute("checked", "checked"));

                element.Add(new XAttribute("value", val.ToString()));

                return new MvcHtmlString(element.OuterXml());
            }
            catch (Exception e)
            {
                return new MvcHtmlString(string.Format("<span class=\"d-state-error d-inline\">{0}</span>", e.Message));
            }
        }

        /// <summary>
        /// Gets the widget descriptor icon url
        /// </summary>
        /// <returns></returns>
        public string IconUrl
        {
            get
            {
                var durl = this.Model.WidgetDescriptor.IconUrl;
                if (string.IsNullOrEmpty(durl))
                    return "~/content/images/app.gif";
                return durl;
            }
        }

        /// <summary>
        /// Gets open user preferences dialog
        /// </summary>
        public string OpenPrefs
        {
            get
            {
                return "$('#" + this.ClientID + "').widget('openPrefs')";
            }
        }

        /// <summary>
        /// Render a icon and text placeholder.
        /// </summary>
        /// <param name="title">Specified the title text.</param>
        /// <param name="onclick">The javascript that trigger on the holder click</param>
        /// <returns></returns>
        public MvcHtmlString DesignModeHolder(string title, string onclick = "")
        {
            var element = new XElement("div",
                new XAttribute("style", "line-height: 50px;text-align:center;cursor:pointer;"),
                new XAttribute("onclick", "event.preventDefault();event.stopPropagation();" + (string.IsNullOrEmpty(onclick) ? OpenPrefs : onclick)),
                new XElement("img", new XAttribute("src", DNA.Utility.UrlUtility.CreateUrlHelper().Content(IconUrl))),
                new XElement("div", title)
                );
            return MvcHtmlString.Create(element.OuterXml());
        }

    }
}