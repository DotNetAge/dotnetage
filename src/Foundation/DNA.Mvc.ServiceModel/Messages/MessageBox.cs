//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Mail;
using System;
using System.Collections.Generic;

namespace DNA.Web.ServiceModel.Messages
{
    /// <summary>
    /// Represent a class that use to send and manage messages.
    /// </summary>
    public class MessageBox
    {
        /// <summary>
        /// Initializes a new instance of the MessageBox class with message sender instances.
        /// </summary>
        /// <param name="senders">The message sender</param>
        public MessageBox(params IMessageSender[] senders)
        {
            if (senders != null)
            {
                this.Senders = new Dictionary<string, IMessageSender>();
                foreach (var sender in senders)
                {
                    this.Senders.Add(sender.Name, sender);
                }
            }
        }

        #region Boardcast method overloads

        /// <summary>
        /// Boardcast the message to all message accounts and all senders by speicifed subject and body
        /// </summary>
        /// <param name="subject">The message subject.</param>
        /// <param name="body">The message body.</param>
        /// /// <param name="from">Identines who send this message .</param>
        public void Boardcast(string subject, string body, string from)
        {
            var msg = new Message() { Body = body, Subject = subject, Creation = DateTime.Now, ContentType = "text/html" };
            Boardcast(msg, from);
        }

        public void Boardcast(Message message, string from)
        {
            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException("from");

            foreach (var name in Senders.Keys)
            {
                Senders[name].Boardcast(message, from);
            }
        }

        /// <summary>
        /// Boardcast the message to all message accounts by speicifed sender , subject and body
        /// </summary>
        /// <param name="sender">The registered message sender object name.</param>
        /// <param name="subject">The message subject.</param>
        /// <param name="body">The message body.</param>
        /// <param name="from">Identines who send this message .</param>
        public void Boardcast(string sender, string subject, string body, string from)
        {

            if (string.IsNullOrEmpty(sender))
                throw new ArgumentNullException("sender");

            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException("from");

            if (!Senders.ContainsKey(sender))
                throw new Exception(string.Format("{0} message sender not found.", sender));

            Senders[sender].Boardcast(new Message() { Body = body, Subject = subject, Creation = DateTime.Now, ContentType = "text/html" }, from);
        }

        //internal void Boardcast<T>(T sender, string subject, string body) { throw new NotImplementedException(); }

        #endregion

        #region Send method overloads

        /// <summary>
        /// Send message to specified account by all message senders.
        /// </summary>
        /// <param name="subject">The message subject.</param>
        /// <param name="body">The message body.</param>
        /// <param name="from">The account name who send this message.</param>
        /// <param name="to">The account name who receive this message.</param>
        public void Send(string subject, string body, string from, string to)
        {
            Send(from, to, new Message() { Body = body, Subject = subject, Creation = DateTime.Now, ContentType = "text/html" });
        }

        public void Send(string from, string to, Message msg)
        {
            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException("from");

            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException("to");

            foreach (var name in Senders.Keys)
            {
                Senders[name].Send(msg, from, to);
            }
        }

        /// <summary>
        ///  Send message to specified account by sepcified message sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="subject">The message subject.</param>
        /// <param name="body">The message body.</param>
        /// <param name="from">The account name who send this message.</param>
        /// <param name="to">The account name who receive this message.</param>
        public void Send(string sender, string subject, string body, string from, string to)
        {
            Send(sender, from, to, new Message() { Body = body, Subject = subject, Creation = DateTime.Now, ContentType = "text/html" });
        }

        public void Send(string sender, string from, string to, Message msg)
        {
            if (string.IsNullOrEmpty(sender))
                throw new ArgumentNullException("sender");

            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException("from");

            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException("to");

            if (!Senders.ContainsKey(sender))
                throw new Exception(string.Format("{0} message sender not found.", sender));

            Senders[sender].Send(msg, from, to);
        }

        #endregion

        /// <summary>
        /// Gets the registered message senders.
        /// </summary>
        public Dictionary<string, IMessageSender> Senders { get; private set; }

    }


}
