//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DNA.Web.Controllers
{
    /// <summary>
    /// Provides the actions of the system settings of the website.
    /// </summary>
    public class SysController : Controller
    {
        //OutputCache(VaryByParam = "*", Duration = 3600)
        [Authorize, Loc]
        public ActionResult Menus(string locale = "en-US", int type = 0, string requestPath = "", bool localized = false)
        {
            App.Get().SetCulture(locale);

            ViewBag.MenuType = type;
            ViewBag.RequestPath = requestPath;
            ViewBag.Localized = localized;
            switch (type)
            {
                case 0: ViewData.Model = Dashboard.SiteGroups.ToList();
                    break;
                case 1: ViewData.Model = Dashboard.HostGroups.ToList();
                    break;
                default:
                    ViewData.Model = Dashboard.MyGroups;
                    break;
            }

            return PartialView();
        }

        [Authorize, Loc]
        public ActionResult Localize()
        {
            return PartialView();
        }

        [Loc]
        public ActionResult Languages(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView();
        }

        public ActionResult SelectLang(string website, string locale, string returnUrl)
        {
            var web = App.Get().Webs[website];
            //var returnUrl="";
            var cookieName = DNA.Web.ServiceModel.WebContext.UserLocaleCookieName;

            if (Response.Cookies[cookieName] == null)
                Response.Cookies.Add(new System.Web.HttpCookie(cookieName, locale));
            else
                Response.Cookies[cookieName].Value = locale;

            if (!object.ReferenceEquals(web, null) && !returnUrl.Contains("dashboard/"))
            {
                if (web.HasLocale(locale))
                    returnUrl = web.GetDefaultUrl(locale);
            }

            return Redirect(returnUrl);
        }

        [HostDashboard(Text = "Settings",
            Sequence = 1,
            RouteName = "dna_global_settings",
            Icon = "d-icon-cogs",
            ResBaseName = "Managements",
            ResKey = "GlobalSettings"
            )]
        public ActionResult GlobalSettings()
        {
            return View(App.Settings);
        }

        [HostDashboard, ValidateAntiForgeryToken, HttpPost]
        public ActionResult GlobalSettings(FormCollection forms)
        {
            foreach (var key in forms.AllKeys)
            {
                if (WebConfigurationManager.AppSettings[key] != null)
                {
                    if (WebConfigurationManager.AppSettings[key] != forms[key])
                        DnaConfig.UpdateWebConfig("configuration/appSettings/add[@key=\"" + key + "\"]", "value", forms[key]);
                }
                //bool forceEmailValidation,bool allowNewUser,
                if (!forms.AllKeys.Contains("ForceEmailValidation"))
                    DnaConfig.UpdateWebConfig("configuration/appSettings/add[@key=\"ForceEmailValidation\"]", "value", "False");

                if (!forms.AllKeys.Contains("AllowNewUser"))
                    DnaConfig.UpdateWebConfig("configuration/appSettings/add[@key=\"AllowNewUser\"]", "value", "False");

                if (!forms.AllKeys.Contains("EnableSitemenu"))
                    DnaConfig.UpdateWebConfig("configuration/appSettings/add[@key=\"EnableSitemenu\"]", "value", "False");

                if (!forms.AllKeys.Contains("WWWResolved"))
                    DnaConfig.UpdateWebConfig("configuration/appSettings/add[@key=\"WWWResolved\"]", "value", "False");
            }
            return View(App.Settings);
        }


        [SiteDashboard(ResKey = "Categories", Sequence = 2, RouteName = "dna_webcategories", Icon = "d-icon-target-2")]
        public ActionResult Categories()
        {
            return View();
        }

        [SiteDashboard(ResKey = "SiteSettings", ResBaseName = "Managements", Sequence = 0, RouteName = "dna_sitesettings", Icon = "d-icon-globe")]
        [SecurityAction("Management base", "Manage site settings", "Allows users can view the common site settings.",
            TitleResName = "SA_ManageSiteSettings",
            DescResName = "SA_ManageSiteSettingsDesc",
            PermssionSetResName = "SA_Managementbase"
            )]
        public ActionResult SiteSettings()
        {
            var web = AppModel.Get().CurrentWeb;
            //var defaultLocale = web.
            //if (defa
            return View(web);
        }

        [Authorize, HttpPost, ValidateInput(false), SiteDashboard]
        public ActionResult SiteSettings(string title, string description, string logoImageUrl,
            string timeZone, string theme, string defaultLocale,
            string legalName, string taxID, string brand, string currencyCode,
            string email, string tel, string mobile, string fax, string country, string state, string city, string street)
        {
            if (App.Get().Context.HasPermisson(this, "SiteSettings"))
            {
                var web = App.Get().CurrentWeb;
                if (title != null)
                    web.Title = title;

                if (description != null)
                    web.Description = description;

                if (logoImageUrl != null)
                    web.LogoImageUrl = logoImageUrl;

                if (theme != null)
                    web.Theme = theme;

                if (timeZone != null)
                    web.TimeZone = timeZone;

                if (!string.IsNullOrEmpty(defaultLocale) && !defaultLocale.Equals(web.DefaultLocale))
                {
                    //web.SwitchLocale(defaultLocale);
                    web.DefaultLocale = defaultLocale;
                }

                #region organization

                ((IOrganization)web).LegalName = legalName;
                ((IOrganization)web).TaxID = taxID;
                ((IOrganization)web).Brand = brand;
                ((IOrganization)web).CurrencyCode = string.IsNullOrEmpty(currencyCode) ? "USD" : currencyCode;
                #endregion

                #region default address

                ((IAddress)web).Email = email;
                ((IAddress)web).Tel = tel;
                ((IAddress)web).Mobile = mobile;
                ((IAddress)web).Fax = fax;
                ((IAddress)web).Country = country;
                ((IAddress)web).State = state;
                ((IAddress)web).City = city;
                ((IAddress)web).Street = street;

                #endregion

                web.Save();
                return View(web);
            }
            else
            {
                return new HttpUnauthorizedResult();
                //return RedirectToAction("AccessDenied", "Security", new { Area = "" });
            }
        }

        [HostDashboard(Text = "SMTP Settings", Sequence = 1, Icon = "d-icon-compass",
            RouteName = "dna_host_smtp",
            ResBaseName = "Managements",
            ResKey = "SMTP_Settings")]
        public ActionResult Smtp()
        {
            return View(SmtpConfig.Read());
        }

        [HttpPost, HostDashboard, ValidateAntiForgeryToken]
        public ActionResult Smtp(string host, string from = "", bool defaultCredentials = true, bool enableSsl = false, int port = 25, string userName = "", string password = "", string displayName = "")
        {
            var xDoc = XDocument.Load(Server.MapPath("~/web.config"));
            var mailElement = xDoc.Root.Element("system.net").Element("mailSettings");

            var smtpElement = new XElement("smtp",
                new XAttribute("from", from),
                new XAttribute("deliveryMethod", "Network"));

            var networkElement = new XElement("network",
                new XAttribute("port", port),
                new XAttribute("enableSsl", enableSsl),
                new XAttribute("defaultCredentials", defaultCredentials));

            if (!string.IsNullOrEmpty(host))
                networkElement.Add(new XAttribute("host", host));

            if (!defaultCredentials)
            {
                if (!string.IsNullOrEmpty(userName))
                    networkElement.Add(new XAttribute("userName", userName));
                if (!string.IsNullOrEmpty(password))
                    networkElement.Add(new XAttribute("password", password));
            }

            smtpElement.Add(networkElement);
            mailElement.ReplaceNodes(smtpElement);

            //Set app settings
            var appSettings = xDoc.Root.Element("appSettings");
            var senderEl = appSettings.Elements().FirstOrDefault(e => e.StrAttr("key").Equals("EmailSender"));
            if (senderEl != null)
            {
                senderEl.Attribute("value").SetValue(displayName);
            }
            else
            {
                senderEl = new XElement("add", new XAttribute("key", "EmailSender"), new XAttribute("value", displayName));
                appSettings.Add(senderEl);
            }

            xDoc.Save(Server.MapPath("~/web.config"));

            return View(SmtpConfig.Read());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TestSmtp(string to)
        {
            try
            {
                var subject = "Smtp test mail";
                var body = "This is a testing mail.This email sent by DotNetAge.";
                var app = App.Get();
                Mails.Send(!string.IsNullOrEmpty(to) ? to : app.User.Email, subject, body, false);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HostDashboard(Text = "Modules", Sequence = 4,
            RouteName = "dna_host_modules",
            Group = "Extensions",
            GroupResKey = "Extensions",
            Icon = "d-icon-suitcase")]
        public ActionResult Modules() { return View(ModuleRegistration.Modules.Select(s => s.Value).ToList()); }

        [HostOnly]
        public ActionResult Restart(string returnUrl)
        {
            DnaConfig.Restart();
            return Redirect(returnUrl);
        }

        [HostOnly, HttpPost]
        public ActionResult UnlockFullTrust()
        {
            if (!DnaConfig.IsFullTrust)
            {
                ///UNDONE:Before we unlock the trust level we can try to test the host whether support full trust level.
                DnaConfig.UpdateWebConfig("configuration/system.web/trust", "level", "Full");
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 3600)]
        public ActionResult Resource(string solution, string name)
        {
            if (ModuleRegistration.Modules.ContainsKey(solution))
            {
                var module = ModuleRegistration.Modules[solution];
                if (module != null)
                {
                    var resName = Server.UrlDecode(name).Replace("/", ".");
                    var bytes = module.GetResourceData(resName);
                    var ext = System.IO.Path.GetExtension(name);
                    var contentType = WebResourceInfo.GetContentTypeByExtension(ext);
                    return File(bytes, contentType);
                }
            }

            return HttpNotFound();
        }

        [HostOnly]
        public ActionResult SetDebug()
        {
            if (App.IsDebug)
                DnaConfig.UpdateWebConfig("configuration/system.web/compilation", "debug", "false");
            else
                DnaConfig.UpdateWebConfig("configuration/system.web/compilation", "debug", "true");
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }

    public class Collation
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }
}
