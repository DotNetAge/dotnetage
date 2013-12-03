//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace DNA.Xml.Widgets
{
    /// <summary>
    /// This is the root element and serves as a container for the other elements of the configuration document. 
    /// </summary>

    [
    GeneratedCodeAttribute("xsd", "4.0.30319.1"),
    DesignerCategory("code"),
    XmlType("http://www.w3.org/ns/widgets", AnonymousType = true),
    XmlRoot("widget", Namespace = "http://www.w3.org/ns/widgets", IsNullable = false),
    Serializable
    ]
    public class WidgetElement : ICloneable
    {
        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language;

        [XmlAttribute("dir")]
        public string Direction = "ltr";

        /// <summary>
        /// Denotes an identifier for the widget. Usage is optional.
        /// </summary>
        [XmlAttribute("id")]
        public string ID;

        /// <summary>
        /// Specifies the version of the widget. Usage is optional.
        /// </summary>
        [XmlAttribute("version")]
        public string Version = "1.0";

        /// <summary>
        /// A numeric attribute greater than 0 that indicates the preferred viewport width of the instantiated custom start file in CSS pixels. Usage is optional. 
        /// </summary>
        [XmlAttribute("width")]
        public int Width;

        /// <summary>
        /// A numeric attribute greater than 0 that indicates the preferred viewport height of the instantiated custom start file in CSS pixels. Usage is optional.
        /// </summary>
        [XmlAttribute("height")]
        public int Height;

        /// <summary>
        /// A language attribute that specifies, through a language tag, the author's preferred locale for the widget. Its intended use is to provide a fallback in case the user agent cannot match any of the widget's localized content to the user agent locales list or in case the author has not provided any unlocalized content. 
        /// </summary>
        [XmlAttribute("defaultlocale")]
        public string DefaultLocale = "en-us";

        /// <summary>
        /// Denotes the author's preferred view mode, followed by the next most preferred view mode, and so forth. Usage is optional. 
        /// </summary>
        [XmlAttribute("viewmodes")]
        public string ViewModes;

        /// <summary>
        /// Represents people or an organization attributed with the creation of the widget.
        /// </summary>
        [XmlElement("author")]
        public AuthorElement Author;

        /// <summary>
        /// Represents the full human-readable name for a widget that is used; for example, in an application menu or in other contexts. 
        /// </summary>
        [XmlElement("name")]
        public NameElement Name;

        /// <summary>
        /// Represents a human-readable description of the widget.
        /// </summary>
        [XmlElement("description")]
        public string Description;

        /// <summary>
        /// Represents a custom icon for the widget.
        /// </summary>
        [XmlElement("icon", Type = typeof(IconElement))]
        public List<IconElement> Icons;

        /// <summary>
        ///  A feature is a URI identifiable runtime component (e.g. an Application Programming Interface or video decoder). The feature element serves as a standardized means to request the binding of an IRI identifiable runtime component to a widget for use at runtime. Using a feature element denotes that, at runtime, a widget can attempt to access the feature identified by the feature element's name attribute. A feature has zero or more parameters associated with it. 
        /// </summary>
        [XmlElement("feature", Type = typeof(FeatureElement))]
        public List<FeatureElement> Features;

        /// <summary>
        ///  Represents a software license which includes, for example, a usage agreement, redistribution statement, and/or a copyright license terms under which the content of the widget package is provided. 
        /// </summary>
        [XmlElement("license", Type = typeof(LicenseElement))]
        public List<LicenseElement> Licenses;

        /// <summary>
        ///  Allows authors to declare one or more preferences. A preference is a persistently stored name-value pair that is associated with the widget the first time the widget is initiated. 
        /// </summary>
        [XmlElement("preference", Type = typeof(PreferenceElement))]
        public List<PreferenceElement> Preferences;

        /// <summary>
        /// Used by an author to declare which custom start file the user agent is expected to use when it instantiates the widget. 
        /// </summary>
        [XmlElement("content", Type = typeof(ContentElement))]
        public ContentElement Content;

        public WidgetElement Clone()
        {
            return (WidgetElement)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            var copy = new WidgetElement()
            {
                ID = this.ID,
                Version = this.Version,
                Language = this.Language,
                Height = this.Height,
                Width = this.Width,
                ViewModes = this.ViewModes,
                Name = this.Name,
                DefaultLocale = this.DefaultLocale,
                Description = this.Description,
                Direction = this.Direction
            };

            if (this.Author != null)
                copy.Author = this.Author.Clone();

            if (this.Content != null)
                copy.Content = this.Content.Clone();

            if (this.Features != null && this.Features.Count() > 0)
            {
                copy.Features = new List<FeatureElement>();
                foreach (var f in this.Features)
                    copy.Features.Add(f.Clone());
            }

            if (this.Preferences != null && this.Preferences.Count() > 0)
            {
                copy.Preferences = new List<PreferenceElement>();
                foreach (var f in this.Preferences)
                    copy.Preferences.Add(f.Clone());
            }

            if (this.Licenses != null && this.Licenses.Count() > 0)
            {
                copy.Licenses = new List<LicenseElement>();
                foreach (var l in this.Licenses)
                    copy.Licenses.Add(l.Clone());
            }

            if (this.Icons != null && this.Icons.Count() > 0)
            {
                copy.Icons = new List<IconElement>();
                foreach (var l in this.Icons)
                    copy.Icons.Add(l.Clone());
            }

            return copy;
        }

    
    }
}
