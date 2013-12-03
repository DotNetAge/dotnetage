//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DNA.Xml.Solutions
{
    public class SolutionTemplatePackage : PackageBase<WebElement>
    {
        public SolutionTemplatePackage(string path) : base(path) { }

        public override string DefaultNamespace
        {
            get
            {
                return "http://www.dotnetage.com/XML/Schema/solution";
            }
        }

        public List<PageElement> LoadPages(string lang = "")
        {
            var pages = new List<PageElement>();
            if (this.Model.PageRefs != null)
            {
                foreach (var pref in Model.PageRefs)
                {
                    if (string.IsNullOrEmpty(lang) || Model.DefaultLanguage.Equals(lang, StringComparison.OrdinalIgnoreCase))
                    {
                        //Default language
                        if (!string.IsNullOrEmpty(pref.Language) && !pref.Language.Equals(lang, StringComparison.OrdinalIgnoreCase))
                            continue;
                    }
                    else
                    {
                        if (!pref.Language.Equals(lang, StringComparison.OrdinalIgnoreCase))
                            continue;
                    }

                    var pageFile = ResolveFileName(pref.Ref.Replace("/", "\\"), lang);
                    if (File.Exists(pageFile))
                    {
                        var pageXml = File.ReadAllText(pageFile, Encoding.UTF8);
                        pageXml = this.ReplaceParams(pageXml);
                        var pageElement = Deserialize<PageElement>(pageXml, "http://www.dotnetage.com/XML/Schema/page");
                        LocalizePage(pageElement, lang);
                        pages.Add(pageElement);
                    }
                }
            }
            return pages;
        }

        public List<XElement> LoadContentLists(string lang = "")
        {
            var lists = new List<XElement>();
            if (this.Model.ListRefs != null)
            {
                foreach (var pref in Model.ListRefs)
                {
                    var filePath = (string.IsNullOrEmpty(Model.DefaultLanguage) || Model.DefaultLanguage.Equals(lang, StringComparison.OrdinalIgnoreCase)) ? ResolveFileName(pref.Ref) : ResolveFileName(pref.Ref, lang);
                    var listXml = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                    listXml = this.ReplaceParams(listXml);
                    var xdoc = XDocument.Parse(listXml);
                    lists.Add(xdoc.Root);
                }
            }
            return lists;
        }

        private void LocalizePage(PageElement pageElement, string lang = "")
        {
            if (!string.IsNullOrEmpty(lang))
                pageElement.Locale = lang;

            if (pageElement.Icon != null && !string.IsNullOrEmpty(pageElement.Icon.Source))
                pageElement.Icon.Source = ResolveUri(pageElement.Icon.Source, lang);

            if (pageElement.Image != null && !string.IsNullOrEmpty(pageElement.Image.Source))
                pageElement.Image.Source = ResolveUri(pageElement.Image.Source, lang);

            if (pageElement.StyleSheets != null && pageElement.StyleSheets.Count > 0)
            {
                foreach (var style in pageElement.StyleSheets)
                {
                    if (!string.IsNullOrEmpty(style.Source))
                        style.Source = ResolveUri(style.Source, lang);
                }
            }

            if (pageElement.Scripts != null && pageElement.Scripts.Count > 0)
            {
                foreach (var script in pageElement.Scripts)
                {
                    if (!string.IsNullOrEmpty(script.Source))
                        script.Source = ResolveUri(script.Source, lang);
                }
            }

            if (pageElement.Children != null && pageElement.Children.Count > 0)
            {
                foreach (var child in pageElement.Children)
                    LocalizePage(child, lang);
            }

            ///TODO:Localize the widget resources
        }

        public override WebElement Locale(string lang)
        {
            if (lang.Equals(Model.DefaultLanguage, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(lang))
            {
                if (Model.Pages == null)
                    Model.Pages = LoadPages();

                if (Model.LogoImage != null && !string.IsNullOrEmpty(Model.LogoImage.Ref))
                    Model.LogoImage.Ref = ResolveUri(Model.LogoImage.Ref);

                if (Model.ShortcutIcon != null && !string.IsNullOrEmpty(Model.ShortcutIcon.Ref))
                    Model.ShortcutIcon.Ref = ResolveUri(Model.ShortcutIcon.Ref);

                return Model;
            }

            var copy = Model.Clone();
            copy.DefaultLanguage = lang;

            var titleEle = GetLocalizableElement("title", lang);
            if (titleEle != null)
                copy.Title = titleEle;

            if (copy.Title != null)
                copy.Title.Language = lang;

            var descEle = GetLocalizableElement("description", lang);
            if (descEle != null)
                copy.Description = descEle;

            if (copy.Description != null)
                copy.Description.Language = lang;

            var logoNode = GetLocalizedNode("logo", lang);
            if (logoNode != null && logoNode.Attributes != null && logoNode.Attributes["src"] != null && !string.IsNullOrEmpty(logoNode.Attributes["src"].Value))
            {
                copy.LogoImage = new RefElement()
                {
                    Language = lang,
                    Ref = ResolveUri(logoNode.Attributes["src"].Value, lang)
                };
            }

            var shotcutNode = GetLocalizedNode("title", lang);
            if (shotcutNode != null && shotcutNode.Attributes != null && shotcutNode.Attributes["src"] != null && !string.IsNullOrEmpty(shotcutNode.Attributes["src"].Value))
            {
                copy.ShortcutIcon = new RefElement()
                {
                    Language = lang,
                    Ref = ResolveUri(shotcutNode.Attributes["src"].Value, lang)
                };
            }

            copy.Pages = LoadPages(lang);
            var cats = GetLocalizedNode("categories", lang);
            if (cats != null)
            {
                copy.Categories = new CategoriesElement() { Locale = lang };
                if (cats.ChildNodes.Count > 0)
                {
                    copy.Categories.Categories = new List<CategoryElement>();
                    foreach (XmlNode c in cats.ChildNodes)
                    {
                        copy.Categories.Categories.Add(new CategoryElement()
                        {
                            ID = c.Attributes != null && c.Attributes["id"] != null ? Convert.ToInt16(c.Attributes["id"].Value) : 0,
                            ParentID = c.Attributes != null && c.Attributes["parentId"] != null ? Convert.ToInt16(c.Attributes["parentId"].Value) : 0,
                            Name = c.Attributes != null && c.Attributes["name"] != null ? c.Attributes["name"].Value : "",
                        });
                    }
                }
            }
            
            if (copy.ListRefs != null)
            {
                var _listRefs = copy.ListRefs.Where(r => r.Language.Equals(lang,StringComparison.OrdinalIgnoreCase)).ToList();
                copy.ListRefs = _listRefs;
            }

            //copy.PageRefs = new List<RefElement>();

            return copy;
        }
    }
}
