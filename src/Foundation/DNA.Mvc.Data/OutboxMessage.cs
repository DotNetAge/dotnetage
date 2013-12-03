//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Mail;
using System;

namespace DNA.Web
{
    /// <summary>
    /// Represent the message in outbox.
    /// </summary>
    public class OutboxMessage:Message
    {
        /// <summary>
        /// Initializes a new instace of the OutboxMessage class.
        /// </summary>
        public OutboxMessage() { }

        /// <summary>
        /// Initializes a new instance of the OutboxMessage class.
        /// </summary>
        /// <param name="msg"></param>
        public OutboxMessage(Message msg)
        {
            msg.CopyTo(this);
            Sent = DateTime.Now;
        }

        /// <summary>
        /// Gets/Sets the receiver user name.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets/Sets the date when message was sent.
        /// </summary>
        public DateTime Sent { get; set; }
    }
}
