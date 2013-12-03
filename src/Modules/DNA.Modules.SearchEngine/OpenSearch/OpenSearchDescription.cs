//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.OpenSearch
{
    [Serializable]
    [XmlRoot("OpenSearchDescription", Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
    public class OpenSearchDescription
    {       
        private string description;
        private string shortName;
        private bool adultContent=false;
        private OpenSearchSyndicationRights syndicationRight = OpenSearchSyndicationRights.Open;

        /// <summary>
        /// Contains a brief human-readable title that identifies this search engine. 
        /// </summary>
        /// <remarks>
        /// Restrictions: The value must contain 16 or fewer characters of plain text. The value must not contain HTML or other markup.
        /// Requirements: This element must appear exactly once. 
        /// </remarks>
        [XmlElement("ShortName")]
        public string ShortName
        {
            get { return shortName; }
            set { shortName = value; }
        }

        /// <summary>
        /// Contains a human-readable text description of the search engine. 
        /// </summary>
        /// <remarks>
        /// Restrictions: The value must contain 1024 or fewer characters of plain text. The value must not contain HTML or other markup. 
        /// Requirements: This element must appear exactly once. 
        /// </remarks>
        [XmlElement("LongName")]
        public string LongName;

        /// <summary>
        /// Contains a human-readable text description of the search engine. 
        /// </summary>
        [XmlElement("Description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Contains a set of words that are used as keywords to identify and categorize this search content. Tags must be a single word and are delimited by the space character (' ')
        /// </summary>
        [XmlElement("Tags")]
        public string Tags;

        [XmlElement("Contact")]
        public string Contact;

        /// <summary>
        /// Contains the human-readable name or identifier of the creator or maintainer of the description document.
        /// </summary>
        [XmlElement("Developer")]
        public string Developer;

        /// <summary>
        /// Contains a value that indicates the degree to which the search results provided by this search engine can be queried, displayed, and redistributed.
        /// </summary>
        [XmlElement("SyndicationRight")]
        public OpenSearchSyndicationRights SyndicationRight
        {
            get { return syndicationRight; }
            set { syndicationRight = value; }
        }

        /// <summary>
        /// Contains a list of all sources or entities that should be credited for the content contained in the search feed.
        /// </summary>
        [XmlElement("Attribution")]
        public string Attribution;
        
        /// <summary>
        /// Contains a boolean value that should be set to true if the search results may contain material intended only for adults.
       /// </summary>
       /// <remarks>
       /// As there are no universally applicable guidelines as to what constitutes "adult" content, the search engine should make a good faith effort to indicate when there is a possibility that search results may contain material inappropriate for all audiences.
       /// </remarks>
        [XmlElement("AdultContent")]
        public bool AdultContent
        {
            get { return adultContent; }
            set { adultContent = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Language",Type=typeof(string))]
        public List<string> Languages;

        /// <summary>
        /// Contains a string that indicates that the search engine supports search requests encoded with the specified character encoding.
         /// </summary>
         /// <remarks>
         /// An OpenSearch description document should include one "InputEncoding" element for each character encoding that can be used to encode search requests. The "inputEncoding" template parameter in the OpenSearch URL template can be used to require the search client to identify which encoding is being used to encode the current search request. 
         /// </remarks>
        [XmlElement("InputEncoding", Type = typeof(string))]
        public List<string> InputEncodings;

        /// <summary>
        /// Contains a string that indicates that the search engine supports search responses encoded with the specified character encoding.
        /// </summary>
        /// <remarks>An OpenSearch description document should include one "OutputEncoding" element for each character encoding that can be used to encode search responses. The "outputEncoding" template parameter in the OpenSearch URL template can be used to allow the search client to choose a character encoding in the search response. </remarks>
        [XmlElement("OutputEncoding", Type = typeof(string))]
        public List<string> OutputEncodings;

        [XmlElement("Url", Type = typeof(OpenSearchUrl))]
        public List<OpenSearchUrl> Urls { get; set; }

        /// <summary>
        /// Contains a URL that identifies the location of an image that can be used in association with this search content.
        /// </summary>
        [XmlElement("Image", Type = typeof(OpenSearchImage))]
        public List<OpenSearchImage> Images { get; set; }

        /// <summary>
        /// Defines a search query that can be performed by search clients.
        /// </summary>
        [XmlElement("Query", Type = typeof(OpenSearchQuery))]
        public List<OpenSearchQuery> Queries { get; set; }
    }
}
