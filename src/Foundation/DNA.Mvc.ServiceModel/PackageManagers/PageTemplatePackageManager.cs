//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Xml.Solutions;
using System;
using System.Web;

namespace DNA.Web.ServiceModel
{
    public sealed class PageTemplatePackageManager : PackageManager<PageTemplatePackage, PageTemplatePackageFactory, PageElement>
    {
        private const string PKG_PATH = "~/content/pages/";

        public IDataContext DataContext { get; set; }

        private HttpContextBase httpContext { get; set; }

        public PageTemplatePackageManager() : this(new HttpContextWrapper(HttpContext.Current)) { }

        public PageTemplatePackageManager(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            this.httpContext = httpContext;

            Init(httpContext.Server.MapPath(PKG_PATH));
        }

        public PageTemplatePackageManager(HttpContextBase httpContext, IDataContext context)
            : this(httpContext)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.DataContext = context;
        }

        public PageTemplatePackageManager(string path) { Init(path); }

        public WebPageDecorator InstantiateIn(string name, Web parentWeb, int parentPageID = 0, string lang = "en-us", string slug = "")
        {
            if (DataContext == null)
                throw new ArgumentNullException("DataContext could not be null");

            var pkg = Packages[name];
            if (pkg == null)
                throw new PageTemplateNotFoundException();

            var localeModel = pkg.Locale(lang);
            localeModel.Locale = lang;
            if (!string.IsNullOrEmpty(slug))
                localeModel.Slug = slug;

            return new WebPageDecorator(localeModel.Install(this.DataContext, parentWeb, parentPageID), this.DataContext);
        }

        public WebPageDecorator InstantiateIn(string name, Web parentWeb, string title, string desc, string keywords, int parentPageID = 0, string lang = "en-us")
        {
            var slug = Utility.TextUtility.Slug(title);
            var page = InstantiateIn(name, parentWeb, parentPageID, lang, slug);
            page.Title = title;
            page.Description = desc;
            page.Keywords = keywords;
            page.Save();
            return page;
        }
    }
}
