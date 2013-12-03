//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to ContentForm object model. 
    /// </summary>
    public class ContentFormDecorator : ContentForm
    {
        private ContentListDecorator parent = null;
        private ContentEditorFieldCollection fields = null;

        private string[] roles;

        internal ContentForm Model { get; set; }

        private IDataContext Context { get; set; }

        /// <summary>
        /// Gets the form editor field collection. 
        /// </summary>
        public ContentEditorFieldCollection Fields
        {
            get
            {
                if (fields == null)
                    fields = new ContentEditorFieldCollection(Context, this);
                return fields;
            }
        }

        /// <summary>
        /// Gets the access roles of the form.
        /// </summary>
        public new string[] Roles
        {
            get
            {
                if (roles == null)
                {
                    if (!string.IsNullOrEmpty(Model.Roles))
                    {
                        roles = Model.Roles.Split(',');
                    }
                    else
                        roles = new string[0];
                }
                return roles;
            }
            set
            {
                roles = value;
            }
        }

        /// <summary>
        /// Gets the setting url of the form.
        /// </summary>
        public string SettingUrl
        {
            get
            {
                return string.Format("~/dashboard/{0}/{1}/{2}/forms/{3}", this.Parent.Web.Name, this.Parent.Locale, this.Parent.Name, this.FormTypeString);
            }
        }

        /// <summary>
        /// Gets the parent list ojbect.
        /// </summary>
        public new ContentListDecorator Parent
        {
            get
            {
                if (parent == null)
                {
                    parent = new ContentListDecorator(Context, Model.Parent);
                }
                return parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ContentFormDecorator class.
        /// </summary>
        /// <param name="context">The data context.</param>
        /// <param name="form"></param>
        public ContentFormDecorator(IDataContext context, ContentForm form)
        {
            Model = form;
            Context = context;
            form.CopyTo(this, "Parent", "Roles");
        }

        /// <summary>
        /// Save form changes to database.
        /// </summary>
        /// <returns>A boolean value identity whether save is successful.</returns>
        public bool Save()
        {
            this.CopyTo(Model, "Parent", "Roles");
            if (this.roles != null && this.roles.Length > 0)
                Model.Roles = string.Join(",", this.roles);
            else
                Model.Roles = "";
            Context.Update(Model);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Gets  the form whether has template definition
        /// </summary>
        public bool HasTemplate
        {
            get
            {
                return Body != null && (!string.IsNullOrEmpty(Body.Source) || !string.IsNullOrEmpty(Body.Text));
            }
        }

        /// <summary>
        /// Format form url by specified params.
        /// </summary>
        /// <param name="urlparams">The url params.</param>
        /// <returns>A string contains the form url.</returns>
        public string Url(params object[] urlparams)
        {
            switch ((ContentFormTypes)FormType)
            {
                case ContentFormTypes.Display:
                    return string.Format("~/{0}/{1}/lists/{2}/items/{3}.html", Parent.Web.Name, this.Parent.Locale, Parent.Name, urlparams[0]);
                case ContentFormTypes.Edit:
                    return string.Format("~/{0}/{1}/lists/{2}/edit/{3}.html", Parent.Web.Name, this.Parent.Locale, Parent.Name, urlparams[0]);
                default:
                    return string.Format("~/{0}/{1}/lists/{2}/new.html", Parent.Web.Name, this.Parent.Locale, Parent.Name);
            }
        }

        public override XElement Element()
        {
            XNamespace ns = ContentList.DefaultNamespace;
            var formEl = base.Element();
            if (Fields != null && Fields.Count > 0)
                formEl.Add(Fields.Element());
            return formEl;
        }

        /// <summary>
        /// Gets the form type string.
        /// </summary>
        public string FormTypeString
        {
            get
            {
                return ((ContentFormTypes)this.FormType).ToString();
            }
        }

        /// <summary>
        /// Identity the current user whether authorized.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>If authorized returns true.</returns>
        public bool IsAuthorized(HttpContextBase context)
        {
            if (AllowAnonymous)
                return true;

            if (context.Request.IsAuthenticated)
            {
                if (context.User.Identity.Name.Equals(this.Parent.Owner))
                    return true;

                if (Roles != null && Roles.Length > 0)
                {
                    foreach (var role in this.Roles)
                    {
                        if (App.Get().User.IsInRole(role))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Set editor fields and display orders.
        /// </summary>
        /// <param name="orders">The ordered field names</param>
        public void SetFields(string[] orders)
        {
            var tmpFields = Fields.ToList();
            Fields.Clear();

            foreach (var f in orders)
            {
                var editor = tmpFields.FirstOrDefault(e => e.Name.Equals(f));
                if (editor == null)
                    editor = new ContentEditorField(this, Parent.Fields[f]);
                Fields.Add(editor);
            }

            Fields.Save();
        }

        /// <summary>
        /// Gets the css class name of the form.
        /// </summary>
        public string CssClass
        {
            get
            {
                var cls = this.Parent.BaseType + " " + this.Parent.Name + " d-form-" + this.FormTypeString.ToLower();
                return cls;
            }
        }
    }
}
