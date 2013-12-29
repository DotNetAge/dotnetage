//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace DNA.Web.UI
{
    /// <summary>
    /// Represents a component class to render the widget component
    /// </summary>
    public class WidgetView : HtmlComponent
    {
        /// <summary>
        /// Gets/Sets the WidgetInstance object.
        /// </summary>
        public WidgetInstance Model { get; set; }

        /// <summary>
        /// Gets/Sets the current HttpContextBase object.
        /// </summary>
        public HttpContextBase Context { get; set; }

        /// <summary>
        /// Gets/Sets the html helper object.
        /// </summary>
        public HtmlHelper Html { get; internal set; }

        /// <summary>
        /// Gets the Response object from current context.
        /// </summary>
        public HttpResponseBase Reponse { get { return Context.Response; } }

        /// <summary>
        /// Gets the Request object from current context.
        /// </summary>
        public HttpRequestBase Request { get { return Context.Request; } }

        /// <summary>
        /// Initialize the WidgetView instance.
        /// </summary>
        public WidgetView()
        {
            UserPreferencesTemplate = new HtmlTemplate<WidgetHelper>();
            HeaderTemplate = new HtmlTemplate<WidgetHelper>();
            ContentTemplate = new HtmlTemplate<WidgetHelper>();
            DesignTemplate = new HtmlTemplate<WidgetHelper>();
            PreviewTemplate = new HtmlTemplate<WidgetHelper>();
        }

        /// <summary>
        /// Gets/Sets the user preferences settings inline template.
        /// </summary>
        public HtmlTemplate<WidgetHelper> UserPreferencesTemplate { get; set; }

        /// <summary>
        /// Gets/Sets the content inline template
        /// </summary>
        public HtmlTemplate<WidgetHelper> ContentTemplate { get; set; }

        /// <summary>
        /// Gets/Sets the design inline template
        /// </summary>
        public HtmlTemplate<WidgetHelper> DesignTemplate { get; set; }

        [Obsolete]
        public HtmlTemplate<WidgetHelper> HeaderTemplate { get; set; }

        [Obsolete]
        public HtmlTemplate<WidgetHelper> PreviewTemplate { get; set; }

        /// <summary>
        /// Gets the widget is in design mode.
        /// </summary>
        public bool IsDesignMode
        {
            get
            {
                return Request.IsAjaxRequest();
            }
        }

        /// <summary>
        /// Gets/Sets whether hide the User preferences in UI.
        /// </summary>
        public bool HideUserPreferences { get; set; }

        /// <summary>
        /// Gets/Sets whether the Widget can auto save the user preferences when value changes.
        /// </summary>
        public bool AutoSave { get; set; }

        /// <summary>
        /// Render the widget to current response output.
        /// </summary>
        /// <param name="writer"></param>
        public override void Render(HtmlTextWriter writer)
        {
            var locale = App.Get().Context.Locale;
            if (!string.IsNullOrEmpty(locale))
                App.Get().SetCulture(locale);

            bool isPreviewMode = false;
            Dictionary<string, PropertyDescriptor> propertyDescriptors = null;
            Dictionary<string, object> userdata = new Dictionary<string, object>();

            var bag = Html.ViewContext.Controller.ViewBag;
            if (bag != null)
            {
                if (bag.PropertyDescriptors != null)
                {
                    propertyDescriptors = bag.PropertyDescriptors as Dictionary<string, PropertyDescriptor>;
                    if (propertyDescriptors != null)
                    {
                        foreach (var key in propertyDescriptors.Keys)
                            userdata.Add(key, propertyDescriptors[key].Value);
                    }
                }

                if (bag.IsPreview != null)
                    isPreviewMode = bag.IsPreview;

                Model = bag.WidgetInstance;
            }

            var widgetHelper = new WidgetHelper()
            {
                Model = Model,
                UserPreferences = userdata,
                PropertyDescriptors = propertyDescriptors
            };

            if (isPreviewMode)
            {
                if (!PreviewTemplate.IsEmpty)
                    PreviewTemplate.WriteTo(widgetHelper, writer);
                else
                {
                    if (!ContentTemplate.IsEmpty)
                        ContentTemplate.WriteTo(widgetHelper, writer);
                }
            }
            else
            {
                if (IsDesignMode)
                {
                    if (userdata.Count > 0 && this.Model.WidgetDescriptor.WidgetType==1)
                    {
                        var urlhelper = new UrlHelper(Request.RequestContext);

                        writer.WriteBeginTag("form");
                        writer.WriteAttribute("class", "d-widget-prefs d-tran-fast d-form");
                        writer.WriteAttribute("data-ajax", "true");
                        writer.WriteAttribute("data-ajax-url", urlhelper.Content("~/api/" + AppModel.Get().Context.Website + "/widgets/apply"));
                        writer.WriteAttribute("data-ajax-method", "post");
                        //writer.WriteAttribute("data-ajax-begin", "$.loading()");
                        writer.WriteAttribute("data-ajax-success", "$('#widget_" + this.Model.ID.ToString() + "').widget('refresh');" + (AutoSave ? "" : "$.closePanels();"));
                        if (this.HideUserPreferences)
                            writer.WriteAttribute("data-hidden", "true");

                        if (this.AutoSave)
                        {
                            writer.WriteAttribute("data-auto-save", "true");
                            writer.WriteAttribute("onchange", "$(this).submit();");
                        }

                        //    writer.WriteAttribute("data-allow-pop", "false");

                        writer.Write(HtmlTextWriter.TagRightChar);

                        writer.WriteBeginTag("input");
                        writer.WriteAttribute("type", "hidden");
                        writer.WriteAttribute("name", "id");
                        writer.WriteAttribute("value", Model.ID.ToString());
                        writer.Write(HtmlTextWriter.SelfClosingTagEnd);

                        if (!UserPreferencesTemplate.IsEmpty)
                        {
                            UserPreferencesTemplate.WriteTo(widgetHelper, writer);
                        }
                        else
                        {
                            foreach (var key in propertyDescriptors.Keys)
                            {
                                if (!propertyDescriptors[key].IsReadonly)
                                    RenderPropertyControl(writer, widgetHelper, propertyDescriptors[key]);
                            }
                        }

                        writer.WriteEndTag("form");

                    }

                    if (!DesignTemplate.IsEmpty)
                        DesignTemplate.WriteTo(widgetHelper, writer);
                    else
                        if (!ContentTemplate.IsEmpty)
                            ContentTemplate.WriteTo(widgetHelper, writer);
                    writer.Write("<script type=\"text/javascript\">$(function(){ $('#widget_" + this.Model.ID.ToString() + "').taoUI(); });</script>");
                    //writer.Write("<script type=\"text/javascript\">$(function(){ $('#widget_" + this.Model.ID.ToString() + "').unobtrusive_ajax().taoUI(); });</script>");
                }
                else
                {
                    if (Model.Cached)
                    {
                        //Render cachable widget eg:Html widget
                        var cachedHtml = "";
                        var cachedKey = "widget" + Model.ID.ToString() + "_caching_html";
                        if (Context.Cache[cachedKey] != null)
                        {
                            cachedHtml = Context.Cache[cachedKey].ToString();
                        }
                        else
                        {
                            var cachedBuilder = new StringBuilder();
                            using (var cachedTextWriter = new System.IO.StringWriter(cachedBuilder))
                            {
                                using (var cachedWriter = new Html32TextWriter(cachedTextWriter))
                                {
                                    if (!ContentTemplate.IsEmpty)
                                        ContentTemplate.WriteTo(widgetHelper, cachedWriter);
                                    if (!HeaderTemplate.IsEmpty && Model.ShowHeader)
                                    {
                                        cachedWriter.Write("<div id=\"widget_" + this.Model.ID.ToString() + "_header_holder\" style=\"display:none;\">");
                                        HeaderTemplate.WriteTo(widgetHelper, cachedWriter);
                                        cachedWriter.Write("</div>");
                                        cachedWriter.Write("<script type=\"text/javascript\">$(function(){ if ($('#widget_" + this.Model.ID.ToString() + "').find('.d-widget-header').length) { $('#widget_" + this.Model.ID.ToString() + "').find('.d-widget-header').empty().append($('#widget_" + this.Model.ID.ToString() + "_header_holder').children());$('#widget_" + this.Model.ID.ToString() + "_header_holder').remove(); }  });</script>");
                                    }
                                }
                            }
                            cachedHtml = cachedBuilder.ToString();
                            Context.Cache.Add(cachedKey, cachedHtml, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                        }
                        writer.Write(cachedHtml);
                    }
                    else
                    {
                        if (!ContentTemplate.IsEmpty)
                            ContentTemplate.WriteTo(widgetHelper, writer);

                        if (!HeaderTemplate.IsEmpty && Model.ShowHeader)
                        {
                            writer.Write("<div id=\"widget_" + this.Model.ID.ToString() + "_header_holder\" style=\"display:none;\">");
                            HeaderTemplate.WriteTo(widgetHelper, writer);
                            writer.Write("</div>");
                            writer.Write("<script type=\"text/javascript\">$(function(){ if ($('#widget_" + this.Model.ID.ToString() + "').find('.d-widget-header').length) { $('#widget_" + this.Model.ID.ToString() + "').find('.d-widget-header').empty().append($('#widget_" + this.Model.ID.ToString() + "_header_holder').children());$('#widget_" + this.Model.ID.ToString() + "_header_holder').remove(); }  });</script>");
                        }
                    }
                }
            }
            //writer.Write(System.Web.Optimization.Scripts.Render("~/bundles/jqueryval"));
            //writer.Write(html.StartupScripts().ToString());

        }

        private void RenderPropertyControl(HtmlTextWriter writer, WidgetHelper helper, PropertyDescriptor descriptor)
        {
            //var ajax = new AjaxHelper(ViewContext, ViewDataContainer);
            string fieldID = helper.GenerateFieldID(descriptor.Name);
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "d-field");
            writer.Write(Html32TextWriter.TagRightChar);

            if (descriptor.PropertyControl != ControlTypes.Checkbox && descriptor.PropertyControl != ControlTypes.Radiobox)
            {
                writer.WriteBeginTag("label");
                writer.WriteAttribute("for", fieldID);
                writer.Write(Html32TextWriter.TagRightChar);
                writer.WriteEncodedText(string.IsNullOrEmpty(descriptor.DisplayName) ? descriptor.Name : descriptor.DisplayName);
                writer.WriteEndTag("label");
            }

            switch (descriptor.PropertyControl)
            {
                case ControlTypes.TextArea:
                    writer.WriteBeginTag("textarea");
                    writer.WriteAttribute("id", fieldID);
                    writer.WriteAttribute("name", descriptor.Name);
                    writer.Write(Html32TextWriter.TagRightChar);
                    writer.WriteEncodedText(descriptor.Value.ToString());
                    writer.WriteEndTag("textarea");
                    break;
                case ControlTypes.Checkbox:
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("type", "checkbox");
                    writer.WriteAttribute("name", descriptor.Name);
                    writer.WriteAttribute("id", fieldID);
                    writer.WriteAttribute("data-label", string.IsNullOrEmpty(descriptor.DisplayName) ? descriptor.Name : descriptor.DisplayName);
                    writer.WriteAttribute("value", descriptor.Value.ToString());
                    writer.Write(Html32TextWriter.SelfClosingTagEnd);
                    break;
                case ControlTypes.DateTimePicker:
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("type", "datetime");
                    writer.WriteAttribute("name", descriptor.Name);
                    writer.WriteAttribute("id", fieldID);
                    writer.WriteAttribute("value", descriptor.Value.ToString());
                    writer.Write(Html32TextWriter.SelfClosingTagEnd);
                    break;
                case ControlTypes.Number:
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("type", "number");
                    writer.WriteAttribute("name", descriptor.Name);
                    writer.WriteAttribute("id", fieldID);
                    writer.WriteAttribute("value", descriptor.Value.ToString());
                    writer.Write(Html32TextWriter.SelfClosingTagEnd);
                    break;
                case ControlTypes.Slider:
                    //writer.Write(ajax.Dna().Slider(fieldID)
                    //.Value((int)descriptor.Value)
                    //.GetHtml());
                    break;
                case ControlTypes.Radiobox:
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("type", "radio");
                    writer.WriteAttribute("name", descriptor.Name);
                    writer.WriteAttribute("data-label", string.IsNullOrEmpty(descriptor.DisplayName) ? descriptor.Name : descriptor.DisplayName);
                    writer.WriteAttribute("id", fieldID);
                    writer.WriteAttribute("value", descriptor.Value.ToString());
                    writer.Write(Html32TextWriter.SelfClosingTagEnd);
                    break;
                case ControlTypes.Richtext:
                    //writer.Write(ajax.Dna().RichTextBox(fieldID)
                    // .Value(descriptor.Value.ToString())
                    //.Resizable()
                    //.GetHtml());
                    break;
                default:
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("type", "text");
                    writer.WriteAttribute("name", descriptor.Name);
                    writer.WriteAttribute("id", fieldID);
                    writer.WriteAttribute("value", descriptor.Value.ToString());
                    writer.Write(Html32TextWriter.SelfClosingTagEnd);
                    //writer.Write(ajax.Dna().TextBox(fieldID)
                    // .Value(descriptor.Value == null ? "" : descriptor.Value.ToString())
                    //.GetHtml());
                    break;
            }
            writer.WriteEndTag("div");
        }
    }
}