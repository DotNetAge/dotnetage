//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represents a base class of the ContentField.
    /// </summary>
    public abstract class ContentField
    {
        #region schema
        protected const string REQUIRED = "required";
        protected const string NAME = "name";
        protected const string READONLY = "readonly";
        protected const string SLUG = "slug";
        protected const string LINKTOITEM = "linkToItem";
        protected const string HIDEINDISP = "hideInDispForm";
        protected const string HIDEINEDIT = "hideInEditForm";
        protected const string HIDEINACT = "hideInActForm";
        protected const string HIDEINNEW = "hideInNewForm";
        protected const string HIDEINVIEW = "hideInView";
        protected const string VIEWTMPL = "viewTmpl";
        protected const string DISPTMPL = "dispTmpl";
        protected const string EDITTMPL = "editTmpl";
        protected const string NEWTMPL = "newTmpl";
        protected const string ACTTMPL = "actTmpl";
        protected const string HIDDEN = "hidden";
        protected const string FILTERABLE = "filterable";
        protected const string SORTABLE = "sortable";
        protected const string TITLE = "title";
        protected const string DESC = "description";
        protected const string PLACEHOLDER = "placeHolder";
        protected const string DEFAULT = "default";
        protected const string NUMBER_FROMAT = "numberFormat";
        protected const string MINIMUM = "minimum";
        protected const string MAXIMUM = "maximum";
        protected const string SYMBOL = "symbol";
        protected const string DIGITS = "digits";
        protected const string DECIMAL_SPARATOR = "decimalSeparator";
        protected const string GROUP_SEPARATOR = "groupSeparator";
        protected const string POSITIVE_PATTERN = "positivePattern";
        protected const string SHOW_AS_PERCENTAGE = "showAsPercentage";
        protected const string FORMAT = "format";
        protected const string LANG = "lang";
        protected const string ITEM_TYPE = "itemtype";
        protected const string ITEM_PROP = "itemprop";
        #endregion

        /// <summary>
        /// Gets/Sets the field name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets/Sets the field title display text
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets/Sets the field description text.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets/Sets the MicroData itemprop property of the field.
        /// </summary>
        public virtual string ItemProp { get; set; }

        /// <summary>
        /// Gets/Sets the MicroData itemtype property of the field.
        /// </summary>
        public virtual string ItemType { get; set; }

        /// <summary>
        /// Gets/Sets the place holder text.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Identity this field could not be edit.
        /// </summary>
        public virtual bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets/Sets whether this field is hidden.
        /// </summary>
        public virtual bool IsHidden { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in display form.
        /// </summary>
        public virtual bool HideInDisplayForm { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in new form.
        /// </summary>
        public virtual bool HideInNewForm { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in activity form.
        /// </summary>
        public virtual bool HideInActForm { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in edit form.
        /// </summary>
        public virtual bool HideInEditForm { get; set; }

        /// <summary>
        /// Gets/Sets whether the field render as hidden input in all view.
        /// </summary>
        public virtual bool HideInView { get; set; }

        /// <summary>
        /// Indicates whether the field value is requried
        /// </summary>
        public virtual bool IsRequired { get; set; }

        /// <summary>
        /// Indicates whether the field is the slug seed. 
        /// </summary>
        /// <remarks>
        /// This property only availdable for : TextField, IntegerField and DateField
        /// </remarks>
        public virtual bool IsSlug { get; set; }

        /// <summary>
        /// Gets/Sets the privacy value.
        /// </summary>
        public virtual int Privacy { get; set; }

        /// <summary>
        /// Returns the field's default value
        /// </summary>
        public virtual object DefaultValue { get; set; }

        /// <summary>
        /// Gets/Sets the parent list object.
        /// </summary>
        public ContentList Parent { get; set; }

        /// <summary>
        /// Specified this field render as a link and refer to the current item
        /// </summary>
        public virtual bool IsLinkToItem { get; set; }

        private bool isSortable = true;

        /// <summary>
        /// Indicates whether the field value is sortable.
        /// </summary>
        public virtual bool IsSortable
        {
            get
            {
                return isSortable;
            }
            set
            {
                isSortable = value;
            }
        }

        /// <summary>
        /// Gets whether the field value can be filtered.
        /// </summary>
        public virtual bool IsFilterable { get { return true; } }

        /// <summary>
        /// Gets/Sets the field type value.
        /// </summary>
        public int FieldType { get; set; }

        /// <summary>
        /// Gets/Sets the display field type string.
        /// </summary>
        public string FieldTypeString
        {
            get
            {
                return ((ContentFieldTypes)FieldType).ToString();
            }
        }

        /// <summary>
        /// Gets/Sets the field template for display form.
        /// </summary>
        /// <remarks>
        /// This property only use to initialize the display form field tempalte in list creation. 
        /// It's the default template value for the editor field in display form. When the field template defines in &lt;tmpl&gt; element this property
        /// will be ingored.
        /// </remarks>
        public string DisplayTemplate { get; set; }

        /// <summary>
        /// Gets/Sets the field template for edit form.
        /// </summary>
        /// This property only use to initialize the edit form field tempalte in list creation. 
        /// It's the default template value for the editor field in edit form. When the field template defines in &lt;tmpl&gt; element this property
        /// will be ingored.
        public string EditTemplate { get; set; }

        /// <summary>
        /// Gets/Sets the field template file for new form.
        /// </summary>
        /// This property only use to initialize the new form field tempalte in list creation. 
        /// It's the default template value for the editor field in new form. When the field template defines in &lt;tmpl&gt; element this property
        /// will be ingored.
        public string NewTemplate { get; set; }

        /// <summary>
        /// Gets/Sets the field template file for activity form.
        /// </summary>
        /// This property only use to initialize the activity form field tempalte in list creation.         
        /// It's the default template value for the editor field in activity form. When the field template defines in &lt;tmpl&gt; element this property
        /// will be ingored.
        public string ActivityTemplate { get; set; }

        /// <summary>
        /// Gets/Sets the field template file for view.
        /// </summary>
        /// This property only use to initialize the view field tempalte in view creation.        
        /// It's the default template value for the view field for all view. When the view field template defines in &lt;tmpl&gt; element this property
        /// will be ingored.
        public string ViewTemplate { get; set; }

        /// <summary>
        /// Gets whether the field has display template definition.
        /// </summary>
        public bool HasDisplayTemlpate { get { return !string.IsNullOrEmpty(DisplayTemplate); } }

        /// <summary>
        /// Gets whether the field has edit form template definition
        /// </summary>
        public bool HasEditTemlpate { get { return !string.IsNullOrEmpty(EditTemplate); } }

        /// <summary>
        /// Gets whether the field has new form template definition
        /// </summary>
        public bool HasNewTemlpate { get { return !string.IsNullOrEmpty(NewTemplate); } }

        /// <summary>
        /// Gets whether the field has activity template definition
        /// </summary>
        public bool HasActivityTemplate { get { return !string.IsNullOrEmpty(ActivityTemplate); } }

        /// <summary>
        /// Gets whether the field has view value template definition
        /// </summary>
        public bool HasViewTemplate { get { return !string.IsNullOrEmpty(ViewTemplate); } }

        /// <summary>
        /// Identity this field do not save the values to database
        /// </summary>
        public virtual bool IsIngored { get { return false; } }

        /// <summary>
        /// Gets/Sets the field value type of the .NET framework.
        /// </summary>
        public virtual Type SystemType { get { return typeof(string); } }

        /// <summary>
        /// Gets the formatted value of the field by specified value object.
        /// </summary>
        /// <param name="value">The field value object.</param>
        /// <returns>A string contains the formatted value.</returns>
        public virtual string Format(object value)
        {
            if (value != null)
                return value.ToString();
            return "";
        }

        /// <summary>
        /// Convert the field value to xml string.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>A xml formatted string.</returns>
        public virtual string ToXmlValue(object value)
        {
            if (value != null)
                return value.ToString();
            return "";
        }

        /// <summary>
        /// Gets/Sets the client id for web browser.
        /// </summary>
        public string ClientID
        {
            get
            {
                return "_list" + Parent.ID.ToString() + "_" + Name;
            }
        }

        protected virtual void SaveTo(XElement element) { }

        /// <summary>
        /// Load field properties from xml.
        /// </summary>
        /// <param name="element">The XElement.</param>
        /// <param name="locale">The locale name.</param>
        public virtual void Load(XElement element, string locale = "")
        {
            this.Name = element.StrAttr(NAME);
            this.IsRequired = element.BoolAttr(REQUIRED);
            this.IsReadOnly = element.BoolAttr(READONLY);
            this.IsSlug = element.BoolAttr(SLUG);
            this.IsLinkToItem = element.BoolAttr(LINKTOITEM);
            this.HideInDisplayForm = element.BoolAttr(HIDEINDISP);
            this.HideInActForm = element.BoolAttr(HIDEINACT);
            this.HideInNewForm = element.BoolAttr(HIDEINNEW);
            this.HideInEditForm = element.BoolAttr(HIDEINEDIT);
            this.HideInView = element.BoolAttr(HIDEINVIEW);
            this.IsHidden = element.BoolAttr(HIDDEN);
            this.IsSortable = element.BoolAttr(SORTABLE);
            this.ItemProp = element.StrAttr(ITEM_PROP);
            this.ItemType = element.StrAttr(ITEM_TYPE);
            var ns = element.GetDefaultNamespace();
            this.ActivityTemplate = element.StrAttr(ACTTMPL);
            this.EditTemplate = element.StrAttr(EDITTMPL);
            this.NewTemplate = element.StrAttr(NEWTMPL);
            this.ViewTemplate = element.StrAttr(VIEWTMPL);
            this.DisplayTemplate = element.StrAttr(DISPTMPL);
            var title = element.ElementWithLocale(ns + TITLE, locale);
            var desc = element.ElementWithLocale(ns + DESC, locale);
            var placeHolder = element.ElementWithLocale(ns + PLACEHOLDER, locale);

            if (title == null)
                title = element.ElementWithLocale(ns + TITLE, "");

            if (desc == null)
                desc = element.ElementWithLocale(ns + DESC, "");

            if (placeHolder == null)
                placeHolder = element.ElementWithLocale(ns + PLACEHOLDER, "");

            if (title != null)
                this.Title = title.Value;

            if (desc != null)
                this.Description = desc.Value;

            if (placeHolder != null)
                this.Placeholder = placeHolder.Value;
        }

        /// <summary>
        /// Convert ContentField to XElement object.
        /// </summary>
        /// <returns>A XElement object.</returns>
        public virtual XElement Element()
        {
            //var locale = Parent.Locale;
            XNamespace ns = ContentList.DefaultNamespace;

            var element = new XElement(ns + "field", new XAttribute(NAME, Name));

            if (IsRequired)
                element.Add(new XAttribute(REQUIRED, IsRequired));

            if (IsReadOnly)
                element.Add(new XAttribute(READONLY, IsReadOnly));

            if (IsSlug)
                element.Add(new XAttribute(SLUG, IsSlug));

            if (IsLinkToItem)
                element.Add(new XAttribute(LINKTOITEM, IsLinkToItem));

            if (HideInDisplayForm)
                element.Add(new XAttribute(HIDEINDISP, HideInDisplayForm));

            if (HideInActForm)
                element.Add(new XAttribute(HIDEINACT, HideInActForm));

            if (HideInEditForm)
                element.Add(new XAttribute(HIDEINEDIT, HideInEditForm));

            if (HideInNewForm)
                element.Add(new XAttribute(HIDEINNEW, HideInNewForm));

            if (HideInView)
                element.Add(new XAttribute(HIDEINVIEW, HideInView));

            if (IsHidden)
                element.Add(new XAttribute(HIDDEN, IsHidden));

            if (!string.IsNullOrEmpty(ItemType))
                element.Add(new XAttribute(ITEM_TYPE, ItemType));

            if (!string.IsNullOrEmpty(ItemProp))
                element.Add(new XAttribute(ITEM_PROP, ItemProp));

            if (!string.IsNullOrEmpty(EditTemplate))
                element.Add(new XAttribute(EDITTMPL, EditTemplate));

            if (!string.IsNullOrEmpty(NewTemplate))
                element.Add(new XAttribute(NEWTMPL, NewTemplate));

            if (!string.IsNullOrEmpty(DisplayTemplate))
                element.Add(new XAttribute(DISPTMPL, DisplayTemplate));

            if (!string.IsNullOrEmpty(ViewTemplate))
                element.Add(new XAttribute(VIEWTMPL, ViewTemplate));

            // if (IsFilterable)
            // element.Add(new XAttribute(FILTERABLE, IsFilterable));

            if (IsSortable)
                element.Add(new XAttribute(SORTABLE, IsSortable));

            if (!string.IsNullOrEmpty(Title))
                element.Add(new XElement(ns + TITLE, Title));

            if (!string.IsNullOrEmpty(Description))
                element.Add(new XElement(ns + DESC, Description));

            if (!string.IsNullOrEmpty(Placeholder))
                element.Add(new XElement(ns + PLACEHOLDER, Placeholder));

            SaveTo(element);

            return element;
        }
    }
}
