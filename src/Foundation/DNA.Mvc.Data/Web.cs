//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Represents the Web site object.
    /// </summary>
    public class Web : DNA.Web.IWeb, IAddress, IOrganization
    {
        public Web()
        {
            Created = DateTime.UtcNow;
            MostOnlined = DateTime.UtcNow;
            Type = 0;
            //CacheDuration = 0;
            IsEnabled = true;
            Dir = "ltr";
        }

        /// <summary>
        /// Gets/Sets the web site id.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets/Sets the web site unique name. Top site is "home"
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Get/Sets the display title text of the website.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets/Sets the current theme name.
        /// </summary>
        public virtual string Theme { get; set; }

        /// <summary>
        /// Gets/Sets the timezone name of the website.
        /// </summary>
        public virtual string TimeZone { get; set; }

        [Obsolete]
        public virtual int Type { get; set; }

        [Obsolete]
        public virtual string Copyright { get; set; }

        /// <summary>
        /// Gets/Sets the web creation date.
        /// </summary>
        public virtual DateTime Created { get; set; }

        /// <summary>
        /// Gets/Sets the css text for all page belong the web.
        /// </summary>
        public virtual string CssText { get; set; }

        /// <summary>
        /// Gets/Sets the additional data json formated string.
        /// </summary>
        public virtual string Data { get; set; }

        /// <summary>
        /// Gets/Sets the default locale name.
        /// </summary>
        public virtual string DefaultLocale { get; set; }

        /// <summary>
        /// Gets/Sets the default url of the web.
        /// </summary>
        public virtual string DefaultUrl { get; set; }

        /// <summary>
        /// Gets/Sets the description text.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets/Sets the localization data of the web.
        /// </summary>
        public virtual string LocData { get; set; }

        /// <summary>
        /// Indicates whether the web is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; set; }

        /// <summary>
        /// Gets/Sets the web default layout view.
        /// </summary>
        public virtual string Layout { get; set; }

        /// <summary>
        /// Gets/Sets the logo image url.
        /// </summary>
        public virtual string LogoImageUrl { get; set; }

        /// <summary>
        /// Gets/Sets the master view.
        /// </summary>
        public virtual string MasterName { get; set; }

        /// <summary>
        /// Gets/Sets the most user online date.
        /// </summary>
        public virtual DateTime MostOnlined { get; set; }

        /// <summary>
        /// Gets/Sets the most user online count.
        /// </summary>
        public virtual int MostOnlineUserCount { get; set; }

        /// <summary>
        /// Gets/Sets the owner user name
        /// </summary>
        public virtual string Owner { get; set; }

        /// <summary>
        /// Gets/Sets the shortcut icon url.
        /// </summary>
        public virtual string ShortcutIconUrl { get; set; }

        /// <summary>
        /// Gets/Sets the text direction.
        /// </summary>
        public virtual string Dir { get; set; }

        /// <summary>
        /// Gets/Sets the installed solution  names.
        /// </summary>
        public virtual string InstalledSolutions { get; set; }

        /// <summary>
        /// Gets/Sets content lists of the website.
        /// </summary>
        public virtual ICollection<ContentList> Lists { get; set; }

        /// <summary>
        /// Gets/Sets webpages of the website.
        /// </summary>
        public virtual ICollection<WebPage> Pages { get; set; }

        /// <summary>
        /// Gets/Sets the moved urls of the website.
        /// </summary>
        public virtual ICollection<MovedUrl> MovedUrls { get; set; }

        #region Extension properties

        //  private WebMasterTools masterTools;
        private IDictionary<string, object> properties;

        /// <summary>
        /// Gets the additional setting values for the website.
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get
            {
                if (properties == null)
                {
                    if (!string.IsNullOrEmpty(Data))
                    {
                        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        properties = (IDictionary<string, object>)serializer.DeserializeObject(Data);
                    }
                    else
                        properties = new Dictionary<string, object>();
                }
                return properties;
            }
        }

        private void SetValue(string key, object value)
        {
            if (Properties.ContainsKey(key))
                Properties[key] = value;
            else
                Properties.Add(key, value);
            this.Data = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(Properties);
        }

        private string GetValue(string key)
        {
            if (Properties.ContainsKey(key))
                return Properties[key] as string;
            else
                return "";
        }

        private T GetValue<T>(string key)
        {
            if (Properties.ContainsKey(key))
                return (T)Properties[key];
            return default(T);
        }

        public LogoLayouts LogoLayout
        {
            get
            {
                return GetValue<LogoLayouts>("LogoLayout");
            }
            set
            {
                SetValue("LogoLayout", value);
            }
        }

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (string.IsNullOrEmpty(this.TimeZone))
                    return TimeZoneInfo.Local;
                else
                    return TimeZoneInfo.FindSystemTimeZoneById(this.TimeZone);
            }
        }

        ///// <summary>
        ///// Sets the extensions allow to upload to the website
        ///// </summary>
        ///// <param name="extension"></param>
        ///// <returns></returns>
        //public bool IsAllowUpload(string extension)
        //{
        //    //if (string.IsNullOrEmpty(this.AllowExtensions))
        //    //    return false;

        //    //string[] exts = AllowExtensions.ToLower().Split(new char[] { '|' });
        //    //return exts.Contains(extension.ToLower());
        //    return true;
        //}

        //public bool IsTrusted(Uri appUrl, Uri url)
        //{

        //    if (appUrl.Authority.Equals(url.Authority, StringComparison.OrdinalIgnoreCase))
        //        return true;

        //    if (MasterTools.PreventUntrustLinks)
        //    {
        //        if (!string.IsNullOrEmpty(MasterTools.TrustDomains))
        //            return MasterTools.GetTrustDomains().Contains(url.Authority.ToLower());
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// Indicates whether current web is the root one of the application.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return false;

                return Name.Equals("home", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets the web root folder path
        /// </summary>
        public string WebFolder
        {
            get
            {
                return "/webshared/" + Name + "/";
            }
        }

        /// <summary>
        /// Popuplate the data from IWeb object
        /// </summary>
        /// <param name="webData"></param>
        /// <remarks>
        /// This method only populate the simple type properties.The WebPages will not populate from IWeb
        /// </remarks>
        public void Popuplate(IWeb webData)
        {
            this.DefaultLocale = webData.DefaultLocale;
            this.Copyright = string.IsNullOrEmpty(webData.Copyright) ? "" : webData.Copyright;
            this.CssText = string.IsNullOrEmpty(webData.CssText) ? "" : webData.CssText;
            this.LogoImageUrl = string.IsNullOrEmpty(webData.LogoImageUrl) ? "" : webData.LogoImageUrl;
            this.DefaultUrl = string.IsNullOrEmpty(webData.DefaultUrl) ? "" : webData.DefaultUrl;
            this.ShortcutIconUrl = string.IsNullOrEmpty(webData.ShortcutIconUrl) ? "" : webData.ShortcutIconUrl;
            this.Title = webData.Title;
            this.Description = webData.Description;
            this.Theme = webData.Theme;
            //if (includePages && this.Pages != null)
            //this.WebPages = (IEnumerable<IWebPage>)this.Pages;
        }

        #endregion

        IEnumerable<IWebPage> IWeb.Pages
        {
            get
            {
                return this.Pages;
            }
        }

        #region Impletement IAddress inerface

        string IAddress.Country
        {
            get
            {
                return GetValue("Country");
            }
            set
            {
                SetValue("Country", value);
            }
        }

        string IAddress.City
        {
            get
            {
                return GetValue("City");
            }
            set
            {
                SetValue("City", value);
            }
        }

        string IAddress.Email
        {
            get
            {
                return GetValue("Email");
            }
            set
            {
                SetValue("Email", value);
            }
        }

        string IAddress.Fax
        {
            get
            {
                return GetValue("Fax");
            }
            set
            {
                SetValue("Fax", value);
            }
        }

        string IAddress.Mobile
        {
            get
            {
                return GetValue("Mobile");
            }
            set
            {
                SetValue("Mobile", value);
            }
        }

        string IAddress.Name
        {
            get
            {
                return this.Owner;
            }
            set
            {
                //Do nothing 
            }
        }

        string IAddress.State
        {
            get
            {
                return GetValue("State");
            }
            set
            {
                SetValue("State", value);
            }
        }

        string IAddress.Street
        {
            get
            {
                return GetValue("Street");
            }
            set
            {
                SetValue("Street", value);
            }
        }

        string IAddress.Tel
        {
            get
            {
                return GetValue("Tel");
            }
            set
            {
                SetValue("Tel", value);
            }
        }

        string IAddress.Zip
        {
            get
            {
                return GetValue("Zip");
            }
            set
            {
                SetValue("Zip", value);
            }
        }

        string IAddress.Company
        {
            get { return this.Title; }
            set { this.Title = value; }
        }

        double IAddress.Latitude
        {
            get
            {
                return GetValue<double>("Latitude");
            }
            set
            {
                SetValue("Latitude", value);
            }
        }

        double IAddress.Longitude
        {
            get
            {
                return GetValue<double>("Longitude");
            }
            set
            {
                SetValue("Longitude", value);
            }
        }

        string IAddress.FirstName
        {
            get
            {
                return GetValue("FirstName");
            }
            set
            {
                SetValue("FirstName", value);
            }
        }

        string IAddress.LastName
        {
            get
            {
                return GetValue("LastName");
            }
            set
            {
                SetValue("LastName", value);
            }
        }

        #endregion

        #region Impletement IOrganization interface

        string IOrganization.Name
        {
            get
            {
                return this.Title;
            }
            set
            {
                this.Title = value;
            }
        }

        string IOrganization.CurrencyCode
        {
            get
            {
                return GetValue("CurrencyCode");
            }
            set
            {
                SetValue("CurrencyCode", value);
            }
        }

        string IOrganization.LegalName
        {
            get
            {
                return GetValue("LegalName");
            }
            set
            {
                SetValue("LegalName", value);
            }
        }

        string IOrganization.Logo
        {
            get
            {
                return this.LogoImageUrl;
            }
            set
            {
                this.LogoImageUrl = value;
            }
        }

        string IOrganization.Brand
        {
            get
            {
                return GetValue("Brand");
            }
            set
            {
                SetValue("Brand", value);
            }
        }

        string IOrganization.TaxID
        {
            get
            {
                return GetValue("TaxID");
            }
            set
            {
                SetValue("TaxID", value);
            }
        }

        string IOrganization.Description
        {
            get
            {
                return this.Description;
            }
            set
            {
                this.Description = value;
            }
        }

        #endregion


    }
}
