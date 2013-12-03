//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Mail;
namespace DNA.Web.ServiceModel.Messages
{
    /// <summary>
    /// Defines a sender use to send or boardcast message.
    /// </summary>
    [Inject]
    public interface IMessageSender : INamingObject
    {
        /// <summary>
        /// Send message to all accounts.
        /// </summary>
        /// <param name="message">The message object.</param>
        /// <param name="from">The message sender user name.</param>
        void Boardcast(Message message, string from);

        /// <summary>
        /// Send message to specified account(s)
        /// </summary>
        /// <param name="message">The message subject.</param>
        /// <param name="from">The message sender user name.</param>
        /// <param name="to">The message receiver user name.</param>
        void Send(Message message, string from, string to);
    }
}
