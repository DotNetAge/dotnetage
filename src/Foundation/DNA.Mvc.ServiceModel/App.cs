//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using DNA.Data.Documents;
using DNA.Web.Events;
using DNA.Web.ServiceModel.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents alternative class for App to use in WebViewPage.
    /// </summary>
    /// <remarks>
    /// The AppModel class implement the singleton pattern that you should use the Get() method to get AppModel instance instead "new".
    /// </remarks>
    /// <example>
    /// <para>The following example shows how to get the current website title.</para>
    /// <code language="aspx">
    /// @{
    /// var web=AppModel.Get().CurrentWeb;
    /// }
    /// &lt;h2&gt;@web.title&lt;/h2&gt;
    /// </code>
    /// </example>
    public class AppModel
    {
        private static SettingsGroup settings = null;
        public static string BaseThemePath = "~/Content/themes/base/css";

        /// <summary>
        /// Gets the AppModel instance
        /// </summary>
        /// <returns>the singlon object instance of the AppModel class</returns>
        public static App Get() { return App.Get(); }

        /// <summary>
        /// Gets the current DotNetAge version
        /// </summary>
        public static Version Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// Gets the global setting groups
        /// </summary>
        public static SettingsGroup Settings
        {
            get
            {
                if (settings == null)
                    settings = new SettingsGroup();
                return settings;
            }
        }

        /// <summary>
        /// Gets resource string by specified resource base class and resource key.
        /// </summary>
        /// <param name="class">The base resource class name.</param>
        /// <param name="key">The resource key.</param>
        /// <returns>A string read from resource file.</returns>
        public static string GetResourceString(string @class, string key)
        {
            return App.GetResourceString(@class, key);
        }

        /// <summary>
        /// Gets resource string from default resource file by specified resource key.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <returns>A string read from resource file.</returns>
        public static string GetResourceString(string key)
        {
            return App.GetResourceString(key);
        }

        /// <summary>
        /// Indicates whether current application is run under full trust model
        /// </summary>
        public static bool IsFullTrust
        {
            get
            {
                return App.IsFullTrust;
            }
        }

        /// <summary>
        /// Get if is debug mode
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                return App.IsDebug;
            }
        }


        /// <summary>
        /// Gets current application trust level
        /// </summary>
        /// <returns></returns>
        public static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            return App.GetCurrentTrustLevel();
        }

    }

    /// <summary>
    /// Represents an application entrance class that use to get global service and application objects such as widgets, pages, webs and ect.
    /// </summary>
    /// <remarks>
    ///  The App class implement the singleton pattern that you should use the Get() method to get AppModel instance instead "new".
    /// </remarks>
    /// <example>
    /// <para>The following example shows how to get page count of current website.</para>
    /// <code language="cs">
    /// public int GetPageCount()
    /// {
    ///     var web=App.Get().CurrentWeb;
    ///     return web.Pages.Count();
    /// }
    /// </code>
    /// </example>
    public class App
    {
        private static SettingsGroup settings = null;
        private EmailTemplateManager emailMgr = null;
        private UserDecorator user = null;
        private MessageBox messages = null;
        private List<RegionInfo> countries = null;
        private IUnitOfWorks blobs = null;
        public static string BaseThemePath = "~/Content/themes/base/css";
        private IQueues queues;

        /// <summary>
        /// Gets the relative path of current theme.
        /// </summary>
        public string ThemePath
        {
            get
            {
                return "~/Content/themes/" + this.CurrentWeb.Theme + "/css";
            }
        }

        /// <summary>
        /// Gets current user theme name.
        /// </summary>
        public string MyThemePath
        {
            get
            {
                if (HttpContext.Request.IsAuthenticated)
                    return "~/Content/themes/" + (string.IsNullOrEmpty(this.User.DefaultProfile.Theme) ? "default" : this.User.DefaultProfile.Theme) + "/css";
                return "";
            }
        }

        /// <summary>
        /// Gets the App instance
        /// </summary>
        /// <returns>App instance</returns>
        public static App Get()
        {
            return GetService<App>();
        }

        ///// <summary>
        ///// Gets the message box instance to send message / email to user.
        ///// </summary>
        //public MessageBox Messages
        //{
        //    get
        //    {
        //        if (messages == null)
        //        {
        //            var senders = GetServices<IMessageSender>().ToArray();
        //            messages = new MessageBox(senders);
        //        }
        //        return messages;
        //    }
        //}

        ///// <summary>
        ///// Gets email templates
        ///// </summary>
        //public EmailTemplateManager EmailTemplates
        //{
        //    get
        //    {
        //        if (emailMgr == null)
        //            emailMgr = new EmailTemplateManager(HttpContext.Server.MapPath("~/content/emails/"));
        //        return emailMgr;
        //    }
        //}

        /// <summary>
        /// Return a services object by specified type which register in DI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetServices<T>()
        {
            return DependencyResolver.Current.GetServices<T>();
        }

        /// <summary>
        /// Return a service object by specified type which register in DI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        public static bool IsDebug
        {
            get
            {
                var xdoc=XDocument.Load(System.Web.Hosting.HostingEnvironment.MapPath("~/web.config"));
                return xdoc.Root.Descendants().FirstOrDefault(x=>x.Name.LocalName.Equals("compilation")).BoolAttr("debug");
            }
        }

        /// <summary>
        /// Trigger an event
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="sender">The event sender object.</param>
        /// <param name="eventArgs">The event argument object.</param>
        public static void Trigger(string eventName, object sender, object eventArgs = null)
        {
            var eventAggregator = GetService<IEventAggregator>();
            if (eventAggregator != null)
                eventAggregator.Publish(eventName, sender, eventArgs);
        }

        /// <summary>
        /// Gets the settings group
        /// </summary>
        public static SettingsGroup Settings
        {
            get
            {
                if (settings == null)
                    settings = new SettingsGroup();
                return settings;
            }
        }

        /// <summary>
        /// Indicates whether current application is run under full trust model
        /// </summary>
        public static bool IsFullTrust
        {
            get
            {
                return GetCurrentTrustLevel().HasFlag(AspNetHostingPermissionLevel.Unrestricted);
            }
        }

        /// <summary>
        /// Gets current application trust level
        /// </summary>
        /// <returns></returns>
        public static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            foreach (AspNetHostingPermissionLevel trustLevel in
                    new AspNetHostingPermissionLevel[] {
            AspNetHostingPermissionLevel.Unrestricted,
            AspNetHostingPermissionLevel.High,
            AspNetHostingPermissionLevel.Medium,
            AspNetHostingPermissionLevel.Low,
            AspNetHostingPermissionLevel.Minimal 
        })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch (System.Security.SecurityException)
                {
                    continue;
                }

                return trustLevel;
            }

            return AspNetHostingPermissionLevel.None;
        }

        /// <summary>
        /// Only use for testing.
        /// </summary>
        public App() { }

        /// <summary>
        /// Initializes a new instance of the App class by specified data context object.
        /// </summary>
        /// <param name="dataContext">The data context object.</param>
        public App(IDataContext dataContext)
        {
            this.DataContext = dataContext;
            this.HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
            this.HttpContext.User = System.Web.HttpContext.Current.User;
        }

        /// <summary>
        /// Initializes a new instance of the App class by specified data context object and http context.
        /// </summary>
        /// <param name="dataContext">The data context object.</param>
        /// <param name="httpContext">The http context object.</param>
        public App(IDataContext dataContext, HttpContextBase httpContext)
        {
            this.DataContext = dataContext;
            this.HttpContext = httpContext;
        }

        #region Private variables

        private WidgetPackageManager widgetMgr = null;
        private ContentPackageManager contentMgr = null;
        private PageTemplatePackageManager pagemgr = null;
        private ThemePackageManager themesMgr = null;
        private SolutionPackageManager solutionsMgr = null;
        private WebCollection webs = null;
        private WidgetDescriptorCollection descriptors = null;
        private UserCollection users = null;
        private RoleManager roleMgr = null;
        private ProfileDecorator profile = null;
        private WebContext webContext = null;
        private WebPageDecorator page = null;
        private WebDecorator web = null;
        private Searcher searcher = null;
        private UrlSaver saver = null;
        private List<ViewFileTemplate> contentViewFileTmpls = null;
        private List<ViewFileTemplate> pageViewFileTmpls = null;
        private const string contentViewFilePath = "~/content/types/base/views/";
        private const string pageViewFilePath = "~/views/dynamicui/layouts/";

        #endregion

        /// <summary>
        /// Gets the NetDrive service object.
        /// </summary>
        public INetDriveService NetDrive { get { return GetService<INetDriveService>(); } }

        /// <summary>
        /// Gets the ContentViewLayout template collection.
        /// </summary>
        public List<ViewFileTemplate> ContentViewLayouts
        {
            get
            {
                if (contentViewFileTmpls == null)
                    contentViewFileTmpls = ViewFileTemplateManager.GetTemplates(HttpContext, contentViewFilePath);
                return contentViewFileTmpls;
            }
        }

        /// <summary>
        /// Gets the PageLayout template file collection.
        /// </summary>
        public List<ViewFileTemplate> PageLayouts
        {
            get
            {
                if (pageViewFileTmpls == null)
                    pageViewFileTmpls = ViewFileTemplateManager.GetTemplates(HttpContext, pageViewFilePath);
                return pageViewFileTmpls;
            }
        }

        /// <summary>
        /// Gets the UrlSaver object to manage renamed and moved urls.
        /// </summary>
        public UrlSaver Urls
        {
            get
            {
                if (saver == null)
                    saver = new UrlSaver(this.DataContext);
                return saver;
            }
        }

        /// <summary>
        /// Gets the Searcher object.
        /// </summary>
        public Searcher Searcher
        {
            get
            {
                if (searcher == null)
                    searcher = new Searcher();
                return searcher;
            }
        }

        /// <summary>
        /// Gets current http context object.
        /// </summary>
        public virtual HttpContextBase HttpContext { get; set; }

        /// <summary>
        /// Gets current web context object.
        /// </summary>
        public virtual WebContext Context
        {
            get
            {
                if (webContext == null)
                {
                    webContext = new WebContext(HttpContext);
                }
                return webContext;
            }
        }

        /// <summary>
        /// Gets current http request object.
        /// </summary>
        public virtual HttpRequestBase HttpRequest { get { return HttpContext.Request; } }

        /// <summary>
        /// Gets current data context object.
        /// </summary>
        public virtual IDataContext DataContext { get; set; }

        /// <summary>
        /// Gets the global document storage.
        /// </summary>
        public virtual IUnitOfWorks Storage
        {
            get
            {
                if (blobs == null)
                    blobs = new DocumentStorage(HttpContext.Server.MapPath("~/app_data/blobs/"));
                return blobs;
            }
        }

        /// <summary>
        /// Gets the global queue storage
        /// </summary>
        public virtual IQueues Queues
        {
            get
            {
                if (queues == null)
                    queues = new QueueStorage(HttpContext.Server.MapPath("~/app_data/queues/"));
                return queues;
            }
        }

        /// <summary>
        /// Gets current login user
        /// </summary>
        public virtual UserDecorator User
        {
            get
            {
                if (user == null)
                {
                    if (HttpRequest.IsAuthenticated)
                    {
                        user = Users[HttpContext.User.Identity.Name];
                    }
                }
                return user;
            }
        }

        /// <summary>
        /// Gets current login user's default profile object.
        /// </summary>
        public virtual ProfileDecorator Profile
        {
            get
            {
                if (profile == null)
                {
                    if (HttpRequest.IsAuthenticated)
                    {
                        var _profile = DataContext.Find<UserProfile>(u => u.UserName.Equals(HttpContext.User.Identity.Name) && u.IsDefault);
                        if (_profile == null)
                        {
                            _profile = DataContext.Add(new UserProfile()
                            {
                                IsDefault = true,
                                AppName = "dotnetage",
                                Account = HttpContext.User.Identity.Name,
                                UserName = HttpContext.User.Identity.Name
                            });
                            DataContext.SaveChanges();
                        }

                        profile = new ProfileDecorator(DataContext, _profile);
                    }
                }
                return profile;
            }
        }

        /// <summary>
        /// Create a new web
        /// </summary>
        /// <param name="name">The web name</param>
        /// <param name="title">The web title text</param>
        /// <param name="desc">The description text of the web</param>
        /// <param name="locale">The locale name of the web</param>
        /// <param name="theme">The default theme of the web</param>
        /// <returns>Web instance object</returns>
        public virtual WebDecorator CreateWeb(string name, string title, string desc = "", string locale = "", string theme = "")
        {
            if (DataContext.Count<Web>(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) > 0)
                throw new Exception("The web is exists");

            var newWeb = DataContext.Add(new Web()
            {
                Name = name,
                Title = title,
                Description = desc,
                Owner = User.UserName,
                DefaultLocale = string.IsNullOrEmpty(locale) ? App.Settings.DefaultLocale : locale,
                Theme = string.IsNullOrEmpty(name) ? "default" : theme,
                Pages = new List<WebPage>()
            });

            //dataContext.Add(newWeb);
            DataContext.SaveChanges();
            return new WebDecorator(newWeb, DataContext);
        }

        /// <summary>
        /// Create a new web by specified solution template
        /// </summary>
        /// <param name="solutionName">The solution template name</param>
        /// <param name="name">The web name</param>
        /// <param name="title">The web title text</param>
        /// <param name="desc">The description text of the web</param>
        /// <returns>Web instance object</returns>
        public virtual WebDecorator CreateWeb(string solutionName, string name, string title, string desc = "")
        {
            var web = Solutions.Install(solutionName, name, User.UserName, title, desc);
            if (web != null)
                return new WebDecorator(web, DataContext);
            return null;
        }

        /// <summary>
        /// Gets web object from current context.
        /// </summary>
        public virtual WebDecorator CurrentWeb
        {
            get
            {
                if (web == null)
                {
                    if (Context.Web != null)
                        web = new WebDecorator(Context.Web, DataContext);
                }

                if (web != null)
                {
                    if (!string.IsNullOrEmpty(Context.Locale))
                    {
                        //if (!web.Culture.Equals(Context.Locale))
                        web.Culture = Context.Locale;
                    }

                    this.SetCulture(web.Culture, Context.UserLocale);
                }

                return web;
            }
        }

        /// <summary>
        /// Set cultures from current context.
        /// </summary>
        public virtual void SetCulture()
        {
            SetCulture(this.Context.Locale);
        }

        /// <summary>
        /// Set content culture by specified locale.
        /// </summary>
        /// <param name="culture">The locale name.</param>
        public void SetCulture(string culture)
        {
            //if (!Thread.CurrentThread.CurrentCulture.Name.Equals(culture, StringComparison.OrdinalIgnoreCase))
            //{
            //    var curCulture = CultureInfo.GetCultureInfo(culture);
            //    if (curCulture != null)
            //    {
            //        Thread.CurrentThread.CurrentUICulture = curCulture;
            //        Thread.CurrentThread.CurrentCulture = curCulture;
            //    }
            //}
            SetCulture(culture, this.Context.UserLocale);
        }

        /// <summary>
        /// Set content culture and UI culture by specified values.
        /// </summary>
        /// <param name="culture">The content locale name.</param>
        /// <param name="uiculture">The UI locale name.</param>
        public void SetCulture(string culture, string uiculture)
        {
            //if (!Thread.CurrentThread.CurrentCulture.Name.Equals(culture, StringComparison.OrdinalIgnoreCase))
            //{
            var curCulture = CultureInfo.GetCultureInfo(culture);
            if (curCulture != null)
                Thread.CurrentThread.CurrentCulture = curCulture;
            // }

            //if (!Thread.CurrentThread.CurrentCulture.Name.Equals(uiculture, StringComparison.OrdinalIgnoreCase))
            //{
            var curUICulture = CultureInfo.GetCultureInfo(uiculture);
            if (curUICulture != null)
                Thread.CurrentThread.CurrentUICulture = curUICulture;
            //}
        }

        /// <summary>
        /// Gets current request web page object.
        /// </summary>
        public virtual WebPageDecorator CurrentPage
        {
            get
            {
                if (page == null)
                {
                    if (Context.Page != null)
                        page = new WebPageDecorator(Context.Page, DataContext);
                }
                return page;
            }
        }

        /// <summary>
        /// Create a widget instance and add to specified page
        /// </summary>
        /// <param name="webname">The web name</param>
        /// <param name="pageID">The page id where the widget to add.</param>
        /// <param name="descriptorID">The widget descriptor id</param>
        /// <param name="zoneID">The widget zone element id</param>
        /// <param name="pos">The widget position of the owner zone.</param>
        /// <returns></returns>
        public WidgetInstanceDecorator AddWidgetTo(string webname, int pageID, int descriptorID, string zoneID, int pos)
        {
            var descriptor = DataContext.Find<WidgetDescriptor>(descriptorID);
            // Descriptors.Find(descriptorID);

            if (descriptor == null)
                throw new Exception(string.Format("Can not found the widget descriptor has id=\"{0}\"not found.", descriptorID));

            var page = Webs[webname].FindPage(pageID);
            if (page == null)
                throw new Exception("Web page not found for \"" + pageID.ToString() + "\"");
            //descriptor.lo
            // var model = descriptor.Model;
            //if (!model.DefaultLocale.Equals(page.Locale)) {
            //    var localeName= model.LocaleName(page.Locale);
            //}
            var widget = DataContext.Widgets.AddWidget(descriptor, pageID, zoneID, pos);
            DataContext.SaveChanges();

            return new WidgetInstanceDecorator(widget, DataContext.Widgets);
        }

        /// <summary>
        /// Gets list collection of root web.
        /// </summary>
        public virtual ContentListCollection Lists
        {
            get
            {
                return Webs.Root.Lists;
            }
        }

        /// <summary>
        /// Gets webs from database
        /// </summary>
        public virtual WebCollection Webs
        {
            get
            {
                if (webs == null)
                    webs = new WebCollection(DataContext);
                return webs;
            }
        }

        private int[] GetAccessableDescriptorIDs()
        {
            if (User == null)
                return new int[0];

            var roles = User.Roles;
            if (roles == null)
            {
                var user = DataContext.Users.Find(User.ID);
                roles = user.Roles;
            }

            var ids = new List<int>();

            foreach (var r in roles)
            {
                var descriptors = r.Descriptors;
                foreach (var d in descriptors)
                {
                    if (!ids.Contains(d.ID))
                        ids.Add(d.ID);
                }
            }
            return ids.ToArray();
        }

        /// <summary>
        /// Fitler and return widget descriptors by specified category.
        /// </summary>
        /// <param name="category">The widget descriptor's category</param>
        /// <returns></returns>
        public WidgetDescriptorCollection GetDescriptorsIn(string category)
        {
            return new WidgetDescriptorCollection(DataContext, GetAccessableDescriptorIDs(), category);
        }

        /// <summary>
        /// Gets descriptors
        /// </summary>
        public WidgetDescriptorCollection Descriptors
        {
            get
            {
                if (descriptors == null)
                    descriptors = new WidgetDescriptorCollection(DataContext, GetAccessableDescriptorIDs());
                return descriptors;
            }
        }

        /// <summary>
        /// Gets the widget package manager object.
        /// </summary>
        public virtual WidgetPackageManager Widgets
        {
            get
            {
                if (widgetMgr == null)
                    widgetMgr = new WidgetPackageManager(HttpContext, DataContext);
                return widgetMgr;
            }
        }

        /// <summary>
        /// Gets the content package manager object.
        /// </summary>
        public virtual ContentPackageManager ContentTypes
        {
            get
            {
                if (contentMgr == null)
                    contentMgr = new ContentPackageManager(HttpContext, DataContext);
                return contentMgr;
            }
        }

        /// <summary>
        /// Gets all registered users.
        /// </summary>
        public UserCollection Users
        {
            get
            {
                if (users == null)
                    users = new UserCollection(DataContext);
                return users;
            }
        }

        /// <summary>
        /// Gets page template package manager object.
        /// </summary>
        public PageTemplatePackageManager Pages
        {
            get
            {
                if (pagemgr == null)
                    pagemgr = new PageTemplatePackageManager(HttpContext, DataContext);
                return pagemgr;
            }
        }

        /// <summary>
        /// Gets theme package manager object.
        /// </summary>
        public ThemePackageManager Themes
        {
            get
            {
                if (themesMgr == null)
                    themesMgr = new ThemePackageManager(HttpContext);
                return themesMgr;
            }
        }

        /// <summary>
        /// Gets solution package manager object.
        /// </summary>
        public SolutionPackageManager Solutions
        {
            get
            {
                if (solutionsMgr == null)
                    solutionsMgr = new SolutionPackageManager(HttpContext, DataContext);
                return solutionsMgr;
            }
        }

        /// <summary>
        /// Gets role manager object.
        /// </summary>
        public RoleManager Roles
        {
            get
            {
                if (roleMgr == null)
                    roleMgr = new RoleManager(DataContext);
                return roleMgr;
            }
        }

        /// <summary>
        /// Gets tag collection for current locale.
        /// </summary>
        public IQueryable<Tag> Tags
        {
            get
            {
                return DataContext.Where<Tag>(t => t.Locale.Equals(CurrentWeb.Culture, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Gets the PermissionSets 
        /// </summary>
        public IEnumerable<PermissionSet> Permissions
        {
            get
            {
                return DataContext.All<PermissionSet>().OrderBy(p => p.Name).ToList();
            }
        }

        /// <summary>
        /// Query and returns commonts object by specified uri
        /// </summary>
        /// <param name="uri">The uri contains comments</param>
        /// <param name="replyTo">The comment id</param>
        /// <returns></returns>
        public IEnumerable<CommentDecorator> FindComments(string uri, int replyTo = 0)
        {
            var comments = DataContext.Where<Comment>(c => c.TargetUri.Equals(uri, StringComparison.OrdinalIgnoreCase) && c.ReplyTo == replyTo).OrderByDescending(c => c.Posted).AsQueryable();
            if (replyTo > 0)
                comments = comments.Where(c => c.ReplyTo == replyTo);
            if (comments.Count() == 0)
                return new List<CommentDecorator>();
            return comments.Select(c => new CommentDecorator(DataContext, c)).ToList();
        }

        /// <summary>
        /// Query and returns commonts object by specified uri with specified data page.
        /// </summary>
        /// <param name="uri">The uri contains comments</param>
        /// <param name="total">Returns the total records of comments in current query</param>
        /// <param name="index">The page index (zero base)</param>
        /// <param name="size">The return rows for data page</param>
        /// <param name="replyTo"></param>
        /// <returns></returns>
        public IEnumerable<CommentDecorator> FindComments(string uri, out int total, int index = 0, int size = 20, int replyTo = 0)
        {
            var comments = DataContext.Where<Comment>(c => c.TargetUri.Equals(uri, StringComparison.OrdinalIgnoreCase) && c.ReplyTo == replyTo).OrderByDescending(c => c.Posted).AsQueryable();

            if (replyTo > 0)
                comments = comments.Where(c => c.ReplyTo == replyTo);

            total = comments.Count();
            if (total == 0)
                return new List<CommentDecorator>();
            var skipCount = index * size;
            var results = comments.Skip(skipCount).Take(size).ToList();
            return results.Select(c => new CommentDecorator(DataContext, c)).ToList();
        }

        ///// <summary>
        ///// Gets register OAuth providers.
        ///// </summary>
        //public OAuthProviderCollection OAuthProviders
        //{
        //    get
        //    {
        //        return OAuthProxy.Providers;
        //    }
        //}

        /// <summary>
        /// Find item object by specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ContentDataItemDecorator FindItem(Guid id)
        {
            if (id == Guid.Empty || id == null)
                throw new ArgumentNullException("Item id not set");
            var item = DataContext.ContentDataItems.Find(id);
            if (item != null)
            {
                return Wrap(item);
            }
            return null;
        }

        /// <summary>
        /// Find the list object by specified id
        /// </summary>
        /// <param name="id">The list id</param>
        /// <returns></returns>
        public virtual ContentListDecorator FindList(int id)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException("List id not set");
            var list = DataContext.Find<ContentList>(id);
            if (list != null)
            {
                return Wrap(list);
            }

            return null;
        }

        /// <summary>
        /// Find the view object by specified id.
        /// </summary>
        /// <param name="id">The view id</param>
        /// <returns></returns>
        public virtual ContentViewDecorator FindView(int id)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException("View id not set");
            var view = DataContext.Find<ContentView>(id);
            if (view != null)
            {
                return new ContentViewDecorator(view, DataContext);
            }
            return null;
        }

        /// <summary>
        /// Find a fom object by specified id
        /// </summary>
        /// <param name="id">The form id</param>
        /// <returns></returns>
        public virtual ContentFormDecorator FindForm(int id)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException("Form id not set");
            var form = DataContext.Find<ContentForm>(id);
            if (form != null)
            {
                return new ContentFormDecorator(DataContext, form);
            }
            return null;
        }

        /// <summary>
        /// Gets the support languages
        /// </summary>
        public List<CultureInfo> SupportedCultures
        {
            get
            {
                string key = "_supported_cultures";
                if (HttpContext.Cache[key] == null)
                {
                    string[] files = Directory.GetFiles(HttpContext.Server.MapPath("~/App_GlobalResources/Commons"));
                    List<CultureInfo> cis = new List<CultureInfo>();
                    cis.Add(CultureInfo.CreateSpecificCulture("en-US"));
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (Path.GetExtension(files[i]).ToLower() == ".resx")
                        {
                            string locale = Path.GetFileNameWithoutExtension(files[i]);
                            if (locale.ToLower() == "commons")
                                continue;
                            locale = locale.Replace("Commons.", "");
                            cis.Add(CultureInfo.CreateSpecificCulture(locale));
                        }
                    }

                    HttpContext.Cache.Add(key, cis, null, DateTime.Now.AddMinutes(10), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);
                }
                return (List<CultureInfo>)HttpContext.Cache[key];
            }
        }

        public ReadOnlyCollection<TimeZoneInfo> TimeZones
        {
            get
            {
                string key = "_global_timeZones";
                if (HttpContext.Cache[key] == null)
                {
                    var zones = TimeZoneInfo.GetSystemTimeZones();
                    HttpContext.Cache.Add(key, zones, null, DateTime.Now.AddMinutes(10), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);
                }
                return (ReadOnlyCollection<TimeZoneInfo>)HttpContext.Cache[key];
            }
        }

        /// <summary>
        /// Gets countries
        /// </summary>
        public List<RegionInfo> Countries
        {
            get
            {
                if (countries == null)
                {
                    var _cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(c => c.Parent.LCID < 71).OrderBy(c => c.Parent.LCID);
                    var keyset = new Dictionary<string, RegionInfo>();
                    foreach (var c in _cultures)
                    {
                        var region = new RegionInfo(c.LCID);
                        if (!keyset.ContainsKey(region.Name) && !region.TwoLetterISORegionName.Equals("029", StringComparison.OrdinalIgnoreCase))
                            keyset.Add(region.Name, region);
                    }
                    countries = keyset.Select(c => c.Value).ToList().OrderBy(c => c.Name).ToList();
                }
                return countries;
            }
        }

        /// <summary>
        /// Gets the states/provinces by specified country code.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public IEnumerable<StateProvince> GetStateProvinces(string countryCode)
        {
            var services = GetServices<IStateProvinceProvider>();
            var service = services.FirstOrDefault(s => s.CountryCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase));
            if (service != null)
                return service.Items;
            return new List<StateProvince>();
        }

        ///// <summary>
        ///// Gets country names
        ///// </summary>
        ///// <returns>An array contains all country name strings.</returns>
        //public string[] GetCountryNames()
        //{
        //    var _CultureList = new Dictionary<string, string>();
        //    var _CultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        //    foreach (CultureInfo _Culture in _CultureInfo)
        //    {
        //        RegionInfo _RegionInfo = new RegionInfo(_Culture.LCID);

        //        if (!(_CultureList.ContainsKey(_RegionInfo.EnglishName)))
        //            _CultureList.Add(_RegionInfo.EnglishName, _RegionInfo.DisplayName);
        //    }
        //    //_CultureList.Sort();
        //    var result = _CultureList.Select(d => d.Value).ToList();
        //    result.Sort();
        //    return result.ToArray();
        //}

        /// <summary>
        /// Get currency native name by specified ISO currency symbol
        /// </summary>
        /// <param name="isoCurrencyCode">The ISO currency symbol</param>
        /// <returns></returns>
        public string GetCurrencyName(string isoCurrencyCode)
        {
            //var countrie =Countries;
            //if (Countries.ContainsKey(isoCurrencyCode))
            //return countrie[isoCurrencyCode];
            var region = Countries.FirstOrDefault(c => c.ISOCurrencySymbol.Equals(isoCurrencyCode, StringComparison.OrdinalIgnoreCase));
            if (region != null)
                return region.ISOCurrencySymbol;
            return "";
        }

        /// <summary>
        /// Create a wrapper object for specified content list.
        /// </summary>
        /// <param name="list">The content list object.</param>
        /// <returns>A ContentListDescorator object.</returns>
        public ContentListDecorator Wrap(ContentList list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            var _list = list as ContentListDecorator;

            if (_list == null)
                return new ContentListDecorator(DataContext, list);

            return _list;
        }

        /// <summary>
        /// Create a wrapper object for sepcified wdiget descriptor object.
        /// </summary>
        /// <param name="descriptor">The widget descriptor</param>
        /// <returns>A WidgetDescriptorDecorator object.</returns>
        public WidgetDescriptorDecorator Wrap(WidgetDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            var _descriptor = descriptor as WidgetDescriptorDecorator;

            if (_descriptor == null)
                return new WidgetDescriptorDecorator(descriptor, DataContext);

            return _descriptor;
        }

        /// <summary>
        /// Create a wrapper object for sepcified wdiget instance object.
        /// </summary>
        /// <param name="widget">The widget instance object.</param>
        /// <returns>A WidgetInstanceDecorator object</returns>
        public WidgetInstanceDecorator Wrap(WidgetInstance widget)
        {
            if (widget == null)
                throw new ArgumentNullException("widget");
            var _widget = widget as WidgetInstanceDecorator;
            if (_widget == null)
                return new WidgetInstanceDecorator(widget, DataContext.Widgets);
            return _widget;
        }

        /// <summary>
        /// Create a wrapper object for sepcified web object.
        /// </summary>
        /// <param name="web">The web object.</param>
        /// <returns>A WebDecorator object.</returns>
        public WebDecorator Wrap(Web web)
        {
            if (web == null)
                throw new ArgumentNullException("web");

            var _web = web as WebDecorator;
            if (_web == null)
                return new WebDecorator(web, DataContext);
            return _web;
        }

        /// <summary>
        /// Create a wrapper object for sepcified webpage object.
        /// </summary>
        /// <param name="page">The web page object.</param>
        /// <returns>A WebPageDecorator object.</returns>
        public WebPageDecorator Wrap(WebPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");
            var _page = page as WebPageDecorator;
            if (_page == null)
                return new WebPageDecorator(page, DataContext);
            return _page;
        }

        /// <summary>
        /// Create a wrapper object for sepcified content data item object.
        /// </summary>
        /// <param name="item">The content data item object.</param>
        /// <returns>A ContentDataItemDecorator object.</returns>
        public ContentDataItemDecorator Wrap(ContentDataItem item)
        {
            return new ContentDataItemDecorator(item, DataContext);
        }

        /// <summary>
        /// Create a wrapper object for sepcified comment object.
        /// </summary>
        /// <param name="comment">The comment object.</param>
        /// <returns>A CommentDecorator object.</returns>
        public CommentDecorator Wrap(Comment comment)
        {
            return new CommentDecorator(DataContext, comment);
        }

        /// <summary>
        /// Get global resource string by specified class name and key
        /// </summary>
        /// <param name="class">The resource class name</param>
        /// <param name="key">The resource key</param>
        /// <returns></returns>
        public static string GetResourceString(string @class, string key)
        {
            try
            {
                return (string)System.Web.HttpContext.GetGlobalResourceObject(@class, key);
            }
            catch
            {
                return key;
            }
        }

        /// <summary>
        /// Get global resource string by sepcified key
        /// </summary>
        /// <param name="key">The resource key</param>
        /// <returns></returns>
        public static string GetResourceString(string key)
        {
            return GetResourceString("Commons", key);
        }

        /// <summary>
        /// Report a new abuse 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="reporter"></param>
        /// <param name="contentOwner"></param>
        /// <param name="objectType"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Abuse ReportAbuse(string url, string reporter, string contentOwner, string objectType, int type, string content)
        {
            if (DataContext.Find<Abuse>(a => a.Reportor.Equals(reporter) && a.Uri.Equals(url, StringComparison.OrdinalIgnoreCase)) != null)
                return null;

            var obj = new Abuse()
            {
                Uri = url,
                ObjectType = objectType,
                ReportingDate = DateTime.Now,
                Owner = contentOwner,
                Reportor = reporter,
                Type = type,
                Content = content
            };
            DataContext.Add(obj);
            DataContext.SaveChanges();
            return obj;
        }

        /// <summary>
        /// Gets abuse collection
        /// </summary>
        public IQueryable<Abuse> Abuses
        {
            get
            {
                return DataContext.All<Abuse>();
            }
        }
    }

    /// <summary>
    /// Represents the setting class that use to define global setting values.
    /// </summary>
    public class SettingsGroup
    {
        /// <summary>
        /// Gets DotNetAge API key.
        /// </summary>
        public string APIKey { get { return WebConfigurationManager.AppSettings["APIKey"]; } }

        /// <summary>
        /// Gets whether ReCaptcha validation service is enabled.
        /// </summary>
        public bool EnableReCaptchaValidation
        {
            get
            {
                return !string.IsNullOrEmpty(ReCaptchaPrivateKey) && !string.IsNullOrEmpty(ReCaptchaPrivateKey);
            }
        }

        /// <summary>
        /// Gets ReCaptcha private key
        /// </summary>
        public string ReCaptchaPrivateKey
        {
            get
            {
                return WebConfigurationManager.AppSettings["ReCaptchaPrivateKey"];
            }
        }

        /// <summary>
        /// Gets ReCaptcha public key.
        /// </summary>
        public string ReCaptchaPublicKey
        {
            get
            { return WebConfigurationManager.AppSettings["ReCaptchaPublicKey"]; }
        }

        /// <summary>
        /// Gets google analytics account.
        /// </summary>
        public string GAAccount
        {
            get
            {
                return WebConfigurationManager.AppSettings["GAAccount"];
            }
        }

        /// <summary>
        /// Gets whether auto start scheduler when appliciation start.
        /// </summary>
        public bool AutoStartScheduler
        {
            get
            {
                var autoStart = WebConfigurationManager.AppSettings.AllKeys.Contains("AutoStartScheduler") ? WebConfigurationManager.AppSettings["AutoStartScheduler"] : "True";
                return Boolean.Parse(autoStart);
            }
        }

        /// <summary>
        /// Gets whether auto migration database
        /// </summary>
        public bool AutomaticMigrationsEnabled
        {
            get {
                var autoMigration = WebConfigurationManager.AppSettings.AllKeys.Contains("AutoMigration") ? WebConfigurationManager.AppSettings["AutoMigration"] : "True";
                return Boolean.Parse(autoMigration);
            }
        }


        /// <summary>
        /// Gets the scheduler frequency in seconds.
        /// </summary>
        public int SchedulerFrequency
        {
            get
            {
                var defaultFrequency = 30;
                if (WebConfigurationManager.AppSettings.AllKeys.Contains("SchedulerFrequency"))
                    defaultFrequency = Int32.Parse(WebConfigurationManager.AppSettings["SchedulerFrequency"]);
                return defaultFrequency;
            }
        }

        /// <summary>
        /// Gets google api key.
        /// </summary>
        public string GoogleKey
        {
            get
            {
                return WebConfigurationManager.AppSettings["GoogleKey"];
            }
        }

        /// <summary>
        /// Gets bing app key.
        /// </summary>
        public string BingKey
        {
            get
            {
                return WebConfigurationManager.AppSettings["BingKey"];
            }
        }

        /// <summary>
        /// Gets whether the DotNetAge was installed.
        /// </summary>
        public bool Initialized
        {
            get
            {
                var _initialized = WebConfigurationManager.AppSettings["Initialized"];
                var initialized = false;
                bool.TryParse(_initialized, out initialized);
                return initialized;
            }
        }

        /// <summary>
        /// Identity whether allow user registeration.
        /// </summary>
        public bool OpenRegister
        {
            get
            {
                var allowNewUserStr = WebConfigurationManager.AppSettings["AllowNewUser"];
                var allowNewUser = true;
                bool.TryParse(allowNewUserStr, out allowNewUser);
                return allowNewUser;
            }
        }

        /// <summary>
        /// Gets allow uploaded file types.
        /// </summary>
        public string[] AllowUploads
        {
            get
            {
                var types = WebConfigurationManager.AppSettings["AllowUploads"];
                if (!string.IsNullOrEmpty(types))
                {
                    return types.Split(',');
                }
                return new string[0];
            }
        }

        /// <summary>
        /// Gets the supur administrator name.
        /// </summary>
        public string Administrator
        {
            get
            {
                return WebConfigurationManager.AppSettings["Administrator"];
            }
        }

        /// <summary>
        /// Gets the default locale name.
        /// </summary>
        public string DefaultLocale
        {
            get
            {
                return WebConfigurationManager.AppSettings["DefaultLocale"];
            }
        }

        /// <summary>
        /// Gets minimum user password length for user registeration.
        /// </summary>
        public int PasswordLength
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PasswordLength"]);
            }
        }

        /// <summary>
        /// Gets minimum user name length for user registeration
        /// </summary>
        public int UserNameLength
        {
            get
            {
                return int.Parse(WebConfigurationManager.AppSettings["UserNameLength"]);
            }
        }

        /// <summary>
        /// Gets maximum NetDrive used quota.
        /// </summary>
        public int DefaultNetDriveQuota
        {
            get
            {
                return int.Parse(WebConfigurationManager.AppSettings["DefaultNetDriveQuota"]);
            }
        }

        /// <summary>
        /// Gets the reserved user names which not allow to register.
        /// </summary>
        public string[] ReservedUserNames
        {
            get
            {
                var names = WebConfigurationManager.AppSettings["ReservedUserNames"];
                if (!string.IsNullOrEmpty(names))
                    return names.Split(',');
                return new string[0];
            }
        }

        /// <summary>
        /// Gets whether show the site menu.
        /// </summary>
        public bool EnableSitemenu
        {
            get
            {
                bool sitemenu = false;
                if (WebConfigurationManager.AppSettings.AllKeys.Contains("EnableSitemenu"))
                    bool.TryParse(WebConfigurationManager.AppSettings["EnableSitemenu"], out sitemenu);
                return sitemenu;
            }
        }

        /// <summary>
        /// Gets whether redirect to www
        /// </summary>
        public bool WWWResolved
        {
            get
            {
                bool wwwResolve = false;
                if (WebConfigurationManager.AppSettings.AllKeys.Contains("WWWResolved"))
                    bool.TryParse(WebConfigurationManager.AppSettings["WWWResolved"], out wwwResolve);
                return wwwResolve;
            }
        }

        /// <summary>
        /// Gets whether enable force email validation feature.
        /// </summary>
        public bool ForceEmailValidation
        {
            get
            {
                var validationHolder = WebConfigurationManager.AppSettings["ForceEmailValidation"];
                var validation = false;
                bool.TryParse(validationHolder, out validation);
                return validation;
            }
        }
    }
}
