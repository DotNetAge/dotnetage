//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Sitemap;
using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace DNA.Web.Controllers
{
    public class SyndicationController : Controller
    {
        [OutputCache(Duration = 36000, VaryByParam = "*")]
        public ActionResult Sitemap()
        {
            var app = App.Get();
            string sitemapPath = Server.MapPath("~/app_data/files/public/sitemaps/");

            if (!Directory.Exists(sitemapPath))
                Directory.CreateDirectory(sitemapPath);

            var sitemapIndex = new SitemapIndex();
            sitemapIndex.SiteMapFiles = new System.Collections.Generic.List<Sitemap.Sitemap>();

            foreach (var web in app.Webs)
            {
                var _recreate = false;
                var sitemap = new Sitemap.Sitemap();

                if (System.IO.File.Exists(sitemapPath + web.Name + ".xml"))
                {
                    var fileInfo = new FileInfo(sitemapPath + web.Name + ".xml");
                    if ((DateTime.Now - fileInfo.CreationTime).Days == 0)
                        sitemap.LastModified = fileInfo.CreationTime.ToString("yyyy-MM-dd");
                    else
                        _recreate = true;
                }
                else
                    _recreate = true;

                if (_recreate)
                {
                    var _pageUrlset = new SitemapUrlset() { Urls = new List<SitemapUrl>() };
                    var pages = web.Pages.Where(p => p.ShowInSitemap && p.AllowAnonymous).OrderByDescending(p => p.LastModified);

                    foreach (var page in pages)
                    {
                        if (page.IsShared)
                            continue;

                        _pageUrlset.Urls.Add(new SitemapUrl()
                        {
                            Url = page.FullUrl,
                            LastModified = page.LastModified.ToString("yyyy-MM-dd"),
                            Priority = 0.5M,
                            ChangeFrequently = SitemapChangeFrequently.Weekly
                        });
                    }
                    sitemap.LastModified = DateTime.Now.ToString("yyyy-MM-dd");
                    DNA.Utility.XmlSerializerUtility.SerializeToXmlFile(sitemapPath + web.Name + ".xml", typeof(SitemapUrlset), _pageUrlset);
                }
                
                BuildListSiteMap(sitemapIndex, sitemapPath, web.Lists);

                sitemap.Location = app.Context.AppUrl.ToString() + "webshared/home/sitemaps/" + web.Name + ".xml";
                sitemapIndex.SiteMapFiles.Add(sitemap);
            }

            if (sitemapIndex.SiteMapFiles.Count == 1)
            {
                var sitemapFile = app.NetDrive.MapPath(new Uri(sitemapIndex.SiteMapFiles.FirstOrDefault().Location));
                return File(sitemapFile, "text/xml");
            }
            else
            {
                var xml = DNA.Utility.XmlSerializerUtility.SerializeToXml(sitemapIndex);
                return Content(xml, "text/xml");
            }
        }

        private void BuildListSiteMap(SitemapIndex index, string sitemapPath, IEnumerable<ContentListDecorator> lists)
        {
            foreach (var list in lists)
            {
                var _recreate = false;
                var sitemap = new Sitemap.Sitemap();
                var fileName = list.Web.Name + "-" + list.Name + "." + list.Locale.ToLower();
                if (System.IO.File.Exists(sitemapPath + fileName+ ".xml"))
                {
                    var fileInfo = new FileInfo(sitemapPath + fileName + ".xml");
                    if ((list.LastModified - fileInfo.CreationTime).Days == 0)
                        sitemap.LastModified = fileInfo.CreationTime.ToString("yyyy-MM-dd");
                    else
                        _recreate = true;
                }
                else
                    _recreate = true;

                if (_recreate)
                {
                    var _pageUrlset = new SitemapUrlset() { Urls = new List<SitemapUrl>() };
                    //var pages = web.Pages.Where(p => p.ShowInSitemap && p.AllowAnonymous).OrderByDescending(p => p.LastModified);
                    var approved=(int)ModerateStates.Approved;
                    var notset=(int)ModerateStates.Notset;
                    var items = list.Items.Where(i=>i.IsCurrentVersion && i.IsPublished && ( i.ModerateState==approved || i.ModerateState==notset) && i.ShareID==Guid.Empty);
                    
                    if (items.Count() == 0)
                        continue;

                    foreach (var item in items)
                    {
                        var itemWrapper = App.Get().Wrap(item);
                        _pageUrlset.Urls.Add(new SitemapUrl()
                        {
                            Url = itemWrapper.UrlComponent,
                            LastModified =itemWrapper.Published.Value.ToString("yyyy-MM-dd"),
                            Priority = 0.5M,
                            ChangeFrequently = SitemapChangeFrequently.Weekly
                        });
                    }
                    sitemap.LastModified =list.LastModified.ToString("yyyy-MM-dd");
                    DNA.Utility.XmlSerializerUtility.SerializeToXmlFile(sitemapPath +fileName + ".xml", typeof(SitemapUrlset), _pageUrlset);
                }

                sitemap.Location = App.Get().Context.AppUrl.ToString() + "webshared/home/sitemaps/" +fileName+ ".xml";
                index.SiteMapFiles.Add(sitemap);
            }
        }

        [OutputCache(Duration = 1200)]
        public ActionResult Read(string uri)
        {
            using (var reader = XmlReader.Create(Server.UrlDecode(uri)))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                return Json(feed, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
