//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using DNA.Data.Documents;
using DNA.Utility;
using DNA.Web.ServiceModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to Web object model. 
    /// </summary>
    public class WebDecorator : Web
    {
        private WebPageCollection webPages = null;
        private WidgetInstanceCollection widgets = null;
        private ContentListCollection contents = null;
        private CategoryCollection categories = null;
        private IUnitOfWorks blobs;
        private IQueues queues;

        /// <summary>
        /// Gets/Sets the web model object.
        /// </summary>
        public Web Model { get; private set; }

        private IDataContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the WebDecorator class for internal testing.
        /// </summary>
        public WebDecorator() { }

        /// <summary>
        /// Initializes a new instance of the WebDecorator class using given web and data context object.
        /// </summary>
        /// <param name="web">The web model.</param>
        /// <param name="context">The data context.</param>
        public WebDecorator(Web web, IDataContext context)
        {
            Model = web;
            Context = context;
            web.CopyTo(this, new string[] { "Pages", "Lists", "Categories" });
        }

        /// <summary>
        /// Get web site default url by specified locale.
        /// </summary>
        /// <remarks>
        /// If the specified locale not found in current web will return the url for default locale.
        /// </remarks>
        /// <param name="locale">The culture name in the format "{languagecode2}-{country/regioncode2}".</param>
        /// <returns>A string that contains the url</returns>
        public string GetDefaultUrl(string locale)
        {
            CultureInfo c;
            if (locale.Equals(this.Culture) || !this.InstalledLocales.Contains(locale.ToLower()))
            {
                if (DefaultPage != null)
                    return DefaultPage.FullUrl;
                else
                    return "";
            }
            else
            {
                var defPage = Context.WebPages.Find(p => p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase) && p.Slug.Equals("default"));

                if (defPage == null)
                    defPage = Context.WebPages.Filter(p => p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase))
                                                                        .OrderBy(p => p.Pos)
                                                                        .FirstOrDefault();

                if (defPage == null) //There is no page in the website //return string.Format("~/{0}/{1}/default.html", this.Name, locale);
                    return "";
                else
                    return (new WebPageDecorator(defPage, Context)).FullUrl;
            }
        }

        [Obsolete]
        public string Url
        {
            get
            {
                if (!string.IsNullOrEmpty(DefaultUrl))
                    return DefaultUrl;

                return string.Format("~/{0}/{1}/default.html", this.Name, this.Culture);
            }
        }

        private WebPageDecorator defaultPage = null;

        /// <summary>
        /// Get default page by default locale
        /// </summary>
        public WebPageDecorator DefaultPage
        {
            get
            {
                if (defaultPage == null)
                {
                    defaultPage = this.Pages.FirstOrDefault(p => p.Slug.Equals("default") && p.Locale.Equals(this.Culture, StringComparison.OrdinalIgnoreCase));

                    if (defaultPage == null)
                        defaultPage = this.Pages.OrderBy(p => p.Pos).FirstOrDefault(p => p.Locale.Equals(this.Culture, StringComparison.OrdinalIgnoreCase));

                    if (!object.ReferenceEquals(defaultPage, null))
                        defaultPage = new WebPageDecorator(defaultPage, this.Context);
                }

                return defaultPage;
            }
        }

        /// <summary>
        /// Gets web pages of website.
        /// </summary>
        public new WebPageCollection Pages
        {
            get
            {
                if (webPages == null)
                    webPages = new WebPageCollection(Context, Model);
                return webPages;
            }
        }

        /// <summary>
        /// Gets widgets of website
        /// </summary>
        public WidgetInstanceCollection Widgets
        {
            get
            {
                if (widgets == null)
                    widgets = new WidgetInstanceCollection(Context, Model);
                return widgets;
            }
        }

        /// <summary>
        /// Gets content lists of the website
        /// </summary>
        public new ContentListCollection Lists
        {
            get
            {
                if (contents == null)
                    contents = new ContentListCollection(Context, Model);
                return contents;
            }
        }

        /// <summary>
        /// Gets categories os the website.
        /// </summary>
        public CategoryCollection Categories
        {
            get
            {
                if (categories == null)
                    categories = new CategoryCollection(Context, this);
                return categories;
            }
        }

        /// <summary>
        /// Create a new web page to current website
        /// </summary>
        /// <param name="title">The page title</param>
        /// <param name="desc">The page description</param>
        /// <param name="keywords">The page keywords</param>
        /// <returns>A web page decorator object that wrapps the web page model.</returns>
        public WebPageDecorator CreatePage(string title, string desc = "", string keywords = "")
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("title");

            var userName = "";
            var ctx = HttpContext.Current;
            if (ctx != null && ctx.User.Identity != null)
            {
                //if (!ctx.Request.IsAuthenticated)
                //  throw new AccessDenyException();
                userName = ctx.User.Identity.Name;
            }
            else
            {
                //Only for test
                userName = this.Owner;
            }

            var page = new WebPage()
            {
                ParentID = 0,
                WebID = Id,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                Owner = userName,
                AllowAnonymous = true,
                Title = title,
                Description = desc,
                Keywords = keywords,
                ShowInMenu = true,
                ShowInSitemap = true,
                ViewName = ""
            };

            if (string.IsNullOrEmpty(page.Locale))
                page.Locale = this.Culture;

            var slugArgs = Context.WebPages.Filter(p => p.WebID == this.Id && p.Locale == page.Locale).Select(s => s.Slug).ToArray();

            var slug = Utility.TextUtility.Slug(title);
            var counter = 0;
            page.Slug = slug;

            while (slugArgs.Contains(page.Slug))
                page.Slug = slug + (++counter).ToString();

            //page.Path = WebPageDecorator.GeneratePermalink(page, this.Name, Context.WebPages);

            Context.Add(page);
            Context.SaveChanges();
            page.Path = "0/" + page.ID;
            Context.SaveChanges();
            return new WebPageDecorator(page, Context);
        }

        /// <summary>
        /// Create a new web page for content view
        /// </summary>
        /// <param name="view">The content view </param>
        /// <param name="parentId">The parent page id.</param>
        /// <param name="title">The page title</param>
        /// <param name="desc">The page description</param>
        /// <returns>A web page decorator object that wrapps the web page model.</returns>
        public WebPageDecorator CreatePage(ContentViewDecorator view, int parentId = 0, string title = "", string desc = "")
        {
            var slugFormatted = string.Format("lists/{0}/views/{1}", view.Parent.Name, view.Name);
            var locale = view.Parent.Locale;

            var existingPage = FindPage(locale, slugFormatted);

            if (existingPage != null)
                return new WebPageDecorator(existingPage, Context);

            var newpage = new WebPage()
            {
                Slug = slugFormatted,
                IsShared = false,
                IsStatic = false,
                Title = view.IsDefault ? view.Parent.Title : view.Title,
                Description = view.Description,
                ShowInMenu = false,
                ShowInSitemap = true,
                AllowAnonymous = true,
                Created = DateTime.Now,
                LastModified = DateTime.Now,
                WebID = this.Id,
                Owner = this.Owner,
                Dir = this.Dir,
                Locale = locale,
                ParentID = parentId
            };

            if (!string.IsNullOrEmpty(title))
                newpage.Title = title;

            if (!string.IsNullOrEmpty(desc))
                newpage.Description = desc;

            Context.Add(newpage);
            Context.SaveChanges();
            if (parentId > 0)
            {
                var parentPage = Context.Find<WebPage>(parentId);
                if (parentPage != null)
                {
                    newpage.Path = parentPage.Path + "/" + newpage.ID.ToString();
                    Context.SaveChanges();
                }
            }

            if (string.IsNullOrEmpty(newpage.Path))
            {
                newpage.Path = "0/" + newpage.ID.ToString();
            }

            #region create static widgets
            var app = App.Get();
            var listInfoDescriptor = app.Descriptors[@"content\listinfo"];
            var listInfoWidget = app.AddWidgetTo(this.Name, newpage.ID, listInfoDescriptor.ID, "ContentZone", 0);
            listInfoWidget.ShowHeader = false;
            listInfoWidget.ShowBorder = false;
            listInfoWidget.Title = view.Parent.Title;
            listInfoWidget.SetUserPreference("listName", view.Parent.Name);
            listInfoWidget.SetUserPreference("showTitle", false);
            listInfoWidget.SetUserPreference("showTools", true);
            listInfoWidget.Save();

            //create view widget
            var viewDescriptor = app.Descriptors[@"content\dataview"];
            var viewWidget = app.AddWidgetTo(this.Name, newpage.ID, viewDescriptor.ID, "ContentZone", 1);
            viewWidget.ShowHeader = false;
            viewWidget.ShowBorder = false;
            viewWidget.IsStatic = true;
            viewWidget.Title = view.Title;
            viewWidget.SetUserPreference("listName", view.Parent.Name);
            viewWidget.SetUserPreference("viewName", view.Name);
            viewWidget.SetUserPreference("allowFiltering", false);
            viewWidget.SetUserPreference("allowSorting", false);
            viewWidget.SetUserPreference("allowPaging", view.AllowPaging);
            viewWidget.SetUserPreference("pagingInfo", true);
            viewWidget.SetUserPreference("pageIndex", view.PageIndex);
            viewWidget.SetUserPreference("pageSize", view.PageSize);
            viewWidget.Save();

            #endregion

            var pageWrapper = new WebPageDecorator(newpage, Context);
            if (view.Roles != null && view.Roles.Count() > 0)
                pageWrapper.AddRoles(pageWrapper.Roles);

            //Context.SaveChanges();
            pageWrapper.UpdatePositions();
            app.CurrentWeb.ClearCache();
            return pageWrapper;
        }

        /// <summary>
        /// Create a new web page by specified content form.
        /// </summary>
        /// <param name="form">The content form object.</param>
        /// <param name="parentId">The parent page id.</param>
        /// <param name="title">The page title</param>
        /// <param name="desc">The page description</param>
        /// <returns>A web page decorator object that wrapps the web page model.</returns>
        public WebPageDecorator CreatePage(ContentFormDecorator form, int parentId = 0, string title = "", string desc = "")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new content list by specified content type.
        /// </summary>
        /// <param name="contentType">The content type object.</param>
        /// <param name="title">The content list title</param>
        /// <param name="name">The new content list name.</param>
        /// <returns>A content list object</returns>
        public ContentListDecorator CreateList(ContentList contentType, string title, string name)
        {
            var listNames = Lists.Select(l => l.Name).ToArray();

            if (contentType.IsSingle)
            {
                if (Context.Count<ContentList>(c => c.Name.Equals(contentType.BaseType)) > 0)
                    throw new Exception("ContentType already created.");
            }

            if (!string.IsNullOrEmpty(title))
            {
                contentType.Title = title;
                if (string.IsNullOrEmpty(name))
                    contentType.Name = TextUtility.Slug(title);
            }

            if (!string.IsNullOrEmpty(name))
                contentType.Name = TextUtility.Slug(name);

            contentType.WebID = this.Id;
            contentType.Locale = this.Culture;
            contentType.Owner = this.Owner;
            contentType.LastModified = DateTime.Now;
            var uniqueName = contentType.Name;
            var i = 0;

            while (listNames.Contains(uniqueName))
                uniqueName = contentType.Name + (++i).ToString();

            if (uniqueName != contentType.Name)
                contentType.Name = uniqueName;

            var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
            var names = new List<string>();

            //        if (v.StartupScripts != null && !string.IsNullOrEmpty(v.StartupScripts.Source))
            //            view.StartupScripts = string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Url.Content(pkg.ResolveUri(v.StartupScripts.Source, v.StartupScripts.Language.Equals(contentType.DefaultLocale, StringComparison.OrdinalIgnoreCase) ? "" : lang)));

            //        if (v.StyleSheet != null && !string.IsNullOrEmpty(v.StyleSheet.Source))
            //            view.StyleSheet = string.Format("<link type=\"text/css\" rel=\"StyleSheet\" href=\"{0}\"></script>", Url.Content(pkg.ResolveUri(v.StyleSheet.Source, v.StyleSheet.Language.Equals(contentType.DefaultLocale, StringComparison.OrdinalIgnoreCase) ? "" : lang)));

            #region unqurie slug
            if (contentType.Views != null)
            {
                foreach (var v in contentType.Views)
                {
                    var slug = v.Name;
                    var flex = slug;
                    var j = 0;
                    while (names.Contains(slug))
                        slug = flex + (++j).ToString();
                    names.Add(slug);

                    if (v.Name != slug)
                        v.Name = slug;
                }

                if (contentType.Views.Count(v => v.IsDefault) == 0)
                    contentType.Views.FirstOrDefault().IsDefault = true;
            }
            #endregion

            if (contentType.Items != null)
            {
                foreach (var dataItem in contentType.Items)
                {
                    if (dataItem.ID == Guid.Empty || Context.Find<ContentDataItem>(dataItem.ID) != null)
                        dataItem.ID = Guid.NewGuid();

                    dataItem.Locale = this.Culture;
                    if (string.IsNullOrEmpty(dataItem.Owner))
                        dataItem.Owner = this.Owner;
                }
            }

            Context.Add(contentType);
            Context.SaveChanges();
            var defaultViewPageID = 0;

            if (contentType.Views != null)
            {
                foreach (var v in contentType.Views)
                {
                    #region new page

                    if (!v.NoPage)
                    {
                        var newpage = CreatePage(new ContentViewDecorator(v, Context), defaultViewPageID);
                        if (newpage == null)
                            continue;

                        if (v.IsDefault)
                            defaultViewPageID = newpage.ID;
                    }

                    #endregion

                    Context.SaveChanges();
                }
            }
            var returns = new ContentListDecorator(Context, contentType);
            returns.ClearCache();
            return returns;
        }

        /// <summary>
        /// Create a new content list by specified content type xml file url.
        /// </summary>
        /// <param name="url">The content list xml file url.</param>
        /// <param name="title">The content list title</param>
        /// <param name="name">The new content list name.</param>
        /// <returns>A content list object</returns>
        public ContentListDecorator CreateList(string url, string title = "", string name = "")
        {
            var xdoc = XDocument.Load(url);
            return CreateList(xdoc.Root, title, name);
        }

        /// <summary>
        /// Create a new content list by specified content type element.
        /// </summary>
        /// <param name="listElement">The content list element</param>
        /// <param name="title">The content list title</param>
        /// <param name="name">The new content list name.</param>
        /// <returns>A content list object</returns>
        public ContentListDecorator CreateList(XElement listElement, string title = "", string name = "")
        {
            var list = new ContentList();
            list.Load(listElement);
            return CreateList(list, title, name);
        }

        internal List<WebPage> cachingPages = null;

        /// <summary>
        /// Get current locale pages from cache
        /// </summary>
        internal List<WebPage> CachingPages
        {
            get
            {
                if (this.cachingPages == null)
                {
                    var cache = HttpContext.Current.Cache;
                    var locale = App.Get().Context.Locale;
                    var key = ("web-" + this.Id + "-pages-" + locale).ToLower();
                    if (cache[key] != null)
                        cachingPages = (List<WebPage>)cache[key];
                    else
                    {
                        cachingPages = Context.WebPages.Filter(p => p.WebID == this.Id && p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase)).ToList();
                        cache.Add(key, cachingPages, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration,
                            System.Web.Caching.CacheItemPriority.Default, null);
                    }
                }
                return cachingPages;
            }
        }

        /// <summary>
        /// Clear all page cache and release resources.
        /// </summary>
        public void ClearCache()
        {
            cachingPages = null;
            var locale = App.Get().Context.Locale;
            var key = ("web-" + this.Id + "-pages-" + locale).ToLower();
            if (HttpContext.Current.Cache[key] != null)
                HttpContext.Current.Cache.Remove(key);
        }

        /// <summary>
        /// Find web page by specified id
        /// </summary>
        /// <param name="id">The page id.</param>
        /// <returns>A web page decorator object that wrapps the web page model.</returns>
        public WebPageDecorator FindPage(int id)
        {
            //            var page = CachingPages.Find(w => w.ID.Equals(id));
            //          if (page == null)
            var page = Context.Find<WebPage>(p => p.WebID == this.Id && p.ID.Equals(id));

            if (page == null)
                return null;

            return new WebPageDecorator(page, Context);
        }

        /// <summary>
        /// Find web page by specified locale and name
        /// </summary>
        /// <param name="locale">The page locale</param>
        /// <param name="slug">The page name</param>
        /// <returns>A web page decorator object that wrapps the web page model.</returns>
        public WebPageDecorator FindPage(string locale, string slug)
        {
            //var page = CachingPages.Find(p => p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase) && p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            //if (page == null)
            //{
            var page = Context.WebPages.Find(p => p.WebID == this.Id && p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase) && p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            //if (page != null)
            //    CachingPages.Add(page);
            //}

            if (page != null)
                return new WebPageDecorator(page, Context);
            return null;
        }

        private string[] supportedLanguaes = null;

        [Obsolete("Use InstalledLocales to instead", true)]
        public string[] SupportedLanguages
        {
            get
            {
                if (supportedLanguaes == null)
                    supportedLanguaes = Context.WebPages.Filter(p => p.WebID == this.Id).Select(p => p.Locale.ToLower()).Distinct().ToArray();
                return supportedLanguaes;
            }
        }

        private string[] installedLocales = null;

        /// <summary>
        /// Gets all installed locales
        /// </summary>
        public string[] InstalledLocales
        {
            get
            {
                //if (object.ReferenceEquals(installedLocales, null))
                //{
                //    if (string.IsNullOrEmpty(this.LocData))
                //    {
                //        installedLocales = new string[] { DefaultLocale };
                //    }
                //    else
                //    {
                //        var locales = JsonConvert.DeserializeObject<IEnumerable<WebLocData>>(this.LocData);
                //        installedLocales = locales.Select(w => w.Locale).Distinct().ToArray();
                //    }
                //}
                if (installedLocales == null)
                    installedLocales = Context.WebPages.Filter(p => p.WebID == this.Id).Select(p => p.Locale.ToLower()).Distinct().ToArray();
                return installedLocales;
            }
        }

        /// <summary>
        /// Gets installed culture objects
        /// </summary>
        public List<CultureInfo> InstalledCultures
        {
            get
            {
                var cultures = new List<CultureInfo>();
                var langs = this.InstalledLocales;
                foreach (var lang in langs)
                {
                    cultures.Add(new CultureInfo(lang));
                }
                return cultures;
            }
        }

        /// <summary>
        /// Gets whether the web has specified locale
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public bool HasLocale(string locale)
        {
            if (string.IsNullOrEmpty(locale))
                return false;

            return InstalledLocales.Contains(locale.ToLower());
        }

        /// <summary>
        /// Get master layout for current context.
        /// </summary>
        /// <remarks>
        ///  Try to get the master layout file (_master.cshtml) from current theme path. In this step 
        ///  when the master layout found in locale theme path it will be return. If not any master file in 
        ///  theme path this method will returns the default master file in shared path.
        /// </remarks>
        /// <param name="httpContext">The http context object</param>
        /// <returns></returns>
        public string GetMasterLayout(HttpContextBase httpContext)
        {
            //Find default locale
            var themeMasterPath = string.Format("~/content/themes/{0}/_master.cshtml", this.Theme);
            var localeMasterPath = string.Format("~/content/themes/{0}/{1}/_master.cshtml", this.Theme, Culture);

            if (System.IO.File.Exists(httpContext.Server.MapPath(localeMasterPath)))
                return localeMasterPath;

            if (System.IO.File.Exists(httpContext.Server.MapPath(themeMasterPath)))
                return themeMasterPath;

            return "~/views/shared/_master.cshtml";
        }

        ///// <summary>
        ///// Switch the default language to specfied locale
        ///// </summary>
        ///// <param name="locale"></param>
        ///// <returns></returns>
        //public bool SwitchLocale(string locale = "")
        //{
        //    if (string.IsNullOrEmpty(locale))
        //        locale = App.Settings.DefaultLocale;

        //    if (!DefaultLocale.Equals(locale, StringComparison.OrdinalIgnoreCase))
        //    {
        //        IEnumerable<WebLocData> locales = null;
        //        var curLocData = new WebLocData(this);

        //        if (!string.IsNullOrEmpty(this.LocData))
        //        {
        //            locales = JsonConvert.DeserializeObject<IEnumerable<WebLocData>>(this.LocData);
        //            var targetLocData = locales.FirstOrDefault(l => l.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase));
        //            var excludes = locales.Where(l => !l.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase) && !l.Locale.Equals(curLocData.Locale, StringComparison.OrdinalIgnoreCase));
        //            var tmpCollection = new List<WebLocData>();
        //            if (excludes.Count() > 0)
        //                tmpCollection.AddRange(excludes);

        //            if (targetLocData != null)
        //            {
        //                this.Title = targetLocData.Title;
        //                this.Description = targetLocData.Description;
        //                this.CssText = targetLocData.CssText;
        //                this.DefaultUrl = targetLocData.DefaultUrl;
        //                this.Theme = targetLocData.Theme;
        //                this.TimeZone = targetLocData.TimeZone;
        //                this.LogoImageUrl = targetLocData.LogoImageUrl;
        //                this.MasterName = targetLocData.MasterName;
        //            }

        //            tmpCollection.Add(curLocData);
        //            this.LocData = JsonConvert.SerializeObject(tmpCollection);
        //            this.DefaultLocale = locale;
        //            Save();
        //        }
        //        else
        //        {
        //            this.DefaultLocale = locale;
        //            var newCollection = new List<WebLocData>() { curLocData };
        //            this.LocData = JsonConvert.SerializeObject(newCollection);
        //            this.Save();
        //        }
        //        this.categories = null;
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Gets/Sets the culture of current request context
        /// </summary>
        public string Culture
        {
            get
            {
                if (string.IsNullOrEmpty(_culture))
                    _culture = this.DefaultLocale;
                return _culture;
            }
            set
            {
                this.SetCurrentCulture(value);
            }
        }

        private string _culture = "";

        /// <summary>
        /// Gets setting url by specified locale name.
        /// </summary>
        /// <param name="locale">The culture name in the format "{languagecode2}-{country/regioncode2}".</param>
        /// <returns>A string contains locale setting url.</returns>
        public string GetLocaleSettingUrl(string locale)
        {
            return string.Format("~/dashboard/{0}/{1}/settings", this.Name, locale);
        }

        /// <summary>
        /// Gets the contents settings dashboard url
        /// </summary>
        public string ContentsSettingUrl
        {
            get
            {
                return string.Format("~/dashboard/{0}/{1}/contents", this.Name, this.Culture);
            }
        }

        /// <summary>
        /// Gets the page manager dashboard url.
        /// </summary>
        public string PageManagerUrl
        {
            get
            {
                return string.Format("~/dashboard/{0}/{1}/pages", this.Name, this.Culture);
            }
        }

        /// <summary>
        /// Gets the website setting dashboard url.
        /// </summary>
        public string SettingUrl
        {
            get
            {
                return GetLocaleSettingUrl(this.Culture);
            }
        }

        /// <summary>
        /// Gets the installed solution's names.
        /// </summary>
        /// <returns>An array contains installed soluton names.</returns>
        public string[] GetInstalledSolutions()
        {
            if (string.IsNullOrEmpty(Model.InstalledSolutions))
                return new string[0];
            else
                return Model.InstalledSolutions.Split(',');
        }

        private void SetCurrentCulture(string culture = "")
        {
            if (string.IsNullOrEmpty(culture))
                culture = App.Settings.DefaultLocale;

            if (!string.IsNullOrEmpty(culture))
            {
                if (!string.IsNullOrEmpty(this.LocData))
                {
                    var key = "_loccache_" + culture;
                    var locales = JsonConvert.DeserializeObject<IEnumerable<WebLocData>>(this.LocData);
                    var targetLocData = locales.FirstOrDefault(l => l.Locale.Equals(culture, StringComparison.OrdinalIgnoreCase));
                    if (targetLocData != null)
                    {
                        this.Title = targetLocData.Title;
                        this.Description = targetLocData.Description;
                        this.CssText = targetLocData.CssText;
                        this.DefaultUrl = targetLocData.DefaultUrl;
                        this.Theme = targetLocData.Theme;
                        this.TimeZone = targetLocData.TimeZone;
                        this.LogoImageUrl = targetLocData.LogoImageUrl;
                    }
                    this._culture = culture;
                    this.categories = null;
                }
            }
        }

        /// <summary>
        /// Save current web data to LocData
        /// </summary>
        private void SaveLocData()
        {
            var localeObjects = new List<WebLocData>();
            if (!string.IsNullOrEmpty(this.LocData))
            {
                var locales = JsonConvert.DeserializeObject<IEnumerable<WebLocData>>(this.LocData);
                if (locales != null && locales.Count() > 0)
                {
                    var queryLocales = locales.Where(l => !l.Locale.Equals(this.Culture, StringComparison.OrdinalIgnoreCase));
                    if (queryLocales.Count() > 0)
                        localeObjects.AddRange(queryLocales.ToList());
                }
            }
            var curLocalData = new WebLocData(this);
            curLocalData.Locale = this.Culture;
            localeObjects.Add(curLocalData);
            this.LocData = JsonConvert.SerializeObject(localeObjects.ToArray());
        }

        /// <summary>
        /// Save changes and submit to database.
        /// </summary>
        /// <returns>true if success otherwrise false.</returns>
        public bool Save()
        {
            SaveLocData();
            this.CopyTo(Model, new string[] { "Pages", "Lists", "Categories" });
            Model.Data = this.Data;
            Context.Update(Model);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Save pages, lists and widgets to xml file.
        /// </summary>
        /// <param name="file">The solution xml file name.</param>
        /// <returns>true if success otherwrise false.</returns>
        public bool SaveAs(string file)
        {
            throw new NotImplementedException();
            ////Generate the page xmls

            ////Generate the list xmls

            ////Generate the solution xml file

            //return false;
        }

        /// <summary>
        /// Indicates whether current user is the website owner.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>true if success otherwrise false.</returns>
        public bool IsOwner(HttpContextBase context)
        {
            return context.Request.IsAuthenticated && context.User.Identity.Name.Equals(this.Owner);
        }

        /// <summary>
        /// Convert to expando object.
        /// </summary>
        /// <returns></returns>
        public dynamic ToObject()
        {
            dynamic webJson = new ExpandoObject();
            webJson.name = this.Name;
            webJson.title = this.Title;
            webJson.desc = this.Description;
            webJson.url = this.Url;
            webJson.owner = this.Owner;
            webJson.lang = this.Culture;
            return webJson;
        }

        /// <summary>
        /// Gets the organization object.
        /// </summary>
        public virtual IOrganization Organization
        {
            get
            {
                return this.Model as IOrganization;
            }
        }

        public virtual IAddress Address
        {
            get {
                return this.Model as IAddress;
            }
        }

        /// <summary>
        /// Gets the three-letter code defined in ISO 3166 for the country/region.
        /// </summary>
        public virtual string ThreeLetterISORegionName
        {
            get
            {
                var cultureInfo = new CultureInfo(Culture);
                var regionInfo = new RegionInfo(cultureInfo.LCID);
                return regionInfo.ThreeLetterISORegionName;
            }
        }

        /// <summary>
        /// Gets the two-letter code defined in ISO 3166 for the country/region.
        /// </summary>
        public virtual  string TwoLetterISORegionName
        {
            get
            {
                var cultureInfo = new CultureInfo(Culture);
                var regionInfo = new RegionInfo(cultureInfo.LCID);
                return regionInfo.TwoLetterISORegionName;
            }
        }

        //public IEnumerable<WebLocData> Locales
        //{
        //    get
        //    {
        //        IEnumerable<WebLocData> locales = null;
        //        var curLocData = new WebLocData(this);

        //        if (!string.IsNullOrEmpty(this.LocData))
        //        {
        //            locales = JsonConvert.DeserializeObject<IEnumerable<WebLocData>>(this.LocData);
        //            var targetLocData = locales.FirstOrDefault(l => l.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase));
        //            var excludes = locales.Where(l => !l.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase) && !l.Locale.Equals(curLocData.Locale, StringComparison.OrdinalIgnoreCase));
        //            var tmpCollection = new List<WebLocData>();
        //            if (excludes.Count() > 0)
        //                tmpCollection.AddRange(excludes);

        //            if (targetLocData != null)
        //            {
        //                this.Title = targetLocData.Title;
        //                this.Description = targetLocData.Description;
        //                this.CssText = targetLocData.CssText;
        //                this.DefaultUrl = targetLocData.DefaultUrl;
        //                this.Theme = targetLocData.Theme;
        //                this.TimeZone = targetLocData.TimeZone;
        //                this.LogoImageUrl = targetLocData.LogoImageUrl;
        //                this.MasterName = targetLocData.MasterName;
        //            }

        //            tmpCollection.Add(curLocData);
        //            this.LocData = JsonConvert.SerializeObject(tmpCollection);
        //            this.DefaultLocale = locale;
        //            Save();
        //        }
        //        else
        //        {
        //            this.DefaultLocale = locale;
        //            var newCollection = new List<WebLocData>() { curLocData };
        //            this.LocData = JsonConvert.SerializeObject(newCollection);
        //            this.Save();
        //        }
        //    }
        //}

        /// <summary>
        /// Gets the document storage of current website
        /// </summary>
        public virtual IUnitOfWorks Storage
        {
            get
            {
                if (blobs == null)
                    blobs = new DocumentStorage(HttpContext.Current.Server.MapPath("~/app_data/blobs/" + this.Name + "/"));
                return blobs;
            }
        }

        /// <summary>
        /// Gets the queue storage of  current website.
        /// </summary>
        public virtual IQueues Queues
        {
            get
            {
                if (queues == null)
                {
                    var directory = HttpContext.Current.Server.MapPath("~/app_data/queues/" + this.Name + "/");
                    queues = new QueueStorage(directory);
                    
                }
                return queues;
            }
        }
    }


}
