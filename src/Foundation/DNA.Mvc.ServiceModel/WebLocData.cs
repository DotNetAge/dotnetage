//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the localization data.
    /// </summary>
    public class WebLocData
    {
        /// <summary>
        /// Gets / Sets locale name.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets/Sets web title text.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Gets/Sets web description text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets global css text.
        /// </summary>
        public string CssText { get; set; }

        /// <summary>
        /// Gets/Sets locale default url.
        /// </summary>
        public string DefaultUrl { get; set; }

        /// <summary>
        /// Gets/Sets theme name.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Gets/Sets the timezone.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets/Sets the web logo image url.
        /// </summary>
        public string LogoImageUrl { get; set; }

        /// <summary>
        /// Gets/Sets master file name.
        /// </summary>
        public string MasterName { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the WebLocData class.
        /// </summary>
        public WebLocData() { }
        
        /// <summary>
        /// Initializes a new instance of the WebLocData with given web model.
        /// </summary>
        /// <param name="web">The web model.</param>
        public WebLocData(Web web) { this.Popuplate(web); }

        /// <summary>
        /// Popuplate data from given web instance.
        /// </summary>
        /// <param name="web">The web model.</param>
        public void Popuplate(Web web)
        {
            this.Locale = web.DefaultLocale;
            this.Title = string.IsNullOrEmpty(web.Title) ? "" : web.Title;
            this.Description = string.IsNullOrEmpty(web.Description) ? "" : web.Description;
            this.CssText = string.IsNullOrEmpty(web.CssText) ? "" : web.CssText;
            this.DefaultUrl = string.IsNullOrEmpty(web.DefaultUrl) ? "" : web.DefaultUrl;
            this.Theme = string.IsNullOrEmpty(web.Theme) ? "default" : web.Theme;
            this.TimeZone = string.IsNullOrEmpty(web.TimeZone) ? "" : web.TimeZone;
            this.LogoImageUrl = string.IsNullOrEmpty(web.LogoImageUrl) ? "" : web.LogoImageUrl;
            this.MasterName =  string.IsNullOrEmpty(web.MasterName) ? "" :web.MasterName;    
        }
    }
}
