//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Mail;
using System.Net.Mail;

namespace DNA.Web.ServiceModel.Messages.Senders
{
    /// <summary>
    /// Implement the IMessageSender interface use to send the message to email queue
    /// </summary>
    [Inject(MapTo="IMessageSender")]
    public class EmailQueueSender : NamingObject, IMessageSender
    {
        public App AppContext { get { return App.Get(); } }

        public void Boardcast(Message message, string from)
        {
            var sender = AppContext.Users[from];

            foreach (var receiver in AppContext.Users)
            {
                if (receiver.UserName.Equals(from))
                    continue;

                var msg = new MailMessage()
                 {
                     Body = message.Body,
                     Subject = message.Subject,
                     IsBodyHtml = message.ContentType.Equals("text/html")
                 };

                msg.From = new MailAddress(sender.Email, sender.DisplayName);
                msg.To.Add(new MailAddress(receiver.Email, receiver.DisplayName));
                AppContext.Queues.Enqueue(msg);
            }
        }

        public void Send(Message message, string from, string to)
        {
            var fromUser = AppContext.Users[from];
            var toUser = AppContext.Users[to];

            Mails.Enqueue(to, message.Subject, "sys_contacts", new
            {
                to = toUser.Email,
                toName = toUser.DisplayName,
                from = fromUser.Email,
                fromName = fromUser.DisplayName,
                message = message.Body,
                appUrl = App.Get().Context.AppUrl.ToString()
            });
        }
    }
}
