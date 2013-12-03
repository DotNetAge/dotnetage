//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DNA.OpenSearch
{
    [Serializable]
    public class OpenSearchSuggestion
    {
        public string SearchTerms { get; set; }

        public string Description { get; set; }

        public string QueryUrl { get; set; }
    }

    public class OpenSearchSuggestionComparer : IEqualityComparer<OpenSearchSuggestion>
    {
        public bool Equals(OpenSearchSuggestion x, OpenSearchSuggestion y)
        {
            if (x.SearchTerms.Equals(y.SearchTerms, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        public int GetHashCode(OpenSearchSuggestion obj)
        {
            return obj.ToString().ToLower().GetHashCode();
        }
    }
}
