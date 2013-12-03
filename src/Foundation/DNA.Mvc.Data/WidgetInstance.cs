//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace DNA.Web
{
    /// <summary>
    /// Represents widget instance model.
    /// </summary>
    [Serializable]
    public class WidgetInstance : ICloneable, DNA.Web.IWidget
    {
        public int ID { get; set; }

        /// <summary>
        /// The Id refer to the published widget
        /// </summary>
        /// <remarks>
        /// When this id set that means this widget is a shodowcopy of the refer widget
        /// </remarks>
        public int RefID { get; set; }

        /// <summary>
        /// Gets / Sets the widget's change state .Only use when the page enable versioning 
        /// </summary>
        public int TrackState { get; set; }

        /// <summary>
        /// Gets/Sets the widget descriptor id.
        /// </summary>
        public int DescriptorID { get; set; }

        /// <summary>
        /// Gets/Sets the owner page id.
        /// </summary>
        public int PageID { get; set; }

        /// <summary>
        /// Gets/Sets the widget body css class name.
        /// </summary>
        public string BodyClass { get; set; }

        /// <summary>
        /// Gets/Sets the widget body css text.
        /// </summary>
        public string BodyCssText { get; set; }

        /// <summary>
        /// Gets/Sets the widget css text.
        /// </summary>
        public string CssText { get; set; }

        /// <summary>
        /// Gets/Sets the user preferences raw data.
        /// </summary>
        [ScriptIgnore]
        public virtual string Data { get; set; }

        /// <summary>
        /// Gets/Sets the widget header css class name.
        /// </summary>
        public string HeaderClass { get; set; }

        /// <summary>
        /// Gets/Sets the widget header css style text.
        /// </summary>
        public string HeaderCssText { get; set; }

        /// <summary>
        /// Gets/Sets icon url where on the left of the widget header.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets/Sets whether the widget body is expanded.
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Gets/Sets the widget instance create by sytem.
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Gets/Sets the position index of the owner zone.
        /// </summary>
        public int Pos { get; set; }

        /// <summary>
        /// Gets/Sets whether show the widget header.
        /// </summary>
        public bool ShowHeader { get; set; }

        /// <summary>
        /// Gets/Sets whether show the widget border.
        /// </summary>
        public bool ShowBorder { get; set; }

        /// <summary>
        /// Gets/Sets the widget has no background
        /// </summary>
        public bool Transparent { get; set; }

        /// <summary>
        /// Gets/Sets whether the widget can be cached.
        /// </summary>
        public bool Cached { get; set; }

        /// <summary>
        /// Gets/Sets the title text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the title link url.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets/Sets the owner zone client id.
        /// </summary>
        public string ZoneID { get; set; }

        /// <summary>
        /// Gets/Sets the widget locale name.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets/Sets the current view mode
        /// </summary>
        public string ViewMode { get; set; }

        /// <summary>
        /// Get/Sets the widget show model.
        /// </summary>
        public int ShowMode { get; set; }

        string DNA.Web.IWidget.ShowMode
        {
            get
            {
                if (this.ShowMode == 1)
                    return "all";
                if (this.ShowMode == 2)
                    return "descendant";
                return "parent";
            }
        }

        /// <summary>
        /// Get/Sets the owner page instance.
        /// </summary>
        [ScriptIgnore, XmlIgnore]
        public virtual WebPage WebPage { get; set; }

        /// <summary>
        /// Gets/Sets the widget descriptor instance.
        /// </summary>
        [ScriptIgnore, XmlIgnore]
        public virtual WidgetDescriptor WidgetDescriptor { get; set; }

        /// <summary>
        /// Gets/Sets the access roles
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }

        /// <summary>
        /// Save the user preferences data to Data property.
        /// </summary>
        /// <param name="data">The data to be saved.</param>
        public void SaveUserPreferences(List<IDictionary<string, object>> data)
        {
            if (data != null)
            {
                if (data.Count > 0)
                {
                    var serializer = new JavaScriptSerializer();
                    Data = serializer.Serialize(data);
                }
            }
            else
                Data = "";// null;
        }

        /// <summary>
        /// Update user preferences
        /// </summary>
        /// <param name="data"></param>
        public void SaveUserPreferences(List<Dictionary<string, object>> data)
        {
            if (data != null)
                SaveUserPreferences(data.Select(d => (IDictionary<string, object>)d).ToList());
            else
                Data = "";
        }

        /// <summary>
        /// Read the user preferences data into the Dictionary object
        /// </summary>
        /// <param name="widget">The widget object</param>
        /// <returns></returns>
        public List<IDictionary<string, object>> ReadUserPreferences()
        {
            var properties = new List<IDictionary<string, object>>();

            if (!string.IsNullOrEmpty(Data))
            {
                //System.Runtime.Serialization.Json
                //Add v3
                var serializer = new JavaScriptSerializer();
                properties = serializer.Deserialize<List<IDictionary<string, object>>>(Data);
            }
            return properties;
        }

        /// <summary>
        /// Popuplate widget properties by specified widget data.
        /// </summary>
        /// <param name="widgetData"></param>
        public void Popuple(IWidget widgetData)
        {
            Title = widgetData.Title;
            Link = widgetData.Link;
            IconUrl = widgetData.IconUrl;
            ShowHeader = widgetData.ShowHeader;
            HeaderCssText = widgetData.HeaderCssText;
            HeaderClass = widgetData.HeaderClass;
            CssText = widgetData.CssText;
            BodyCssText = widgetData.BodyCssText;
            BodyClass = widgetData.BodyClass;
            IsStatic = widgetData.IsStatic;
            if (!string.IsNullOrEmpty(widgetData.Data))
            {
                //Replace the default data
                if (string.IsNullOrEmpty(Data))
                    Data = widgetData.Data;
                else
                {
                    var serializer = new JavaScriptSerializer();
                    var props = serializer.Deserialize<List<Dictionary<string, object>>>(widgetData.Data);
                    var defProps = this.ReadUserPreferences();
                    foreach (var pro in props)
                    {
                        var defPro = defProps.Find(d => d["name"].ToString().Equals(pro["name"].ToString()));
                        if (defPro != null)
                            defPro["value"] = pro["value"];
                        else
                            defProps.Add(pro);
                    }
                    SaveUserPreferences(defProps);
                }
            }

            if (widgetData.ShowMode == "all")
                ShowMode = 1;
            if (widgetData.ShowMode == "descendant")
                ShowMode = 2;

        }

        /// <summary>
        /// Clone a new instance by current widget instance.
        /// </summary>
        /// <param name="culture">The locale name.</param>
        /// <returns>A new widget instance returns.</returns>
        public WidgetInstance Clone(string culture = "")
        {
            var copy = new WidgetInstance();
            this.CopyTo(copy, "ID", "RefID", "TrackState", "WebPage", "WidgetDescriptor", "Roles");

            if (!string.IsNullOrEmpty(culture))
                copy.Locale = culture;
            return copy;
        }

        /// <summary>
        /// Gets/Sets the widget id.
        /// </summary>
        public string UID
        {
            get
            {
                if (WidgetDescriptor != null)
                    return WidgetDescriptor.InstalledPath;
                return "";
            }
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        //public void Load(XElement element, string locale)
        //{
        //    throw new NotImplementedException();
        //}

        //public XElement Element()
        //{
        //    throw new NotImplementedException();
        //}
    }

    //public class WidgetShadowcopy : WidgetInstance { }
}
