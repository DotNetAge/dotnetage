//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Mail;
namespace DNA.Web.ServiceModel.Messages.Senders
{
    /// <summary>
    /// Represents a message sender use to send internal short message.
    /// </summary>
    [Inject(MapTo = "IMessageSender")]
    public class ShortMessageSender : NamingObject, IMessageSender
    {
        public App AppContext { get { return App.Get(); } }

        public void Boardcast(Message message, string from)
        {
            var sender = AppContext.Users[from];

            sender.Storage.Add(new OutboxMessage(message) { To = "*" });
            sender.Storage.SaveChanges();

            foreach (var user in AppContext.Users)
            {
                var receiver = AppContext.Users[user.UserName];
                receiver.Storage.Add(new InboxMessage(message) { Sender = from });
                receiver.Storage.SaveChanges();
            }
        }

        public void Send(Message message, string from, string to)
        {
            var sender = AppContext.Users[from];
            var receiver = AppContext.Users[to];

            sender.Storage.Add(new OutboxMessage(message) { To = to });
            sender.Storage.SaveChanges();

            receiver.Storage.Add(new InboxMessage(message) { Sender = from });
            receiver.Storage.SaveChanges();
        }
    }

}
