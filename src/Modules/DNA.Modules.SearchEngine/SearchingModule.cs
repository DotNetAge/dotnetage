
using DNA.Web;
using DNA.Web.ServiceModel;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA.Modules.Searching
{
    public class SearchingModule:ServiceModule
    {
        public override string Name
        {
            get { return "Searching"; }
        }

        public override bool DisableAutoRoutesRegistration
        {
            get
            {
                return true;
            }
        }

        public override void RegisterRoutes(RouteCollection routes)
        {
            var defaultLocale = App.Settings.DefaultLocale;
            routes.MapRoute("dna_opensearch", "{website}/{locale}/{search}/{source}",
                new { controller = "Search", action = "Search", source = "", locale = defaultLocale, website = "home" },
                new { search = "search", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_search_suggest", "{suggest}/{locale}/{source}",
                new { controller = "Search", action = "Suggest", locale = defaultLocale, source = "", website = "home" },
                new { suggest = "search-suggest", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });

            routes.MapRoute("dna_osd", "{osd}.{locale}.xml",
                new { controller = "Search", action = "OSD", locale = defaultLocale },
                new { osd = "osd", locale = @"([a-z]{2})-([a-z]{2}|[A-Z]{2})" });
        }

        public override void OnAppStart(System.Web.HttpApplication app)
        {
        }
    }
}
