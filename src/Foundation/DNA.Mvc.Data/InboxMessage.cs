//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Mail;
using System;

namespace DNA.Web
{
    /// <summary>
    /// Represent the inbox message class.
    /// </summary>
    public class InboxMessage:Message
    {
        /// <summary>
        /// Initializes a new instance of the InboxMessage class.
        /// </summary>
        public InboxMessage() { }

        /// <summary>
        /// Initializes a new instance of the InboxMessage class with message object.
        /// </summary>
        /// <param name="msg">The message object.</param>
        public InboxMessage(Message msg)
        {
            msg.CopyTo(this);
            Received = DateTime.Now;
        }

        /// <summary>
        /// Gets/Sets the sender user name.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets/sets the date when message received.
        /// </summary>
        public DateTime Received { get; set; }

        /// <summary>
        /// Gets/Sets the message is read.
        /// </summary>
        public bool IsRead { get; set; }
    }
}
