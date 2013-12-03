//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web;

namespace DNA.Xml.Solutions
{
    public class PageTemplatePackage : PackageBase<PageElement>
    {
        public PageTemplatePackage(string path) : base(path) { }

        public override string DefaultNamespace
        {
            get
            {
                return "http://www.dotnetage.com/XML/Schema/page";
            }
        }

        public override PageElement Locale(string lang)
        {
            var pkg = this.Model.Clone();

            if (!string.IsNullOrEmpty(lang))
                GetLocalePageElement(pkg, "", lang);

            return pkg;
        }

        private void GetLocalePageElement(PageElement parentElement, string path, string lang)
        {
            ///var parentPath = string.IsNullOrEmpty(path) ? "" : path + "/";
            var title = GetLocalizableElement(path + "title", lang);
            if (title != null) parentElement.Title = title;

            var desc = GetLocalizableElement(path + "description", lang);
            if (desc != null) parentElement.Description = desc;

            var keywords = GetLocalizableElement(path + "keywords", lang);
            if (keywords != null) parentElement.Keywords = keywords;

            var imgLink = GetImageLinkElement(path + "image", lang);
            if (imgLink != null) parentElement.Image = imgLink;

            var icon = GetImageLinkElement(path + "icon", lang);
            if (icon != null) parentElement.Icon = icon;

            if (parentElement.Children != null)
            {
                parentElement.Children = new System.Collections.Generic.List<PageElement>();
                foreach (var p in parentElement.Children)
                {
                    var pageClone = p.Clone();
                    GetLocalePageElement(pageClone, path + "pages/page/", lang);
                    parentElement.Children.Add(pageClone);
                }
            }

            if (parentElement.Widgets != null)
            {
                parentElement.Widgets = new System.Collections.Generic.List<WidgetDataElement>();
                foreach (var w in parentElement.Widgets)
                {
                    var widgetClone = w.Clone();
                    var widgetTitle = GetLocalizableElement(path + "widgets/widget/title", lang);
                    if (widgetTitle != null) widgetClone.Title = widgetTitle;
                    var linkNode = GetLocalizedNode(path + "widgets/widget/link", lang);
                    if (linkNode != null) widgetClone.Link = new LinkElement()
                    {
                        Language = linkNode.Attributes["lang"] != null ? linkNode.Attributes["lang"].Value : "en-us",
                        Rel = linkNode.Attributes["rel"] != null ? linkNode.Attributes["rel"].Value : "",
                        Title = linkNode.Attributes["dir"] != null ? linkNode.Attributes["dir"].Value : "ltr",
                        Source = linkNode.Attributes["src"] != null ? linkNode.Attributes["src"].Value : "",
                        Target = linkNode.Attributes["target"] != null ? linkNode.Attributes["target"].Value : ""
                    };
                }
            }
        }
    }
}
