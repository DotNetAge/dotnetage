//  Copyright (c) 2012 Ray Liang (http://www.dotnetage.com)
//  Licensed MIT: http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DNA.Web
{
    public class ContentView
    {
        private int pageIndex = 1;

        public int ID { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }

        //public string Link { get; set; }
        public virtual int Privacy { get; set; }

        //public bool IsClientView { get; set; }

        public bool AllowPaging { get; set; }

        public int PageIndex { get { return pageIndex; } set { pageIndex = value; } }

        public int PageSize { get; set; }

        /// <summary>
        /// Gets/Sets the view template file
        /// </summary>
        public string ViewTemplate { get; set; }

        public int ParentID { get; set; }

        public string FieldRefsXml { get; set; }

        //public string QueryXml { get; set; }

        public string Icon { get; set; }

        public virtual bool IsHierarchy { get; set; }

        public virtual bool HideToolbar { get; set; }

        // public virtual string FeedItemMaps { get; set; }

        /// <summary>
        /// Gets/Sets  the view display pattern definition
        /// </summary>
        public string BodyTemplateXml { get; set; }

        ///// <summary>
        ///// Gets/Sets the row item template definition
        ///// </summary>
        ///// <remarks>
        /////  The row template contains a item row template in jQuery tmpl format. And it's only availdable for client view.
        ///// </remarks>
        //public string RowTemplate { get; set; }

        public string ScriptsXml { get; set; }

        public string StyleSheetsXml { get; set; }

        public string EmptyTemplateXml { get; set; }

        public virtual ContentList Parent { get; set; }

        /// <summary>
        /// Gets/Sets the view whether has dynamic page.
        /// </summary>
        public bool NoPage { get; set; }
        //public virtual string Engine { get; set; }

        ///// <summary>
        ///// Gets/Sets the view style
        ///// </summary>
        ///// <remarks>
        ///// Possible values : list | floating | tree | grid
        ///// </remarks>
        //public virtual string Style { get; set; }

        public string Roles { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public string GroupBy { get; set; }

        /// <summary>
        /// Gets / Sets the view file generate date
        /// </summary>
        /// <remarks>
        /// If this date is less then the List.LastModified the view need to be regenerated
        /// </remarks>
        public DateTime? Generated { get; set; }

        #region schema
        private const string NAME = "name";
        private const string DEFAULT = "default";
        private const string TMPL = "tmpl";
        private const string HIERARCHY = "hierarchy";
        private const string HIDE_TOOLBAR = "hideToolbar";
        private const string ACCESS_ROLES = "accessRoles";
        //private const string RUNAT = "runat";
        private const string ICON = "icon";
        private const string TITLE = "title";
        private const string DESC = "description";
        private const string PAGING = "paging";
        private const string ALLOW = "allow";
        private const string INDEX = "index";
        private const string SIZE = "size";
        private const string FIELDS = "fields";
        private const string FIELD_REF = "fieldRef";
        private const string TO_FEED = "toFeed";
        //private const string ROW_TMPL = "rowTemplate";
        private const string BODY = "body";
        private const string EMPTY_PATTERN = "emptyPattern";
        private const string STYLE_SHEET = "styleSheet";
        private const string SCRIPT = "script";
        private const string STYLE = "style";
        private const string TAG_NAME = "view";
        private const string SORT = "sort";
        private const string FILTER = "filter";
        private const string NOPAGE = "nopage";
        private const string ANONYMOUS = "anonymous";
        #endregion

        public virtual void Load(XElement element, string locale = "")
        {
            var ns = element.GetDefaultNamespace();

            this.Name = element.StrAttr(NAME);
            this.IsDefault = element.BoolAttr(DEFAULT);
            this.Icon = element.StrAttr(ICON);
            this.IsHierarchy = element.BoolAttr(HIERARCHY);
            this.HideToolbar = element.BoolAttr(HIDE_TOOLBAR);
            this.Roles = element.StrAttr(ACCESS_ROLES);
            this.Sort = element.StrAttr(SORT);
            this.Filter = element.StrAttr(FILTER);
            var title = element.ElementWithLocale(ns + TITLE, locale);
            var desc = element.ElementWithLocale(ns + DESC, locale);
            var paging = element.Element(ns + PAGING);

            if (element.Attribute(ANONYMOUS) != null)
                this.AllowAnonymous = element.BoolAttr(ANONYMOUS);
            else
                this.AllowAnonymous = true;

            if (title == null)
                title = element.ElementWithLocale(ns + TITLE, "");

            if (desc == null)
                desc = element.ElementWithLocale(ns + DESC, "");

            if (title != null)
                this.Title = title.Value;

            if (desc != null)
                this.Description = desc.Value;


            if (paging != null)
            {
                this.PageIndex = paging.IntAttr(INDEX);
                this.PageSize = paging.IntAttr(SIZE);
                this.AllowPaging = paging.BoolAttr(ALLOW);
            }

            var fieldsEle = element.Element(ns + FIELDS);
            if (fieldsEle != null)
                FieldRefsXml = fieldsEle.OuterXml();
            //FieldRefsXml = string.Format("<fields>{0}</fields>", fieldsEle.InnerXml());

            var body = element.ElementWithLocale(ns + BODY, locale);

            if (body == null)
                body = element.ElementWithLocale(ns + BODY, "");

            if (body != null)
                this.BodyTemplateXml = body.OuterXml();

            var empty = element.ElementWithLocale(ns + EMPTY_PATTERN, locale);

            if (empty == null)
                element.ElementWithLocale(ns + EMPTY_PATTERN, "");

            if (empty != null)
                this.EmptyTemplateXml = empty.OuterXml();

            this.NoPage = element.BoolAttr(NOPAGE);

            #region style and scripts
            var scripts = element.ElementsWithLocale(ns + SCRIPT, locale);
            var styles = element.ElementsWithLocale(ns + STYLE_SHEET, locale);

            if (scripts == null || (scripts != null && scripts.Count() == 0))
                scripts = element.ElementsWithLocale(ns + SCRIPT, "");

            if (styles == null || (styles != null && styles.Count() == 0))
                styles = element.ElementsWithLocale(ns + STYLE_SHEET, "");

            if (scripts != null && scripts.Count() > 0)
            {
                var scriptEl = new XElement("scripts", scripts);
                this.ScriptsXml = scriptEl.OuterXml();
            }

            if (styles != null && styles.Count() > 0)
            {
                var styleEl = new XElement("styles", styles);
                this.StyleSheetsXml = styleEl.OuterXml();
            }
            #endregion
        }

        public virtual XElement Element()
        {
            XNamespace ns = ContentList.DefaultNamespace;

            var element = new XElement(ns + TAG_NAME,
                new XAttribute(NAME, this.Name));

            if (!string.IsNullOrEmpty(this.ViewTemplate))
                element.Add(new XAttribute(TMPL, this.ViewTemplate));

            if (this.AllowPaging)
            {
                var pagingElement = new XElement(ns + PAGING, new XAttribute(ALLOW, this.AllowPaging));

                if (this.PageIndex > 0)
                    pagingElement.Add(new XAttribute(INDEX, this.PageIndex));

                if (this.PageSize > 0)
                    pagingElement.Add(new XAttribute(SIZE, this.PageSize));

                element.Add(pagingElement);
            }

            if (!string.IsNullOrEmpty(Roles))
                element.Add(new XAttribute(ACCESS_ROLES, Roles));

            if (this.IsDefault)
                element.Add(new XAttribute(DEFAULT, true));

            if (this.IsHierarchy)
                element.Add(new XAttribute(HIERARCHY, true));

            if (this.HideToolbar)
                element.Add(new XAttribute(HIDE_TOOLBAR, true));

            if (!string.IsNullOrEmpty(this.Filter))
                element.Add(new XAttribute(FILTER, this.Filter));

            if (!string.IsNullOrEmpty(this.Sort))
                element.Add(new XAttribute(SORT, Sort));

            //if (!string.IsNullOrEmpty(this.Style))
            //    element.Add(new XAttribute(STYLE, this.Style));

            if (this.NoPage)
                element.Add(new XElement(ns + NOPAGE, this.NoPage));

            if (!string.IsNullOrEmpty(this.Title))
                element.Add(new XElement(ns + TITLE, new XCData(this.Title)));

            if (!string.IsNullOrEmpty(this.Description))
                element.Add(new XElement(ns + DESC, new XCData(this.Title)));

            //if (!string.IsNullOrEmpty(this.RowTemplate))
            //    element.Add(new XElement(ROW_TMPL, new XCData(this.RowTemplate)));
            
            if (this.AllowAnonymous)
                element.Add(new XAttribute(ANONYMOUS, true));

            if (!string.IsNullOrEmpty(this.BodyTemplateXml))
            {
                var tmpl = new ContentTemplate(this.BodyTemplateXml);
                element.Add(tmpl.Element());
            }

            if (!string.IsNullOrEmpty(this.EmptyTemplateXml))
                element.Add(new XElement(ns + EMPTY_PATTERN, XElement.Parse(this.EmptyTemplateXml)));

            if (!string.IsNullOrEmpty(this.StyleSheetsXml))
            {
                var stylesEl = XElement.Parse(this.StyleSheetsXml);
                if (stylesEl != null && stylesEl.HasElements)
                {
                    var _styles = stylesEl.Elements();
                    foreach (var s in _styles)
                    {
                        var styleElement = new XElement(ns + "styleSheet");
                        if (s.HasAttributes)
                            styleElement.Add(s.Attributes());
                        if (!string.IsNullOrEmpty(s.Value))
                            styleElement.Add(new XCData(s.Value));
                        element.Add(styleElement);
                    }
                }
            }

            if (!string.IsNullOrEmpty(this.ScriptsXml))
            {
                var scriptsEl = XElement.Parse(this.ScriptsXml);
                if (scriptsEl != null && scriptsEl.HasElements)
                {
                    var _scripts = scriptsEl.Elements();
                    foreach (var s in _scripts)
                    {
                        var scriptElement = new XElement(ns + "script");
                        if (s.HasAttributes)
                            scriptElement.Add(s.Attributes());
                        if (!string.IsNullOrEmpty(s.Value))
                            scriptElement.Add(new XCData(s.Value));
                        element.Add(scriptElement);
                    }
                    //element.Add(scriptsEl.Elements());
                }
            }

            return element;
        }

        public ContentTemplate Body
        {
            get { return this.GetBodyTemplate(); }
        }

        public ContentTemplate GetBodyTemplate()
        {
            if (!string.IsNullOrEmpty(this.BodyTemplateXml))
                return new ContentTemplate(this.BodyTemplateXml);
            return new ContentTemplate();
        }

        public ContentTemplate GetEmptyTempalte()
        {
            if (!string.IsNullOrEmpty(this.EmptyTemplateXml))
                return new ContentTemplate(this.EmptyTemplateXml);
            return new ContentTemplate();
        }

        public bool AllowAnonymous { get; set; }
        //private ContentTemplate empty = null;
        //private ContentTemplate body = null;
    }
}
