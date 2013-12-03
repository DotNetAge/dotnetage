//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DNA.Web
{
    public static class XElementExtensions
    {
        public static string StrAttr(this XElement element, string name)
        {
            if (element.HasAttributes && element.Attribute(name) != null)
            {
                return element.Attribute(name).Value;
            }
            return "";
        }

        public static int IntAttr(this XElement element, string name)
        {
            if (element.HasAttributes && element.Attribute(name) != null)
            {
                var val = 0;
                if (int.TryParse(element.Attribute(name).Value, out val))
                    return val;
            }
            return 0;
        }

        public static decimal DecimalAttr(this XElement element, string name)
        {
            if (element.HasAttributes && element.Attribute(name) != null)
            {
                var val = 0m;
                if (decimal.TryParse(element.Attribute(name).Value, out val))
                    return val;
            }
            return 0;
        }

        public static bool BoolAttr(this XElement element, string name)
        {
            if (element.HasAttributes && element.Attribute(name) != null)
            {
                var val = false;
                if (bool.TryParse(element.Attribute(name).Value, out val))
                    return val;
            }
            return false;
        }

        public static DateTime DateAttr(this XElement element, string name)
        {
            var dateStr = element.StrAttr(name);
            if (!string.IsNullOrEmpty(dateStr))
            {
                var returnDate = DateTime.MinValue;
                if (DateTime.TryParse(dateStr, out returnDate))
                    return returnDate;
            }

            return DateTime.MinValue;
        }

        public static string InnerXml(this XElement element)
        {
            if (element == null)
                return "";
            var reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        public static string OuterXml(this XElement element)
        {
            if (element == null)
                return "";
            var reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadOuterXml();
        }

        public static XElement ElementWithLocale(this XElement element, string name, string locale)
        {
            XElement ele = null;
            var ns = element.GetDefaultNamespace();
            var children = element.Elements(ns+name);

            if (!string.IsNullOrEmpty(locale))
            {
                if (children.Count() > 0)
                    ele = children.FirstOrDefault(t => t.Attribute(XNamespace.Xml + "lang") != null && !string.IsNullOrEmpty(t.Attribute(XNamespace.Xml + "lang").Value) && t.Attribute(XNamespace.Xml + "lang").Value.Equals(locale, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                if (children.Count() > 0)
                    ele = children.FirstOrDefault(t => t.Attribute(XNamespace.Xml + "lang") == null);
            }

            return ele;
        }

        public static XElement ElementWithLocale(this XElement element, XName name, string locale)
        {
            XElement ele = null;
            var children = element.Elements(name);

            if (!string.IsNullOrEmpty(locale))
            {
                if (children.Count() > 0)
                    ele = children.FirstOrDefault(t => t.Attribute(XNamespace.Xml + "lang") != null && !string.IsNullOrEmpty(t.Attribute(XNamespace.Xml + "lang").Value) && t.Attribute(XNamespace.Xml + "lang").Value.Equals(locale, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                if (children.Count() > 0)
                    ele = children.FirstOrDefault(t => t.Attribute(XNamespace.Xml + "lang") == null);
            }

            return ele;
        }

        public static IEnumerable<XElement> ElementsWithLocale(this XElement element, XName name, string locale)
        {
            IEnumerable<XElement> eles = null;
            var children = element.Elements(name);

            if (!string.IsNullOrEmpty(locale))
            {
                if (children.Count() > 0)
                    eles = children.Where(t => t.Attribute(XNamespace.Xml + "lang") != null && !string.IsNullOrEmpty(t.Attribute(XNamespace.Xml + "lang").Value) && t.Attribute(XNamespace.Xml + "lang").Value.Equals(locale, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                if (children.Count() > 0)
                    eles = children.Where(t => t.Attribute(XNamespace.Xml + "lang") == null);
            }

            return eles;
        }
        public static void AddHtmlAttributes(this XElement element,object htmlAttributes)
        {
            if (htmlAttributes != null)
            {
                var dict = htmlAttributes.ToDictionary();
                foreach (var k in dict.Keys)
                {
                    var key = k;
                    if (key.StartsWith("data_"))
                        key = key.Replace("_", "-");
                    element.Add(new XAttribute(key, dict[k]));
                }
            }
        }
    }
}
