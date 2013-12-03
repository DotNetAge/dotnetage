//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using Microsoft.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using System.Web.WebPages;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace DNA.Web.UI
{
    /// <summary>
    /// Provides the helper methods for ContentType UI.
    /// </summary>
    public static class ContentExtensions
    {
        private static string RES_REQURIED = "The {0} is requried.";
        private static string RangeAttribute_ValidationError = "The {0} must be between {1} and {2}.";
        private static string RangeAttribute_Min = "The {0} must be less then {1}";
        private static string RangeAttribute_Max = "The {0} must be greater then {1}";

        /// <summary>
        /// Render the dispaly html to response output for specified field name.
        /// </summary>
        /// <param name="helper">The html helper object</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="dataItem">The data item to display</param>
        /// <param name="withLabel">Specified whether show the label on the left of the field value</param>
        public static void ForDisp(this HtmlHelper helper, string fieldName, ContentDataItemDecorator dataItem, bool? withLabel)
        {
            if (dataItem == null)
                return;

            if (string.IsNullOrEmpty(fieldName))
            {
                return;
                //helper.ForDisp(dataItem);
            }
            else
            {
                var fieldEditor = dataItem.Parent.DetailForm.Fields[fieldName];
                if (fieldEditor != null)
                    helper.ForDisp(fieldEditor, dataItem, withLabel);
            }
        }

        /// <summary>
        /// Render all fields display html to response output.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="dataItem">The data item to display.</param>
        public static void ForDispAll(this HtmlHelper helper, ContentDataItemDecorator dataItem)
        {
            foreach (var field in dataItem.Parent.DetailForm.Fields)
            {
                if (field.Name.Equals("title", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (field.HideInDisplayForm)
                    continue;

                helper.ForDisp(field, dataItem, field.ShowLabel);
            }
        }

        /// <summary>
        /// Render the field editor to response output by specified editor field object.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="field">The editor field which defined in target EditForm</param>
        /// <param name="dataItem">The data item to edit.</param>
        /// <param name="withLabel">Specified whether show the label on the left of the field editor.</param>
        public static void ForDisp(this HtmlHelper helper, ContentEditorField field, ContentDataItemDecorator dataItem, bool? withLabel)
        {
            if (field == null)
                return;

            var writer = helper.ViewContext.Writer;

            if (field.HideInDisplayForm || field.IsHidden)
            {
                helper.Hidden(field.Name, dataItem[field.Name].Raw);
                if (!string.IsNullOrEmpty(field.ItemProp))
                    writer.Write("<meta itemprop=\"" + field.ItemProp + "\" content=\"" + dataItem[field.Name].Raw + "\" />");
                return;
            }


            var list = App.Get().Wrap(field.Parent);
            var server = helper.ViewContext.RequestContext.HttpContext.Server;
            ContentFieldValue val = dataItem.Value(field.Name);

            if (val.IsNull && field.Template.IsEmpty && field.Field.FieldType != (int)ContentFieldTypes.Computed)
                return;

            var showLabel = field.ShowLabel;

            if (withLabel.HasValue)
                showLabel = withLabel.Value;

            if (!string.IsNullOrEmpty(field.ItemProp))
                writer.Write(string.Format("<div class=\"d-field d-{0}-field" + (field.IsCaption ? " d-caption-field" : "") + " {1}\" itemprop=\"{2}\">", field.FieldTypeString.ToLower(), field.Name, field.ItemProp));
            else
                writer.Write(string.Format("<div class=\"d-field d-{0}-field" + (field.IsCaption ? " d-caption-field" : "") + " {1}\">", field.FieldTypeString.ToLower(), field.Name));

            if (showLabel && !val.IsNull)
            {
                var labelEl = new XElement("label", field.Field.Title);
                helper.ViewContext.Writer.Write(labelEl.OuterXml());
            }

            var linkToItem = field.IsLinkToItem;
            if (linkToItem && field.FieldType == (int)ContentFieldTypes.Lookup)
            {
                //if ()
                //{
                var lookupField = field.Field as LookupField;
                if (!lookupField.IsLinkToItem)
                {
                    var lookupList = list.Web.Lists[lookupField.ListName];
                    var lookupItem = lookupList.GetItem(Guid.Parse(dataItem[field.Name].Raw.ToString()));
                    writer.Write(string.Format("<a href=\"{0}\" class=\"d-link\">", lookupItem.UrlComponent));
                }
                else
                    linkToItem = false;
                //}
                //else
                //    helper.ViewContext.Writer.Write(string.Format("<a href=\"{0}\" class=\"d-link\">", dataItem.UrlComponent));
            }
            else
                linkToItem = false;

            var tmpl = field.Template;

            if (!tmpl.IsEmpty)
            {
                if (tmpl.ContentType.Equals(TemplateContentTypes.Razor, StringComparison.OrdinalIgnoreCase))
                    helper.RenderEditorFieldTemplate(field, "_disp", dataItem);

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xslt, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xml, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                if (field.Field.FieldType == (int)ContentFieldTypes.Computed)
                {
                    //Render Computed Field should we put this code back to ComputedField object?
                    var f = field.Field as ComputedField;
                    f.RenderPattern(dataItem).WriteTo(helper.ViewContext.Writer);
                }
                else
                {
                    var explicitTmpl = "~/content/types/base/fields/disp/_" + field.Field.FieldTypeString.ToLower() + ".cshtml";
                    var commonTmpl = "~/content/types/base/fields/disp/_common.cshtml";
                    var actalTmpl = explicitTmpl;

                    if (!File.Exists(server.MapPath(explicitTmpl)))
                        actalTmpl = commonTmpl;

                    helper.RenderPartial(actalTmpl, val);
                }
            }

            if (linkToItem)
                writer.Write("</a>");

            writer.Write("</div>");
        }

        /// <summary>
        /// Render the field editor to response output by specified editor field for Activity form
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="field">The editor field object which defined in target ActivityForm</param>
        /// <param name="dataItem">The data item for display.</param>
        /// <param name="withLabel">Specified whether show the label on the left of the field editor.</param>
        public static void ForAct(this HtmlHelper helper, ContentEditorField field, ContentDataItemDecorator dataItem, bool? withLabel)
        {
            if (field.IsHidden)
            {
                helper.Hidden(field.Name, dataItem[field.Name].Raw);
                return;
            }

            if (field == null)
                return;

            var list = App.Get().Wrap(field.Parent);
            var server = helper.ViewContext.RequestContext.HttpContext.Server;
            ContentFieldValue val = dataItem.Value(field.Name);

            if (val.IsNull && field.Template.IsEmpty && field.Field.FieldType != (int)ContentFieldTypes.Computed)
                return;

            var showLabel = field.ShowLabel;

            if (withLabel.HasValue)
                showLabel = withLabel.Value;

            var writer = helper.ViewContext.Writer;
            writer.Write(string.Format("<div class=\"d-field d-{0}-field {1}\"" + (field.IsCaption ? " d-caption-field" : "") + " {1}\" itemprop=\"{2}\">", field.FieldTypeString.ToLower(), field.Name, field.ItemProp));

            if (showLabel && !val.IsNull)
            {
                var labelEl = new XElement("label", field.Field.Title);
                helper.ViewContext.Writer.Write(labelEl.OuterXml());
            }
            var linkToItem = field.IsLinkToItem;

            if (linkToItem)
            {
                if (field.FieldType == (int)ContentFieldTypes.Lookup)
                {
                    var lookupField = field.Field as LookupField;
                    if (lookupField.IsLinkToItem)
                        linkToItem = false;
                    else
                    {
                        var lookupList = list.Web.Lists[lookupField.ListName];
                        var lookupItem = lookupList.GetItem(Guid.Parse(dataItem[field.Name].Raw.ToString()));
                        writer.Write(string.Format("<a href=\"{0}\" class=\"d-link\">", lookupItem.UrlComponent));
                    }
                }
                else
                    helper.ViewContext.Writer.Write(string.Format("<a href=\"{0}\" class=\"d-link\">", dataItem.UrlComponent));
            }

            var tmpl = field.Template;

            if (!tmpl.IsEmpty)
            {
                if (tmpl.ContentType.Equals(TemplateContentTypes.Razor, StringComparison.OrdinalIgnoreCase))
                    helper.RenderEditorFieldTemplate(field, "_act", dataItem);

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xslt, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xml, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                if (field.Field.FieldType == (int)ContentFieldTypes.Computed)
                {
                    //Render Computed Field should we put this code back to ComputedField object?
                    var f = field.Field as ComputedField;
                    f.RenderPattern(dataItem).WriteTo(helper.ViewContext.Writer);
                }
                else
                {
                    var explicitTmpl = "~/content/types/base/fields/act/_" + field.Field.FieldTypeString.ToLower() + ".cshtml";
                    var commonTmpl = "~/content/types/base/fields/act/_common.cshtml";
                    var actalTmpl = explicitTmpl;

                    if (!File.Exists(server.MapPath(explicitTmpl)))
                        actalTmpl = commonTmpl;

                    helper.RenderPartial(actalTmpl, val);
                }
            }

            if (linkToItem)
                writer.Write("</a>");

            writer.Write("</div>");
        }

        /// <summary>
        /// Render fields elements to response output by specified form and item object.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="form">The form object.</param>
        /// <param name="item">The data item to render.</param>
        public static void RenderFields(this HtmlHelper helper, ContentFormDecorator form, ContentDataItemDecorator item = null)
        {
            foreach (var f in form.Fields)
            {
                switch ((ContentFormTypes)form.FormType)
                {
                    case ContentFormTypes.New:
                        helper.ForNew(f);
                        break;
                    case ContentFormTypes.Edit:
                        helper.ForEdit(f, item);
                        break;
                    case ContentFormTypes.Activity:
                        helper.ForAct(f, item, null);
                        break;
                    default:
                        helper.ForDisp(f, item, null);
                        break;
                }
            }
        }

        private static void RenderViewFieldTemplate(this HtmlHelper helper, ContentFieldRef field, string filePrefix, ContentQueryResultItem item)
        {
            var tmpl = field.Template;
            var list = field.Parent;
            var server = helper.ViewContext.RequestContext.HttpContext.Server;
            if (!string.IsNullOrEmpty(tmpl.Source))
            {
                var viewFileName = list.Package.ResolveUri(field.Template.Source);
                if (File.Exists(server.MapPath(viewFileName)))
                    helper.RenderPartial(viewFileName, new ContentViewFieldValue(field, item));
                else
                {
                    //Find template file in default path
                    viewFileName = "~/content/types/base/fields/view/" + field.Template.Source;
                    if (File.Exists(server.MapPath(viewFileName)))
                        helper.RenderPartial(viewFileName, new ContentViewFieldValue(field, item));
                    else
                        helper.ViewContext.Writer.Write("<span>Field template file not found.</span>");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tmpl.Text))
                {
                    var viewFile = string.Format("_view_{0}_field_{1}_tmpl.cshtml", field.ParentView.Name.ToLower(), field.Name.ToLower());
                    var viewUrl = TemplateHelper.SaveAsView(field.Parent, tmpl.Text, viewFile);
                    helper.RenderPartial(viewUrl, new ContentViewFieldValue(field, item));
                }
            }
        }

        private static void RenderEditorFieldTemplate(this HtmlHelper helper, ContentEditorField field, string filePrefix, ContentDataItemDecorator item = null)
        {
            var tmpl = field.Template;
            var list = field.Parent;
            var server = helper.ViewContext.RequestContext.HttpContext.Server;
            if (!string.IsNullOrEmpty(tmpl.Source))
            {
                var viewFileName = list.Package.ResolveUri(field.Template.Source);
                if (File.Exists(server.MapPath(viewFileName)))
                {
                    if (item == null)
                        helper.RenderPartial(viewFileName, field.Field);
                    else
                        helper.RenderPartial(viewFileName, item[field.Name]);
                }
                else
                    helper.ViewContext.Writer.Write("<span>Field template file not found.</span>");
            }
            else
            {
                if (!string.IsNullOrEmpty(tmpl.Text))
                {
                    var viewFile = string.Format("_form_{0}_field_{1}_tmpl.cshtml", field.ParentForm.FormTypeString.ToLower(), field.Name.ToLower());
                    var viewUrl = TemplateHelper.SaveAsView(field.Parent, tmpl.Text, viewFile);
                    if (item == null)
                        helper.RenderPartial(viewUrl, field.Field);
                    else
                        helper.RenderPartial(viewUrl, item[field.Name]);
                }
            }
        }

        /// <summary>
        /// Render the new field editor to response ouput by specified editor field object.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="editor">The field editor object.</param>
        /// <param name="withLabel">Specified whether show the label on the left of the field editor.</param>
        /// <param name="withNotes">Specified whether show the field notes on the bottom of the field editor.</param>
        public static void ForNew(this HtmlHelper helper, ContentEditorField editor, bool withLabel = true, bool withNotes = true)
        {
            if (editor == null) return;
            if (editor.Field.IsIngored) return;

            var list = editor.Parent;
            var field = editor.Field;
            var server = helper.ViewContext.RequestContext.HttpContext.Server;

            if (editor.IsHidden)
            {
                helper.Hidden(field).WriteTo(helper.ViewContext.Writer);
                return;
            }

            var writer = helper.ViewContext.Writer;
            writer.Write(string.Format("<div class=\"d-field d-{0}-field {1}\">", field.FieldTypeString.ToLower(), field.Name));

            if (withLabel)
            {
                // helper.ViewContext.Writer.Write("<div>");
                helper.Label(field).WriteTo(helper.ViewContext.Writer);
            }

            var tmpl = editor.Template;
            if (!tmpl.IsEmpty)
            {
                if (tmpl.ContentType.Equals(TemplateContentTypes.Razor, StringComparison.OrdinalIgnoreCase))
                    helper.RenderEditorFieldTemplate(editor, "_new");

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xslt, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xml, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }

            }
            else
                helper.RenderPartial("~/content/types/base/fields/new/_" + field.FieldTypeString.ToLower() + ".cshtml",
                    editor.Field);

            if (withNotes && !string.IsNullOrEmpty(field.Description))
            {
                //writer.Write("<div>");
                helper.Notes(field).WriteTo(helper.ViewContext.Writer);
                //writer.Write("</div>");
            }

            writer.Write("</div>");

        }

        /// <summary>
        ///  Render the field editor to response ouput by specified editor field object.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="editor">The field editor object.</param>
        /// <param name="dataItem">The data item object to edit.</param>
        /// <param name="withLabel">Specified whether show the label on the left of the field editor.</param>
        /// <param name="withNotes">Specified whether show the field notes on the bottom of the field editor.</param>
        public static void ForEdit(this HtmlHelper helper, ContentEditorField editor, ContentDataItemDecorator dataItem, bool withLabel = true, bool withNotes = true)
        {
            if (editor == null) return;

            var field = editor.Field;

            if (field == null || field.IsIngored) return;


            var list = editor.Parent;

            //App.Get().Wrap(field.Parent);
            var server = helper.ViewContext.RequestContext.HttpContext.Server;

            if (editor.IsHidden)
            {
                helper.Hidden(field, dataItem.Value(editor.Name).Raw).WriteTo(helper.ViewContext.Writer);
                return;
            }
            var writer = helper.ViewContext.Writer;
            writer.Write(string.Format("<div class=\"d-field d-{0}-field {1}\">", field.FieldTypeString.ToLower(), field.Name));

            if (withLabel)
            {
                //helper.ViewContext.Writer.Write("<div>");
                helper.Label(field).WriteTo(helper.ViewContext.Writer);
            }

            var tmpl = editor.Template;
            if (!tmpl.IsEmpty)
            {
                if (tmpl.ContentType.Equals(TemplateContentTypes.Razor, StringComparison.OrdinalIgnoreCase))
                    helper.RenderEditorFieldTemplate(editor, "_edit", dataItem);

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xslt, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xml, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }

            }
            else
                helper.RenderPartial("~/content/types/base/fields/edit/_" + ((ContentFieldTypes)field.FieldType).ToString().ToLower() + ".cshtml", dataItem.Value(editor.Name));

            if (withNotes && !string.IsNullOrEmpty(field.Description))
            {
                // helper.ViewContext.Writer.Write("<div>");
                helper.Notes(field).WriteTo(helper.ViewContext.Writer);
                //helper.ViewContext.Writer.Write("</div>");
            }
            writer.Write("</div>");
        }

        /// <summary>
        /// Render the field value to response output for View by specified field name and data item object.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="fieldName">The field name which defined in ViewFields</param>
        /// <param name="dataItem">The data item to display.</param>
        /// <param name="withLabel">Specified whether show the label on the left of the field value.</param>
        public static void ForView(this HtmlHelper helper, string fieldName, ContentQueryResultItem dataItem, bool? withLabel = null)
        {
            if (dataItem == null)
                return;

            var view = dataItem.View;
            var fieldRef = view.FieldRefs[fieldName];

            if (fieldRef == null)
            {
                //Render the error holder && view.Parent.Fields[fieldName] == null
                var title = fieldName;
                if (view.Parent.Fields[fieldName] != null)
                    title = view.Parent.Fields[fieldName].Title;
                helper.ViewContext.Writer.Write("<div class='d-state-error' style=\"padding:10px;\">Can not found the <strong>" + title + "</strong> field in this view</div>");
                return;
            }

            if (fieldRef.IsHidden)
            {
                helper.Hidden(fieldRef.Field, dataItem[fieldName]).WriteTo(helper.ViewContext.Writer);
                return;
            }

            var showLabel = fieldRef.ShowLabel;

            if (withLabel.HasValue)
                showLabel = withLabel.Value;

            if (showLabel && !dataItem.IsNull(fieldName))
            {
                var labelEl = new XElement("label", fieldRef.Title + ":");
                helper.ViewContext.Writer.Write(labelEl.OuterXml());
            }

            var tmpl = fieldRef.Template;

            if (!fieldRef.Template.IsEmpty)
            {
                if (tmpl.ContentType.Equals(TemplateContentTypes.Razor, StringComparison.OrdinalIgnoreCase))
                    helper.RenderViewFieldTemplate(fieldRef, "_viewitem", dataItem);

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xslt, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }

                if (tmpl.ContentType.Equals(TemplateContentTypes.Xml, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }

                //When the view field has template definition exit this process
                return;
            }
            else
            {
                var field = fieldRef.Field;
                var server = helper.ViewContext.RequestContext.HttpContext.Server;
                var explicitTmpl = "~/content/types/base/fields/view/_" + field.FieldTypeString.ToLower() + ".cshtml";

                if (File.Exists(server.MapPath(explicitTmpl)))
                {
                    // render default field tmpl
                    helper.RenderPartial(explicitTmpl, new ContentViewFieldValue(fieldRef, dataItem));
                }
                else
                {
                    var linkToItem = field.IsLinkToItem;
                    var url = new UrlHelper(helper.ViewContext.RequestContext);
                    var writer = helper.ViewContext.Writer;
                    var linkFormat = "<a href=\"{0}\" class=\"d-link\">";
                    var fieldVal = field.Format(dataItem[field.Name]);
                    var link = url.Content(dataItem);

                    if (linkToItem)
                        writer.Write(string.Format(linkFormat, link));

                    if (field.FieldType == (int)ContentFieldTypes.Computed)
                    {
                        var f = field as ComputedField;
                        if (f != null)
                            f.RenderPattern(dataItem).WriteTo(writer);
                    }

                    writer.Write(fieldVal);

                    if (linkToItem)
                    {
                        writer.Write("</a>");
                    }
                }
            }
        }

        /// <summary>
        /// Render the tags element for specified data item.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="dataItem">The data item instance.</param>
        /// <returns></returns>
        public static HelperResult Tags(this HtmlHelper helper, ContentDataItem dataItem)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    if (!string.IsNullOrEmpty(dataItem.Tags))
                    {
                        writer.WriteBeginTag("input");
                        writer.WriteAttribute("data-role", "tags");
                        writer.WriteAttribute("readonly", "readonly");
                        writer.WriteAttribute("value", dataItem.Tags);
                        writer.Write(Html32TextWriter.SelfClosingTagEnd);
                    }
                }
            });
        }

        #region Render patterns

        /// <summary>
        /// Render computed field display pattern html to response output.
        /// </summary>
        /// <param name="field">The computed field instance.</param>
        /// <param name="dataItem">The content query result item object.</param>
        /// <returns></returns>
        public static HelperResult RenderPattern(this ComputedField field, ContentQueryResultItem dataItem)
        {
            return new HelperResult(writer =>
            {
                //var pattern = string.Format("<pattern>{0}</pattern>", field.DispPattern);
                var root = XElement.Parse(field.DispPattern);
                var ns = root.GetDefaultNamespace();
                foreach (var element in root.Elements())
                {
                    if (element.Name.Equals(ns + "html"))
                        writer.Write(element.Value);

                    if (element.Name.Equals(ns + "col") && element.HasAttributes)
                    {
                        var name = element.StrAttr("name");
                        var output = element.StrAttr("output");
                        if (string.IsNullOrEmpty(output))
                            output = "formatted";
                        if (!string.IsNullOrEmpty(name))
                        {
                            var val = dataItem[name];
                            if (val != null)
                            {
                                if (output == "formatted")
                                    writer.Write(field.Format(val));
                                else
                                    writer.Write(val);
                            }
                            else
                            {
                                writer.Write(val);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Render computed field display pattern html to response output.
        /// </summary>
        /// <param name="field">The computed field instance.</param>
        /// <param name="dataItem">The data item instance.</param>
        /// <returns></returns>
        public static HelperResult RenderPattern(this ComputedField field, ContentDataItemDecorator dataItem)
        {
            return new HelperResult(writer =>
            {
                //var pattern = string.Format("<pattern>{0}</pattern>", field.DispPattern);
                var root = XElement.Parse(field.DispPattern);
                var ns = root.GetDefaultNamespace();
                foreach (var element in root.Elements())
                {
                    if (element.Name.Equals(ns + "html"))
                        writer.Write(element.Value);

                    if (element.Name.Equals(ns + "col") && element.HasAttributes)
                    {
                        var name = element.StrAttr("name");
                        var output = element.StrAttr("output");
                        if (string.IsNullOrEmpty(output))
                            output = "formatted";
                        if (!string.IsNullOrEmpty(name))
                        {
                            var val = dataItem[name];
                            if (val != null)
                            {
                                if (output == "formatted")
                                    writer.Write(val.Formatted);
                                else
                                    writer.Write(val.Raw);
                            }
                            else
                            {
                                if (name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                                    writer.Write(dataItem.ID);
                                if (name.Equals("ParentID", StringComparison.OrdinalIgnoreCase))
                                    writer.Write(dataItem.ParentID);
                                if (name.Equals("Owner", StringComparison.OrdinalIgnoreCase))
                                    writer.Write(dataItem.Owner);
                                if (name.Equals("Modified", StringComparison.OrdinalIgnoreCase))
                                    writer.Write(dataItem.Modified);
                            }
                        }
                    }
                }
            });
        }

        private static void RenderColPattern(XElement element, TextWriter w, ContentListDecorator list)
        {
            var prop = element.StrAttr("prop");
            var name = element.StrAttr("name");
            if (!string.IsNullOrEmpty(prop) && !string.IsNullOrEmpty(name))
            {
                var field = list.Fields[name];
                if (field != null)
                {
                    if (prop.Equals("title") && !string.IsNullOrEmpty(field.Title))
                    {
                        w.Write(field.Title);
                        return;
                    }

                    if (prop.Equals("description") && !string.IsNullOrEmpty(field.Description))
                    {
                        w.Write(field.Description);
                        return;
                    }
                }
            }

        }

        private static void RenderRowPattern(XElement rowElement, TextWriter writer, ContentQueryResultItem dataItem)
        {
            var urlHelper = DNA.Utility.UrlUtility.CreateUrlHelper();
            var elements = rowElement.Elements();
            //var nodes = row.ChildNodes;
            foreach (var element in elements)
            {
                if (element.Name.Equals("col"))
                {
                    if (element.HasElements)
                        RenderColPattern(element, writer, dataItem.List);
                    continue;
                }

                if (element.Name.Equals("url"))
                {
                    var rel = element.StrAttr("rel");
                    if (rel.Equals("disp"))
                        writer.Write(dataItem.UrlComponent);
                    else
                    {
                        if (rel.Equals("del"))
                            writer.Write(urlHelper.Action("Delete", "Contents", new { id = dataItem.ID, name = dataItem.List.Name }));
                        else
                            writer.Write(urlHelper.Content(dataItem.List.GetEditItemUrl(dataItem.Slug)));
                    }
                    continue;
                }

                if (element.Name.Equals("val"))
                {
                    var name = element.StrAttr("name");

                    if (!string.IsNullOrEmpty(name))
                    {
                        var field = dataItem.List.Fields[name];
                        if (field != null)
                        {
                            #region computed field
                            if (field.FieldType == (int)ContentFieldTypes.Computed)
                            {
                                var computedField = field as ComputedField;
                                if (computedField != null)
                                {
                                    var pattern = string.Format("<pattern>{0}</pattern>", computedField.DispPattern);
                                    var root = XElement.Parse(pattern);
                                    foreach (var fieldElement in root.Elements())
                                    {
                                        if (element.Name.Equals("html"))
                                            writer.Write(element.Value);

                                        if (element.Name.Equals("col") && element.HasAttributes)
                                        {
                                            var _name = element.StrAttr("name");
                                            var _output = element.StrAttr("output");
                                            if (string.IsNullOrEmpty(_output))
                                                _output = "formatted";

                                            if (!string.IsNullOrEmpty(_name))
                                            {
                                                var _val = dataItem[_name];
                                                if (_val != null)
                                                {
                                                    if (_output == "formatted")
                                                        writer.Write(field.Format(_val));
                                                    else
                                                        writer.Write(_val);
                                                }
                                                else
                                                {
                                                    writer.Write(dataItem[_name]);
                                                }
                                            }
                                        }
                                    }

                                    continue;
                                }
                            }
                            #endregion

                            var output = element.StrAttr("output");
                            var val = dataItem[name];
                            if (val != null)
                            {
                                if (output == "formatted")
                                    writer.Write(field.Format(val));
                                else
                                    writer.Write(val);
                            }
                        }
                        else //internal values
                        {
                            writer.Write(dataItem[name]);
                        }
                    }
                }

                if (element.Name == "html")
                {
                    writer.Write(element.Value);
                    continue;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static HelperResult RenderPattern(this ContentQueryResult model)
        {
            return new HelperResult(w =>
        {
            using (var writer = new Html32TextWriter(w))
            {
                var view = model.View;
            }
        });
        }

        /// <summary>
        /// Render the empty view html to response ouput.
        /// </summary>
        /// <param name="view">The view object.</param>
        /// <returns></returns>
        public static HelperResult RenderEmptyPattern(this ContentViewDecorator view)
        {
            return new HelperResult(w =>
           {
               using (var writer = new Html32TextWriter(w))
               {
                   var url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                   var emptyDocument = new XmlDocument();
                   if (!string.IsNullOrEmpty(view.EmptyTemplateXml))
                       emptyDocument.LoadXml(view.EmptyTemplateXml);
                   else
                   {
                       writer.Write("<div style=\"line-height:50px;text-align:center;\" class=\"d-title\">No item found</div>");
                       return;
                   }

                   var emptyNode = emptyDocument.DocumentElement;
                   var contentType = emptyNode.Attributes["contentType"] != null ? emptyNode.Attributes["contentType"].Value : "text/xml";

                   if (contentType == "text/html")
                   {
                       var contentText = emptyNode.InnerText;
                       if (!string.IsNullOrEmpty(contentText))
                       {
                           writer.Write(contentText);
                       }
                       return;
                   }

                   if (contentType == "application/x-ms-aspnet")
                   {
                       return;
                   }

                   if (contentType == "text/xml")
                   {
                       var nodes = emptyNode.ChildNodes;
                       for (int i = 0; i < nodes.Count; i++)
                       {
                           var node = nodes[i];
                           if (node.Name == "url")
                           {
                               var type = node.Attributes["type"] != null && !string.IsNullOrEmpty(node.Attributes["type"].Value) ? node.Attributes["type"].Value : "new";

                               if (type == "new")
                                   writer.Write(url.Content(view.Parent.GetNewItemUrl()));
                               if (type == "request")
                                   writer.Write(HttpContext.Current.Request.Url.ToString());
                               continue;
                           }

                           if (node.Name == "html")
                           {
                               writer.Write(node.InnerText);
                               continue;
                           }
                       }
                   }
               }
           });
        }

        /// <summary>
        /// Render the form html to repsonse by specified the form object for query result item.
        /// </summary>
        /// <remarks>
        /// Supports for 3.0.3
        /// </remarks>
        /// <param name="helper">The html helper object.</param>
        /// <param name="form">The content form object.</param>
        /// <param name="queryResultItem">The query result item.</param>
        /// <returns></returns>
        public static HelperResult RenderForm(this HtmlHelper helper, ContentFormDecorator form, ContentQueryResultItem queryResultItem)
        {
            return RenderForm(helper, form, queryResultItem.RawItem);
        }

        /// <summary>
        /// Render the form html to response output by specified the form object.This method will auto determine which mode to render.
        /// </summary>
        /// <param name="helper">The html helper object.</param>
        /// <param name="form">The content form object.</param>
        /// <param name="model">The data item for edit,display and activity form</param>
        /// <returns></returns>
        public static HelperResult RenderForm(this HtmlHelper helper, ContentFormDecorator form, ContentDataItemDecorator model = null)
        {
            return new HelperResult(writer =>
            {
                var server = helper.ViewContext.HttpContext.Server;
                var list = form.Parent;
                var defaultViewFile = "";
                var format = "~/content/types/base/forms/{0}.cshtml";

                switch (form.FormType)
                {
                    case (int)ContentFormTypes.New:
                        defaultViewFile = string.Format(format, "_new");
                        break;
                    case (int)ContentFormTypes.Edit:
                        defaultViewFile = string.Format(format, "_edit");
                        break;
                    case (int)ContentFormTypes.Activity:
                        defaultViewFile = string.Format(format, "_activity");
                        break;
                    default:
                        defaultViewFile = string.Format(format, "_detail");
                        break;
                }

                var body = form.Body;
                if (form.HasTemplate)
                {
                    //The form must be has template
                    if (!string.IsNullOrEmpty(body.Source))
                    {
                        #region file template
                        var viewFile = list.Package.ResolveUri(form.Body.Source);
                        if (!File.Exists(server.MapPath(viewFile)))
                        {
                            viewFile = defaultViewFile;
                        }
                        #endregion

                        if (model != null)
                            helper.RenderPartial(viewFile, model, helper.ViewData);
                        else
                            helper.RenderPartial(viewFile, helper.ViewData);
                    }
                    else
                    {
                        helper.RenderFormTemplate(form, model);
                    }
                }
                else
                {
                    //The form has no any template definition
                    if (model != null)
                        helper.RenderPartial(defaultViewFile, model, helper.ViewData);
                    else
                        helper.RenderPartial(defaultViewFile, helper.ViewData);
                }
            });
        }

        #endregion

        private static void WriteScripts(TextWriter w, string scriptsXml, ContentPackage pkg)
        {
            if (!string.IsNullOrEmpty(scriptsXml))
            {
                var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
                var scriptsEl = XElement.Parse(scriptsXml);
                if (scriptsEl != null && scriptsEl.HasElements)
                {
                    foreach (var script in scriptsEl.Elements())
                    {
                        var type = string.IsNullOrEmpty(script.StrAttr("type")) ? "text/javascript" : script.StrAttr("type");

                        var src = script.StrAttr("src");
                        if (!string.IsNullOrEmpty(src))
                        {
                            //var scriptElement = new XElement("script", new XAttribute("type", type));
                            var formattedSrc = src;
                            if (!src.StartsWith("http"))
                            {
                                if (src.StartsWith("~"))
                                    formattedSrc = Url.Content(src);
                                else
                                    formattedSrc = Url.Content(pkg.ResolveUri(src));
                            }
                            w.Write("<script type=\"" + type + "\" src=\"" + formattedSrc + "\"></script>");
                        }
                        else
                        {
                            var scriptContent = string.IsNullOrEmpty(script.Value) ? script.InnerXml() : script.Value;
                            w.Write("<script type=\"" + type + "\">");
                            w.Write(scriptContent);
                            w.Write("</script>");
                        }

                    }
                }
            }
        }

        private static void WriteStyles(TextWriter w, string stylesXml, ContentPackage pkg)
        {
            if (!string.IsNullOrEmpty(stylesXml))
            {
                var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
                var stylesEl = XElement.Parse(stylesXml);
                if (stylesEl != null && stylesEl.HasElements)
                {
                    foreach (var style in stylesEl.Elements())
                    {
                        var src = style.StrAttr("src");
                        if (!string.IsNullOrEmpty(src))
                        {
                            #region link element
                            var linkElement = new XElement("link",
                                new XAttribute("type", "text/css"),
                                new XAttribute("rel", "stylesheet"));
                            var formattedSrc = src;
                            if (!src.StartsWith("http"))
                            {
                                if (src.StartsWith("~"))
                                    formattedSrc = Url.Content(src);
                                else
                                    formattedSrc = Url.Content(pkg.ResolveUri(src));
                            }
                            linkElement.Add(new XAttribute("href", formattedSrc));
                            w.Write(linkElement.OuterXml());
                            #endregion
                        }
                        else
                        {
                            w.Write("<style type=\"text/css\" >");
                            var csstext = string.IsNullOrEmpty(style.Value) ? style.InnerXml() : style.Value;
                            if (!string.IsNullOrEmpty(csstext))
                                w.Write(csstext);
                            w.Write("</style>");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static HelperResult Scripts(this ContentViewDecorator view)
        {
            return new HelperResult(w =>
            {
                WriteScripts(w, view.ScriptsXml, view.Parent.Package);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static HelperResult Scripts(this ContentFormDecorator form)
        {
            return new HelperResult(w =>
            {
                WriteScripts(w, form.ScriptsXml, form.Parent.Package);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static HelperResult StyleSheets(this ContentViewDecorator view)
        {
            return new HelperResult(w =>
            {
                WriteStyles(w, view.StyleSheetsXml, view.Parent.Package);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static HelperResult StyleSheets(this ContentFormDecorator form)
        {
            return new HelperResult(w =>
            {
                WriteStyles(w, form.StyleSheetsXml, form.Parent.Package);
            });
        }

        /// <summary>
        /// Render hidden input element for returnUrl.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <returns></returns>
        public static MvcHtmlString ReturnUrlHolder(this HtmlHelper html)
        {
            var request = html.ViewContext.HttpContext.Request;
            if (!string.IsNullOrEmpty(request.QueryString["returnUrl"]) && !request.IsAjaxRequest())
            {
                return MvcHtmlString.Create("<input type=\"hidden\" name=\"returnUrl\" value=\"" + request.QueryString["returnUrl"] + "\"/>");
            }
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Generate validation data attributes for TextField object.
        /// </summary>
        /// <param name="field">The TextField object.</param>
        /// <returns></returns>
        public static HelperResult GetValidationAttributes(this TextField field)
        {
            string ValidationErrorIncludingMinimum = "The {0} must be a string with a minimum length of {2} and a maximum length of {1}.";
            string StringLengthAttribute_ValidationError = "The {0} must be a string with a maximum length of {1}.";

            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    if (field.MaxLength > 0 || field.MinLength > 0 || field.IsRequired)
                    {
                        writer.WriteAttribute("data-val", "true");
                        writer.WriteAttribute("data-val-required", string.Format(RES_REQURIED, field.Title));
                        if (field.MaxLength > 0 && field.MinLength > 0)
                        {
                            writer.WriteAttribute("data-val-length", string.Format(ValidationErrorIncludingMinimum, field.MinLength, field.MinLength));
                            writer.WriteAttribute("data-val-length-max", field.MaxLength.ToString());
                            writer.WriteAttribute("data-val-length-min", field.MinLength.ToString());
                        }
                        else
                        {
                            if (field.MaxLength > 0)
                            {
                                writer.WriteAttribute("data-val-length-max", field.MaxLength.ToString());
                                writer.WriteAttribute("data-val-length", string.Format(StringLengthAttribute_ValidationError, field.MaxLength));
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Generate validation data attributes for NumberField object.
        /// </summary>
        /// <param name="field">The NumberField object.</param>
        /// <returns></returns>
        public static HelperResult GetValidationAttributes(this NumberField field)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    writer.WriteAttribute("data-val", "true");
                    writer.WriteAttribute("data-val-number", string.Format("The {0} must be a vaild number.", field.Title));
                    //if (field.DecimalDigits > 0)
                    //    writer.WriteAttribute("data-val-digits", string.Format("The {0} must be have {1} digits.", field.Title, field.DecimalDigits));

                    if (field.MaximumValue > 0 || field.MaximumValue > 0 || field.IsRequired)
                    {
                        writer.WriteAttribute("data-val-required", string.Format(RES_REQURIED, field.Title));
                        if (field.MaximumValue > 0 && field.MaximumValue > 0)
                        {
                            writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_ValidationError, field.MaximumValue, field.MaximumValue));
                            writer.WriteAttribute("data-val-range-max", field.MaximumValue.ToString());
                            writer.WriteAttribute("data-val-range-min", field.MaximumValue.ToString());
                        }
                        else
                        {
                            if (field.MaximumValue > 0)
                            {
                                writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_Max, field.MaximumValue));
                                writer.WriteAttribute("data-val-range-max", field.MaximumValue.ToString());
                            }

                            if (field.MinimumValue > 0)
                            {
                                writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_Min, field.MinimumValue));
                                writer.WriteAttribute("data-val-range-min", field.MinimumValue.ToString());
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Generate validation data attributes for IntegerField object.
        /// </summary>
        /// <param name="field">The IntegerField object.</param>
        /// <returns></returns>
        public static HelperResult GetValidationAttributes(this IntegerField field)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    writer.WriteAttribute("data-val", "true");
                    // writer.WriteAttribute("data-val-number", string.Format("The {0} must be a vaild integer value.", field.Title));
                    writer.WriteAttribute("data-val-digits", string.Format("The {0} must be a vaild integer value.", field.Title));

                    if (field.MaximumValue > 0 || field.MaximumValue > 0 || field.IsRequired)
                    {

                        writer.WriteAttribute("data-val-required", string.Format(RES_REQURIED, field.Title));
                        if (field.MaximumValue > 0 && field.MaximumValue > 0)
                        {
                            writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_ValidationError, field.Name, field.MaximumValue, field.MaximumValue));
                            writer.WriteAttribute("data-val-range-max", field.MaximumValue.ToString());
                            writer.WriteAttribute("data-val-range-min", field.MaximumValue.ToString());
                        }
                        else
                        {
                            if (field.MaximumValue > 0)
                            {
                                writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_Max, field.Name, field.MaximumValue));
                                writer.WriteAttribute("data-val-range-max", field.MaximumValue.ToString());
                            }

                            if (field.MinimumValue > 0)
                            {
                                writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_Min, field.Name, field.MinimumValue));
                                writer.WriteAttribute("data-val-range-min", field.MinimumValue.ToString());
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Generate validation data attributes for CurrentField object.
        /// </summary>
        /// <param name="field">The CurrencyField object.</param>
        /// <returns></returns>
        public static HelperResult GetValidationAttributes(this CurrencyField field)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    writer.WriteAttribute("data-val", "true");
                    writer.WriteAttribute("data-val-number", string.Format("The {0} must be a vaild number.", field.Title));
                    //if (field.DecimalDigits > 0)
                    //    writer.WriteAttribute("data-val-digits", string.Format("The {0} must be have {1} digits.", field.Title, field.DecimalDigits));

                    if (field.MaximumValue > 0 || field.MaximumValue > 0 || field.IsRequired)
                    {

                        writer.WriteAttribute("data-val-required", string.Format(RES_REQURIED, field.Title));
                        if (field.MaximumValue > 0 && field.MaximumValue > 0)
                        {
                            writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_ValidationError, field.MaximumValue, field.MaximumValue));
                            writer.WriteAttribute("data-val-range-max", field.MaximumValue.ToString());
                            writer.WriteAttribute("data-val-range-min", field.MaximumValue.ToString());
                        }
                        else
                        {
                            if (field.MaximumValue > 0)
                            {
                                writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_Max, field.MaximumValue));
                                writer.WriteAttribute("data-val-range-max", field.MaximumValue.ToString());
                            }

                            if (field.MinimumValue > 0)
                            {
                                writer.WriteAttribute("data-val-range", string.Format(RangeAttribute_Min, field.MinimumValue));
                                writer.WriteAttribute("data-val-range-min", field.MinimumValue.ToString());
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Generate MicroData attributes for specified field object.
        /// </summary>
        /// <param name="field">The field object.</param>
        /// <param name="value">The field value object.</param>
        /// <returns></returns>
        public static HelperResult GetMicroDataAttributes(this ContentField field, object value = null)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    if (!string.IsNullOrEmpty(field.ItemType))
                    {
                        writer.WriteAttribute("itemscope ", "itemscope");
                        writer.WriteAttribute("itemtype", field.ItemType);
                    }

                    if (!string.IsNullOrEmpty(field.ItemProp))
                        writer.WriteAttribute("itemprop", field.ItemProp);
                }
            });
        }

        /// <summary>
        /// Generate the common data attributes for specified field object.
        /// </summary>
        /// <param name="field">The field object.</param>
        /// <param name="value">The field value object.</param>
        /// <returns></returns>
        public static HelperResult GetAttributes(this ContentField field, object value = null)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    writer.WriteAttribute("id", field.ClientID);
                    writer.WriteAttribute("name", field.Name);

                    if (value != null)
                    {
                        writer.WriteAttribute("value", value.ToString());
                    }
                    else
                    {
                        if (field.DefaultValue != null)
                            writer.WriteAttribute("value", field.DefaultValue.ToString());
                    }

                    if (!string.IsNullOrEmpty(field.Placeholder))
                        writer.WriteAttribute("placeholder", field.Placeholder);
                    else
                        writer.WriteAttribute("placeholder", field.Title);

                    if (field.IsReadOnly)
                    {
                        writer.WriteAttribute("readonly", "readonly");
                        writer.WriteAttribute("class", "d-state-disable");
                    }
                }
            });
        }

        /// <summary>
        /// Render hidden input element for field value.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="field">The field object</param>
        /// <param name="value">the field value</param>
        /// <returns></returns>
        public static HelperResult Hidden(this HtmlHelper html, ContentField field, object value = null)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("type", "hidden");
                    writer.WriteAttribute("id", field.ClientID);
                    writer.WriteAttribute("name", field.Name);
                    if (value != null)
                    {
                        writer.WriteAttribute("value", value.ToString());
                    }
                    else
                    {
                        if (field.DefaultValue != null)
                            writer.WriteAttribute("value", field.DefaultValue.ToString());
                    }
                    writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                }
            });
        }

        /// <summary>
        /// Render html element for specified field object.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="field">The field object</param>
        /// <returns></returns>
        public static HelperResult Label(this HtmlHelper html, ContentField field)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    try
                    {
                        writer.WriteBeginTag("label");
                        writer.WriteAttribute("for", field.ClientID);
                        writer.Write(HtmlTextWriter.TagRightChar);
                        if (!string.IsNullOrEmpty(field.Title))
                            writer.WriteEncodedText(field.Title);
                        else
                            writer.WriteEncodedText(field.Name);
                        writer.WriteEndTag("label");
                    }
                    catch
                    {

                    }
                }
            });
        }

        /// <summary>
        /// Render field description text for specified field object.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="field">The field object.</param>
        /// <returns></returns>
        public static HelperResult Notes(this HtmlHelper html, ContentField field)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    if (!string.IsNullOrEmpty(field.Description))
                    {
                        writer.WriteBeginTag("small");
                        //writer.WriteAttribute("class", "d-field-notes d-notes");
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.WriteEncodedText(field.Description);
                        writer.WriteEndTag("small");
                    }
                }
            });
        }

        /// <summary>
        /// Render validation HTML elements for specified field object.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="field">The field object.</param>
        /// <returns></returns>
        public static HelperResult ValidationMessage(this HtmlHelper html, ContentField field)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    writer.WriteBeginTag("span");
                    writer.WriteAttribute("class", "d-field-val-msg d-valmsg");
                    writer.WriteAttribute("data-valmsg-for", field.Name);
                    writer.WriteAttribute("data-valmsg-replace", "true");
                    writer.Write(HtmlTextWriter.TagRightChar);
                    //writer.WriteEncodedText(field.Description);
                    writer.WriteEndTag("span");
                }
            });
        }

        [Obsolete]
        public static HelperResult NewStatus(this HtmlHelper html, ContentDataItemDecorator dataItem)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    if (dataItem.IsNew)
                    {
                        writer.WriteBeginTag("sup");
                        //writer.WriteAttribute("for", field.ClientID);
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Write("new");
                        //writer.WriteEncodedText(field.Title);
                        writer.WriteEndTag("sup");
                    }
                }
            });
        }

        [Obsolete]
        public static HelperResult AttachIcon(this HtmlHelper html, ContentDataItemDecorator dataItem)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    if (dataItem.HasAttachments)
                    {
                        var url = new UrlHelper(html.ViewContext.RequestContext);
                        writer.WriteBeginTag("a");
                        writer.WriteAttribute("href", url.Content(dataItem) + "#attachments");
                        writer.WriteAttribute("class", "d-icon d-icon-attach");
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.WriteEndTag("a");
                    }
                }
            });
        }

        [Obsolete]
        public static HelperResult DraftIcon(this HtmlHelper html, ContentDataItemDecorator dataItem)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    if (!dataItem.IsPublished)
                    {
                        writer.WriteBeginTag("span");
                        writer.WriteAttribute("title", "Draft");
                        writer.WriteAttribute("class", "d-icon d-icon-draft");
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.WriteEndTag("span");
                    }
                }
            });
        }

        [Obsolete]
        public static HelperResult ModerateStatus(this HtmlHelper html, ContentDataItemDecorator dataItem)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    writer.WriteBeginTag("span");
                    writer.Write(HtmlTextWriter.TagRightChar);
                    if (dataItem.IsPublished)
                    {
                        if (dataItem.Parent.IsModerated)
                        {
                            var state = (ModerateStates)dataItem.ModerateState;
                            switch (state)
                            {
                                case ModerateStates.Pending:
                                    writer.Write("Pending review");
                                    break;
                                case ModerateStates.Rejected:
                                    writer.Write("Rejected");
                                    break;
                                default:
                                    writer.Write("Published");
                                    break;
                            }
                        }
                        else
                            writer.Write("Published");
                    }
                    else
                        writer.Write("Draft");

                    writer.WriteEndTag("span");
                }
            });
        }

        /// <summary>
        /// Create a decorator for current data item.
        /// </summary>
        /// <param name="dataItem">The data item to wrap.</param>
        /// <returns>The decorator object for current data item.</returns>
        public static ContentDataItemDecorator Wrap(this ContentDataItem dataItem)
        {
            return App.Get().Wrap(dataItem);
        }

        private static string GetParams(object values)
        {
            if (values == null)
                return "";
            var requestData = values.ToDictionary();
            return string.Join("&", requestData.Select(r => r.Key + "=" + r.Value).ToArray());
        }

        /// <summary>
        /// Build an url by specified relavite path and parameters.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="path">The relavite path.</param>
        /// <param name="values">The parameters of query string.</param>
        /// <returns>A string contains the new absolute url</returns>
        public static string BuildUrl(this UrlHelper helper, string path, object values)
        {
            var _params = GetParams(values);
            return helper.Content(path) + (!string.IsNullOrEmpty(_params) ? ("?" + _params) : "");
        }

        /// <summary>
        /// Marge parameters to querystring for current request url.
        /// </summary>
        /// <param name="helper">The url helper object.</param>
        /// <param name="parameters">The query string parameters.</param>
        /// <returns>A string contains new url that merged specified parameters.</returns>
        public static string MergeQueryParameters(this UrlHelper helper, object parameters)
        {
            return helper.RequestContext.HttpContext.Request.MergeQueryParameters(parameters);
        }

        /// <summary>
        /// Marge parameters to querystring for specified request url
        /// </summary>
        /// <param name="request">The request object</param>
        /// <param name="parameters">The query string parameters.</param>
        /// <returns>A string contains new url that merged specified parameters.</returns>
        public static string MergeQueryParameters(this HttpRequestBase request, object parameters)
        {
            var _params = parameters.ToDictionary();
            var _results = new List<string>();
            var queryString = request.QueryString;

            if (queryString != null)
            {
                foreach (var key in queryString.AllKeys)
                {
                    if (!_params.ContainsKey(key))
                        _params.Add(key, queryString[key]);
                }

                foreach (var key in _params.Keys)
                {
                    _results.Add(string.Format("{0}={1}", key, _params[key]));
                }
                return "?" + string.Join("&", _results);
            }
            return "";
        }

        /// <summary>
        /// Render HTML pager element for specified content query result.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="model"></param>
        /// <param name="htmlAttributes">The html attributes object for the output html element.</param>
        /// <returns></returns>
        public static HelperResult Pager(this HtmlHelper html, ContentQueryResult model, object htmlAttributes = null)
        {
            return new HelperResult(writer =>
            {
                var Url = new UrlHelper(html.ViewContext.RequestContext);

                if (model.View.AllowPaging)
                {
                    var maxPageCount = 10;
                    var curIndex = model.Query.Index;
                    var startIndex = 1;
                    var endIndex = maxPageCount * startIndex;
                    var curPage = (int)Math.Ceiling((decimal)((decimal)curIndex / (decimal)maxPageCount));

                    if (curPage > 1)
                        startIndex = (curPage * maxPageCount) - maxPageCount + 1;

                    endIndex = startIndex + maxPageCount;

                    if (endIndex > model.Query.TotalPages)
                        endIndex = model.Query.TotalPages;

                    var _params = new List<string>();
                    var queryString = html.ViewContext.RequestContext.HttpContext.Request.QueryString;
                    if (queryString != null)
                    {
                        foreach (var key in queryString.AllKeys)
                        {
                            if (key.Equals("index", StringComparison.OrdinalIgnoreCase) || key.Equals("size", StringComparison.OrdinalIgnoreCase))
                                continue;
                            _params.Add(string.Format("{0}={1}", key, queryString[key]));
                        }
                    }

                    if (model.Query.TotalPages <=1)
                        return;

                    var isLast = curPage * maxPageCount >= model.Query.TotalPages;
                    var isFirst = startIndex == 1;
                    var builder = new UriBuilder();

                    if (isLast)
                        endIndex++;

                    var pageElement = new XElement("div", new XAttribute("data-role", "pager"),
                        new XAttribute("data-index", curIndex - 1));

                    pageElement.AddHtmlAttributes(htmlAttributes);

                    if (!isFirst)
                    {
                        pageElement.Add(
                            new XElement("a",
                                new XAttribute("href", Url.BuildUrl(model.View.Url, _params)),
                                new XAttribute("title", "Go to first page"),
                                new XAttribute("class", "d-ui-widget d-button"),
                                new XElement("span", new XAttribute("class", "d-icon d-icon-first"))),
                            new XElement("a",
                                new XAttribute("href", Url.Content(model.View.Url) + "?index=" + (curIndex - 1).ToString() + "&size=" + model.Query.Size + (_params.Count > 0 ? ("&" + string.Join("&", _params.ToArray())) : "")),
                                new XAttribute("title", "Go to prev page"),
                                new XAttribute("class", "d-ui-widget d-button"),
                                new XElement("span", new XAttribute("class", "d-icon d-icon-prev")))
                            );
                    }

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var pagerItem = new XElement("a", i != curIndex ? i : curIndex);

                        //if (i == curIndex)
                        //    pagerItem.Add(new XAttribute("class", "d-ui-widget d-button d-state-active"));
                        //else
                        pagerItem.Add(new XAttribute("class", "d-ui-widget d-button"));

                        if (i != curIndex)
                            pagerItem.Add(new XAttribute("href", Url.Content(model.View.Url) + "?index=" + i.ToString() + "&size=" + model.Query.Size + (_params.Count > 0 ? ("&" + string.Join("&", _params.ToArray())) : "")));

                        pageElement.Add(pagerItem);
                    }

                    if (!isLast)
                    {
                        pageElement.Add(
                            new XElement("a",
                                new XAttribute("href", Url.Content(model.View.Url) + "?index=" + (curIndex + 1).ToString() + "&size=" + model.Query.Size + (_params.Count > 0 ? ("&" + string.Join("&", _params.ToArray())) : "")),
                                new XAttribute("title", "Go to next page"),
                                new XAttribute("class", "d-ui-widget d-button"),
                                new XElement("span", new XAttribute("class", "d-icon d-icon-next"))),
                            new XElement("a",
                                new XAttribute("href", Url.Content(model.View.Url) + "?index=" + (model.Query.TotalPages).ToString() + "&size=" + model.Query.Size + (_params.Count > 0 ? ("&" + string.Join("&", _params.ToArray())) : "")),
                                new XAttribute("title", "Go to last page"),
                                new XAttribute("class", "d-ui-widget d-button"),
                                new XElement("span", new XAttribute("class", "d-icon d-icon-last")))
                                );
                    }

                    writer.Write(pageElement.OuterXml());
                }

            });
        }

        /// <summary>
        ///  Render HTML pager element for specified search query.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="query">The search query object.</param>
        /// <param name="htmlAttributes">The html attributes object for the output html element.</param>
        /// <returns></returns>
        public static HelperResult Pager(this HtmlHelper html, SearchQuery query, object htmlAttributes = null)
        {
            return new HelperResult(writer =>
            {
                var Url = new UrlHelper(html.ViewContext.RequestContext);
                var website = html.ViewContext.RouteData.Values["website"].ToString();
                var locale = html.ViewContext.RouteData.Values["locale"].ToString();
                var _url = App.Get().Context.AppUrl + website + "/" + locale + "search/" + query.Source;

                var maxPageCount = 10;
                var curIndex = query.Index;
                var startIndex = 1;
                var endIndex = maxPageCount * startIndex;
                var curPage = (int)Math.Ceiling((decimal)((decimal)curIndex / (decimal)maxPageCount));

                if (curPage > 1)
                    startIndex = (curPage * maxPageCount) - maxPageCount + 1;

                endIndex = startIndex + maxPageCount;

                if (endIndex > query.TotalPages)
                    endIndex = query.TotalPages;

                var _params = new List<string>();
                var queryString = html.ViewContext.RequestContext.HttpContext.Request.QueryString;
                if (queryString != null)
                {
                    foreach (var key in queryString.AllKeys)
                    {
                        if (key.Equals("index", StringComparison.OrdinalIgnoreCase) || key.Equals("size", StringComparison.OrdinalIgnoreCase))
                            continue;
                        _params.Add(string.Format("{0}={1}", key, queryString[key]));
                    }
                }

                var isLast = curPage * maxPageCount >= query.TotalPages;
                var isFirst = startIndex == 1;
                var builder = new UriBuilder();

                //if (startIndex == endIndex && startIndex == 1)
                //    return;

                if (isLast)
                    endIndex++;

                var pageElement = new XElement("div", new XAttribute("class", "d-pager"));
                pageElement.AddHtmlAttributes(htmlAttributes);

                if (!isFirst)
                {
                    pageElement.Add(
                        new XElement("a",
                            new XAttribute("href", Url.BuildUrl(_url, _params)),
                            new XAttribute("title", "First"),
                            new XAttribute("class", "d-ui-widget d-button"),
                            new XElement("span", new XAttribute("class", "d-icon d-icon-first"))),
                        new XElement("a",
                            new XAttribute("href", _url + "?index=" + (curIndex - 1).ToString() + "&size=" + query.Size + (_params.Count > 0 ? ("&" + string.Join("&", _params.ToArray())) : "")),
                            new XAttribute("title", "Prev"),
                            new XElement("span", new XAttribute("class", "d-icon d-icon-prev")))
                        );
                }

                for (int i = startIndex; i < endIndex; i++)
                {
                    var pagerItem = new XElement("a", i != curIndex ? i : curIndex);


                    if (i == curIndex)
                        pagerItem.Add(new XAttribute("class", "d-ui-widget d-button d-state-active"));
                    else
                        pagerItem.Add(new XAttribute("class", "d-ui-widget d-button"));

                    if (i != curIndex)
                        pagerItem.Add(new XAttribute("href", _url + "?index=" + i.ToString() + "&size=" + query.Size + (_params.Count > 0 ? ("&" + string.Join("&", _params.ToArray())) : "")));

                    pageElement.Add(pagerItem);
                }

                if (!isLast)
                {
                    pageElement.Add(
                        new XElement("a",
                            new XAttribute("href", _url + "?index=" + (curIndex + 1).ToString() + "&size=" + query.Size + (_params.Count > 0 ? ("&" + string.Join("&", _params.ToArray())) : "")),
                            new XAttribute("title", "Next"),
                            new XAttribute("class", "d-ui-widget d-button"),
                            new XElement("span", new XAttribute("class", "d-icon d-icon-next"))),
                        new XElement("a",
                            new XAttribute("href", _url + "?index=" + (query.TotalPages).ToString() + "&size=" + query.Size + (_params.Count > 0 ? ("&" + string.Join("&", _params.ToArray())) : "")),
                            new XAttribute("title", "Last"),
                            new XAttribute("class", "d-ui-widget d-button"),
                            new XElement("span", new XAttribute("class", "d-icon d-icon-last")))
                            );
                }

                writer.Write(pageElement.OuterXml());
            });
        }

        /// <summary>
        /// Renders the data items in ContentQueryResult with view template what defined in result's view.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="results">The content query result object.</param>
        /// <returns></returns>
        public static HelperResult RenderViewTemplate(this HtmlHelper html, ContentQueryResult results)
        {
            return new HelperResult(w =>
            {
                var view = results.View;
                var body = view.GetBodyTemplate();
                if (!string.IsNullOrEmpty(body.Text))
                {
                    #region xslt
                    if (body.ContentType == TemplateContentTypes.Xslt)
                    {
                        var doc = new XDocument(results.Element());
                        var xslt = new XslCompiledTransform();
                        if (!string.IsNullOrEmpty(body.Text))
                        {
                            try
                            {
                                xslt.Load(XmlReader.Create(new StringReader(body.Text)));
                                var args = new XsltArgumentList();
                                args.AddParam("appPath", "", App.Get().Context.AppUrl.ToString());
                                args.AddParam("web", "", view.Parent.Web.Name);
                                args.AddParam("lang", "", view.Parent.Locale);
                                args.AddParam("list", "", view.Parent.Name);
                                args.AddParam("view", "", view.Name);
                                xslt.Transform(doc.CreateReader(), args, w);
                            }
                            catch
                            {
                            }
                        }
                        return;
                    }
                    #endregion

                    #region razor
                    if (body.ContentType == TemplateContentTypes.Razor)
                    {
                        var viewFile = TemplateHelper.SaveAsView(view.Parent, body.Text, string.Format("_view_{0}_tmpl.cshtml", view.Name));
                        html.RenderPartial(viewFile);
                        return;
                    }
                    #endregion

                    #region xml
                    if (body.ContentType == TemplateContentTypes.Xml)
                    {
                        var doc = XDocument.Parse("<dispPattern>" + body.Text + "</dispPattern>");
                        var elements = doc.Root.Elements();
                        foreach (var element in elements)
                        {
                            if (element.Name == "rows")
                            {
                                //var rows = model.Items;
                                foreach (var row in results)
                                {
                                    RenderRowPattern(element, w, row);
                                }
                                continue;
                            }

                            if (element.Name.Equals("col"))
                            {
                                RenderColPattern(element, w, view.Parent);
                                continue;
                            }

                            if (element.Name == "html")
                            {
                                w.Write(element.Value);
                                continue;
                            }
                        }
                        return;
                    }
                    #endregion
                }
            });

        }

        /// <summary>
        /// Renders the data item with form item template.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="form">The form object.</param>
        /// <param name="dataItem">The data item object.</param>
        /// <returns></returns>
        public static HelperResult RenderFormTemplate(this HtmlHelper html, ContentFormDecorator form, ContentDataItemDecorator dataItem = null)
        {
            return new HelperResult(writer =>
            {
                var body = form.Body;
                var _contentType = body.ContentType;

                #region razor template
                if (_contentType == TemplateContentTypes.Razor)
                {
                    var viewFile = string.Format("_form_{0}_tmpl.cshtml", form.FormTypeString.ToLower());
                    var viewUrl = TemplateHelper.SaveAsView(form.Parent, body.Text, viewFile);
                    html.RenderPartial(viewUrl, dataItem);
                    return;
                }
                #endregion

                if (_contentType == TemplateContentTypes.Html)
                {
                    throw new NotImplementedException();
                }

                #region xml template

                if (_contentType == TemplateContentTypes.Xml)
                {
                    var doc = XDocument.Parse(body.Text);

                    foreach (var element in doc.Elements())
                    {
                        if (element.Name == "label")
                        {
                            var nameAttr = element.StrAttr("name");
                            var field = form.Parent.Fields[nameAttr];
                            if (field != null)
                                html.ViewContext.Writer.Write(html.Label(field));
                            continue;
                        }

                        if (element.Name == "notes")
                        {
                            var nameAttr = element.StrAttr("name");
                            var field = form.Parent.Fields[nameAttr];
                            if (field != null)
                                html.ViewContext.Writer.Write(html.Notes(field));
                            continue;
                        }

                        if (element.Name.Equals("col"))
                        {
                            var nameAttr = element.StrAttr("name");
                            var field = form.Fields[nameAttr];
                            var hasLabel = element.BoolAttr("hasLabel");
                            var hasNotes = element.BoolAttr("hasNotes");

                            if (field != null)
                            {
                                switch ((ContentFormTypes)form.FormType)
                                {
                                    case ContentFormTypes.Display:
                                        html.ForDisp(field, dataItem, hasLabel);
                                        break;
                                    case ContentFormTypes.Edit:
                                        html.ForEdit(field, dataItem, hasLabel, hasNotes);
                                        break;
                                    case ContentFormTypes.New:
                                        html.ForNew(field, hasLabel, hasNotes);
                                        break;
                                }
                            }
                        }

                        if (element.Name == "html")
                        {
                            writer.Write(element.InnerXml());
                            continue;
                        }
                    }
                }

                #endregion

                #region xslt template

                if (_contentType == TemplateContentTypes.Xslt)
                {
                    throw new NotImplementedException();
                }

                #endregion

            });
        }

        /// <summary>
        /// Renders the data items for current user.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="helper">The personal list helper object.</param>
        /// <param name="pager">Identity whether show the pager elements.</param>
        public static void RenderQueryResults(this HtmlHelper html, PersonalListHelper helper, bool pager = true)
        {
            var view = helper.Parent.DefaultView;
            RenderQueryResults(html, view, helper.Query);
        }

        /// <summary>
        /// Renders the data items that returns by a content query.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="view">The content view object that use to execute query.</param>
        /// <param name="query">The content query object.</param>
        public static void RenderQueryResults(this HtmlHelper html, ContentViewDecorator view, ContentQuery query)
        {
            RenderQueryResults(html, view.GetItems(query));
        }

        /// <summary>
        /// Render the specified view object to HTML
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="view">The view to render.</param>
        public static void RenderViewResults(this HtmlHelper html, ContentViewDecorator view)
        {
            html.RenderQueryResults(view.GetItems());
        }

        /// <summary>
        /// Render the content query results to HTML.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="results">The query results object.</param>
        public static void RenderQueryResults(this HtmlHelper html, ContentQueryResult results)
        {
            html.RenderPartial("~/Views/Contents/_ServerView.cshtml", results);
            if (results.View != null && results.View.AllowPaging)
                html.ViewContext.Writer.Write(html.Pager(results));
            //html.ViewContext.HttpContext.Response.Write();
        }

        /// <summary>
        /// Render a HTML button that use to show the new form for specified content list object.
        /// </summary>
        /// <param name="html">The html helper object.</param>
        /// <param name="list">The content list object.</param>
        /// <returns></returns>
        public static HelperResult NewItemDialogButton(this HtmlHelper html, ContentListDecorator list)
        {
            return new HelperResult(writer =>
            {
                var newForm = list.NewForm;
                if (newForm == null) return;

                var Url = new UrlHelper(html.ViewContext.RequestContext);
                var addNewUrl = UrlBuilder.Create(Url.Content(list.GetNewItemUrl())).AddParam("locale", list.Locale).ToString();

                var xEle = new XElement("a", string.IsNullOrEmpty(newForm.Title) ? HttpContext.GetGlobalResourceObject("Contents", "AddItem").ToString() : newForm.Title,
                    new XAttribute("data-role", "button"),
                    new XAttribute("data-rel", "dialog"),
                    new XAttribute("data-default", "true"),
                    new XAttribute("data-dialog-title", newForm.Title),
                    new XAttribute("data-dialog-cache", "false"),
                    new XAttribute("data-dialog-modal", "true"),
                    new XAttribute("data-dialog-fullscreen", "true"),
                    new XAttribute("href", addNewUrl));


                writer.Write(xEle.OuterXml());
            });
        }
    }
}
