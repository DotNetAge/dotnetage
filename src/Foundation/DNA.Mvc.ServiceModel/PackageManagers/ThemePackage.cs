//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.IO;

namespace DNA.Web.ServiceModel
{
    public class ThemePackage : PackageBase<Theme>
    {
        private Theme model = null;

        public ThemePackage(string path) : base(path) { }

        protected override void Init(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);

            var curDir = new DirectoryInfo(path);
            Name = curDir.Name;
            this.InstalledPath = path;
            configurationFile = path + "\\" + configurationFileName;

            if (!File.Exists(configurationFile))
            {
                var theme = new Theme
                {
                    Name = this.Name,
                    Title = this.Name,
                    Version = "1.0.0"
                };
                theme.Element().Save(configurationFile);
            }
        }

        public override Theme Model
        {
            get
            {
                if (model == null)
                    model = this.Locale("");

                return model;
            }
        }

        public override Theme Locale(string lang)
        {
            return Theme.Parse(ConfigXml, lang);
        }
    }
}
