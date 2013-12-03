//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;

namespace DNA.Web
{
    public class Searcher
    {
        public Searcher()
        {
            var sources = DependencyResolver.Current.GetServices<ISearchSource>();
            Sources = new SearchSourceCollection(sources);
        }

        public IEnumerable<SyndicationItem> Search(SearchQuery query)
        {
            var src = string.IsNullOrEmpty(query.Source) ? Sources.FirstOrDefault() : Sources[query.Source];

            if (src != null)
            {
                var items = src.Search(query);
                query.TotalItems = items.Count();
                return items;
            }
            return null;
        }

        public string[] Suggest(string terms, string locale, string source, int returns = 10)
        {
            return Sources[source].GetSuggests(terms, locale, returns);
        }

        public SearchSourceCollection Sources { get; private set; }
    }
}
