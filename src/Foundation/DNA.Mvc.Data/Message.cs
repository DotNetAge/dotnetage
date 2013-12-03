//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;

namespace DNA.Web.Mail
{
    /// <summary>
    /// Represent the message class.
    /// </summary>
    public class Message
    {
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the title text.
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets/Sets the message body.
        /// </summary>
        public virtual string Body { get; set; }

        /// <summary>
        /// Gets/Sets the message content type.
        /// </summary>
        public virtual string ContentType { get; set; }

        /// <summary>
        /// Gets/Sets the message creation date.
        /// </summary>
        public virtual DateTime Creation { get; set; }

        ///// <summary>
        ///// Gets/Sets the message sender address
        ///// </summary>
        //public MessageAddress From { get; set; }

        /// <summary>
        /// Gets/Sets the destation address.
        /// </summary>
        public List<MessageAddress> To { get; set; }
    }

    /// <summary>
    /// Represents a message address class
    /// </summary>
    public class MessageAddress
    {
        /// <summary>
        /// Gets/Sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets/Sets the email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets/Sets the display name.
        /// </summary>
        public string DisplayName { get; set; }
    }
}
