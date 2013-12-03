//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Repesents the field editor defined in form fields.
    /// </summary>
    public class ContentEditorField : ContentField
    {
        private bool showLabel = true;

        /// <summary>
        /// Gets the parent form object.
        /// </summary>
        public ContentFormDecorator ParentForm { get; private set; }

        /// <summary>
        /// Gets the parent list 
        /// </summary>
        public new ContentListDecorator Parent { get; private set; }

        /// <summary>
        /// Returns the original list field instance
        /// </summary>
        public ContentField Field { get { return Parent.Fields[this.Name]; } }

        /// <summary>
        /// Gets/Sets whether the field show it's title on the left of the editor 
        /// </summary>
        public bool ShowLabel
        {
            get
            {
                return showLabel;
            }
            set
            {
                showLabel = value;
            }
        }

        /// <summary>
        /// Identity show this field's value as caption
        /// </summary>
        public bool IsCaption { get; set; }

        /// <summary>
        /// Initializes a new instance of ContentEditorField class.
        /// </summary>
        /// <param name="form">The form object.</param>
        /// <param name="field">The content field.</param>
        public ContentEditorField(ContentFormDecorator form, ContentField field)
        {
            this.ParentForm = form;
            this.Parent = form.Parent;
            field.CopyTo(this, "Parent");
            Template = new ContentTemplate();

            switch ((ContentFormTypes)form.FormType)
            {
                case ContentFormTypes.Display:
                    if (field.HasDisplayTemlpate)
                        Template.Source = field.DisplayTemplate;
                    break;
                case ContentFormTypes.Activity:
                    if (field.HasActivityTemplate)
                        Template.Source = field.ActivityTemplate;
                    break;
                case ContentFormTypes.Edit:
                    if (field.HasEditTemlpate)
                        Template.Source = field.EditTemplate;
                    break;
                case ContentFormTypes.New:
                    if (field.HasNewTemlpate)
                        Template.Source = field.NewTemplate;
                    break;
            }
        }

        /// <summary>
        /// Gets/Sets the editor field template
        /// </summary>
        public ContentTemplate Template { get; set; }

        public override XElement Element()
        {
            XNamespace ns = ContentList.DefaultNamespace;
            var fieldEle = new XElement(ns + "field", new XAttribute("name", Name));

            if (!string.IsNullOrEmpty(Template.Source) || !string.IsNullOrEmpty(Template.Text))
                fieldEle.Add(Template.Element("tmpl"));

            if (IsCaption)
                fieldEle.Add(new XAttribute("caption", true));

            if (IsHidden)
                fieldEle.Add(new XAttribute("hidden", true));

            //if (!ShowLabel)
            fieldEle.Add(new XAttribute("showLabel", ShowLabel));

            return fieldEle;
        }
    }
}
