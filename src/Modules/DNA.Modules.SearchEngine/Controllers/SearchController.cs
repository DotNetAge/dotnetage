//  Copyright (c) 2011 Ray Liang (http://www.dotnetage.com)
//  Dual licensed under the MIT and GPL licenses:
//  http://www.opensource.org/licenses/mit-license.php
//  http://www.gnu.org/licenses/gpl.html

using DNA.OpenSearch;
using DNA.Web.ServiceModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class SearchController : Controller
    {
        [SiteMapAction(Title = "Search",
            ShowInMenu = false,
            IsShared = true,
            IgnoreRouteDataKeys = new string[] { "terms", "source", "locale", "index", "size", "format" })]
       // [Loc]
        public ActionResult Search(string terms, string source, string locale, int index = 1, int size = 20, string format = "")
        {
            //App.Get().SetCulture(locale);
            var src = string.IsNullOrEmpty(source) ? App.Get().Searcher.Sources.First().Name : source;
            if (terms == null)
            {
                ViewBag.Query = new SearchQuery()
                {
                    Source = src,
                    Locale = locale
                };
                return View();
            }

            var searchQuery = new SearchQuery()
            {
                Index = index - 1,
                Size = size,
                Source = src,
                Terms = terms,
                Locale = locale
            };

            var model = App.Get().Searcher.Search(searchQuery);
            var feed = new SyndicationFeed(string.Format("Search for {0}", terms), src, Request.Url, model);
            feed.Generator = "DotNetAge";

            if (format.Equals("rss", System.StringComparison.OrdinalIgnoreCase) ||
                format.Equals("atom", System.StringComparison.OrdinalIgnoreCase))
            {
                var sb = new StringBuilder();
                using (var writer = System.Xml.XmlWriter.Create(sb))
                {
                    if (format.Equals("rss", System.StringComparison.OrdinalIgnoreCase))
                    {
                        feed.SaveAsRss20(writer);
                        writer.Flush();
                        return Content(sb.ToString(), "text/xml", Encoding.UTF8);
                    }

                    if (format.Equals("atom", System.StringComparison.OrdinalIgnoreCase))
                    {
                        feed.SaveAsAtom10(writer);
                        writer.Flush();
                        return Content(sb.ToString(), "text/xml", Encoding.UTF8);
                    }
                }
            }
            ViewBag.Query = searchQuery;

            return Json(feed,JsonRequestBehavior.AllowGet);
        }

        public JsonResult Suggest(string term, string locale, string source = "", int count = 10)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var result = App.Get().Searcher.Suggest(term, locale, source, count);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Osd()
        {
            var web = App.Get().CurrentWeb;  
            var domain = Request.Url.Authority;

            var osdBuilder = new OsdBuilder(string.IsNullOrEmpty(web.Title) ? domain : web.Title, string.IsNullOrEmpty(web.Description) ? ("Search " + domain + " content") : web.Description);
            osdBuilder.AddIcon(web.ShortcutIconUrl);

            string baseUrl = Request.Url.Scheme + "://" + domain + Request.ApplicationPath + (Request.ApplicationPath.EndsWith("/") ? "" : "/") + web.Name + "/" + web.DefaultLocale;
            osdBuilder.AddSearchUrl(baseUrl + "/search?terms={searchTerms}", "text/html");

            //Add suggession
            osdBuilder.AddSearchUrl(baseUrl + "/search-suggest?terms={searchTerms}", "application/x-suggestions+json");
            osdBuilder.EnableClientPlugin(baseUrl + "/osd.xml");
            return Content(osdBuilder.GetXml(), "text/xml");
        }
    }
}
