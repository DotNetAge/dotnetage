//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.ServiceModel.Syndication;
using System.Xml;

namespace DNA.Web.ServiceModel.Syndication
{
    /// <summary>
    /// Represent the syndication item that use to support the itune podcast feed item.
    /// </summary>
    public class ItunesItem : SyndicationItem
    {
        private string @namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private string prefix = "itunes";

        /// <summary>
        /// Gets/Sets the feed item sub title.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets/Sets the feed item author name.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets/Sets the duration.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Gets/Sets the feed item keywords.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Indicates whether the feed object is explicit.
        /// </summary>
        public bool Explicit { get; set; }

        /// <summary>
        /// Gets/Sets the feed item image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Initializes a new instance of the ItunesItem class
        /// </summary>
        public ItunesItem() { }

        /// <summary>
        /// Initializes a new instance of the ItunesItem class with the specified System.ServiceModel.Syndication.SyndicationItem instance.
        /// </summary>
        /// <param name="source">
        /// A ItunesItem instance used to initialize the new System.ServiceModel.Syndication.SyndicationItem instance.
        /// </param>
        protected ItunesItem(SyndicationItem source) : base(source) { }

        /// <summary>
        ///  Initializes a new instance of the ItunesItem class with the specified title, content, and link.
        /// </summary>
        /// <param name="title">The item title.</param>
        /// <param name="content"> The item content.</param>
        /// <param name="itemAlternateLink">The item URL.</param>
        public ItunesItem(string title, string content, Uri itemAlternateLink) : base(title, content, itemAlternateLink) { }

        /// <summary>
        ///  Initializes a new instance of the ItunesItem class with the specified title, content, and link.
        /// </summary>
        /// <param name="title">The item title.</param>
        /// <param name="content"> The item content.</param>
        /// <param name="itemAlternateLink">The item URL.</param>
        /// <param name="id">The ID of the syndication item.</param>
        /// <param name="lastUpdatedTime">The System.DateTimeOffset that contains the last time the syndication item  was last updated.</param>
        public ItunesItem(string title, string content, Uri itemAlternateLink, string id, DateTimeOffset lastUpdatedTime)
            : base(title, content, itemAlternateLink, id, lastUpdatedTime) { }

        /// <summary>
        ///  Initializes a new instance of the ItunesItem class.
        /// </summary>
        /// <param name="title">The item title.</param>
        /// <param name="content">
        /// A System.ServiceModel.Syndication.SyndicationContent instance containing the content of the syndication item.
        /// </param>
        /// <param name="itemAlternateLink">The item URL.</param>
        /// <param name="id">The ID of the syndication item.</param>
        /// <param name="lastUpdatedTime">The System.DateTimeOffset that contains the last time the syndication item  was last updated.</param>
        public ItunesItem(string title, SyndicationContent content, Uri itemAlternateLink, string id, DateTimeOffset lastUpdatedTime)
            : base(title, content, itemAlternateLink, id, lastUpdatedTime) { }

        protected override void WriteElementExtensions(XmlWriter writer, string version)
        {
            WriteItunesElement(writer, "subtitle", Subtitle);
            WriteItunesElement(writer, "author", Author);
            WriteItunesElement(writer, "summary", Summary.Text);
            WriteItunesElement(writer, "duration", Duration);
            WriteItunesElement(writer, "keywords", Keywords);
            WriteItunesElement(writer, "explicit", Explicit ? "yes" : "no");
            if (!string.IsNullOrEmpty(ImageUrl))
            {
                writer.WriteStartElement(prefix, "image", @namespace);
                writer.WriteAttributeString("href", ImageUrl);
                writer.WriteEndElement();
            }
        }

        private void WriteItunesElement(XmlWriter writer, string name, string value)
        {
            if (value != null)
            {
                writer.WriteStartElement(prefix, name, @namespace);
                writer.WriteValue(value);
                writer.WriteEndElement();
            }
        }
    }
}
