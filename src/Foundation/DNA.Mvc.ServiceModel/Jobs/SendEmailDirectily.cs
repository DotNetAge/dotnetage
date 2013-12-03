//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Scheduling;
using System.Net.Mail;

namespace DNA.Web.ServiceModel.Jobs
{
    /// <summary>
    /// Represent a job use to send email.
    /// </summary>
    public class SendEmailDirectily : Job
    {
        private void Send(string subject, string body, string to)
        {
            Mails.Send(to, subject, body, true);
        }

        public override string Title
        {
            get
            {
                return "Send an email";
            }
        }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string To { get; set; }

        public override string ConfigureView
        {
            get
            {
                return "~/Views/Jobs/SendEmailJobConfiguration.cshtml";
            }
        }

        protected override void OnExecute()
        {
            if (!string.IsNullOrEmpty(To) && !string.IsNullOrEmpty(Body))
                Send(Subject, Body, To);
        }

    }
}
