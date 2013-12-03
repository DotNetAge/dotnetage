//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using DNA.Web.ServiceModel;
using System.Web.Helpers;

namespace DNA.Web.Controllers
{
    public class AccountAPIController : Controller
    {
        IDataContext context;

        public AccountAPIController(IDataContext dbContext)
        {
            context = dbContext;
        }

        public ActionResult List(QueryParams _params, string terms = "")
        {
            int total = 0;
            var query = context.Users.All().OrderBy(u => u.UserName);

            if (!string.IsNullOrEmpty(terms))
            {
                query = context.Users.Filter(u => u.UserName.Contains(terms) || u.Email.Contains(terms)).OrderBy(u => u.UserName);
            }

            var values = new List<dynamic>();
            total = query.Count();
            var members = _params.GetPageResult(query, true).ToList();

            foreach (var member in members)
            {
                var m = new
                {
                    id = member.UserName,
                    dispName = member.DefaultProfile.DisplayName,
                    email = member.Email,
                    created = member.CreationDate,
                    approved = member.IsApproved,
                    picture = string.IsNullOrEmpty(member.DefaultProfile.Avatar) ? "/content/images/no-avatar.gif" : member.DefaultProfile.Avatar,
                    roles = member.Roles.Select(r => r.Name).ToArray(),
                    validated = member.IsVaildMail
                };
                values.Add(m);
            }

            return Json(new ModelWrapper()
            {
                Model = values,
                Total = total
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SetOAuth(string name, string key, string secret, string requestToken, string authorization, string accessToken, string userInfo, string icon)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var configDoc = new XmlDocument();
            configDoc.Load(Server.MapPath("~/web.config"));
            var providersNode = configDoc.SelectSingleNode("configuration/oauth/proxy/providers");
            var providerNode = configDoc.SelectSingleNode("configuration/oauth/proxy/providers/add[@name=\"" + name + "\"]");

            if (providerNode == null)
            {
                providerNode = configDoc.CreateElement("add");
                providersNode.AppendChild(providerNode);

                var nameAttr = configDoc.CreateAttribute("name");
                nameAttr.Value = name;
                providerNode.Attributes.Append(nameAttr);
            }

            _SetAttribute(providerNode, "type", "DNA.OAuth.OAuthProvider,DNA.OAuth");
            _SetAttribute(providerNode, "consumerKey", key);
            _SetAttribute(providerNode, "consumerSecret", secret);
            _SetAttribute(providerNode, "requestTokenEndPoint", requestToken);
            _SetAttribute(providerNode, "authorizationEndPoint", authorization);
            _SetAttribute(providerNode, "accessTokenEndPoint", accessToken);
            _SetAttribute(providerNode, "userInfoEndPoint", userInfo);
            _SetAttribute(providerNode, "icon", icon);

            configDoc.Save(Server.MapPath("~/web.config"));
        }

        private void _SetAttribute(XmlNode node, string name, string value)
        {
            if (node.Attributes[name] != null)
            {
                node.Attributes[name].Value = value;
            }
            else
            {
                var attr = node.OwnerDocument.CreateAttribute(name);
                attr.Value = value;
                node.Attributes.Append(attr);
            }
        }

        [HttpPost]
        public void SetOAuth2(string name, string id, string secret, string scropes, string authorization, string token, string userInfo, string icon)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var configDoc = new XmlDocument();
            configDoc.Load(Server.MapPath("~/web.config"));
            var providersNode = configDoc.SelectSingleNode("configuration/oauth/proxy/providers");
            var providerNode = configDoc.SelectSingleNode("configuration/oauth/proxy/providers/add[@name=\"" + name + "\"]");

            if (providerNode == null)
            {
                providerNode = configDoc.CreateElement("add");
                providersNode.AppendChild(providerNode);
                var nameAttr = configDoc.CreateAttribute("name");
                nameAttr.Value = name;
                providerNode.Attributes.Append(nameAttr);

            }

            _SetAttribute(providerNode, "type", "DNA.OAuth.OAuth2Provider,DNA.OAuth");
            _SetAttribute(providerNode, "clientID", id);
            _SetAttribute(providerNode, "clientSecret", secret);
            _SetAttribute(providerNode, "authorizationEndPoint", authorization);
            _SetAttribute(providerNode, "tokenEndPoint", token);
            _SetAttribute(providerNode, "scropes", scropes);
            _SetAttribute(providerNode, "userInfoEndPoint", userInfo);
            _SetAttribute(providerNode, "icon", icon);

            configDoc.Save(Server.MapPath("~/web.config"));
        }

        public ActionResult ValidatEmail(string email)
        {
            if (context.Find<User>(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) != null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateName(string username)
        {
            if (context.Find<User>(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)) != null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
            {
                if (App.Settings.ReservedUserNames.Contains(username))
                    return Json(false, JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FindEmail(string email)
        {
            if (context.Find<User>(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) != null)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken, Loc]
        public ActionResult Mailto(string to, string toName, string from, string fromName, string subject, string message)
        {
            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException("to");

            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException("from");

            if (string.IsNullOrEmpty(toName))
            {
                var toUser = context.Users.Find(u => u.Email.Equals(to, StringComparison.OrdinalIgnoreCase));
                if (toUser != null)
                    toName = (new UserDecorator(toUser, context)).DisplayName;
            }

            Mails.Enqueue(to, subject, "sys_contact", new
            {
                to = to,
                toName = toName,
                from = from,
                fromName = fromName,
                message = message,
                appUrl = App.Get().Context.AppUrl.ToString()
            });

            return new HttpStatusCodeResult(200);
        }

        [HostOnly]
        public ActionResult Remove(int id)
        {
            var usr = App.Get().DataContext.Users.Find(id);
            App.Get().DataContext.Delete(usr);
            App.Get().DataContext.SaveChanges();
            return new HttpStatusCodeResult(200);
        }
    }
}
