//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Represents a web page.
    /// </summary>
    [Serializable]
    public class WebPage : DNA.Web.IWebPage
    {
        /// <summary>
        /// Initializes a new instance of WebPage class.
        /// </summary>
        public WebPage()
        {
            this.Created = DateTime.UtcNow;
            this.LastModified = DateTime.UtcNow;
        }

        #region Properties

        /// <summary>
        /// Gets/Sets ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the web id.
        /// </summary>
        public int WebID { get; set; }

        /// <summary>
        /// Gets/Sets the title text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the custom data.
        /// </summary>
        public string ViewData { get; set; }

        /// <summary>
        /// Gets/Sets the page template view name.
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Indicates whether allow anonymous access.
        /// </summary>
        public bool AllowAnonymous { get; set; }

        [Obsolete]
        public bool EnableVersioning { get; set; }

        /// <summary>
        /// Gets/Sets the creation date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/Sets the description text up to 255 charaters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets the icon url.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets/Sets the image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets/Sets the access privacy.
        /// </summary>
        public virtual int Privacy { get; set; }

        /// <summary>
        /// Indicates whether the page is shared.
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// Gets/Sets whether is static page.
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Indicates whether output the nofollow meta tag.
        /// </summary>
        public bool NoFollow { get; set; }

        /// <summary>
        /// Gets/Sets the additonal data.
        /// </summary>
        public string AdditionalData { get; set; }

        /// <summary>
        /// Gets the master page where the page inhert layouts and styles from 
        /// </summary>
        public int MasterID { get; set; }

        /// <summary>
        /// Gets/Sets the page keywords
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Gets/Sets last modified date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/Sets alternative url.
        /// </summary>
        public string LinkTo { get; set; }

        /// <summary>
        /// Gets/Sets owner user name.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets/Sets the parent page id.
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// Gets/Sets the friend page name.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets/Sets the text direction.
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// Gets/Sets the route name.
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// Gets/Sets the position of siblings.
        /// </summary>
        public int Pos { get; set; }

        /// <summary>
        /// Indicates whether the web page show in main menu.
        /// </summary>
        public bool ShowInMenu { get; set; }

        /// <summary>
        /// Indicates whether the web page show in sitemap and sitemap path.
        /// </summary>
        public bool ShowInSitemap { get; set; }

        /// <summary>
        /// Gets / Sets the open target.
        /// </summary>
        public string Target { get; set; }

        [Obsolete]
        public string UnsaveContent { get; set; }

        /// <summary>
        /// Gets/Sets the locale name.
        /// </summary>
        public string Locale { get; set; }

        [Obsolete]
        public int Version { get; set; }

        /// <summary>
        /// Gets/Sets the in-page css style text
        /// </summary>
        public string CssText { get; set; }

        /// <summary>
        /// Gets/Sets the startup scripts render in the end of the page
        /// </summary>
        public string StartupScripts { get; set; }

        /// <summary>
        /// Gets/Sets the custom in page css text.
        /// </summary>
        public string StyleSheets { get; set; }

        /// <summary>
        /// Gets/Sets the custom user scripts.
        /// </summary>
        public string Scripts { get; set; }

        /// <summary>
        /// Gets/Sets the page.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets/Sets the page
        /// </summary>
        public virtual Web Web { get; set; }

        /// <summary>
        /// Gets/Sets the page view mode.
        /// </summary>
        public virtual int ViewMode { get; set; }

        /// <summary>
        /// Gets access role collection.
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }

        /// <summary>
        /// Gets widgets
        /// </summary>
        public virtual ICollection<WidgetInstance> Widgets { get; set; }

        #endregion

        /// <summary>
        /// Popuplate properties from IWebPage data object.
        /// </summary>
        /// <param name="pageData"></param>
        public virtual void Populate(IWebPage pageData)
        {
            this.Title = pageData.Title;
            this.Description = pageData.Description;
            this.Keywords = pageData.Keywords;
            this.ImageUrl = pageData.ImageUrl;
            this.IconUrl = pageData.IconUrl;
            this.LinkTo = pageData.LinkTo;
            this.Target = pageData.Target;
            this.ViewData = pageData.ViewData;
            this.ViewName = pageData.ViewName;
            this.AllowAnonymous = pageData.AllowAnonymous;
            this.ShowInMenu = pageData.ShowInMenu;
            this.ShowInSitemap = pageData.ShowInSitemap;
            this.IsStatic = pageData.IsStatic;
            this.IsShared = pageData.IsShared;
            this.Locale = pageData.Locale;
            this.Slug = pageData.Slug;
            this.ViewMode = pageData.ViewMode;
            this.CssText = pageData.CssText;
            this.Scripts = pageData.Scripts;
            this.StartupScripts = pageData.StartupScripts;
            this.StyleSheets = pageData.StyleSheets;
            this.Dir = string.IsNullOrEmpty(pageData.Dir) ? "ltr" : pageData.Dir;

            if (!string.IsNullOrEmpty(this.ViewName) && this.ViewName.StartsWith("Layout", StringComparison.OrdinalIgnoreCase))
            {
                var formattedLayout = "~/views/dynamicUI/layouts/" + this.ViewName;
                if (!formattedLayout.EndsWith("cshtml", StringComparison.OrdinalIgnoreCase))
                    formattedLayout += ".cshtml";
                this.ViewName = formattedLayout;
            }
        }

        IEnumerable<IWidget> IWebPage.Widgets
        {
            get
            {
                return this.Widgets;
            }
        }

        IEnumerable<IWebPage> IWebPage.Children
        {
            get { return null; }
        }

        #region page schema
        protected const string TITLE = "title";
        protected const string DESC = "description";
        protected const string KEY_WORDS = "keywords";
        protected const string LANG = "lang";
        protected const string LINK = "link";
        protected const string ICON = "icon";
        protected const string IMAGE = "image";
        protected const string SRC = "src";
        protected const string LAYOUT = "layout";
        protected const string NAME = "name";
        protected const string STYLE = "style";
        protected const string SCRIPT = "script";
        protected const string TYPE = "type";
        protected const string PAGES = "pages";
        protected const string PAGE = "page";
        protected const string WIDGETS = "widgets";
        protected const string WIDGET = "widget";
        protected const string TARGET = "target";
        protected const string DIR = "dir";
        protected const string ANONYMOUS = "anonymous";
        protected const string SHOW_IN_MENU = "showInMenu";
        protected const string SHOW_IN_SITEMAP = "showInSitemap";
        protected const string STATIC = "static";
        protected const string SHARED = "shared";
        protected const string SLUG = "slug";
        protected const string ROLES = "roles";
        protected const string VIEWMODE = "viewmode";
        #endregion

        //public void Load(XElement element, string locale)
        //{
        //    var ns = element.GetDefaultNamespace();
        //    var defaultLocale = element.StrAttr(LANG);
        //    var targetLocale = locale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase) ? "" : locale;

        //    var title = element.ElementWithLocale(ns + TITLE, targetLocale);
        //    var desc = element.ElementWithLocale(ns + DESC, targetLocale);
        //    var keywords = element.ElementWithLocale(ns + KEY_WORDS, targetLocale);
        //    var icon = element.ElementWithLocale(ns + ICON, targetLocale);
        //    var img = element.ElementWithLocale(ns + IMAGE, targetLocale);
        //    var styles = element.Elements(ns + STYLE);
        //    var scripts = element.Elements(ns + SCRIPT);
        //    var layout = element.Element(ns + LAYOUT);

        //    this.Locale = targetLocale;
        //    this.LinkTo = element.StrAttr(LINK);
        //    this.Target = element.StrAttr(TARGET);
        //    this.AllowAnonymous = element.BoolAttr(ANONYMOUS);
        //    this.ShowInMenu = element.BoolAttr(SHOW_IN_MENU);
        //    this.ShowInSitemap = element.BoolAttr(SHOW_IN_SITEMAP);
        //    this.IsStatic = element.BoolAttr(STATIC);
        //    this.IsShared = element.BoolAttr(SHARED);
        //    this.Slug = element.StrAttr(SLUG);
        //    this.ViewMode = (int)((PageViewModes)(Enum.Parse(typeof(PageViewModes), element.StrAttr(VIEWMODE))));
        //    this.Dir = string.IsNullOrEmpty(element.StrAttr(DIR)) ? "ltr" : element.StrAttr(DIR);
        //    var pagesEl = element.Element("pages");

        //    //this.Roles
        //    if (title != null)
        //        this.Title = title.Value;

        //    if (desc != null)
        //        this.Description = desc.Value;

        //    if (keywords != null)
        //        this.Keywords = keywords.Value;

        //    if (icon != null)
        //        this.IconUrl = icon.StrAttr(SRC);

        //    if (img != null)
        //        this.ImageUrl = icon.StrAttr(SRC);

        //    if (layout != null)
        //    {
        //        this.ViewData = string.IsNullOrEmpty(layout.Value) ? "" : layout.Value;
        //        this.ViewName = layout.StrAttr(NAME);
        //    }

        //    if (scripts != null)
        //    {
        //        var scriptArgs = new List<string>();
        //        foreach (var s in scripts)
        //        {
        //            if (string.IsNullOrEmpty(s.Value) && !string.IsNullOrEmpty(s.StrAttr(SRC)))
        //                scriptArgs.Add(s.StrAttr(SRC));

        //            if (!string.IsNullOrEmpty(s.Value))
        //                this.StartupScripts += string.Format("<script type=\"{0}\">{1}</script>", string.IsNullOrEmpty(s.StrAttr(TYPE)) ? "text/javascript" : s.StrAttr(TYPE), s.Value);
        //        }

        //        if (scriptArgs.Count > 0)
        //            this.Scripts = string.Join(",", scriptArgs);
        //    }

        //    if (styles != null)
        //    {
        //        foreach (var style in styles)
        //        {
        //            if (!string.IsNullOrEmpty(style.Value))
        //                this.CssText += style.Value;
        //        }
        //    }

        //    if (pagesEl != null)
        //    {
        //        var pages = pagesEl.Elements("page");
                
        //        foreach (var p in pages)
        //        {
                    
        //        }
        //    }
        //}
    }
}
