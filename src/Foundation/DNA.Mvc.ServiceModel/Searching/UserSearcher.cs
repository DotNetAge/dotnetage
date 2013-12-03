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
    public class UserSearcher : ISearchSource
    {
        public string Name
        {
            get
            {
                return "Users";
            }

        }

        public string Title
        {
            get
            {
                return "Users";
            }

        }

        public bool SupportSuggestion
        {
            get { return true; }
        }

        public IEnumerable<SyndicationItem> Search(SearchQuery query)
        {
            var queryable = App.Get().DataContext.Where<UserProfile>(u => u.UserName.Contains(query.Terms)
                || (!string.IsNullOrEmpty(u.DisplayName) && (u.DisplayName.Contains(query.Terms)
                || u.Signature.Contains(query.Terms))));

            var skipCount = query.Index * query.Size;
            IQueryable<UserProfile> result = null;
            var size = 50;
            if (query.Size > 0)
                size = query.Size;

            if (skipCount > 0)
                result = queryable.Skip(skipCount).Take(size);
            else
                result = queryable.Take(size);

            query.TotalItems = queryable.Count();

            var searchResults = from u in result
                                orderby u.DisplayName
                                select new SyndicationItem(u.DisplayName, u.Signature, new Uri(u.Link));

            return searchResults.ToList();
            //return result.OrderBy(u => u.DisplayName).ToList().Select(u => new SearchResultItem()
            //{
            //    Title = u.DisplayName,
            //    Author = u.UserName,
            //    ImageUrl = u.Avatar,
            //    Link = u.Link,
            //    Source = this.Name,
            //    Summary = u.Signature
            //});

        }

        public string[] GetSuggests(string terms, string locale, int returns = 50)
        {
            var queryable = App.Get().DataContext.Where<UserProfile>(u => u.UserName.Contains(terms) || (!string.IsNullOrEmpty(u.DisplayName) && (u.DisplayName.Contains(terms) || u.Signature.Contains(terms))));
            return queryable.OrderBy(q => q.UserName).Select(q => q.UserName).Take(returns).ToArray();
        }
    }
}
