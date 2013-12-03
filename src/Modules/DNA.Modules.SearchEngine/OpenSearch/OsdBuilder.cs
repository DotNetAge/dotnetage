//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.OpenSearch
{
    /// <summary>
    /// Provide the class to build the OpendSearchDocument object
    /// </summary>
    public class OsdBuilder
    {
        private OpenSearchDescription osd;

        public OsdBuilder(string shortName, string description)
        {
            if (string.IsNullOrEmpty(shortName))
                throw new ArgumentNullException("shortName");

            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");

            //if (shortName.Length > 16)
            //    throw new ArgumentOutOfRangeException("shortName", "shortName must be contain less then 16 characters.");

            //if (description.Length > 1024)
            //    throw new ArgumentOutOfRangeException("description", "shortName must be contain less then 1024 characters.");

            osd = new OpenSearchDescription()
            {
                ShortName = shortName,
                Description = description,
                Urls = new List<OpenSearchUrl>()
            };
        }

        /// <summary>
        /// Set contact information
        /// </summary>
        /// <param name="contact"></param>
        public void ContactInfo(string contact)
        {
            ContactInfo(contact, null);
        }

        /// <summary>
        /// Set contact information
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="developer"></param>
        public void ContactInfo(string contact, string developer)
        {
            if (!string.IsNullOrEmpty(osd.Contact))
                osd.Contact = contact;

            if (!string.IsNullOrEmpty(developer))
                osd.Developer = developer;
        }

        /// <summary>
        /// Set syndication rights.
        /// </summary>
        /// <param name="rights"></param>
        public void CopyRight(OpenSearchSyndicationRights rights)
        {
            osd.SyndicationRight = rights;
        }

        public void AddIcon(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Uri iconUrl = null;
                if (Uri.TryCreate(url, UriKind.Absolute, out iconUrl))
                    AddIcon(iconUrl);
            }
        }

        /// <summary>
        /// Add the icon url for OSD
        /// </summary>
        /// <param name="uri"></param>
        public void AddIcon(Uri uri)
        {
            this.AddImage(uri, 16, 16, "image/x-icon");
        }

        /// <summary>
        /// Add the image that can be used in association with this search content.
        /// </summary>
        /// <param name="uri"></param>
        public void AddImage(Uri uri, int? width = 64, int? height = 64, string type = "image/jpeg")
        {
            if (osd.Images == null)
                osd.Images = new List<OpenSearchImage>();

            osd.Images.Add(new OpenSearchImage()
            {
                Width = width.Value,
                Height = height.Value,
                Type = type,
                Url = uri.ToString()
            });
        }

        public void AddLanguage(string lan)
        {
            if (osd.Languages == null)
                osd.Languages = new List<string>();
            if (!osd.Languages.Contains(lan))
                osd.Languages.Add(lan);
        }

        public OpenSearchUrl AddSearchUrl(string template, string type = "application/opensearchdescription+xml",
            OpenSearchUrlRelValues? rel = OpenSearchUrlRelValues.Results)
        {
            if (string.IsNullOrEmpty(template))
                throw new ArgumentNullException(template);
            var _url = new OpenSearchUrl()
            {
                Type = type,
                Rel = rel.Value,
                Template = template
            };
            osd.Urls.Add(_url);
            return _url;
        }

        /// <summary>
        /// Add the self link url to support the OpenSearch Client plugin function
        /// </summary>
        /// <param name="osdUrl">Specified the OpenSearchDocument uri</param>
        public void EnableClientPlugin(string template)
        {
            osd.Urls.Add(new OpenSearchUrl()
            {
                Type = "application/opensearchdescription+xml",
                Rel = OpenSearchUrlRelValues.Self,
                Template = template
            });
        }

        /// <summary>
        /// Returns OpenSearchDescription object in xml format
        /// </summary>
        /// <returns></returns>
        public string GetXml()
        {
            var ser = new XmlSerializer(typeof(OpenSearchDescription));
            var xml = "<?xml version=\"1.0\">";
            using (var stream = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8))
                {
                    writer.Formatting = Formatting.None;
                    ser.Serialize(writer, osd);
                    writer.Flush();
                    stream.Position = 0;
                    var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
            }
            
            return xml;
        }

        /// <summary>
        /// Resturns the OpenSearchDescription result object.
        /// </summary>
        /// <returns></returns>
        public OpenSearchDescription GetOsd()
        {
            return osd;
        }
    }
}
