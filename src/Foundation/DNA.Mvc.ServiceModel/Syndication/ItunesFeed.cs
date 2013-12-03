//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace DNA.Web.ServiceModel.Syndication
{
    /// <summary>
    /// Represets the iTunes feed object to support the itune podcast rss.
    /// </summary>
    public class ItunesFeed : SyndicationFeed
    {
        private string @namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private string prefix = "itunes";

        /// <summary>
        /// Gets/Sets the subtitle of the feed.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets / Sets the author name.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets/Sets the feed summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets/Sets the owner name.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// Gets/Sets the owner email.
        /// </summary>
        public string OwnerEmail { get; set; }

        /// <summary>
        /// Indicates whether the feed object is explicit.
        /// </summary>
        public bool Explicit { get; set; }

        /// <summary>
        /// Gets/Sets the category collection
        /// </summary>
        public List<List<string>> ItunesCategories = new List<List<string>>();

        protected override void WriteAttributeExtensions(XmlWriter writer, string version)
        {
            writer.WriteAttributeString("xmlns", prefix, null, @namespace);
        }

        protected override void WriteElementExtensions(XmlWriter writer, string version)
        {
            WriteItunesElement(writer, "subtitle", Subtitle);
            WriteItunesElement(writer, "author", Author);
            WriteItunesElement(writer, "summary", Summary);
            if (ImageUrl != null)
            {
                WriteItunesElement(writer, "image", ImageUrl.ToString());
            }
            WriteItunesElement(writer, "explicit", Explicit ? "yes" : "no");

            writer.WriteStartElement(prefix, "owner", @namespace);
            WriteItunesElement(writer, "name", OwnerName);
            WriteItunesElement(writer, "email", OwnerEmail);
            writer.WriteEndElement();

            foreach (var category in ItunesCategories)
            {
                writer.WriteStartElement(prefix, "category", @namespace);
                writer.WriteAttributeString("text", category[0]);
                if (category.Count == 2)
                {
                    writer.WriteStartElement(prefix, "category", @namespace);
                    writer.WriteAttributeString("text", category[1]);
                    writer.WriteEndElement();
                }
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
