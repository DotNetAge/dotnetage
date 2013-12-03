//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.Management
{
    /// <summary>
    /// Represents the Smtp settings.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// Initializes a new instance of SmtpSettings class
        /// </summary>
        public SmtpSettings()
        {
            DefaultCredentials = true;
            Port = 25;
        }

        /// <summary>
        /// Gets / Sets the default sender email address.
        /// </summary>
        /// <value>A string that contains an email address.</value>
        public string From { get; set; }

        /// <summary>
        /// Gets/Sets the default sender display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Determines whether or not default user credentials are used to access an SMTP server. The default value is false.
        /// </summary>
        /// <value>
        ///    true indicates that default user credentials will be used to access the SMTP server; otherwise, false.
        /// </value>
        public bool DefaultCredentials { get; set; }

        /// <summary>
        /// Gets or sets whether SSL is used to access an SMTP mail server. The default value is false.
        /// </summary>
        /// <value> true indicates that SSL will be used to access the SMTP mail server; otherwise, false.</value>
        public bool EnableSsl { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the SMTP server.
        /// </summary>
        /// <value> A string that represents the name of the SMTP server to connect to.</value>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the user password to use to connect to an SMTP mail server.
        /// </summary>
        /// <value>A string that represents the password to use to connect to an SMTP mail server.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the port that SMTP clients use to connect to an SMTP mail server. The default value is 25.
        /// </summary>
        /// <value>A string that represents the port to connect to an SMTP mail server.</value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the user name to connect to an SMTP mail server.
        /// </summary>
        /// <value>A string that represents the user name to connect to an SMTP mail server.</value>
        public string UserName { get; set; }

        public string CriticalMail { get; set; }
    }
}