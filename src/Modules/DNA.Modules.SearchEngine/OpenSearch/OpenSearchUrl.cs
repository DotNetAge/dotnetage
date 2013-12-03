//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.OpenSearch
{
    /// <summary>
    /// Describes an interface by which a client can make requests for an external resource, such as search results, search suggestions, or additional description documents.
    /// </summary>
    [XmlRoot("Url")]
    [Serializable]
    public class OpenSearchUrl
    {
        private int indexOffset = 1;
        private int pageOffset = 1;
        private OpenSearchUrlRelValues relation = OpenSearchUrlRelValues.Results;

        /// <summary>
        /// Describes an interface by which a client can make requests for an external resource, such as search results, search suggestions, or additional description documents.
        /// </summary>
        /// <remarks>
        /// The URL template to be processed according to the OpenSearch URL template syntax. 
        /// </remarks>
        [XmlAttribute("template")]
        public string Template { get; set; }

        /// <summary>
        /// The MIME type of the resource being described
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The role of the resource being described in relation to the description document. 
        /// </summary>
        [XmlAttribute("rel")]
        public OpenSearchUrlRelValues Rel
        {
            get { return relation; }
            set { relation = value; }
        }

        /// <summary>
        /// The page number of the first set of search results. 
        /// </summary>
        [XmlAttribute("pageOffset")]
        public int PageOffset
        {
            get { return pageOffset; }
            set { pageOffset = value; }
        }

        /// <summary>
        /// The index number of the first search result. 
        /// </summary>
        [XmlAttribute("indexOffset")]
        public int IndexOffset
        {
            get { return indexOffset; }
            set { indexOffset = value; }
        }

    }

    /// <summary>
    /// Rel attribute strings can contain a space-delimited list of one or more semantically meaningful rel value tokens. An empty rel attribute value should be treated by the client as if the rel attribute was not present at all.
    /// If a client does not recognize the semantic meaning of any rel value token, then the containing Url should be ignored by the client.
    /// Rel value tokens may be either fully qualified tokens (e.g., "http://example.com/rel#foo") or unqualified tokens (e.g., "results").
    /// All fully qualified tokens must be a valid URL. The semantic meaning of any fully qualified token is outside the scope of this specification, but convention dictates that the URL should resolve to a resource that describes the relationship.
    /// All unqualified tokens must be a lowercase alphanumeric string of the form [a-z][a-z\-]+. Only those tokens listed below have meaning defined in this specification
    /// </summary>
    public enum OpenSearchUrlRelValues:short
    {
        /// <summary>
        /// Represents a request for search results in the specified format.
        /// </summary>
        [XmlEnum("results")]
        Results=1,

        /// <summary>
        /// Represents a request for search suggestions in the specified format. See the OpenSearch Suggestions extension for further details. 
        /// </summary>
        [XmlEnum("suggestions")]
        Suggestions =2,

        /// <summary>
        /// Represents the canonical URL of this description document. 
        /// </summary>
        [XmlEnum("self")]
        Self=3,

        /// <summary>
        /// Represents a request for a set of resources. 
        /// </summary>
        [XmlEnum("collection")]
        Collection=4
    }
}
