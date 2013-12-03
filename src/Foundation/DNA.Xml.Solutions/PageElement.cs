//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    [XmlRoot("page", Namespace = "http://www.dotnetage.com/XML/Schema/page"),
    Serializable]
    public class PageElement : IWebPage, ICloneable
    {
        [XmlAttribute("id")]
        public string Name;

        [XmlElement("title")]
        public LocalizableElement Title;

        [XmlElement("description")]
        public LocalizableElement Description;

        [XmlElement("keywords")]
        public LocalizableElement Keywords;

        [XmlElement("link")]
        public string LinkUrl;

        [XmlAttribute("target")]
        public string Target = "_self";

        //[XmlElement("data")]
        //public string Data;

        //[XmlElement("layout")]
        //public string Layout;
        [XmlElement("layout")]
        public LayoutElement Layout;

        [XmlElement("icon")]
        public ImageElement Icon;

        [XmlElement("image")]
        public ImageElement Image;

        [XmlAttribute("anonymous")]
        public bool AllowAnonymous = true;

        [XmlAttribute("showInMenu")]
        public bool ShowInMenu = true;

        [XmlAttribute("showInSitemap")]
        public bool ShowInSitemap = true;

        [XmlAttribute("static")]
        public bool IsStatic;

        [XmlAttribute("shared")]
        public bool IsShared;

        [XmlAttribute("slug")]
        public string Slug;

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Locale = "en-us";

        [XmlAttribute("dir")]
        public string Direction = "ltr";

        [XmlAttribute("viewmode")]
        public string ViewMode = "center";

        [XmlElement("style")]
        public List<ResElement> StyleSheets;

        [XmlElement("script")]
        public List<ScriptElement> Scripts;

        [XmlArray("pages"), XmlArrayItem("page", Type = typeof(PageElement),
            Namespace = "http://www.dotnetage.com/XML/Schema/page")]
        public List<PageElement> Children;

        [XmlArray("widgets"), XmlArrayItem("widget", Type = typeof(WidgetDataElement),
            Namespace = "http://www.dotnetage.com/XML/Schema/widget-data")]
        public List<WidgetDataElement> Widgets;

        #region Implement IWebPage

        string IWebPage.Description
        {
            get
            {
                if (this.Description != null)
                    return this.Description.Text;
                return "";
            }
        }

        string IWebPage.Locale
        {
            get
            {
                return this.Locale;
            }
        }

        string IWebPage.Dir
        {
            get
            {
                return this.Direction;
            }
        }

        string IWebPage.IconUrl
        {
            get
            {
                if (this.Icon != null)
                    return this.Icon.Source;
                return "";
            }
        }

        string IWebPage.ImageUrl
        {
            get
            {
                if (this.Image != null)
                    return this.Image.Source;
                return "";
            }
        }

        bool IWebPage.IsShared
        {
            get { return this.IsShared; }
        }

        bool IWebPage.IsStatic
        {
            get { return this.IsStatic; }
        }

        string IWebPage.Keywords
        {
            get
            {
                if (this.Keywords != null)
                    return this.Keywords.Text;
                return "";
            }
        }

        string IWebPage.LinkTo
        {
            get { return this.LinkUrl; }
        }

        bool IWebPage.NoFollow
        {
            get { return false; }
        }

        string IWebPage.Slug
        {
            get { return this.Slug; }
        }

        int IWebPage.ViewMode
        {
            get
            {
                if (ViewMode == "Full")
                    return 1;
                if (ViewMode == "Left")
                    return 2;
                if (ViewMode == "Right")
                    return 3;
                return 0;
            }
        }

        int IWebPage.Pos
        {
            get { return 0; }
        }

        bool IWebPage.ShowInMenu
        {
            get { return this.ShowInMenu; }
        }

        bool IWebPage.ShowInSitemap
        {
            get { return this.ShowInSitemap; }
        }

        bool IWebPage.AllowAnonymous
        {
            get { return this.AllowAnonymous; }
        }

        string IWebPage.Target
        {
            get { return this.Target; }
        }

        string IWebPage.Title
        {
            get
            {
                if (this.Title != null)
                    return this.Title.Text;
                return "";
            }
        }

        string IWebPage.ViewData
        {
            get {
                if (this.Layout != null)
                    return this.Layout.Text;
                return "";
            }
        }

        string IWebPage.ViewName
        {
            get 
            {
                if (this.Layout != null)
                    return this.Layout.Name;
                return "";
            }
        }

        IEnumerable<IWidget> IWebPage.Widgets
        {
            get { return this.Widgets; }
        }

        IEnumerable<IWebPage> IWebPage.Children
        {
            get { return this.Children; }
        }

        string IWebPage.StartupScripts
        {
            get
            {
                var scripts = "";
                if (this.Scripts != null && this.Scripts.Count > 0)
                {
                    foreach (var s in this.Scripts)
                    {
                        if (!string.IsNullOrEmpty(s.Text))
                            scripts += string.Format("<script type=\"{0}\">{1}</script>", string.IsNullOrEmpty(s.Type) ? "text/javascript" : s.Type, s.Text);
                    }
                }
                return scripts;
            }
        }

        string IWebPage.Scripts
        {
            get
            {
                if (this.Scripts != null && this.Scripts.Count > 0)
                {
                    var scriptArgs = new List<string>();
                    foreach (var s in this.Scripts)
                    {
                        if (string.IsNullOrEmpty(s.Text) && !string.IsNullOrEmpty(s.Source))
                            scriptArgs.Add(s.Source);
                    }
                    if (scriptArgs.Count > 0)
                        return string.Join(",", scriptArgs);
                }
                return "";
            }
        }

        string IWebPage.CssText
        {
            get
            {
                var css = "";
                if (this.StyleSheets != null && this.StyleSheets.Count > 0)
                {
                    foreach (var s in this.StyleSheets)
                    {
                        if (!string.IsNullOrEmpty(s.Text))
                            css += s.Text;
                    }
                }
                return css;
            }
        }

        string IWebPage.StyleSheets
        {
            get
            {
                if (this.StyleSheets != null && this.StyleSheets.Count > 0)
                {
                    var styleArgs = new List<string>();
                    foreach (var s in this.StyleSheets)
                    {
                        if (string.IsNullOrEmpty(s.Text) && !string.IsNullOrEmpty(s.Source))
                            styleArgs.Add(s.Source);
                    }
                    if (styleArgs.Count > 0)
                        return string.Join(",", styleArgs);
                }
                return "";
            }
        }

        #endregion

        public PageElement Clone()
        {
            var copy = new PageElement()
            {
                AllowAnonymous = this.AllowAnonymous,
                //Data = this.Data,
                Title = this.Title != null ? this.Title.Clone() : null,
                Description = this.Description != null ? this.Description.Clone() : null,
                Keywords = this.Keywords != null ? this.Keywords.Clone() : null,
                IsShared = this.IsShared,
                IsStatic = this.IsStatic,
                ShowInMenu = this.ShowInMenu,
                ShowInSitemap = this.ShowInSitemap,
                Layout = this.Layout!=null ? this.Layout.Clone() : null,
                Slug = this.Slug,
                LinkUrl = this.LinkUrl,
                Locale = this.Locale,
                Direction = this.Direction,
                Icon = this.Icon != null ? this.Icon.Clone() : null,
                Image = this.Image != null ? this.Image.Clone() : null,
                Target = this.Target,
                ViewMode = this.ViewMode
            };

            if (this.StyleSheets != null)
            {
                copy.StyleSheets = new List<ResElement>();
                foreach (var s in this.StyleSheets)
                    copy.StyleSheets.Add(s.Clone());
            }

            if (this.Scripts != null)
            {
                copy.Scripts = new List<ScriptElement>();
                foreach (var s in this.Scripts)
                    copy.Scripts.Add(s.Clone());
            }

            if (this.Widgets != null)
            {
                copy.Widgets = new List<WidgetDataElement>();
                foreach (var w in this.Widgets)
                    copy.Widgets.Add(w.Clone());
            }

            if (this.Children != null)
            {
                copy.Children = new List<PageElement>();
                foreach (var p in copy.Children)
                    copy.Children.Add(p.Clone());
            }

            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}