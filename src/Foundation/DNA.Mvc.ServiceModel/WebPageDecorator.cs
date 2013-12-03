//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to WebPage object model. 
    /// </summary>
    public class WebPageDecorator : WebPage
    {
        private string[] roles = null;
        private string[] sheets = null;
        private string[] scripts = null;
        private WebPageCollection children = null;
        private WidgetInstanceCollection widgets = null;
        private WebPageDecorator parentPage = null;
        internal WebPage Model { get; set; }
        private IDataContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the WebPageDecorator class for internal testing.
        /// </summary>
        public WebPageDecorator() { }

        /// <summary>
        /// Initializes a new instance of the WebPageDecorator class
        /// </summary>
        /// <param name="webPage">The web page model.</param>
        /// <param name="context">The data context.</param>
        public WebPageDecorator(WebPage webPage, IDataContext context)
        {
            if (webPage is WebPageDecorator)
                Model = ((WebPageDecorator)webPage).Model;
            else
                Model = webPage;
            this.Context = context;
            Model.CopyTo(this, new string[] { "Widgets", "Roles", "Scripts", "StyleSheets" });
        }

        /// <summary>
        /// Gets the access roles.
        /// </summary>
        public new string[] Roles
        {
            get
            {
                if (roles == null)
                {
                    if (Model.Roles != null)
                        roles = Model.Roles.Select(r => r.Name).ToArray();
                    //roles = GetRolesFromCache();
                    //if (roles == null)
                    //{
                    //    roles = Context.WebPages.GetRoles(this.ID);
                    //    if (roles != null)
                    //        AddRolesToCache(roles);
                    //}
                }
                return roles;
            }
        }

        /// <summary>
        /// Gets in page scripts.
        /// </summary>
        public new string[] Scripts
        {
            get
            {
                if (!string.IsNullOrEmpty(Model.Scripts) && scripts == null)
                    scripts = Model.Scripts.Split(',');
                return scripts;
            }
        }

        /// <summary>
        /// Gets in page style sheets.
        /// </summary>
        public new string[] StyleSheets
        {
            get
            {
                if (!string.IsNullOrEmpty(Model.StyleSheets) && sheets == null)
                    sheets = Model.StyleSheets.Split(',');
                return sheets;
            }
        }

        /// <summary>
        /// Gets the all parent pages
        /// </summary>
        public IQueryable<WebPage> Parents()
        {
            var idsFromPath = this.Path.Split('/').Where(p => !string.IsNullOrEmpty(p)).Select(c => int.Parse(c)).ToArray();
            return Context.WebPages.Filter(p => idsFromPath.Contains(p.ID) && p.ID != this.ID);
        }

        /// <summary>
        /// Gets the parent page instance
        /// </summary>
        public WebPageDecorator Parent
        {
            get
            {
                if (this.ParentID > 0 && this.parentPage == null)
                {
                    this.parentPage = new WebPageDecorator(Context.WebPages.Find(this.ParentID), Context);
                }
                return parentPage;
            }
        }

        /// <summary>
        /// Gets all sibling pages in current locale
        /// </summary>
        public IQueryable<WebPage> Siblings()
        {

            return Context.WebPages.Filter(p => p.Locale.Equals(this.Locale) && p.ParentID == this.ParentID &&
                  p.WebID == this.WebID && p.ID != this.ID);

        }

        /// <summary>
        /// Gets all descendant pages
        /// </summary>
        public IQueryable<WebPage> Descendants()
        {
            var thisPath = this.Path + "/";
            return Context.WebPages.Filter(p => p.Locale.Equals(this.Locale) && p.WebID == this.WebID && p.Path.StartsWith(thisPath));
        }

        /// <summary>
        /// Gets widgets of current page.
        /// </summary>
        public new WidgetInstanceCollection Widgets
        {
            get
            {
                if (widgets == null)
                    widgets = new WidgetInstanceCollection(Context, Model);
                return widgets;
            }
        }

        /// <summary>
        /// Gets children pages.
        /// </summary>
        public WebPageCollection Children
        {
            get
            {
                if (children == null)
                    children = new WebPageCollection(Context, Model);
                return children;
            }
        }

        /// <summary>
        /// Copy current page as a new page instance.
        /// </summary>
        /// <param name="targetCulture"></param>
        /// <returns></returns>
        public WebPageDecorator Clone(string targetCulture = "")
        {

            var locale = string.IsNullOrEmpty(targetCulture) ? this.Locale : targetCulture;
            var web = Context.Find<Web>(WebID);
            var newPage = new WebPage();
            newPage.Populate(this.Model);

            newPage.WebID = web.Id;
            newPage.ParentID = this.ParentID;
            newPage.Owner = this.Owner;

            if (!string.IsNullOrEmpty(locale))
                newPage.Locale = locale;

            var slugs = Context.WebPages.Filter(p => p.Locale.Equals(newPage.Locale, StringComparison.OrdinalIgnoreCase) &&
                //p.ID != this.ID &&
                p.WebID.Equals(newPage.WebID)).Select(p => p.Slug).ToArray();

            var slug = TextUtility.Slug(newPage.Title);
            newPage.Slug = slug;

            var i = 0;
            while (slugs.Contains(newPage.Slug))
                newPage.Slug = slug + "-" + (i++).ToString();

            Context.WebPages.Create(newPage);
            Context.SaveChanges();

            //Generate path
            if (this.ParentID == 0)
                newPage.Path = "0/" + newPage.ID;
            else
            {
                newPage.Path = this.Parent.Path + "/" + newPage.ID;
            }

            Context.SaveChanges();

            //Copy roles
            if (Model.Roles != null && Model.Roles.Count() > 0)
                Context.WebPages.AddRoles(newPage.ID, this.Roles);

            //Copy widgets
            if (Model.Widgets != null && Model.Widgets.Count() > 0)
            {
                foreach (var w in Model.Widgets)
                {
                    if (w.ShowMode > 0) continue;
                    var nw = w.Clone(locale);
                    nw.PageID = newPage.ID;
                    Context.Widgets.Create(nw);
                    if (w.Roles != null && w.Roles.Count > 0)
                    {
                        var widgetWrapper = new WidgetInstanceDecorator(nw, Context.Widgets);
                        var _roles = w.Roles.Select(r => r.Name).ToArray();
                        widgetWrapper.AddRoles(_roles);
                    }
                }
                Context.SaveChanges();
            }

            var newresult = new WebPageDecorator(newPage, Context);
            newresult.UpdatePositions();
            return newresult;
        }

        /// <summary>
        /// Gets the master page where the page inherit the layout and style from
        /// </summary>
        /// 
        public WebPageDecorator Master
        {
            get
            {
                if (this.MasterID > 0 && this.master == null)
                {
                    var tmp = Context.Find<WebPage>(this.MasterID);
                    if (tmp != null)
                        master = new WebPageDecorator(tmp, Context);
                }

                return master;
            }
        }

        private WebPageDecorator master = null;

        /// <summary>
        /// Add access roles to current page.
        /// </summary>
        /// <param name="roles"></param>
        public void AddRoles(string[] roles)
        {
            this.roles = null;
            Context.WebPages.AddRoles(this.ID, roles);
        }

        /// <summary>
        /// Clear all access roles.
        /// </summary>
        public void ClearRoles()
        {
            this.roles = null;
            Context.WebPages.ClearRoles(this.ID);
        }

        public void SetAsDefault()
        {
            Context.WebPages.SetToDefault(this.WebID, this.ID);
        }

        public void SetVisible(bool value)
        {
            this.ShowInMenu = Model.ShowInMenu = value;
            this.ShowInSitemap = Model.ShowInSitemap = value;
            Context.WebPages.Update(Model);
            Context.SaveChanges();
        }

        public void Rollback(int version)
        {
            if (this.Version < version)
                throw new Exception("Unknow page version.Rollback fail!");

            Model = Context.WebPages.Rollback(ID, version);
            Model.CopyTo(this, new string[] { "Widgets", "Roles" });
            children = null;
            widgets = null;
        }

        public void Publish(string remarks = "")
        {
            Model.Version = Version = Context.WebPages.Publish(ID, remarks);
        }

        /// <summary>
        /// Move this page other parent page by specified parent page object and position.
        /// </summary>
        /// <param name="page">The parent page instance.</param>
        /// <param name="pos">The page position.</param>
        public void MoveTo(WebPage page, int pos = 0)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            this.MoveTo(page.ID, pos);
        }

        /// <summary>
        /// Move this page other parent page by specified parent id and position.
        /// </summary>
        /// <param name="parentID">The parent page id.</param>
        /// <param name="pos">The page position.</param>
        public void MoveTo(int parentID, int pos = 0)
        {
            if (parentID == this.ID)
                throw new Exception("Can not move to page under itself.");
            Context.WebPages.Move(parentID, this.ID, pos);
            Context.SaveChanges();
            UpdatePositions();
        }

        public void MoveTo(Web parentWeb)
        {
            var webID = parentWeb.Id;
            var descendants = this.Descendants();
            foreach (var descendant in descendants)
                descendant.WebID = webID;
            this.WebID = webID;
            this.ParentID = 0;
            this.Model.WebID = webID;
            this.Model.ParentID = 0;
            Context.SaveChanges();
        }

        internal void UpdatePositions()
        {
            var siblings = Context.WebPages.Filter(p => p.Locale.Equals(this.Locale) && p.ParentID == this.ParentID && p.WebID == this.WebID).OrderBy(p => p.Pos).ToList();
            for (int i = 0; i < siblings.Count; i++)
                siblings[i].Pos = i;

            Context.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(ToObject());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public dynamic ToObject()
        {
            //var Url = new UrlHelper(httpContext.Request.RequestContext);

            return new
            {
                id = this.ID,
                isDefault = false, ///TODO Get default
                isMenu = this.ShowInMenu,
                isSitemap = this.ShowInSitemap,
                linkto = this.LinkTo,
                title = this.Title,
                desc = this.Description,
                keywords = this.Keywords,
                icon = this.IconUrl,
                picture = this.ImageUrl,
                parentId = this.ParentID,
                target = !string.IsNullOrEmpty(this.Target) ? this.Target : "_self",
                shared = this.IsShared,
                @static = this.IsStatic,
                layout = this.ViewName,
                modified = this.LastModified,
                path = Url,//Url.Content(this.Path),
                roles = Roles,
                allowAnonymous = this.AllowAnonymous
            };
        }

        /// <summary>
        /// Gets the absolute url of current page
        /// </summary>
        public string Url
        {
            get
            {
                var request = HttpContext.Current.Request;
                return string.Format(request.ApplicationPath + "{0}/{1}/{2}.html", Web.Name, this.Locale, Slug);
            }
        }

        /// <summary>
        /// Gets absolute page url.
        /// </summary>
        public string FullUrl
        {
            get
            {
                return this._GenUrl(this.Slug);
            }
        }

        private string _GenUrl(string slug)
        {
            var request = HttpContext.Current.Request;
            return string.Format(request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath + "{0}/{1}/{2}.html", Web.Name, this.Locale, slug);
        }

        /// <summary>
        /// Save all change to database
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            //try
            //{
            var orgSlug = this.Model.Slug;
            var newSlug = this.Slug;

            this.CopyTo(Model, "Scripts", "StyleSheets", "Roles");
            Context.Update(Model);
            var count = Context.SaveChanges() > 0;

            if (!orgSlug.Equals(newSlug, StringComparison.OrdinalIgnoreCase))
            {
                var orgUrl = _GenUrl(orgSlug);
                var newUrl = _GenUrl(newSlug);
                var saver = new UrlSaver(Context);
                saver.Rename(orgUrl, newUrl);
            }

            return count;
            //}
            //catch (System.ObjectDisposedException e)
            //{
            //    this.Model = Context.Find<WebPage>(this.ID);
            //    return Save();
            //}
        }

        /// <summary>
        /// Save current page to xml file.
        /// </summary>
        /// <param name="file"></param>
        public void Save(string file)
        {
            var el = this.Element(true);
            el.Save(file);
        }

        /// <summary>
        /// Where the page has custom view data use this method the ensure the view file is created or update the view file content.
        /// </summary>
        public void EnsureViewFile(HttpServerUtilityBase server)
        {
            if (!string.IsNullOrEmpty(ViewData))
            {
                var website = this.Web.Name;
                var path = string.Format("~/content/layouts/{0}/{1}.cshtml", website, ID.ToString());
                var pageFile = server.MapPath(path);
                var pageFileInfo = new FileInfo(pageFile);

                if (!pageFileInfo.Exists || (pageFileInfo.LastWriteTime - this.LastModified).Seconds != 0)
                {
                    var _dir = server.MapPath(string.Format("~/content/layouts/{0}", website));
                    if (!Directory.Exists(_dir))
                        Directory.CreateDirectory(_dir);

                    System.IO.File.WriteAllText(pageFile, ViewData, System.Text.Encoding.UTF8);
                    ViewName = path;
                    Save();
                }
            }
        }

        /// <summary>
        /// Convert the page object to XElement.
        /// </summary>
        /// <param name="withNamespace"></param>
        /// <returns>A xelement contains web page definition.</returns>
        public XElement Element(bool withNamespace = false)
        {
            XNamespace ns = "http://www.dotnetage.com/XML/Schema/page";
            XNamespace wns = "http://www.dotnetage.com/XML/Schema/widget-data";

            var element = new XElement(ns + PAGE,
                new XAttribute("xmlns", ns),
                new XAttribute(VIEWMODE, (PageViewModes)this.ViewMode),
                new XAttribute(SLUG, this.Slug));

            if (withNamespace)
                element.Add(new XAttribute(XNamespace.Xmlns + "w", wns));

            #region attributes

            if (!this.ShowInMenu)
                element.Add(new XAttribute(SHOW_IN_MENU, false));

            if (!this.ShowInSitemap)
                element.Add(new XAttribute(SHOW_IN_SITEMAP, false));

            if (!this.AllowAnonymous)
            {
                element.Add(new XAttribute(ANONYMOUS, false));
                if (this.Roles != null && this.Roles.Length > 0)
                    element.Add(new XAttribute(ROLES, string.Join(",", this.Roles)));
            }

            if (this.IsStatic)
                element.Add(new XAttribute(STATIC, true));

            if (this.IsShared)
                element.Add(new XAttribute(SHARED, true));

            if (!string.IsNullOrEmpty(this.Target))
                element.Add(new XAttribute(TARGET, this.Target));

            if (!string.IsNullOrEmpty(this.Dir))
                element.Add(new XAttribute(DIR, this.Dir));

            #endregion

            #region elements

            if (!string.IsNullOrEmpty(Title))
                element.Add(new XElement(ns + TITLE, this.Title));

            if (!string.IsNullOrEmpty(Description))
                element.Add(new XElement(ns + DESC, new XCData(this.Description)));

            if (!string.IsNullOrEmpty(Keywords))
                element.Add(new XElement(ns + KEY_WORDS, this.Keywords));

            if (!string.IsNullOrEmpty(this.LinkTo))
                element.Add(new XElement(ns + LINK, this.LinkTo));

            if (!string.IsNullOrEmpty(this.IconUrl))
            {
                if (IconUrl.StartsWith("data:image"))
                    element.Add(new XElement(ns + ICON, new XCData(IconUrl)));
                else
                    element.Add(new XElement(ns + ICON, "images/" + System.IO.Path.GetFileName(this.IconUrl)));
            }

            if (!string.IsNullOrEmpty(this.ImageUrl))
            {
                if (ImageUrl.StartsWith("data:image"))
                    element.Add(new XElement(ns + ICON, new XCData(ImageUrl)));
                else
                    element.Add(new XElement(ns + IMAGE, new XAttribute("src", "images/" + System.IO.Path.GetFileName(this.ImageUrl))));
            }

            if (!string.IsNullOrEmpty(this.ViewName) || !string.IsNullOrEmpty(this.ViewData))
            {
                var layout = new XElement(ns + LAYOUT);
                if (!string.IsNullOrEmpty(this.ViewName))
                {
                    if (string.IsNullOrEmpty(this.ViewData))
                        layout.Add(new XAttribute(NAME, this.ViewName));
                }

                if (!string.IsNullOrEmpty(this.ViewData))
                    layout.Add(new XCData(this.ViewData));

                element.Add(layout);
            }

            if (this.StyleSheets != null && this.StyleSheets.Length > 0)
            {
                foreach (var ss in StyleSheets)
                {
                    var fileName = System.IO.Path.GetFileName(ss);
                    element.Add(new XElement(ns + STYLE, new XAttribute(SRC, "css/" + fileName)));
                }
            }


            if (this.Scripts != null && this.Scripts.Length > 0)
            {
                foreach (var s in Scripts)
                {
                    var fileName = System.IO.Path.GetFileName(s);
                    element.Add(new XElement(ns + SCRIPT, new XAttribute(SRC, "scripts/" + fileName)));
                }
            }
            #endregion

            #region pages

            if (this.Children != null && this.Children.Count() > 0)
            {
                var childrenElement = new XElement(ns + PAGES);
                foreach (var child in Children)
                {
                    if (child.IsStatic && child.IsStatic)
                        continue;

                    childrenElement.Add(child.Element());
                }

                element.Add(childrenElement);
            }

            #endregion

            #region widgets
            if (this.Widgets != null && this.Widgets.Count() > 0)
            {
                var widgetsElement = new XElement(ns + WIDGETS);
                foreach (var widget in this.Widgets)
                {
                    if (widget.PageID == this.ID)
                        widgetsElement.Add(widget.Element());
                }
                element.Add(widgetsElement);
            }
            #endregion

            return element;
        }

        /// <summary>
        /// Indicates whether current user is this page owner.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <returns>true is owner otherwrise false.</returns>
        public bool IsOwner(HttpContextBase context)
        {
            if (context.Request.IsAuthenticated)
                return this.IsOwner(context.User.Identity.Name);
            return false;
        }

        /// <summary>
        /// Indicates whether specified user is this page owner.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <returns>true is owner otherwrise false.</returns>
        public bool IsOwner(string userName)
        {
            return userName.Equals(this.Owner, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Indicates whether allow current user acces this page.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>true is authorized otherwrise false.</returns>
        public bool IsAuthorized(HttpContextBase context)
        {
            if (AllowAnonymous)
                return true;

            if (context.Request.IsAuthenticated)
            {
                if (context.User.Identity.Name.Equals(this.Owner))
                    return true;

                if (Roles != null && Roles.Length > 0)
                {
                    foreach (var role in this.Roles)
                    {
                        if (context.User.IsInRole(role))
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
