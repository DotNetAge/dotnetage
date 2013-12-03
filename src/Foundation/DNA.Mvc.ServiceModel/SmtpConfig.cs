//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Management;
using System.Web;
using System.Web.Configuration;
using System.Xml;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the SMTP configuration helper.
    /// </summary>
    public class SmtpConfig
    {
        /// <summary>
        /// Read SMTP settings from web.config.
        /// </summary>
        /// <returns>A SmtpSettings object.</returns>
        public static SmtpSettings Read()
        {
            var xdoc = new XmlDocument();
            xdoc.Load(System.Web.Hosting.HostingEnvironment.MapPath("~/Web.config"));
            var smtpSeciton = xdoc.SelectSingleNode("configuration/system.net/mailSettings/smtp");
            var netSection = xdoc.SelectSingleNode("configuration/system.net/mailSettings/smtp/network");
            var dispName = "";

            if (WebConfigurationManager.AppSettings["EmailSender"] != null)
                dispName = WebConfigurationManager.AppSettings["EmailSender"];

            if (smtpSeciton != null)
            {
                var defaultPort = 25;
                var defaultCredentials = true;
                var enableSsl = false;

                if (netSection.Attributes["defaultCredentials"] != null)
                {
                    if (!string.IsNullOrEmpty(netSection.Attributes["defaultCredentials"].Value))
                        bool.TryParse(netSection.Attributes["defaultCredentials"].Value, out defaultCredentials);
                }

                if (netSection.Attributes["port"] != null)
                {
                    if (!string.IsNullOrEmpty(netSection.Attributes["port"].Value))
                        int.TryParse(netSection.Attributes["port"].Value, out defaultPort);
                }

                if (netSection.Attributes["enableSsl"] != null)
                {
                    if (!string.IsNullOrEmpty(netSection.Attributes["enableSsl"].Value))
                        bool.TryParse(netSection.Attributes["enableSsl"].Value, out enableSsl);
                }


                return new SmtpSettings()
                 {
                     From = smtpSeciton.Attributes["from"] != null ? smtpSeciton.Attributes["from"].Value : "",
                     DefaultCredentials = defaultCredentials,
                     EnableSsl = enableSsl,
                     Host = netSection.Attributes["host"] != null ? netSection.Attributes["host"].Value : "",
                     Password = netSection.Attributes["password"] != null ? netSection.Attributes["password"].Value : "",
                     UserName = netSection.Attributes["userName"] != null ? netSection.Attributes["userName"].Value : "",
                     DisplayName=dispName,
                     //CriticalMail = System.Web.Configuration.WebConfigurationManager.AppSettings["CriticalMail"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["CriticalMail"] : "",
                     Port = defaultPort
                 };
            }
            return null;
        }

        /// <summary>
        /// Save SMTP settings to web.config.
        /// </summary>
        /// <param name="smtp">The SmtpSetting object.</param>
        public static void Save(SmtpSettings smtp)
        {
            var xdoc = new XmlDocument();
            var filename = HttpContext.Current.Server.MapPath("~/Web.config");
            xdoc.Load(filename);
            var smtpSeciton = xdoc.SelectSingleNode("configuration/system.net/mailSettings/smtp");
            var netSection = xdoc.SelectSingleNode("configuration/system.net/mailSettings/smtp/network");
            var network = netSection;//xmlDoc.DocumentElement.SelectSingleNode("network");
            network.Attributes["port"].Value = smtp.Port.ToString();
            network.Attributes["userName"].Value = smtp.UserName;
            network.Attributes["password"].Value = smtp.Password;
            network.Attributes["host"].Value = smtp.Host;
            network.Attributes["enableSsl"].Value = smtp.EnableSsl.ToString().ToLower();
            network.Attributes["defaultCredentials"].Value = smtp.DefaultCredentials.ToString().ToLower();
            xdoc.Save(filename);
        }
    }
}
