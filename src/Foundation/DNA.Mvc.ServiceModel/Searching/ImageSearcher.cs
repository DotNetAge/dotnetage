//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace DNA.Web.Searching
{
    public class ImageSearcher : ISearchSource
    {
        public string Name { get { return "images"; } }

        public string Title { get { return "Images"; } }

        public bool SupportSuggestion
        {
            get { return true; }
        }

        public IEnumerable<SyndicationItem> Search(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        public string[] GetSuggests(string terms, string locale, int returns = 50)
        {
            throw new NotImplementedException();
        }
    }
}
