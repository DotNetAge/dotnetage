//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Security;
using DNA.Web.ServiceModel;
using Microsoft.Web.Helpers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace DNA.Web.Controllers
{
    public class AccountController : Controller
    {
        private IDataContext dbContext;
        public AccountController(IDataContext context)
        {
            dbContext = context;
        }

        [Authorize, Loc]
        public ActionResult Settings(string app = "dotnetage")
        {
            var profiles = App.Get().User.Profiles;
            ViewBag.Profiles = profiles;
            if (string.IsNullOrEmpty(app))
                return View(App.Get().User.DefaultProfile);
            else
            {
                return View(profiles.FirstOrDefault(p => p.AppName.Equals(app, StringComparison.OrdinalIgnoreCase)));
            }
        }

        [Authorize, HttpPost, ValidateInput(false), Loc]
        public ActionResult Settings(FormCollection forms, string app = "", string account = "")
        {
            if (!string.IsNullOrEmpty(app) && !string.IsNullOrEmpty(account))
            {
                App.Get().User.SetDefaultProfile(app, account);
                if (Request.IsAjaxRequest())
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return View("Settings", new { app = app });
            }
            else
            {
                var defaultProfile = App.Get().User.Profiles.FirstOrDefault(p => p.AppName.Equals("dotnetage"));

                if (TryUpdateModel(defaultProfile, forms))
                {
                    var dbCtx = App.Get().DataContext;
                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        var file = Request.Files[0];
                        var dest = string.Format("{0}/webshared/{1}/images/", App.Get().Context.AppUrl.ToString(), this.CurrentWebName());
                        var fileUri = new Uri(dest + User.Identity.Name + Path.GetExtension(file.FileName));
                        var filename = App.Get().NetDrive.MapPath(fileUri);
                        file.SaveAs(filename);
                        defaultProfile.Avatar = fileUri.ToString();
                    }
                    dbCtx.Update(defaultProfile);
                    dbCtx.SaveChanges();
                }
                return View(defaultProfile);
            }
        }

        public ActionResult ValidateAccount(string account)
        {
            var returns = App.Get().DataContext.Find<User>(u => u.UserName.Equals(account) || u.Email.Equals(account, StringComparison.OrdinalIgnoreCase)) != null;
            return Json(returns, JsonRequestBehavior.AllowGet);
        }

        [Loc]
        public ActionResult Receive()
        {
            return PartialView();
        }

        [Loc, HttpPost]
        public ActionResult Receive(string account)
        {
            if (string.IsNullOrEmpty(account))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var user = App.Get().DataContext.Find<User>(u => u.UserName.Equals(account) || u.Email.Equals(account, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                //Set reset password url to user mail
                ViewBag.Email = user.Email;
                var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                user.RetrievalToken = token;
                App.Get().DataContext.SaveChanges();

                //Send mail here
            }
            return PartialView();
        }

        [Loc]
        public ActionResult Validate(string token)
        {
            var user = App.Get().DataContext.Find<User>(u => !string.IsNullOrEmpty(u.RetrievalToken) && u.RetrievalToken.Equals(token));
            return View(user);
        }

        [Loc]
        public ActionResult Validate(string token, string pwd, string confirmPwd)
        {
            var user = App.Get().DataContext.Find<User>(u => !string.IsNullOrEmpty(u.RetrievalToken) && u.RetrievalToken.Equals(token));
            if (user != null)
            {
                var userWarpper = new UserDecorator(user, App.Get().DataContext);
                userWarpper.ChangePassword(pwd, confirmPwd);
                user.RetrievalToken = "";
                user.IsVaildMail = true;
                App.Get().DataContext.SaveChanges();
                FormsAuthentication.SetAuthCookie(user.UserName, true);
            }
            return View(user);
        }

        public ActionResult ValidateEmail(string userName)
        {
            var app = App.Get();
            var user = app.Users[userName];
            user.VaildToken = Guid.NewGuid().ToString();
            var uri = app.Context.AppUrl.ToString() + "account/ConfirmEmail?userName=" + userName + "&token=" + user.VaildToken;

            Mails.Send(userName, "Confrim your email address", "sys_validatemail", new
            {
                to = user.DisplayName,
                confirmUrl = uri,
                web = app.Webs["home"].Title
            });

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmEmail(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = dbContext.Find<User>(u => u.VaildToken.Equals(id));
                ViewBag.Valid = false;
                if (user != null)
                {
                    user.IsVaildMail = true;
                    user.VaildToken = "";
                    dbContext.SaveChanges();
                    ViewBag.Valid = true;
                }
                FormsAuthentication.SetAuthCookie(user.UserName, true);
            }
            return View();
        }

        [Authorize, HttpPost]
        public ActionResult SyncProfile(string app, string account)
        {
            if (string.IsNullOrEmpty(app))
                throw new ArgumentNullException("app");

            if (string.IsNullOrEmpty(account))
                throw new ArgumentNullException("account");

            var result = App.Get().User.SyncProfile(app, account);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Loc]
        public ActionResult ViewProfile(string user)
        {
            var _user = AppModel.Get().Users[user];
            return PartialView(_user);
        }

        [Loc]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login with user name or register email
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <remark>
        /// v.2.1.3 changed log:Supports login with user name and register email to login to system. 
        /// Fix when login user name case bug.
        /// </remark>
        /// <returns></returns>
        [HttpPost, Loc]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var username = model.UserName;
                var member = dbContext.Users.Find(username);

                if (member == null)
                    member = dbContext.Users.Find(u => u.Email.Equals(username));

                if (member != null && dbContext.Users.Validate(model.UserName, model.Password))
                {
                    WebCache.Remove("_Identity_Cache");
                    member.LastLoginDate = DateTime.Now;
                    member.LastLoginIP = Request.UserHostAddress;
                    FormsAuthentication.SetAuthCookie(username, model.RememberMe);
                    this.Trigger("Login", model);
                    //EventDispatcher.RaiseUserLogin(username);

                    if (!Request.IsAjaxRequest())
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);
                            else
                                return Redirect("~/");
                        }
                    }
                    else
                    {
                        var user = new UserDecorator(member, dbContext);
                        string json = JsonConvert.SerializeObject(user.ToObject(), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
                        return Content(json, "application/json", Encoding.UTF8);
                    }
                }
                else
                {
                    if (member == null)
                        ModelState.AddModelError("UserName", String.Format(Resources.Validations.NotExists_Format, model.UserName));
                    else
                        ModelState.AddModelError("Password", Resources.Validations.Password_Incorrect);
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {
            var userName = User.Identity.Name;
            FormsAuthentication.SignOut();
            WebCache.Remove("_Identity_Cache");
            //EventDispatcher.RaiseUserLogout(userName);
            this.Trigger("LogOff", userName);
            return Redirect("~/");
        }

        [Loc]
        public ActionResult Register()
        {
            if (!App.Settings.OpenRegister)
                return HttpNotFound();
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model, string returnUrl = "")
        {
            if (App.Settings.EnableReCaptchaValidation)
            {

                if (!ReCaptcha.Validate(App.Settings.ReCaptchaPrivateKey))
                {
                    ModelState.AddModelError("", Resources.Validations.reCapticha_UnvalidKey);
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                string[] reserves = App.Settings.ReservedUserNames; // new string[] { "host", "home", "index", "default", "{blog}", "sites", "{site}" };
                if (reserves != null && reserves.Contains(model.UserName.ToLower()))
                {
                    ModelState.AddModelError("", string.Format(Resources.Validations.UserName_Reserved, "\"" + model.UserName + "\""));
                }
                else
                {
                    var createStatus = dbContext.Users.CreateUser(model.UserName, model.Password, model.Email);

                    if (createStatus == UserCreateStatus.Success)
                    {
                        WebCache.Remove("_Identity_Cache");
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        App.Trigger("Register", this, model);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return Redirect("~/");
                    }
                    else
                    {
                        ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                    }
                }
            }
            return View(model);
        }

        [Authorize, Loc]
        public ActionResult ChangePassword()
        {
            return PartialView();
        }

        [Authorize, HttpPost, Loc]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    //var _event = new PasswordChangedEvent(User.Identity.Name, model.OldPassword, model.NewPassword);
                    //_event.Raise(HttpContext);
                    this.Trigger("PasswordChanged", model);
                    //var app = App.Get();
                    //var user = app.Users[User.Identity.Name];
                    //App.Get().SendMail(SystemMails.PasswodChanged, model, user.Email);
                }
                else
                {
                    ModelState.AddModelError("", Resources.Validations.Password_Incorrect);
                }
            }
            return PartialView(model);
        }

        [Authorize, Loc]
        public ActionResult MyWebs()
        {
            return PartialView(App.Get().User.Webs);
        }

        [Authorize, Loc]
        public ActionResult SendMail(string to) { return PartialView(); }

        //[Authorize, MyDashboard(Group = "Messages", Text = "Inbox", Icon = "d-icon-envelope")]
        //public ActionResult Inbox()
        //{
        //    return View(App.Get().User.Storage.All<InboxMessage>());
        //}

        //[Authorize, MyDashboard(Group = "Messages", Text = "Outbox", Icon = "d-icon-envelope-2")]
        //public ActionResult Outbox()
        //{
        //    return View(App.Get().User.Storage.All<OutboxMessage>());
        //}

    }
}
