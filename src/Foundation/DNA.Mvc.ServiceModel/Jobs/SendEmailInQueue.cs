//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data.Documents;
using DNA.Web.Scheduling;
using System.Net.Mail;

namespace DNA.Web.ServiceModel.Jobs
{
    public class SendEmailInQueue : Job
    {
        public App AppContext { get { return App.Get(); } }

        public override string Title
        {
            get
            {
                return "Send email in queue";
            }
        }

        protected override void OnExecute()
        {
            Mails.Dequeue();
        }
    }
}
