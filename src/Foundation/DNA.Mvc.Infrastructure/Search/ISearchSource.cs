//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace DNA.Web
{
    /// <summary>
    /// Defines a search source methods.
    /// </summary>
    public interface ISearchSource
    {
        /// <summary>
        /// Gets/Sets the search source name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets/Sets the search source display text
        /// </summary>
        string Title { get; }

        //string ResTitleKey { get; set; }

        /// <summary>
        /// Gets the search source whether support get search suggests.
        /// </summary>
        bool SupportSuggestion { get; }

        /// <summary>
        /// Get search result by specified SearchQuery object.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<SyndicationItem> Search(SearchQuery query);

        /// <summary>
        /// Gets search suggests.
        /// </summary>
        /// <param name="terms">The search terms</param>
        /// <param name="returns">The returns count.</param>
        /// <returns></returns>
        string[] GetSuggests(string terms, string locale, int returns = 50);
    }
}
