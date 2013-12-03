//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DNA.Web.Controllers
{
    public class ViewController : Controller
    {
        private ContentViewDecorator Find(string name, string slug) { return App.Get().CurrentWeb.Lists[name].Views[slug]; }
        private const string EA = "Edit";

        [HttpPost, Loc,
        SecurityAction("Create view", 
            PermssionSet = "Contents",
            Description = "Allows user to create new views.",
            TitleResName = "SA_CreateView",
            DescResName = "SA_CreateViewDesc",
            PermssionSetResName = "SA_Contents"
            )]
        public ActionResult NewView(int id, string name, string style, string mode, string title, string website = "home", bool nopage = false)
        {
            var web = App.Get().Webs[website];
            var list = web.Lists[id];
            var view = list.CreateView(name, title);

            var contentTmpl = new ContentTemplate()
            {
                //ContentType = mode == "server" ? "text/xslt" : "text/x-jquery-tmpl",
                Source = style
                //Source = "_" + style + ".cshtml"
            };

            view.BodyTemplateXml = contentTmpl.ToXml();
            view.Save();

            if (!nopage)
            {
                var defaultView = view.Parent.DefaultView;
                web.CreatePage(view, defaultView != null && defaultView.Page != null ? defaultView.Page.ID : 0, title);
            }

            return Redirect(view.SettingUrl);
        }

        [SiteDashboard, SecurityAction("Edit view", 
            PermssionSet = "Contents",
            Description = "Allows user modify view settings custom styles and behaviors.",
            TitleResName = "SA_EditView",
            DescResName = "SA_EditViewDesc",
            PermssionSetResName = "SA_Contents"
            )]
        public ActionResult Edit(string name, string slug)
        {
            //ViewBag.ActiveMenu = Resources.Dict.Contents;
            return View(Find(name, slug));
        }

        //[SiteControlPanel]
        //public ActionResult Design(string name, string slug) { return View(Find(name, slug)); }

        [HttpPost, SecurityAction("Delete view", 
            PermssionSet = "Contents",
            Description = "Allows user to delete views.",
            TitleResName = "SA_DelView",
            DescResName = "SA_DelViewDesc",
            PermssionSetResName = "SA_Contents"
            )]
        public ActionResult Delete(int id)
        {
            var view = App.Get().DataContext.Find<ContentView>(id);
            var list = App.Get().FindList(view.ParentID);
            if (view != null)
            {
                App.Get().DataContext.Delete(view);
                App.Get().DataContext.SaveChanges();
            }

            return Redirect(list.SettingUrl);
        }

        [HttpPost, PermissionRequired(EA)]
        public ActionResult SetFields(string name, string slug, string[] fields)
        {
            var wrapper = Find(name, slug);
            //Reset the sort and filters
            wrapper.SetViewFields(fields);

            return Redirect(wrapper.SettingUrl);
        }

        [PermissionRequired(EA)]
        public ActionResult EditCss(string name, string slug)
        {
            var view = Find(name, slug);
            string csstext = "";
            if (!string.IsNullOrEmpty(view.StyleSheetsXml))
            {
                var stylesEl = XElement.Parse(view.StyleSheetsXml);
                if (stylesEl != null && stylesEl.HasElements)
                {
                    foreach (var style in stylesEl.Elements())
                    {
                        var src = style.StrAttr("src");
                        if (!string.IsNullOrEmpty(src))
                        {
                            #region link element
                            //var linkElement = new XElement("link",
                            //    new XAttribute("type", "text/css"),
                            //    new XAttribute("rel", "stylesheet"));
                            var formattedSrc = src;
                            if (!src.StartsWith("http"))
                            {
                                if (src.StartsWith("~"))
                                    formattedSrc = Url.Content(src);
                                else
                                    formattedSrc = Url.Content(view.Parent.ResolveUrl(src));
                            }

                            var cssfile = Server.MapPath(formattedSrc);
                            if (System.IO.File.Exists(cssfile))
                                csstext += (string.IsNullOrEmpty(csstext) ? "" : "\t\n") + System.IO.File.ReadAllText(cssfile);
                            #endregion
                        }
                        else
                        {
                            csstext += string.IsNullOrEmpty(style.Value) ? style.InnerXml() : style.Value;
                        }
                    }
                }
            }
            ViewBag.CssText = csstext;
            return PartialView(view);
        }

        [HttpPost, PermissionRequired(EA), ValidateInput(false)]
        public ActionResult EditCss(string name, string slug, string csstext)
        {
            var view = Find(name, slug);
            ViewBag.CssText = csstext;
            if (!string.IsNullOrEmpty(csstext))
            {
                var root = new XElement("styles");
                root.Add(new XElement("styleSheet", new XCData(Server.HtmlDecode(csstext))));
                view.StyleSheetsXml = root.OuterXml();
            }
            else
            {
                view.StyleSheetsXml = "";
            }
            view.Save();
            return PartialView(view);
        }

        [HttpPost, PermissionRequired(EA)]
        public ActionResult SetStyle(string name, string slug, string style)
        {
            var view = Find(name, slug);
            var tmplFile = "~/content/types/base/views/" + style + (style.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? "" : ".cshtml");
            if (!System.IO.File.Exists(Server.MapPath(tmplFile)))
                throw new HttpException("View template file not found.");

            var bodyTmpl = new ContentTemplate()
            {
                ContentType = TemplateContentTypes.Razor,
                Source = style + (style.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? "" : ".cshtml")
            };
            view.BodyTemplateXml = bodyTmpl.Element().OuterXml();
            view.Save();

            return new HttpStatusCodeResult(200);
        }

        [HttpGet, PermissionRequired(EA)]
        public ActionResult EditField(string name, string slug, string field)
        {
            var view = Find(name, slug);
            ViewBag.View = view;
            var _field = view.FieldRefs[field];
            var tmpl = _field.Template;
            var code = "";

            if (tmpl != null && !tmpl.IsEmpty)
            {
                if (string.IsNullOrEmpty(tmpl.Source))
                    code = tmpl.Text;
                else
                {
                    //1.find the field tmpl in package path
                    var url = view.Parent.ResolveUrl(tmpl.Source);
                    var file = Server.MapPath(url);

                    if (System.IO.File.Exists(file))
                    {
                        try
                        {
                            code = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
                        }
                        catch { }
                    }
                    else
                    {
                        //2.find the field tmpl in default field tmpl path
                        file = Server.MapPath("~/content/types/base/fields/view/" + tmpl.Source);
                        if (System.IO.File.Exists(file))
                        {
                            try
                            {
                                code = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
                            }
                            catch { }
                        }
                    }
                }
            }
            ViewBag.Code = code;
            TemplateHelper.SaveAsView(view.Parent, code, string.Format("_view_{0}_field_{1}_tmpl.cshtml", view.Name, name), true);
            return PartialView("_Editor", _field);
        }

        [HttpPost, PermissionRequired(EA), ValidateInput(false)]
        public ActionResult EditField(string name, string slug, string field, bool showLabel, bool isHidden, string feed, string code)
        {
            var view = Find(name, slug);
            var fieldRef = view.FieldRefs[field];
            fieldRef.ShowLabel = showLabel;
            fieldRef.IsHidden = isHidden;
            fieldRef.ToFeedItemField = feed;


            fieldRef.Template = new ContentTemplate()
            {
                ContentType = TemplateContentTypes.Razor,
                Text = code
            };


            view.FieldRefs.Save();
            return new HttpStatusCodeResult(200);
        }

        [HttpGet, PermissionRequired(EA)]
        public ActionResult Code(string name, string slug)
        {
            var view = Find(name, slug);
            var tmpl = view.Body;
            var code = "";

            if (tmpl.IsEmpty)
            {
                var url = "~/content/types/base/views/_list.cshtml";
                var file = Server.MapPath(url);
                if (System.IO.File.Exists(file))
                {
                    try
                    {
                        code = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
                    }
                    catch { }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tmpl.Source))
                {
                    var url = view.Parent.ResolveUrl(tmpl.Source);
                    var file = Server.MapPath(url);
                    if (!System.IO.File.Exists(file))
                    {
                        url = "~/content/types/base/views/" + tmpl.Source;
                        file = Server.MapPath(url);
                    }

                    if (System.IO.File.Exists(file))
                    {
                        try
                        {
                            code = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
                        }
                        catch { }
                    }
                }
                else
                {
                    code = tmpl.Text;
                }
            }

            ViewBag.Code = code;
            return PartialView(view);
        }

        [HttpPost, PermissionRequired(EA), ValidateInput(false)]
        public ActionResult Code(string name, string slug, string code, string locale)
        {
            var view = Find(name, slug);
            var bodyTmpl = new ContentTemplate()
            {
                ContentType = TemplateContentTypes.Razor,
                Text = code
            };
            view.BodyTemplateXml = bodyTmpl.Element().OuterXml();
            view.Save();
            TemplateHelper.SaveAsView(view.Parent, code, string.Format("_view_{0}_tmpl.cshtml", view.Name), true);
            return new HttpStatusCodeResult(200);
        }
    }
}
