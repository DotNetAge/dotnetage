//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DNA.Web.Controllers
{
    public class FormController : Controller
    {
        [SiteDashboard,
        SecurityAction("Edit form", PermssionSet = "Contents", Description = "Allow user can custom forms.",
            TitleResName = "SA_EditForm",
            DescResName = "SA_EditFormDesc",
            PermssionSetResName = "SA_Contents"
            )]
        public ActionResult Edit(string name, string type)
        {
            var formType = (ContentFormTypes)Enum.Parse(typeof(ContentFormTypes), type);
            return View(App.Get().CurrentWeb.Lists[name].Forms.FirstOrDefault(f => f.FormType == (int)formType));
        }
        private const string EA = "Edit";

        [HttpGet, PermissionRequired(EA)]
        public ActionResult EditField(string name, string type, string field)
        {
            var form = _GetForm(name, type);
            ViewBag.Form = form;
            var _field = form.Fields[field];
            var tmpl = _field.Template;
            var code = "";

            if (tmpl != null && !tmpl.IsEmpty)
            {
                if (string.IsNullOrEmpty(tmpl.Source))
                    code = tmpl.Text;
                else
                {
                    var url = form.Parent.ResolveUrl(tmpl.Source);
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
            }
            ViewBag.Code = code;
            return PartialView("_Editor", _field);
        }

        [HttpPost, PermissionRequired(EA),ValidateInput(false)]
        public ActionResult EditField(string name, string type, string field, bool isCaption, bool showLabel, bool isHidden, string code)
        {
            var form = _GetForm(name, type);
            var f = form.Fields[field];
            f.IsCaption = isCaption;
            f.ShowLabel = showLabel;
            f.IsHidden = isHidden;

            if (!string.IsNullOrEmpty(code))
            {
                f.Template = new ContentTemplate()
                {
                    ContentType = TemplateContentTypes.Razor,
                    Text = code
                };
            }

            form.Fields.Save();
            TemplateHelper.SaveAsView(form.Parent, code, string.Format("_form_{0}_field_{1}_tmpl.cshtml", form.FormTypeString.ToLower(), name.ToLower()));
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost, PermissionRequired(EA)]
        public ActionResult UpdateFields(string name, string type, string[] fields)
        {
            var form = _GetForm(name, type);
            form.SetFields(fields);
            return RedirectToAction("Edit", new { name = name, type = type, locale = App.Get().Context.Locale });
        }

        private ContentFormDecorator _GetForm(string name, string type)
        {
            var formType = (ContentFormTypes)Enum.Parse(typeof(ContentFormTypes), type);
            return App.Get().CurrentWeb.Lists[name].Forms.FirstOrDefault(f => f.FormType == (int)formType);
        }

        [PermissionRequired(EA)]
        public ActionResult EditCss(string name, string type)
        {
            var form = _GetForm(name, type);
            string csstext = "";
            if (!string.IsNullOrEmpty(form.StyleSheetsXml))
            {
                var stylesEl = XElement.Parse(form.StyleSheetsXml);
                if (stylesEl != null && stylesEl.HasElements)
                {
                    foreach (var style in stylesEl.Elements())
                    {
                        var src = style.StrAttr("src");
                        if (!string.IsNullOrEmpty(src))
                        {
                            #region link element
                            var formattedSrc = src;
                            if (!src.StartsWith("http"))
                            {
                                if (src.StartsWith("~"))
                                    formattedSrc = Url.Content(src);
                                else
                                    formattedSrc = Url.Content(form.Parent.ResolveUrl(src));
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
            return PartialView(form);
        }

        [HttpPost, PermissionRequired(EA), ValidateInput(false)]
        public ActionResult EditCss(string name, string type, string csstext)
        {
            var form = _GetForm(name, type);
            ViewBag.CssText = csstext;
            if (!string.IsNullOrEmpty(csstext))
            {
                var root = new XElement("styles");
                root.Add(new XElement("styleSheet", new XCData(Server.HtmlDecode(csstext))));
                form.StyleSheetsXml = root.OuterXml();
            }
            else
            {
                form.StyleSheetsXml = "";
            }
            form.Save();
            return PartialView(form);
        }

        [HttpGet, PermissionRequired(EA)]
        public ActionResult Code(string name, string type)
        {
            var form = _GetForm(name, type);
            var tmpl = form.Body;
            var code = "";

            if (!tmpl.IsEmpty)
            {
                if (string.IsNullOrEmpty(tmpl.Source))
                    code = tmpl.Text;
                else
                {
                    var url = form.Parent.ResolveUrl(tmpl.Source);
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
            }

            ViewBag.Code = code;
            return PartialView(form);
        }

        [HttpPost, PermissionRequired(EA), ValidateInput(false)]
        public ActionResult Code(string name, string type, string code, string locale)
        {
            var form = _GetForm(name, type);
            var bodyTmpl = new ContentTemplate()
            {
                ContentType = TemplateContentTypes.Razor,
                Text = code
            };
            form.BodyTemplateXml = bodyTmpl.Element().OuterXml();
            form.Save();
            TemplateHelper.SaveAsView(form.Parent, code, string.Format("_form_{0}_tmpl.cshtml", form.FormTypeString.ToLower()), true);
            return new HttpStatusCodeResult(200);
        }
    }
}
