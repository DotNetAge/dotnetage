//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DNA.Xml.Widgets
{
    /// <summary>
    ///  A feature is a URI identifiable runtime component (e.g. an Application Programming Interface or video decoder). 
    ///  The feature element serves as a standardized means to request the binding of an IRI identifiable runtime component to a widget for use at runtime. 
    ///  Using a feature element denotes that, at runtime, a widget can attempt to access the feature identified by the feature element's name attribute. 
    ///  A feature has zero or more parameters associated with it. 
    /// </summary>
    public class FeatureElement:ICloneable
    {
        /// <summary>
        /// Identifies a feature that is needed by the widget at runtime (such as an API). When the feature element is declared, it is required for authors to use the name attribute. 
        /// </summary>
        [XmlAttribute("name")]
        public string Name;

        /// <summary>
        /// A boolean attribute that indicates whether or not this feature has to be available to the widget at runtime. When set to true, the required attribute denotes that a feature is absolutely needed by the widget to function correctly, and without the availability of this feature the widget serves no useful purpose or won't execute properly. When set to false, the required attribute denotes that a widget can function correctly without the feature being supported or otherwise made available by the user agent. Usage is optional. 
        /// </summary>
        [XmlAttribute("required")]
        public bool IsRequried;

        /// <summary>
        /// Defines a parameter for a feature. A parameter is a name-value pair that is associated with the corresponding feature to which the parameter is declared. An author establishes the relationship between a parameter and feature by having a <param> element as a direct child of a <feature> element in document order. 
        /// </summary>
        [XmlElement("param", Type = typeof(ParamElement))]
        public List<ParamElement> Params;

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language = "en-us";

        //[XmlAttribute("dir")]
        //public string Direction = "ltr";

        public FeatureElement Clone()
        {
            return (FeatureElement)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            var copy = new FeatureElement() { 
            Name=this.Name,
            IsRequried=this.IsRequried,
            Language=this.Language
            //,Direction=this.Direction 
            };
            
            if (this.Params != null && this.Params.Count()>0) 
            {
                copy.Params = new List<ParamElement>();
                foreach (var p in this.Params)
                {
                    copy.Params.Add(p.Clone());
                }
            }
            return copy;
        }
    }
}
