//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data.Documents;
using DNA.Web.Mail;
using DNA.Web.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a email helper class use to send system email
    /// </summary>
    public static class Mails
    {
        internal static App AppContext { get { return App.Get(); } }

        #region Send method

        public static void Send(Message message)
        {
            var smtpConfig = SmtpConfig.Read();
            var smtpClient = new SmtpClient(smtpConfig.Host, smtpConfig.Port);
            var mailMsg = GetMailMessage(message, smtpConfig);
            smtpClient.Send(mailMsg);
        }

        public static void Send(string to, string subject, string body, bool isHtml)
        {
            var msg = new Message()
            {
                Subject = subject,
                Body = body,
                ContentType = isHtml ? "text/html" : "text/plain",
                To = new List<MessageAddress>()
            };

            var toUser = AppContext.Users[to];
            if (toUser != null)
                msg.To.Add(new MessageAddress() { UserName = to, Email = toUser.Email, DisplayName = toUser.DisplayName });
            else
                msg.To.Add(new MessageAddress() { Email = to });
            Send(msg);
        }

        public static void Send(string to, string subject, string templateName, object data)
        {
            Send(to, subject, GetTemplate(templateName, data), true);
        }

        #endregion

        #region Enqueue

        public static void Enqueue(Message message)
        {
            AppContext.Queues.Enqueue(message);
        }

        public static void Enqueue(string to, string subject, string body, bool isHtml)
        {
            var msg = new Message()
            {
                Body = body,
                Subject = subject,
                ContentType = isHtml ? "text/html" : "text/plain",
                To = new List<MessageAddress>()
            };
            msg.To.Add(new MessageAddress() { Email = to });
            AppContext.Queues.Enqueue(msg);
        }

        public static void Enqueue(string to, string subject, string templateName, object data)
        {
            Enqueue(to, subject, GetTemplate(templateName, data), true);
        }

        #endregion

        public static void Dequeue()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/app_data/queues/");
            using (var queue = new QueueStorage(path))
            {
                if (queue.Count<Message>() > 0)
                {
                    var message = queue.Dequeue<Message>();
                    var smtpConfig = SmtpConfig.Read();
                    var smtpClient = new SmtpClient(smtpConfig.Host, smtpConfig.Port);
                    var mailMsg = GetMailMessage(message, smtpConfig);
                    smtpClient.Send(mailMsg);
                }
            }
        }

        private static MailMessage GetMailMessage(Message message, SmtpSettings smtpConfig)
        {
            var mailMsg = new MailMessage();

            if (!string.IsNullOrEmpty(smtpConfig.DisplayName))
                mailMsg.From = new MailAddress(smtpConfig.From, smtpConfig.DisplayName);

            foreach (var addr in message.To)
            {
                if (string.IsNullOrEmpty(addr.DisplayName))
                    mailMsg.To.Add(addr.Email);
                else
                    mailMsg.To.Add(new MailAddress(addr.Email, addr.DisplayName));
            }

            mailMsg.SubjectEncoding = Encoding.UTF8;
            mailMsg.BodyEncoding = Encoding.UTF8;
            mailMsg.Subject = message.Subject;
            mailMsg.Body = message.Body;
            mailMsg.IsBodyHtml = message.ContentType.Equals("text/html");
            return mailMsg;
        }

        public static MailAddress GetDefaultAddress()
        {
            var smtpConfig = SmtpConfig.Read();
            var smtpClient = new SmtpClient(smtpConfig.Host, smtpConfig.Port);
            var mail = new MailMessage();
            return new MailAddress(smtpConfig.From, smtpConfig.DisplayName);
        }

        /// <summary>
        /// Tranform xslt email template file with data by specified template name.
        /// </summary>
        /// <param name="name">The xslt email template without extension name.</param>
        /// <param name="data">The data use to format the email template.</param>
        /// <returns>returns the formatted email body.</returns>
        public static string GetTemplate(string name, object data)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (data == null)
                throw new ArgumentNullException("data");

            var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~/content/emails/");
            var file = Path.Combine(basePath, name + ".xslt");
            if (!File.Exists(file))
                throw new FileNotFoundException(file);
            var readerSettings = new XmlReaderSettings();
            
            var xslt = new XslCompiledTransform();
            xslt.Load(XmlReader.Create(file));
            var dict = data.ToDictionary();
            var xDoc = new XElement("modal");

            foreach (var key in dict.Keys)
                xDoc.Add(new XElement(key, dict[key]));

            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
            {
                var writerSettings = new XmlWriterSettings();
                writerSettings.Encoding = Encoding.UTF8;
                using (var writer = XmlWriter.Create(stringWriter,writerSettings))
                {
                    var reader=xDoc.CreateReader();
                    xslt.Transform(reader, writer);
                    writer.Flush();
                }
            }

            var results = sb.ToString();
            
            if (!string.IsNullOrEmpty(results) && results.StartsWith("<?xml")) 
            {
                results = results.Substring(results.IndexOf("?>") + 2);
            }

            return results;
        }

        /// <summary>
        /// Get all avalidable templates.
        /// </summary>
        /// <returns></returns>
        public static string[] GetTemplates()
        {
            var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~/content/emails/");
            return Directory.GetFiles(basePath, ".xslt");
        }
    }
}
