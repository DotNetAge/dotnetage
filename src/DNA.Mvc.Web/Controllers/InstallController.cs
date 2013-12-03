//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Data;
using DNA.Web.Data.Entity;
using DNA.Web.Security;
using DNA.Web.ServiceModel;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace DNA.Web.Controllers
{
    public class InstallController : Controller
    {
        [InstallationGuard]
        public ActionResult Index(string lang)
        {
            var locale = App.Settings.DefaultLocale;
            if (!string.IsNullOrEmpty(lang))
                locale = lang;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(locale);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
            return View();
        }

        [HttpPost, InstallationGuard]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                string[] reserves = App.Settings.ReservedUserNames;
                //new string[] { "host", "home", "index", "default", "{blog}", "sites", "{site}" };
                if (reserves.Contains(model.UserName.ToLower()))
                {
                    ModelState.AddModelError("", string.Format(Resources.Validations.reCapticha_UnvalidKey, "\"" + model.UserName + "\""));
                }
                else
                {
                    var context = DependencyResolver.Current.GetService<IDataContext>();
                    var status = context.Users.CreateUser(model.UserName, model.Password, model.Email);
                    if (status != UserCreateStatus.Success)
                        ModelState.AddModelError("", AccountValidation.ErrorCodeToString(status));
                    else
                    {
                        //if (App.Get().User.IsInRole(
                        var user = App.Get().Users[model.UserName];
                        if (!user.IsInRole("administartors"))
                            App.Get().Roles.AddUserToRole(model.UserName, "administrators");
                        user.DefaultWeb = "home";
                        user.Save();

                        DnaConfig.UpdateWebConfig("configuration/appSettings/add[@key=\"Administrator\"]", "value", model.UserName);
                    }
                }
            }
            return PartialView(model);
        }

        [HttpPost, InstallationGuard]
        public ActionResult TryConnect(string connectionString, string provider, bool drop = false)
        {
            var msg = "";
            try
            {
                var factory = DbProviderFactories.GetFactory(provider);
                var connection = factory.CreateConnection();
                connection.ConnectionString = connectionString;

                if (Database.Exists(connection))
                {
                    if (drop)
                    {
                        Database.Delete(connection);
                    }
                    else
                        return Json(new { error = Resources.Installations.DBExists }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, code = 200 }, JsonRequestBehavior.AllowGet);
            }
            //catch (System.Data.ProviderIncompatibleException e) {
            //    msg = e.Message;
            //    return Json(new { code = 500, error = string.IsNullOrEmpty(msg) ? null : msg }, JsonRequestBehavior.AllowGet);
            //}
            catch (System.FieldAccessException e)
            {
                msg = e.Message;
                if (provider.Equals("MySql.Data.MySqlClient"))
                {
                    msg = "Host permission required! When you seed this exception it maybe caused the application has not enough permission to drop the data schema in you MySQL database. Your MySQL has not support \"Medium\" trust level yet. To resolve this issue please remove the <trust level=\"Medium\" /> in web.config or update MySQL settings in machine.config to support Medium trust level for ASP.NET.";
                }
                return Json(new { code = 500, error = string.IsNullOrEmpty(msg) ? null : msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                //msg = e.Message;
                var ex = e;

                while (ex.InnerException != null)
                    ex = ex.InnerException;

                msg = ex.Message;
                return Json(new { code = 500, error = string.IsNullOrEmpty(msg) ? null : msg }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, InstallationGuard]
        public ActionResult TryCreate(string connectionString, string provider, bool drop = true)
        {
            var msg = "";
            try
            {
                var factory = DbProviderFactories.GetFactory(provider);
                var connection = factory.CreateConnection();
                connection.ConnectionString = connectionString;
                var recipe = provider.Equals("MySql.Data.MySqlClient", StringComparison.OrdinalIgnoreCase) ? "mysql" : "sql";

                ////Generate sqlscripts for debug
                //var tmpPath = Server.MapPath("~/content/temp/sqlscripts/");
                //if (!System.IO.Directory.Exists(tmpPath))
                //    System.IO.Directory.CreateDirectory(tmpPath);

                //if (drop)
                //    Database.SetInitializer(new DropCreateDatabaseAlways<CoreDbContext>());


                using (var dbContext = new CoreDbContext(connection, true) { Recipe = recipe })
                {
                    if (recipe == "mysql")
                        Database.SetInitializer(new CreateMySqlDatabaseIfNotExists<CoreDbContext>());
                    else
                        Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDbContext>());

                    //#region for debug
                    //string script = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CreateDatabaseScript();
                    //System.IO.File.WriteAllText(tmpPath + DNA.Utility.TextUtility.Slug(provider).ToLower() + ".sql", script);
                    ////System.Data.Entity.Migrations.DbMigrationsConfiguration
                    //#endregion

                    //dbContext.Database.CreateIfNotExists();
                    var count = dbContext.Webs.Count();

                    if (dbContext.Roles.Count(n => n.Name.Equals("administrators")) == 0)
                        dbContext.Roles.Add(new Role() { Name = "administrators", Description = "The system administrators" });

                    if (dbContext.Roles.Count(n => n.Name.Equals("guests")) == 0)
                        dbContext.Roles.Add(new Role() { Name = "guests", Description = "" });

                    dbContext.SaveChanges();
                }

                #region save configuration

                var config = new XmlDocument();
                string configPath = Server.MapPath("~/web.config");
                config.Load(configPath);
                var connectionStringsPath = "configuration/connectionStrings";
                XmlNode connectionStringsNode = null;
                connectionStringsNode = config.SelectSingleNode(connectionStringsPath);

                var dnaCnnNode = connectionStringsNode.SelectSingleNode("add[@name='DNADB']");
                if (dnaCnnNode == null)
                {
                    dnaCnnNode = CreateConnectionNode(config, "DNADB", provider, connectionString);
                    connectionStringsNode.AppendChild(dnaCnnNode);
                }
                else
                {
                    dnaCnnNode.Attributes["providerName"].Value = provider;
                    dnaCnnNode.Attributes["connectionString"].Value = connectionString;
                }

                config.Save(configPath);

                var unityConfig = new XmlDocument();
                string unityConfigPath = Server.MapPath("~/unity.config");
                unityConfig.Load(unityConfigPath);
                NameTable nt = new NameTable();
                var nsmgr = new XmlNamespaceManager(nt);
                nsmgr.AddNamespace("n", "http://schemas.microsoft.com/practices/2010/unity");

                var rNode = unityConfig.SelectSingleNode("//n:container/n:register[@type=\"IDataContext\"]/n:constructor/n:param[@name=\"recipe\"]", nsmgr);
                rNode.Attributes["value"].Value = recipe;
                unityConfig.Save(unityConfigPath);
                #endregion

            }
            catch (Exception e)
            {
                msg = e.Message;
                if (e.InnerException != null)
                {
                    msg += "\t\n" + "Detail:" + e.InnerException.Message;
                }

                return Json(new { code = 500, error = string.IsNullOrEmpty(msg) ? null : msg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, code = 200 }, JsonRequestBehavior.AllowGet);
        }

        private XmlNode CreateConnectionNode(XmlDocument doc, string name, string provider, string connectionString)
        {
            var defaultNode = doc.CreateNode(XmlNodeType.Element, "add", "");
            var nameAttr = doc.CreateAttribute("name");
            nameAttr.Value = "DNADB";
            var providerAttr = doc.CreateAttribute("providerName");
            providerAttr.Value = provider;
            var cnnStrAttr = doc.CreateAttribute("connectionString");
            cnnStrAttr.Value = connectionString;
            defaultNode.Attributes.Append(nameAttr);
            defaultNode.Attributes.Append(cnnStrAttr);
            defaultNode.Attributes.Append(providerAttr);
            return defaultNode;
        }

        [HttpPost, InstallationGuard]
        public ActionResult CreateWeb([Bind(Prefix = "Web")] Web web, string solution = "core", string lang = "")
        {
            try
            {
                var app = App.Get();
                //set default locale
                if (!string.IsNullOrEmpty(lang))
                    DnaConfig.UpdateWebConfig("configuration/appSettings/add[@key=\"DefaultLocale\"]", "value", lang);

                app.Widgets.RegisterAll();
                var topWeb=app.Solutions.Install(solution, "home", web.Owner, web.Title, web.Description, "", lang);

                if (topWeb.Pages.Count() == 0)
                   topWeb.CreatePage("Default");

                PermissionLoader.Load();

                DnaConfig.UpdateWebConfig("configuration/appSettings/add[@key=\"Initialized\"]", "value", "True");
            }
            catch (Exception e)
            {
                //var msg = e.Message;
                Exception innerExpt = e.InnerException;
                var errors = new StringBuilder();

                App.Get().DataContext.Delete<Web>(w => w.Name.Equals("home"));
                App.Get().DataContext.SaveChanges();
                errors.AppendLine(e.Message);

                while (innerExpt != null)
                {
                    errors.AppendLine(innerExpt.Message);
                    //msg = innerExpt.Message;
                    innerExpt = innerExpt.InnerException;
                }
                errors.AppendLine(e.StackTrace);
                //if (innerExpt != null)
                // errors.Append(e.Message);
                //msg = innerExpt.Message;

                return Json(new { error = errors.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        [InstallationGuard]
        public ActionResult Restart()
        {
            try
            {
                DnaConfig.Restart();

                if (!Request.IsAjaxRequest())
                    return Redirect("~/home/"+System.Threading.Thread.CurrentThread.CurrentUICulture.Name+"/default.html");
                else
                    return Content("OK");
            }
            catch (Exception e)
            {
                if (!Request.IsAjaxRequest())
                    return Redirect("~/home/" + System.Threading.Thread.CurrentThread.CurrentUICulture.Name + "/default.html");
                else
                    return Content("OK");
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (username.Equals(AppModel.Settings.Administrator) && password.Equals(System.Web.Configuration.WebConfigurationManager.AppSettings["Password"]))
            {
                var cookie = new System.Web.HttpCookie("dna_administrator");
                cookie.Expires = DateTime.Now.AddHours(1);
                Response.AppendCookie(cookie);
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}