//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;

namespace DNA.Web.Searching
{
    public class WebPageSearcher : ISearchSource
    {
        public string Name { get { return "pages"; } }

        public string Title { get { return "Pages"; } }

        public bool SupportSuggestion { get { return true; } }

        public IEnumerable<SyndicationItem> Search(SearchQuery query)
        {
            var terms = query.Terms;
            var queryable = App.Get().DataContext.Where<WebPage>(p => (!string.IsNullOrEmpty(p.Title) && (p.Title.Contains(terms))) || (!string.IsNullOrEmpty(p.Description) && (p.Description.Contains(terms)) || (!string.IsNullOrEmpty(p.Keywords) && (p.Keywords.Contains(terms)))));
            //var queryable = App.Get().DataContext.Where<WebPage>(p =>p.Title.Contains(terms) || p.Description.Contains(terms) || p.Keywords.Contains(terms));
            var skipCount = query.Index * query.Size;
            IQueryable<WebPage> result = null;
            var size = 50;
            if (query.Size > 0)
                size = query.Size;

            if (skipCount > 0)
                result = queryable.Skip(skipCount).Take(size);
            else
                result = queryable.Take(size);

            query.TotalItems = queryable.Count();

            //var searchResults = from u in result
            //                    orderby u.Title
            //                    select new SyndicationItem(u.Title, u.Description, new Uri(App.Get().Wrap(u).Url));

            //return searchResults.ToList();

            return result.OrderBy(u => u.Title).ToList().Select(u =>
           {
               var wrapper = App.Get().Wrap(u);
               return new SyndicationItem(u.Title, u.Description, new Uri(App.Get().Wrap(u).Url));
               //    var ri = new SyndicationItem(u.Title,u.Description,new Uri(wrapper.Url));
               //    ri.Authors.Add(new SyndicationPerson(
               //    //{
               //    //    Title = u.Title,
               //    //    Author = u.Owner,
               //    //    ImageUrl = u.ImageUrl,
               //    //    Link = wrapper.Url,
               //    //    Source = this.Name,
               //    //    Summary = u.Description
               //    //};
               //    return ri;
           });
        }

        public string[] GetSuggests(string terms, string locale, int returns = 50)
        {
            var pages = App.Get().DataContext.Where<WebPage>(p => p.Locale.Equals(locale) && ((!string.IsNullOrEmpty(p.Title) && (p.Title.Contains(terms))) || (!string.IsNullOrEmpty(p.Description) && (p.Description.Contains(terms)))));
            var finalResults = pages.Where(p => !string.IsNullOrEmpty(p.Title)).OrderBy(p => p.Title).Select(p => p.Title).Take(returns);
            return finalResults.ToArray();
        }
    }
}
