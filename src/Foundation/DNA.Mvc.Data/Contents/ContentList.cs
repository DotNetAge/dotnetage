//  Copyright (c) 2012 Ray Liang (http://www.dotnetage.com)
//  Licensed MIT: http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DNA.Web
{
    public class ContentList
    {
        public const string DefaultNamespace = "http://www.dotnetage.com/XML/Schema/contents";

        #region schema names
        protected const string TAG_NAME = "contentType";
        protected const string TITLE = "title";
        protected const string DESC = "description";
        protected const string ACTIVITY = "activity";
        protected const string DEFAULT_LOCALE = "defaultLocale";
        protected const string NAME = "name";
        protected const string VERSIONING = "versioning";
        protected const string ALLOW_COMMENTS = "allowComments";
        protected const string ALLOW_ATTACHS = "allowAttachs";
        protected const string ALLOW_VOTES = "allowVotes";
        protected const string ALLOW_RESHARING = "allowResharing";
        protected const string ALLOW_CATE_TAGS = "allowCategoriesAndTags";
        protected const string HIERARCHY = "hierarchy";
        protected const string MODERATED = "moderated";
        protected const string SINGLE = "single";
        protected const string BASE = "base";
        protected const string LANG = "lang";
        protected const string FIELDS = "fields";
        protected const string FIELD = "field";
        protected const string VIEWS = "views";
        protected const string FORMS = "forms";
        protected const string ROWS = "rows";
        protected const string SYS = "sys";
        #endregion

        public virtual int ID { get; set; }

        public string UID { get; set; }

        /// <summary>
        /// Gets/Sets the master id
        /// </summary>
        public virtual int MasterID { get; set; }

        /// <summary>
        /// Gets/Sets the master list's base type
        /// </summary>
        public virtual string Master { get; set; }
        /// <summary>
        /// Gets/Sets the base type version
        /// </summary>
        public virtual string Version { get; set; }

        public virtual string Name { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual string ItemType { get; set; }

        public virtual string BaseType { get; set; }

        public virtual int Privacy { get; set; }

        /// <summary>
        /// Gets the moderator user names split with comma
        /// </summary>
        public virtual string Moderators { get; set; }

        /// <summary>
        /// Identity every web can have only one instance of this conentType
        /// </summary>
        public virtual bool IsSingle { get; set; }

        /// <summary>
        /// Gets/Sets whether the comments enable on every items
        /// </summary>
        public virtual bool AllowComments { get; set; }

        public virtual bool AllowAttachments { get; set; }

        public virtual bool AllowVotes { get; set; }

        public virtual bool AllowCategoriesAndTags { get; set; }

        public virtual string Locale { get; set; }

        public virtual bool IsHierarchy { get; set; }

        public virtual bool AllowResharing { get; set; }

        public virtual bool EnableVersioning { get; set; }

        public virtual bool IsModerated { get; set; }

        public virtual bool IsActivity { get; set; }

        public virtual bool IsSystem { get; set; }

        public virtual ContentForm NewForm
        {
            get
            {
                var t = (int)ContentFormTypes.New;
                return Forms.FirstOrDefault(f => f.FormType == t);
            }
        }

        public virtual ContentForm EditForm
        {
            get
            {
                var t = (int)ContentFormTypes.Edit;
                return Forms.FirstOrDefault(f => f.FormType == t);
            }
        }

        public virtual ContentForm DetailForm
        {
            get
            {
                var t = (int)ContentFormTypes.Display;
                return Forms.FirstOrDefault(f => f.FormType == t);
            }
        }

        public virtual ContentForm ActivityForm
        {
            get
            {
                var t = (int)ContentFormTypes.Activity;
                return Forms.FirstOrDefault(f => f.FormType == t);
            }
        }

        public virtual Web Web { get; set; }

        public virtual int WebID { get; set; }

        public virtual string ImageUrl { get; set; }

        public virtual ICollection<ContentView> Views { get; set; }

        public virtual ICollection<ContentForm> Forms { get; set; }

        public virtual ICollection<ContentDataItem> Items { get; set; }

        public virtual ICollection<ContentAction> Actions { get; set; }

        public virtual string Roles { get; set; }

        public virtual ICollection<Follow> Followers { get; set; }

        public virtual string Owner { get; set; }

        /// <summary>
        /// Gets/Sets the last data item modified or created date time.
        /// </summary>
        public virtual DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/Sets the custom fields xml definition
        /// </summary>
        public virtual string FieldsXml { get; set; }

        ///// <summary>
        ///// Gets/Sets the activity disply template
        ///// </summary>
        //public virtual string ActivityDispTemplate { get; set; }

        /// <summary>
        /// Read the fields from configXml
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ContentField> ReadFields()
        {
            if (string.IsNullOrEmpty(FieldsXml))
                throw new NullReferenceException("ConfigXml");

            var element = XElement.Parse(FieldsXml);
            var ns = element.GetDefaultNamespace();
            var fieldEles = element.Elements(ns + FIELD).ToList();

            //if (fieldEles == null || fieldEles.Count() == 0)
            //    throw new Exception("The content list at less has one field defined");

            var factory = new DNA.Web.Contents.ContentFieldFactory();
            return fieldEles.Select(f => { var field = factory.Create(f); field.Parent = this; return field; });
        }

        public virtual void SaveFields(IEnumerable<ContentField> fields)
        {
            XNamespace ns = DefaultNamespace;
            var fieldsElement = new XElement(ns + "fields", new XAttribute("xmlns", DefaultNamespace));
            foreach (var f in fields)
                fieldsElement.Add(f.Element());
            FieldsXml = fieldsElement.OuterXml();
        }

        public virtual void Load(XElement element, string locale = "")
        {
            this.EnableVersioning = element.BoolAttr(VERSIONING);
            this.AllowComments = element.BoolAttr(ALLOW_COMMENTS);
            this.AllowAttachments = element.BoolAttr(ALLOW_ATTACHS);
            this.AllowVotes = element.BoolAttr(ALLOW_VOTES);
            this.AllowResharing = element.BoolAttr(ALLOW_RESHARING);
            this.AllowCategoriesAndTags = element.BoolAttr(ALLOW_CATE_TAGS);
            this.IsHierarchy = element.BoolAttr(HIERARCHY);
            this.IsModerated = element.BoolAttr(MODERATED);
            this.IsSingle = element.BoolAttr(SINGLE);
            this.IsSystem = element.BoolAttr(SYS);
            this.ItemType = element.StrAttr("itemtype");
            this.UID = element.StrAttr("id");
            this.Version = element.StrAttr("version");
            if (string.IsNullOrEmpty(Version))
                this.Version = "1.0.0";
            this.Master = element.StrAttr("master");

            this.IsActivity = element.BoolAttr(ACTIVITY);
            var ns = element.GetDefaultNamespace();
            var defaultLocale = element.StrAttr(DEFAULT_LOCALE);
            var targetLocale = locale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase) ? "" : locale;
            var title = element.ElementWithLocale(ns + TITLE, targetLocale);
            var desc = element.ElementWithLocale(ns + DESC, targetLocale);
            this.Locale = targetLocale;

            if (string.IsNullOrEmpty(targetLocale))
            {
                if (string.IsNullOrEmpty(this.Locale))
                    this.Locale = "en-US";

                string baseType = element.StrAttr(BASE);
                if (string.IsNullOrEmpty(baseType))
                {
                    this.BaseType = element.StrAttr(NAME);
                }
                else
                {
                    this.Name = element.StrAttr(NAME);
                    this.BaseType = element.StrAttr(BASE);
                }
            }

            if (title == null)
                title = element.ElementWithLocale(ns + TITLE, "");

            if (desc == null)
                desc = element.ElementWithLocale(ns + DESC, "");

            if (title != null)
                this.Title = title.Value;

            if (desc != null)
                this.Description = desc.Value;

            var factory = new DNA.Web.Contents.ContentFieldFactory();

            var fields = element.Elements(ns + FIELDS)
                                             .Elements(ns + FIELD)
                                             .Select(f =>
                                             {
                                                 var field = factory.Create(f, targetLocale);
                                                 field.Parent = this;
                                                 return field;
                                             });

            var fieldElements = new XElement(FIELDS, new XAttribute("xmlns", DefaultNamespace),
                fields.Select(f => f.Element()));

            this.FieldsXml = fieldElements.OuterXml();

            //this.FieldsXml = element.Element(ns + FIELDS).OuterXml();

            //Views
            var viewElements = element.Descendants(ns + "view");
            this.Views = new List<ContentView>();
            foreach (var viewEle in viewElements)
            {
                var v = new ContentView() { Parent = this };
                v.Load(viewEle, targetLocale);
                this.Views.Add(v);
            }

            //Forms
            this.Forms = new List<ContentForm>();
            var formElements = element.Descendants(ns + "form");
            this.Forms = new List<ContentForm>();
            foreach (var formEle in formElements)
            {
                var f = new ContentForm() { Parent = this };
                f.Load(formEle, targetLocale);
                this.Forms.Add(f);
            }

            //Items
            this.Items = new List<ContentDataItem>();
            var itemEles = element.Descendants(ns + "row");
            this.Items = new List<ContentDataItem>();
            foreach (var row in itemEles)
            {
                var r = new ContentDataItem() { Parent = this, Locale = this.Locale };
                r.Load(row);
                this.Items.Add(r);
            }
        }

        public virtual XElement Element()
        {
            XNamespace ns = DefaultNamespace;
            var element = new XElement(ns + TAG_NAME,
                new XAttribute("xmlns", DefaultNamespace),
                new XAttribute(DEFAULT_LOCALE, this.Locale),
                new XAttribute(NAME, this.Name),
                new XAttribute("version", this.Version),
                new XAttribute("id", this.UID),
                new XAttribute(BASE, this.BaseType));

            if (this.IsActivity)
                element.Add(new XAttribute(ACTIVITY, true));

            if (!string.IsNullOrEmpty(this.Master))
                element.Add(new XAttribute("master", this.Master));

            if (!string.IsNullOrEmpty(this.ItemType))
                element.Add(new XAttribute("itemtype", this.ItemType));

            if (AllowCategoriesAndTags)
                element.Add(new XAttribute(ALLOW_CATE_TAGS, this.AllowCategoriesAndTags));

            if (IsHierarchy)
                element.Add(new XAttribute(HIERARCHY, this.IsHierarchy));

            if (IsSystem)
                element.Add(new XAttribute(SYS, this.IsSystem));

            if (IsModerated)
                element.Add(new XAttribute(MODERATED, this.IsModerated));

            if (IsSingle)
                element.Add(new XAttribute(SINGLE, this.IsSingle));

            if (AllowResharing)
                element.Add(new XAttribute(ALLOW_RESHARING, this.AllowResharing));

            if (this.EnableVersioning)
                element.Add(new XAttribute(VERSIONING, this.EnableVersioning));

            if (this.AllowAttachments)
                element.Add(new XAttribute(ALLOW_ATTACHS, this.AllowAttachments));

            if (this.AllowComments)
                element.Add(new XAttribute(ALLOW_COMMENTS, this.AllowComments));

            if (this.AllowVotes)
                element.Add(new XAttribute(ALLOW_VOTES, this.AllowVotes));

            if (!string.IsNullOrEmpty(Title))
                element.Add(new XElement(ns + TITLE, new XCData(this.Title)));

            if (!string.IsNullOrEmpty(Description))
                element.Add(new XElement(ns + DESC, new XCData(this.Description)));

            var fields = this.ReadFields();
            element.Add(new XElement(ns + "fields", fields.Select(f => f.Element())));

            //element.Add(XElement.Parse(this.FieldsXml));
            return element;
        }

    }
}
