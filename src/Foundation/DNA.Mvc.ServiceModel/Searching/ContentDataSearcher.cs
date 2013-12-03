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
    public class ContentDataSearcher : ISearchSource
    {
        public string Name { get { return "Contents"; } }

        public string Title { get { return "Contents"; } }

        public bool SupportSuggestion
        {
            get { return true; }
        }

        public IEnumerable<SyndicationItem> Search(SearchQuery query)
        {
            var terms = query.Terms;
            var queryable = App.Get().DataContext.Where<ContentDataItem>(p => p.Locale.Equals(query.Locale)).ToList().Where(i => i.RawData.Contains(query.Terms));
            IEnumerable<ContentDataItem> result = null;// queryable.ToList();

            var skipCount = query.Index * query.Size;
            var size = 50;
            if (query.Size > 0)
                size = query.Size;

            if (skipCount > 0)
                result = queryable.Skip(skipCount).Take(size);
            else
                result = queryable.Take(size);

            query.TotalItems = queryable.Count();

            return result.OrderBy(u => u.Modified).ToList().Select(u =>
            {
                var wrapper = App.Get().Wrap(u);
                var firstNoteField = wrapper.Parent.Fields.FirstOrDefault(f => f.FieldType.Equals((int)ContentFieldTypes.Note));
                var firstImgField = wrapper.Parent.Fields.FirstOrDefault(f => f.FieldType.Equals((int)ContentFieldTypes.Image));
                var imgUrl = "";
                var summary = "";
                if (firstImgField != null)
                    imgUrl = wrapper.Value(firstImgField.Name).Formatted;

                if (firstNoteField != null)
                    summary = wrapper.Value(firstNoteField.Name).Formatted;
                return new SyndicationItem(wrapper.GetDefaultTitleValue(), summary, new Uri(wrapper.UrlComponent));

                //var ri = new SearchResultItem()
                //{
                //    Title =wrapper.GetDefaultTitleValue(),
                //    Author = u.Owner,
                //    ImageUrl =imgUrl, 
                //    Link = wrapper.Url,
                //    Source = this.Name,
                //    Summary =summary
                //};
                //return ri;
            });
        }

        public string[] GetSuggests(string terms, string locale, int returns = 10)
        {
            var queryable = App.Get().DataContext.Where<ContentDataItem>(p => p.Locale.Equals(locale)).ToList().Where(i => i.RawData.Contains(terms));
            IEnumerable<ContentDataItem> result = null;
            result = queryable.Take(returns);
            return result.OrderBy(u => u.Modified).ToList().Select(u =>
            {
                var wrapper = App.Get().Wrap(u);
                return wrapper.GetDefaultTitleValue();
            }).ToArray();
        }
    }
}
