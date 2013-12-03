//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel.Transformers
{
    public class XslTemplateTransformer : ITemplateTransform
    {
        public string ContentType
        {
            get { return TemplateContentTypes.Xslt; }
        }

        public void Transform(string text, ContentView view)
        {
            var xTmpl = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                 "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:msxsl=\"urn:schemas-microsoft-com:xslt\" exclude-result-prefixes=\"msxsl\">" +
                                 "<xsl:output method=\"html\" indent=\"yes\"/>" +
                                 "<xsl:template match=\"/\"></xsl:template></xsl:stylesheet>";
            var xsltDoc = XDocument.Parse(xTmpl);
            XNamespace ns = "http://www.w3.org/1999/XSL/Transform";

            //var text = "<div class=\"d-layout-box d-h-box\" data-box=\"hbox\"><div style=\"width: 105px; height: 100px;\" class=\"d-box d-inline\"><div class=\"d-field-holder d-inline\" data-type=\"image\" data-link=\"true\" data-label=\"Cover\" data-field=\"cover\"></div></div><div style=\"width: 441px;\" class=\"d-box d-inline\"><div class=\"d-field-holder\" data-type=\"text\" data-link=\"true\" data-label=\"Title\" data-field=\"title\"></div><div class=\"d-field-holder\" data-type=\"note\" data-link=\"true\" data-label=\"Body\" data-field=\"body\"></div></div></div>";
            //Add params
            xsltDoc.Root.Add(new XElement(ns + "param", new XAttribute("name", "appPath")),
                new XElement(ns + "param", new XAttribute("name", "web")),
                new XElement(ns + "param", new XAttribute("name", "list")),
                new XElement(ns + "param", new XAttribute("name", "view")),
                new XElement(ns + "param", new XAttribute("name", "lang"))
                );

            var tmplElement = xsltDoc.Root.Element(ns + "template");
            var listElement = new XElement("ul", new XAttribute("class", "d-view-list"));
            tmplElement.Add(listElement);
            var rowTmpl = XElement.Parse(text);
            var forEachElement = new XElement(ns + "for-each",
                new XAttribute("select", @"//rows/row"), new XElement("li",new XAttribute("class","d-view-item"), rowTmpl));

            listElement.Add(forEachElement);

            rowTmpl.DescendantsAndSelf()
                .Where(f => f.Attribute("data-field") != null)
                .ToList()
                .AsParallel()
                .ForAll(e =>
                {
                    var fieldName = e.Attribute("data-field").Value;
                    var linkToItem = e.BoolAttr("data-link");
                    var inline = e.BoolAttr("data-line");
                    var fieldType = e.Attribute("data-type").Value;

                    var showLabel = !e.BoolAttr("data-label-hidden");
                    //label=e.Element("label")
                    //e.ReplaceWith(new XElement(ns + "value-of", new XAttribute("select", fieldName)));
                    var applyTemplate = new XElement(ns + "apply-templates", new XAttribute("select", fieldName));

                    var fieldTmpl = new XElement(ns + "template", new XAttribute("match", fieldName));
                    XElement linkTmpl = null;
                    if (linkToItem)
                    {
                        linkTmpl = new XElement(ns + "element", new XAttribute("name", "a"),
                            new XElement(ns + "attribute", new XAttribute("name", "href"), new XElement(ns + "value-of", new XAttribute("select", "concat($appPath,'/',$web,'/',$lang,'/lists/',$list,'/items/',../@Slug,'.html')"))));
                    }

                    XElement fieldContentTmpl = null;

                    if (fieldType == "image")
                    {
                        fieldContentTmpl = new XElement(ns + "element", new XAttribute("name", "img"),
                            new XElement(ns + "attribute", new XAttribute("name", "src"),
                            new XElement(ns + "value-of", new XAttribute("select", ".")))
                            );
                    }
                    else
                    {
                        fieldContentTmpl = new XElement(ns + "element", new XAttribute("name", inline ? "span" : "div"),
                            new XElement(ns + "value-of", new XAttribute("select", ".")));
                    }

                    if (inline)
                        fieldContentTmpl.Add(new XAttribute("class", "d-inline"));

                    if (linkTmpl != null)
                    {
                        linkTmpl.Add(fieldContentTmpl);
                        fieldTmpl.Add(linkTmpl);
                    }
                    else
                        fieldTmpl.Add(fieldContentTmpl);

                    xsltDoc.Root.Add(fieldTmpl);
                    //new template
                    //e.ReplaceWith(applyTemplate);
                    e.Add(applyTemplate);
                });

            //return xsltDoc.ToString();
            var result = new ContentTemplate()
            {
                ContentType = this.ContentType,
                Text = xsltDoc.ToString()
            };

            view.BodyTemplateXml = result.ToXml();
        }

        public void Transform(string text, ContentForm form)
        {
            throw new NotImplementedException();
        }
    }
}
