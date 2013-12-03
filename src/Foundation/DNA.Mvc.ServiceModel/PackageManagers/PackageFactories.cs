//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Xml.Solutions;
using DNA.Xml.Widgets;

namespace DNA.Web.ServiceModel
{
    public class ThemePackageFactory : IPackageFactory<ThemePackage, Theme>
    {
        private const string THEME_URI = "~/content/themes/";

        public ThemePackage Create(string path)
        {
            var pkg = new ThemePackage(path);
            pkg.BaseUrl = THEME_URI;
            //var basePath = HttpContext.Current.Server.MapPath(THEME_URI+"");
            return pkg;
        }
    }

    public class SolutionPackageFactory : IPackageFactory<SolutionTemplatePackage, WebElement>
    {
        private const string SOLUTION_URI = "~/content/packages/";

        public SolutionTemplatePackage Create(string path)
        {
            var pkg = new SolutionTemplatePackage(path);
            pkg.BaseUrl = SOLUTION_URI;
            return pkg;
        }
    }

    public class ContentPackageFactory : IPackageFactory<ContentPackage, ContentList>
    {
        private const string CONTENT_URI = "~/content/types/";

        public ContentPackage Create(string path)
        {
            var pkg = new ContentPackage(path);
            pkg.BaseUrl = CONTENT_URI;
            return pkg;
        }
    }

    public class PageTemplatePackageFactory : IPackageFactory<PageTemplatePackage, PageElement>
    {
        private const string CONTENT_URI = "~/content/pages/";

        public PageTemplatePackage Create(string path)
        {
            var pkg = new PageTemplatePackage(path);
            pkg.BaseUrl = CONTENT_URI;
            return pkg;
        }
    }

    public class WidgetPackageFactory : IPackageFactory<WidgetPackage, WidgetElement>
    {
        private const string WIDGET_URI = "~/content/widgets/";

        public WidgetPackage Create(string path)
        {
            var pkg = new WidgetPackage(path);
            pkg.BaseUrl = WIDGET_URI;
            return pkg;
        }
    }
}
