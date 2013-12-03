//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DNA.Xml.Solutions
{
    [
    XmlRoot("solution", Namespace = "http://www.dotnetage.com/XML/Schema/solution"),
    Serializable
    ]
    public class WebElement : ICloneable
    {
        [XmlElement("title")]
        public LocalizableElement Title;

        [XmlElement("description")]
        public LocalizableElement Description;

        [XmlAttribute("theme")]
        public string Theme;

        [XmlAttribute("defaultLocale")]
        public string DefaultLanguage = "en-US";

        [XmlElement("shortcutIcon")]
        public RefElement ShortcutIcon;

        [XmlElement("logo")]
        public RefElement LogoImage;

        //[XmlElement("copyright")]
        //public string Copyright;

        [XmlElement("style")]
        public string CssText;

        [XmlElement("defaultUri")]
        public string DefaultUrl;

        [XmlElement("page")]
        public List<RefElement> PageRefs;

        [XmlElement("list")]
        public List<RefElement> ListRefs;

        [XmlIgnore]
        public List<PageElement> Pages { get; set; }

        [XmlElement("categories")]
        public CategoriesElement Categories;

        //#region Implement the IWeb interface

        //string IWeb.Name
        //{
        //    get { return ""; }
        //}

        //string IWeb.Title
        //{
        //    get
        //    {
        //        if (this.Title != null)
        //            return this.Title.Text;
        //        return "";
        //    }
        //}

        //string IWeb.Description
        //{
        //    get
        //    {
        //        if (this.Description != null)
        //            return this.Description.Text;
        //        return "";
        //    }
        //}

        //string IWeb.Theme
        //{
        //    get { return this.Theme; }
        //}

        //IEnumerable<IWebPage> IWeb.Pages
        //{
        //    get { return this.PageRefs; }
        //}


        //string IWeb.DefaultLocale
        //{
        //    get { return this.DefaultLanguage; }
        //}

        //string IWeb.Copyright
        //{
        //    get { return ""; }
        //}

        //string IWeb.CssText
        //{
        //    get { return this.CssText; }
        //}

        //string IWeb.LogoImageUrl
        //{
        //    get { return this.LogoImageUrl; }
        //}

        //string IWeb.DefaultUrl
        //{
        //    get { return this.DefaultUrl; }
        //}

        //string IWeb.ShortcutIconUrl
        //{
        //    get { return this.ShortcutIconUrl; }
        //}

        //#endregion

        public WebElement Clone()
        {
            var copy = new WebElement()
            {
                Title = this.Title != null ? this.Title.Clone() : null,
                Description = this.Description != null ? this.Description.Clone() : null,
                ShortcutIcon = this.ShortcutIcon != null ? this.ShortcutIcon.Clone() : null,
                LogoImage = this.LogoImage != null ? this.LogoImage.Clone() : null,
                Theme = this.Theme,
                DefaultLanguage = this.DefaultLanguage,
                //Copyright = this.Copyright,
                CssText = this.CssText,
                DefaultUrl = this.DefaultUrl
            };

            if (PageRefs != null)
            {
                copy.PageRefs = new List<RefElement>();
                foreach (var p in this.PageRefs)
                    copy.PageRefs.Add(p.Clone());
            }

            if (ListRefs != null)
            {
                copy.ListRefs = new List<RefElement>();
                foreach (var c in this.ListRefs)
                    copy.ListRefs.Add(c.Clone());
            }

            if (Categories != null)
                copy.Categories = Categories.Clone();

            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }

    public class CategoriesElement : ICloneable
    {
        [XmlElement("category")]
        public List<CategoryElement> Categories;

        [XmlAttribute("xml:lang", DataType = "language")]
        public string Locale = "en-us";

        public CategoriesElement Clone()
        {
            var copy = new CategoriesElement() { Locale = this.Locale };
            if (this.Categories != null)
            {
                copy.Categories = new List<CategoryElement>();
                foreach (var c in Categories)
                    copy.Categories.Add(c.Clone());
            }
            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }

    [XmlRoot("category")]
    public class CategoryElement : ICloneable
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("id")]
        public int ID;

        [XmlAttribute("parentId")]
        public int ParentID;
        //[XmlElement("category")]
        //public List<CategoryElement> Categories;

        public CategoryElement Clone()
        {
            var copy = new CategoryElement()
            {
                ID = this.ID,
                ParentID=this.ParentID,
                Name = this.Name
            };

            //if (this.Categories != null)
            //{
            //    copy.Categories = new List<CategoryElement>();
            //    foreach (var c in Categories)
            //        copy.Categories.Add(c.Clone());
            //}

            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}