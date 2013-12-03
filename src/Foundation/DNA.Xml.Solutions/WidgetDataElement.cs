//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    [Serializable, XmlRoot("widget", Namespace = "http://www.dotnetage.com/XML/Schema/widget-data")]
    public class WidgetDataElement : IWidget, ICloneable
    {
        [XmlAttribute("pid")]
        public string WidgetID = "";

        [XmlAttribute("seq")]
        public int Sequence = 0;

        [XmlAttribute("zone")]
        public string Zone = "zone0";

        [XmlElement("title")]
        public LocalizableElement Title;

        [XmlElement("link")]
        public LinkElement Link;

        [XmlAttribute("icon")]
        public string IconUrl;

        [XmlAttribute("showIn")]
        public string ShowMode;

        [XmlAttribute("viewmode")]
        public string ViewMode;

        [XmlAttribute("roles")]
        public string Roles;

        [XmlAttribute("static")]
        public bool IsStatic;

        [XmlElement("style")]
        public WidgetStyleElement Style;

        [XmlElement("preference")]
        public List<PreferenceElement> Preferences;

        #region Implement IWidget

        string IWidget.UID
        {
            get { return this.WidgetID; }
        }

        string IWidget.CssText
        {
            get
            {
                if (this.Style != null && this.Style.Box != null)
                    return this.Style.Box.Text;
                return "";
            }
        }

        string IWidget.BodyClass
        {
            get
            {
                if (this.Style != null && this.Style.Body != null)
                    return this.Style.Body.CssClass;
                return "";
            }
        }

        string IWidget.BodyCssText
        {
            get
            {
                if (this.Style != null && this.Style.Body != null)
                    return this.Style.Body.Text;
                return "";
            }
        }

        string IWidget.HeaderClass
        {
            get
            {
                if (this.Style != null && this.Style.Header != null)
                    return this.Style.Header.CssClass;
                return "";
            }
        }

        string IWidget.HeaderCssText
        {
            get
            {
                if (this.Style != null && this.Style.Header != null)
                    return this.Style.Header.Text;
                return "";
            }
        }

        bool IWidget.ShowHeader
        {
            get
            {
                if (this.Style != null && this.Style.Header != null)
                    return !this.Style.Header.Hidden;
                return true;
            }
        }

        string IWidget.Title
        {
            get
            {
                if (this.Title != null)
                    return this.Title.Text;
                return "";
            }
        }

        string IWidget.IconUrl
        {
            get { return this.IconUrl; }
        }

        string IWidget.Link
        {
            get
            {
                if (this.Link != null)
                    return this.Link.Source;
                return "";
            }
        }

        string IWidget.Data
        {
            get
            {
                if (Preferences != null && Preferences.Count > 0)
                {
                    var objs = new List<dynamic>();
                    //var formattedValue=pref.Value.Replace("'", "\"");

                    foreach (var pref in Preferences)
                    {
                        object val = null;
                        if (!string.IsNullOrEmpty(pref.Value))
                        {
                            if (pref.Value.Equals("true", StringComparison.OrdinalIgnoreCase) || pref.Value.Equals("false", StringComparison.OrdinalIgnoreCase))
                            {
                                //boolean
                                val = pref.Value.Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
                            }
                            else
                            {
                                int intVal = 0;

                                if (Int32.TryParse(pref.Value, out intVal))
                                    val = intVal; //integer
                                else
                                {
                                    decimal dVal = 0;
                                    if (Decimal.TryParse(pref.Value, out dVal))
                                    {
                                        val = dVal; //decimal
                                    }
                                    else
                                    {
                                        //The string value must be remove the ''
                                        if (pref.Value.StartsWith("'") && pref.Value.EndsWith("'"))
                                            val = pref.Value.Replace("'", "");
                                        else
                                            val = pref.Value; //String or else

                                    }
                                }

                            }
                        }

                        var obj = new
                        {
                            name = pref.Name,
                            @readonly = false,
                            value = val
                        };

                        objs.Add(obj);
                    }
                    var result = JsonConvert.SerializeObject(objs.ToArray());
                    return result;
                }
                return "";
            }
        }

        int IWidget.Pos
        {
            get { return this.Sequence; }
        }

        string IWidget.ZoneID
        {
            get { return this.Zone; }
        }

        string IWidget.ViewMode
        {
            get { return this.ViewMode; }
        }

        string IWidget.ShowMode
        {
            get { return this.ShowMode; }
        }

        bool IWidget.IsStatic
        {
            get { return this.IsStatic; }
        }
        #endregion

        public WidgetDataElement Clone()
        {
            var copy = new WidgetDataElement()
            {
                WidgetID = this.WidgetID,
                Sequence = this.Sequence,
                Zone = this.Zone,
                ShowMode = this.ShowMode,
                Roles = this.Roles,
                Title = this.Title != null ? this.Title.Clone() : null,
                Link = this.Link != null ? this.Link.Clone() : null,
                Style = this.Style != null ? this.Style.Clone() : null
            };

            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
