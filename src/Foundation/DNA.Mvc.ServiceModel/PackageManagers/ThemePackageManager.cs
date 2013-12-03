//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.IO;
using System.Web;

namespace DNA.Web.ServiceModel
{
    public sealed class ThemePackageManager : PackageManager<ThemePackage, ThemePackageFactory, Theme>
    {
        private const string THEME_PKG_PATH = "~/content/themes/";

        public ThemePackageManager() : this(new HttpContextWrapper(HttpContext.Current)) { }

        public ThemePackageManager(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            Init(httpContext.Server.MapPath(THEME_PKG_PATH));
        }

        public ThemePackageManager(string path) { Init(path); }

        public override void Init(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException();

            InstalledPath = path;
            var dirs = Directory.GetDirectories(path);
            var packages = new PackageCollection<ThemePackage, Theme>();
            var factory = new ThemePackageFactory();
            foreach (var dir in dirs)
            {
                try
                {
                    var dirInfo = new DirectoryInfo(dir);
                    if (dirInfo.Name.Equals("base", StringComparison.OrdinalIgnoreCase))
                        continue;

                    packages.Add(factory.Create(dir));
                }
                catch { continue; }
            }

            Packages = packages;
        }
    }
}
