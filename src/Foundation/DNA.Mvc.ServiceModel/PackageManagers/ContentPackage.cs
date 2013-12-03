//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    public class ContentPackage : PackageBase<ContentList>
    {
        private ContentList model = null;

        public ContentPackage(string path) : base(path) { }

        public override string DefaultNamespace
        {
            get
            {
                return "http://www.dotnetage.com/XML/Schema/contents";
            }
        }

        public override ContentList Model
        {
            get
            {
                if (model == null)
                    model = Locale("");
                return model;
            }
        }

        public override ContentList Locale(string lang)
        {
            var contentElement = XElement.Parse(ConfigXml);
            var list = new ContentList();
            list.Load(contentElement, lang);

            if (HasResouces)
            {
                //Use resource file localization
                var ns = contentElement.GetDefaultNamespace();

                //title
                var locTitle = GetString(ns + "title", contentElement, lang);
                if (!string.IsNullOrEmpty(locTitle))
                    list.Title = locTitle;

                //desc
                var locDesc = GetString(ns + "description", contentElement, lang);
                if (!string.IsNullOrEmpty(locDesc))
                    list.Description = locDesc;

                #region fields localization
                var FIELDS = "fields";
                var factory = new DNA.Web.Contents.ContentFieldFactory();
                var fields = contentElement.Element(ns + "fields")
                                                 .Elements()
                                                 .Select(f =>
                                                 {
                                                     var field = factory.Create(f, lang);
                                                     field.Parent = list;

                                                     //title
                                                     var _title = GetString(ns + "title", f, lang);
                                                     if (!string.IsNullOrEmpty(_title))
                                                         field.Title = _title;

                                                     //desc
                                                     var _desc = GetString(ns + "description", f, lang);
                                                     if (!string.IsNullOrEmpty(_desc))
                                                         field.Description = _desc;

                                                     //placeholder
                                                     var _placeholder = GetString(ns + "placeHolder", f, lang);
                                                     if (!string.IsNullOrEmpty(_placeholder))
                                                         field.Placeholder = _placeholder;

                                                     return field;
                                                 });

                var fieldElements = new XElement(FIELDS, new XAttribute("xmlns", DefaultNamespace),
                    fields.Select(f => f.Element()));

                list.FieldsXml = fieldElements.OuterXml();

                #endregion

                #region views localization
                var viewElements = contentElement.Descendants(ns + "view");
                foreach (var viewEle in viewElements)
                {
                    var viewName = viewEle.StrAttr("name");
                    var view = list.Views.FirstOrDefault(v => v.Name.Equals(viewName));
                    if (view != null)
                    {
                        //title
                        var vTitle = GetString(ns + "title", viewEle, lang);
                        if (!string.IsNullOrEmpty(vTitle))
                            view.Title = vTitle;

                        //desc
                        var vDesc = GetString(ns + "description", viewEle, lang);
                        if (!string.IsNullOrEmpty(vDesc))
                            view.Description = vDesc;
                    }
                }
                #endregion

                #region forms localization
                var formElements = contentElement.Descendants(ns + "form");
                foreach (var formEle in formElements)
                {
                    var formType = (int)((ContentFormTypes)Enum.Parse(typeof(ContentFormTypes), formEle.StrAttr("type")));
                    var form = list.Forms.FirstOrDefault(f => f.FormType.Equals(formType));
                    if (form != null)
                    {
                        //title
                        var fTitle = GetString(ns + "title", formEle, lang);
                        if (!string.IsNullOrEmpty(fTitle))
                            form.Title = fTitle;

                        //desc
                        var fDesc = GetString(ns + "description", formEle, lang);
                        if (!string.IsNullOrEmpty(fDesc))
                            form.Description = fDesc;
                    }
                }
                #endregion
            }

            return list;
        }

        private string GetString(XName name, XElement element, string locale)
        {
            var titleEle = element.Element(name);
            if (titleEle != null)
            {
                var resKey = titleEle.StrAttr("resKey");
                if (!string.IsNullOrEmpty(resKey))
                    return Loc(resKey, locale);
            }
            return "";
        }

        public override string Loc(string key, System.Globalization.CultureInfo culture)
        {
            var _res = base.Loc(key, culture);
            if (string.IsNullOrEmpty(_res))
            {
                try
                {
                    _res = (string)HttpContext.GetGlobalResourceObject("Commons", key);

                    if (string.IsNullOrEmpty(_res))
                        _res = (string)HttpContext.GetGlobalResourceObject("Contacts", key);
                }
                catch
                {
                    return "";
                }
            }
            return _res;
        }

        public string Icon
        {
            get
            {
                var file = this.ResolveFileName("images/icon.png");
                if (System.IO.File.Exists(file))
                    return this.ResolveUri("images/icon.png");
                else
                    return "~/content/images/list.png";
            }
        }

    }
}
