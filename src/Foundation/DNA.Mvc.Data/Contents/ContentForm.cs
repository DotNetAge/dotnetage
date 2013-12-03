//  Copyright (c) 2012 Ray Liang (http://www.dotnetage.com)
//  Licensed MIT: http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DNA.Web
{
    public class ContentForm 
    {
        public int ID { get; set; }

        public int ParentID { get; set; }

        public virtual ContentList Parent { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsAjax { get; set; }

        public int FormType { get; set; }

        ///// <summary>
        ///// Gets/Sets the template string content
        ///// </summary>
        //public string Template { get; set; }

        public virtual int Privacy { get; set; }

        ///// <summary>
        ///// Gets/Sets the template view file
        ///// </summary>
        //public string TemplateView { get; set; }
        public virtual string BodyTemplateXml { get; set; }

        public virtual string FieldsXml { get; set; }

        public string ScriptsXml { get; set; }

        public string StyleSheetsXml { get; set; }

        public string Roles { get; set; }

        //public bool HideCaption { get; set; }
        //public bool ShowAuthor { get; set; }
        //public string CaptionField { get; set; }

        /// <summary>
        /// Gets/Sets the form whether has dynamic page.
        /// </summary>
        public bool NoPage { get; set; }

        #region schema
        //private const string NAME = "name";
        private const string TMPL = "tmpl";
        //private const string ACCESS_ROLES = "accessRoles";
        //private const string TITLE = "title";
        //private const string DESC = "description";
        //private const string BODY = "body";
        //private const string STYLES = "styleSheet";
        //private const string SCRIPTS = "startupScripts";
        private const string TYPE = "type";
        private const string AJAX = "ajax";
        private const string NOPAGE = "nopage";
        private const string STYLE_SHEET = "styleSheet";
        private const string SCRIPT = "script";
        private const string ANONYMOUS = "anonymous";
        //private const string CAPTION_FIELD = "captionField";
        //private const string HIDE_CAP = "hideCaption";
        //private const string SHOW_AUTHOR = "showAuthor";
        //private const string LANG = "lang";
        #endregion

        public virtual void Load(XElement element, string locale)
        {
            var ns = element.GetDefaultNamespace();
            this.IsAjax = element.BoolAttr(AJAX);
            this.FormType = (int)((ContentFormTypes)Enum.Parse(typeof(ContentFormTypes), element.StrAttr(TYPE)));
            this.Roles = element.StrAttr(DataNames.AccessRoles);

            //this.CaptionField = element.StrAttr(CAPTION_FIELD);
            //this.HideCaption = element.BoolAttr(HIDE_CAP);
            //this.ShowAuthor = element.BoolAttr(SHOW_AUTHOR);
            this.NoPage = element.BoolAttr(NOPAGE);
            var title = element.ElementWithLocale(ns + DataNames.Title, locale);
            var desc = element.ElementWithLocale(ns + DataNames.Description, locale);
            var tmpl = element.ElementWithLocale(ns + DataNames.Body, locale);
            
            //if (element.Attribute(ANONYMOUS) != null)
                this.AllowAnonymous = element.BoolAttr(ANONYMOUS);
            //else
                //this.AllowAnonymous = true;

            if (title==null)
                title=element.ElementWithLocale(ns + DataNames.Title, "");

            if (desc==null)
                desc=element.ElementWithLocale(ns + DataNames.Description, "");

            if (tmpl==null)
                tmpl = element.ElementWithLocale(ns + DataNames.Body, "");

            if (title != null)
                this.Title = title.Value;

            if (desc != null)
                this.Description = desc.Value;

            var fieldsEle = element.Element(ns + "fields");
            if (fieldsEle != null)
                FieldsXml = fieldsEle.OuterXml();
                    //string.Format("<fields>{0}</fields>", fieldsEle.InnerXml());

            if (tmpl != null)
                this.BodyTemplateXml = tmpl.OuterXml();

            #region style and scripts
            var scripts = element.ElementsWithLocale(ns + SCRIPT, locale);
            var styles = element.ElementsWithLocale(ns + STYLE_SHEET, locale);

            if (scripts == null || (scripts != null && scripts.Count() == 0))
                scripts = element.ElementsWithLocale(ns + SCRIPT, "");

            if (styles == null || (styles != null && styles.Count() == 0))
                styles = element.ElementsWithLocale(ns + STYLE_SHEET, "");

            if (scripts!=null && scripts.Count() > 0)
            {
                var scriptEl = new XElement("scripts", scripts);
                this.ScriptsXml = scriptEl.OuterXml();
            }

            if (styles!=null && styles.Count() > 0)
            {
                var styleEl = new XElement("styles", styles);
                this.StyleSheetsXml = styleEl.OuterXml();
            }
            #endregion
        }

        public virtual XElement Element()
        {
            XNamespace ns = ContentList.DefaultNamespace;
            var element = new XElement(ns + DataNames.Form,
                new XAttribute(TYPE, ((ContentFormTypes)this.FormType).ToString()));

            if (!string.IsNullOrEmpty(this.BodyTemplateXml))
                element.Add(this.Body.Element());

            if (this.IsAjax)
                element.Add(new XAttribute(AJAX, true));

            if (this.AllowAnonymous)
                element.Add(new XAttribute(ANONYMOUS, true));

            if (!string.IsNullOrEmpty(Roles))
                element.Add(new XAttribute(DataNames.AccessRoles, this.Roles));

            if (this.NoPage)
                element.Add(new XAttribute(NOPAGE, this.NoPage));

            if (!string.IsNullOrEmpty(this.Title))
                element.Add(new XElement(ns+DataNames.Title, new XCData(this.Title)));

            if (!string.IsNullOrEmpty(this.Description))
                element.Add(new XElement(ns + DataNames.Description, new XCData(this.Description)));

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
                }
            }
            return element;
        }

        public ContentTemplate Body
        {
            get
            {
                if (body == null)
                    body = new ContentTemplate(this.BodyTemplateXml);
                return body;
            }
        }

        private ContentTemplate body = null;

        public bool AllowAnonymous { get; set; }
    }

    //public enum FormTypes
    //{
    //    DisplayForm = 0,
    //    EditForm = 1,
    //    NewForm = 2,
    //}
}
