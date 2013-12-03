//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.OpenSearch
{
    /// <summary>
    /// Defines a search query that can be performed by search clients. Please see the OpenSearch Query element specification for more information.
    /// </summary>
    /// <remarks>
    /// OpenSearch description documents should include at least one Query element of role="example" that is expected to return search results. Search clients may use this example query to validate that the search engine is working properly.
    /// </remarks>
    [XmlRoot("Query"),Serializable]
    public class OpenSearchQuery
    {
        /// <summary>
        /// Contains a string identifying how the search client should interpret the search request defined by this Query element
        /// </summary>
        [XmlAttribute("role")]
        public string Role { get; set; }

        /// <summary>
        /// Replaced with the keyword or keywords desired by the search client.
        /// </summary>
        [XmlAttribute("searchTerms")]
        public string SearchTerms;

        /// <summary>
        /// Replaced with the index of the first search result desired by the search client.
        /// </summary>
        [XmlAttribute("startIndex")]
        public int StartIndex;

        /// <summary>
        /// Replaced with the page number of the set of search results desired by the search client. 
        /// </summary>
        [XmlAttribute("startPage")]
        public int StartPage;

        /// <summary>
        /// Replaced with the number of search results per page desired by the search client.
        /// </summary>
        [XmlAttribute("count")]
        public int Count;

        /// <summary>
        /// Replaced with a string that indicates that the search client desires search results in the specified language. 
        /// </summary>
        [XmlAttribute("language")]
        public string Language;

        /// <summary>
        /// Replaced with a string that indicates that the search client is performing the search request encoded with the specified character encoding.
        /// </summary>
        [XmlAttribute("inputEncoding")]
        public string InputEncoding;

        /// <summary>
        /// Replaced with a string that indicates that the search client desires a search response encoding with the specified character encoding.
        /// </summary>
        [XmlAttribute("outputEncoding")]
        public string OutputEncoding;

        /// <summary>
        /// Contains the expected number of results to be found if the search request were made. 
        /// </summary>
         [XmlAttribute("totalResults")]
        public int TotalResults;

        /// <summary>
         /// Contains a human-readable plain text string describing the search request
        /// </summary>
        [XmlAttribute("title")]
         public string Title;

       
    }
}
