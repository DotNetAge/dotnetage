//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml;

namespace DNA.Web
{
    /// <summary>
    /// Reparesent the snapshot of the installed widget
    /// </summary>
    public class WidgetDescriptor
    {
        private List<Dictionary<string, object>> properties = null;
        private XmlDocument configDoc = null;

        /// <summary>
        /// Gets/Sets the ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the unique id of the widget.
        /// </summary>
        /// <remarks>
        ///  The uid is generate from dotnetage.com gallery the format is : 
        ///  http://www.dotnetage.com/gallery/widgets/{uid}
        /// 
        /// If the widget not publish to gallery yet the uid 's format is a guid string
        /// </remarks>
        public string UID { get; set; }

        /// <summary>
        /// Gets the widget type
        /// </summary>
        public int WidgetType { get; set; }

        /// <summary>
        /// Gets the widget action name
        /// </summary>
        public string Action { get; set; }

        public string Area { get; set; }

        /// <summary>
        /// Gets the controller full name
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Get the controller short name
        /// </summary>
        public string ControllerShortName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Controller))
                {
                    var type = Type.GetType(this.Controller);
                    if (type != null)
                        return type.Name.Replace("Controller", "");
                }
                return this.Controller;
            }
        }

        /// <summary>
        /// Gets the content type
        /// </summary>
        /// <remarks>
        /// application/razor
        /// </remarks>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets/Sets the content encoding name.
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Gets/Sets the content body.
        /// </summary>
        public string ContentText { get; set; }

        /// <summary>
        /// Gets/Sets the widget author name.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets/Sets the widget author email.
        /// </summary>
        public string AuthorEmail { get; set; }

        /// <summary>
        /// Gets/Sets author home page url.
        /// </summary>
        public string AuthorHomePage { get; set; }

        /// <summary>
        /// Gets/Sets the widget content direction.
        /// </summary>
        public string ContentDirection { get; set; }

        /// <summary>
        /// Gets/Sets the default width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets/Sets the default height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets/Sets the widget view modes supports
        /// </summary>
        public string ViewModes { get; set; }

        /// <summary>
        /// Gets the default content url
        /// </summary>
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets / Sets the default locale
        /// </summary>
        public string DefaultLocale { get; set; }

        /// <summary>
        /// Gets/Sets the support locales of this widget
        /// </summary>
        public string Locales { get; set; }

        /// <summary>
        /// The original config.xml text
        /// </summary>
        public string ConfigXml { get; set; }

        /// <summary>
        /// Get the Json string of the default values.
        /// </summary>
        public string Defaults { get; set; }

        /// <summary>
        /// The shortname of the widget
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The full name of the widget
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the widget description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the default icon url
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets/Sets the widget installed path when the widget register
        /// </summary>
        public string InstalledPath { get; set; }

        /// <summary>
        /// Gets/Sets the widget version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Identity the widget has signuature
        /// </summary>
        public string RSAKey { get; set; }

        /// <summary>
        /// Gets the widget registered date
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Indicates whether the widget can be cached.
        /// </summary>
        public bool CacheEnabled { get; set; }

        /// <summary>
        /// Get the widgets that using this descrioptor
        /// </summary>
        public virtual ICollection<WidgetInstance> Widgets { get; set; }

        /// <summary>
        /// Gets/Sets the default user preferences.
        /// </summary>
        public List<Dictionary<string, object>> Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new List<Dictionary<string, object>>();
                    try
                    {
                        if (!string.IsNullOrEmpty(this.Defaults))
                            properties = (new JavaScriptSerializer()).Deserialize<List<Dictionary<string, object>>>(this.Defaults);
                    }
                    catch {
                        return properties;
                    }
                }
                return properties;
            }
            set
            {
                if (value != null)
                    this.Defaults = (new JavaScriptSerializer()).Serialize(value);
                else
                    this.Defaults = "{}";
            }
        }

        /// <summary>
        /// Gets/Sets the access roles.
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }

        NameTable nt = new NameTable();
        XmlNamespaceManager nsmgr = null;

        private XmlDocument ConfigDocument
        {
            get
            {
                if (configDoc == null)
                {
                    nsmgr = new XmlNamespaceManager(nt);
                    nsmgr.AddNamespace("w", "http://www.w3.org/ns/widgets");
                    configDoc = new XmlDocument();
                    configDoc.LoadXml(ConfigXml);
                }

                return configDoc;
            }
        }

        /// <summary>
        /// Gets the locale object by name.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public LocaleName LocaleName(string lang)
        {
            var name = ConfigDocument.SelectSingleNode("//w:name[lang(\"" + lang + "\")]", nsmgr);

            if (name != null)
            {
                if (name.Attributes["short"] != null && !string.IsNullOrEmpty(name.Attributes["short"].Value))
                {
                    return new LocaleName
                    {
                        ShortName = name.Attributes["short"].Value,
                        FullName = name.InnerText,
                        Dir = name.Attributes["ltr"] != null ? name.Attributes["ltr"].Value : "ltr"
                    };
                }
            }

            return new LocaleName
            {
                ShortName = Name,
                FullName = Title
            };
        }

        public string LocaleDesc(string lang)
        {
            var desc = ConfigDocument.SelectSingleNode("//w:description[lang(\"" + lang + "\")]", nsmgr);
            if (desc != null)
                return desc.InnerText;
            return Description;
        }

        public LocaleContent LocaleContent(string lang)
        {
            var content = ConfigDocument.SelectSingleNode("//w:content[lang(\"" + lang + "\")]", nsmgr);
            if (content != null)
            {
                var locContent = new LocaleContent
                {
                    Dir = content.Attributes["ltr"] != null ? content.Attributes["ltr"].Value : "ltr",
                    Type = content.Attributes["type"] != null ? content.Attributes["type"].Value : "text/html",
                    Url = content.Attributes["src"] != null ? content.Attributes["src"].Value : "",
                    Encoding = content.Attributes["encoding"] != null ? content.Attributes["encoding"].Value : "utf-8",
                    Text = content.InnerText
                };

                if (!string.IsNullOrEmpty(locContent.Url))
                    ResolveUri(locContent.Url, lang: lang);

                return locContent;
            }

            return new LocaleContent
            {
                Type = this.ContentType,
                Url = !string.IsNullOrEmpty(this.ContentUrl) ? ResolveUri(this.ContentUrl) : "",
                Encoding = this.Encoding,
                Text = this.ContentText,
                Dir = this.ContentDirection
            };
        }

        /// <summary>
        /// Resolve the relative file name an returns phical file path.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="relativeFileName"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public string ResolveFileName(string basePath, string relativeFileName, string lang = "")
        {
            var absolutePath = basePath + (basePath.EndsWith("\\") ? "" : "\\") + InstalledPath + "\\";

            if (!relativeFileName.StartsWith("http://") && !relativeFileName.StartsWith("ftp://") && !relativeFileName.StartsWith("https://"))
            {
                if (!string.IsNullOrEmpty(lang) && !this.DefaultLocale.Equals(lang,StringComparison.OrdinalIgnoreCase))
                {
                    var supports = string.IsNullOrEmpty(this.Locales) ? Locales.Split(',') : new string[0];
                    if (supports.Length > 0 && supports.Contains(lang))
                        return absolutePath + "locales\\" + lang + "\\" + relativeFileName;
                }
                return absolutePath + relativeFileName;
            }
            return relativeFileName;
        }

        public string ResolveUri(string relativeFileName, string appPath = "~/content/widgets/", string lang = "")
        {
            var relativePath = (appPath.EndsWith("/") ? appPath : (appPath + "/")) + InstalledPath.Replace("\\", "/") + "/";

            if (!relativeFileName.StartsWith("http://") && !relativeFileName.StartsWith("ftp://") && !relativeFileName.StartsWith("https://") && !relativeFileName.StartsWith("~"))
            {
                if (!string.IsNullOrEmpty(lang) && !this.DefaultLocale.Equals(lang, StringComparison.OrdinalIgnoreCase))
                {
                    var supports = string.IsNullOrEmpty(this.Locales) ? Locales.Split(',') : new string[0];
                    if (supports.Length > 0 && supports.Contains(lang))
                        return relativePath + "locales/" + lang + "/" + relativeFileName;
                }
                return relativePath + relativeFileName;
            }
            return relativeFileName;
        }
    }

    public struct LocaleContent
    {
        public string Text;
        public string Type;
        public string Encoding;
        public string Url;
        public string Dir;
    }

    public struct LocaleName
    {
        public string ShortName;
        public string FullName;
        public string Dir;
    }


}
