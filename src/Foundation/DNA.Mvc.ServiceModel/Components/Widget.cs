//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using System.Xml.Linq;

namespace DNA.Web.UI
{
    /// <summary>
    /// Represents an widget html component class that is used to render a widget to html.
    /// </summary>
    public class Widget : HtmlComponent
    {
        private UrlHelper urlHelper;
        private HtmlHelper helper;

        internal HtmlHelper Html
        {
            get { return helper; }
            set { helper = value; }
        }

        private UrlHelper Url
        {
            get
            {
                if (urlHelper == null)
                    urlHelper = UrlUtility.CreateUrlHelper();
                return urlHelper;
            }
        }

        /// <summary>
        /// Gets/Sets the widget instance 
        /// </summary>
        public WidgetInstance Model { get; set; }

        /// <summary>
        /// Gets/Sets whether is in design mode
        /// </summary>
        public bool IsDesignModel
        {
            get
            {
                return true;
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            var descroptor = Model.WidgetDescriptor;

            this.CssClass = "d-ui-widget d-widget";
            if (!Model.ShowBorder)
                this.CssClass += " noborder";

            if (Model.Transparent)
                this.CssClass += " d-transparent";

            if (!string.IsNullOrEmpty(Model.ViewMode))
                this.CssClass += " d-widget-viewmode-" + Model.ViewMode;

            this.CssClass += " d-widget-" + TextUtility.Slug(Model.WidgetDescriptor.Name.ToLower());
            if (descroptor.Height > 0)
            {
                this.Model.CssText += "min-height:" + descroptor.Height.ToString() + "px;";
            }

            if ((Model != null) && (!string.IsNullOrEmpty(Model.CssText)))
                this.HtmlAttributes.Add("style", Model.CssText);

            this.DataAttributes.Add("role", "widget");
            var widget = Model;

            this.DataAttributes.Add("id", widget.ID.ToString());
            //this.DataAttributes.Add("type", descroptor.WidgetType.ToString());
            //this.DataAttributes.Add("expanded", widget.IsExpanded.ToString().ToLower());
            //this.DataAttributes.Add("pos", widget.Pos.ToString());
            //this.DataAttributes.Add("show-header", widget.ShowHeader.ToString().ToLower());
            //this.DataAttributes.Add("show-border", widget.ShowBorder.ToString().ToLower());
            this.DataAttributes.Add("title", GE.GetContent(widget.Title));
            //this.DataAttributes.Add("link", !string.IsNullOrEmpty(widget.Link) ? Url.Content(widget.Link) : "");
            //this.DataAttributes.Add("icon", !string.IsNullOrEmpty(widget.IconUrl) ? Url.Content(widget.IconUrl) : "");
            //this.DataAttributes.Add("itemscope", "itemscope");
            //this.DataAttributes.Add("itemtype", "http://schema.org/WebPageElement");
            var website = "home";
            var routeDate = helper.ViewContext.RouteData;

            if (routeDate != null && routeDate.Values["website"] != null)
                website = (string)routeDate.Values["website"];

            this.DataAttributes.Add("content-url", (!string.IsNullOrEmpty(descroptor.Controller) && !string.IsNullOrEmpty(descroptor.Action)) ? (descroptor.ContentUrl = !string.IsNullOrEmpty(website) ? Url.Action(descroptor.Action, descroptor.Controller, new { Area = string.IsNullOrEmpty(descroptor.Area) ? "" : descroptor.Area, website = website, id = widget.ID }) : Url.Action(descroptor.Action, descroptor.Controller, new { Area = string.IsNullOrEmpty(descroptor.Area) ? "" : descroptor.Area })) : (!string.IsNullOrEmpty(website) ? Url.Action("Generic", "Widget", new { Area = "", website = website, id = widget.ID }) : Url.Action("Generic", "Widget", new { Area = "", id = widget.ID })));
            this.DataAttributes.Add("zone", widget.ZoneID);

            base.RenderBeginTag(writer);
        }

        public override void RenderContent(HtmlTextWriter writer)
        {
            var descroptor = Model.WidgetDescriptor;
            if (Model.ShowHeader)
            {
                writer.WriteBeginTag("div");

                if (!string.IsNullOrEmpty(Model.HeaderClass))
                    writer.WriteAttribute("class", "d-ui-widget-header d-h3 d-widget-header" + Model.HeaderClass);
                else
                    writer.WriteAttribute("class", "d-ui-widget-header d-h3 d-widget-header");

                if (!string.IsNullOrEmpty(Model.HeaderCssText))
                    writer.WriteAttribute("style", Model.HeaderCssText);

                writer.Write(HtmlTextWriter.TagRightChar);

                writer.WriteBeginTag("a");
                writer.WriteAttribute("class", "d-link d-widget-title-link");

                if (!string.IsNullOrEmpty(Model.Link))
                {
                    //writer.WriteAttribute("itemprop", "url");
                    writer.WriteAttribute("href", Url.Content(Model.Link));
                }
                else
                    writer.WriteAttribute("href", "javascript:void(0);");

                writer.Write(HtmlTextWriter.TagRightChar);

                if (!string.IsNullOrEmpty(Model.IconUrl))
                {
                    if (Model.IconUrl.StartsWith("d-icon-"))
                    {
                        writer.WriteBeginTag("span");
                        writer.WriteAttribute("class", "d-widget-icon");
                        writer.WriteAttribute("data-icon", Model.IconUrl);
                        writer.WriteAttribute("title", GE.GetContent(Model.Title));
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.WriteEndTag("span");
                    }
                    else
                    {
                        writer.WriteBeginTag("img");
                        writer.WriteAttribute("class", "d-widget-icon");
                        writer.WriteAttribute("src", Url.Content(Model.IconUrl));
                        writer.WriteAttribute("alt", GE.GetContent(Model.Title));
                        writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                    }
                }

                writer.WriteBeginTag("span");
                writer.WriteAttribute("class", "d-widget-title-text");
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(GE.GetContent(Model.Title));
                writer.WriteEndTag("span");

                writer.WriteEndTag("a");
                writer.WriteEndTag("div");
            }

            writer.WriteBeginTag("div");

            if (!string.IsNullOrEmpty(Model.BodyClass))
                writer.WriteAttribute("class", "d-ui-widget-body d-content d-widget-body " + Model.BodyClass);
            else
                writer.WriteAttribute("class", "d-ui-widget-body d-content d-widget-body");

            if (!string.IsNullOrEmpty(Model.BodyCssText))
                writer.WriteAttribute("style", Model.BodyCssText);
            //else
            //{
            //    if (descroptor.Height > 0)
            //        writer.WriteAttribute("style", "height:" + descroptor.Height.ToString()+"px");
            //}
            writer.Write(HtmlTextWriter.TagRightChar);

            try
            {

                if (!string.IsNullOrEmpty(descroptor.Controller) && !string.IsNullOrEmpty(descroptor.Action))
                {
                    if (string.IsNullOrEmpty(descroptor.Area))
                        Html.RenderAction(descroptor.Action, descroptor.Controller, new { Area = "", id = Model.ID.ToString() });
                    else
                        Html.RenderAction(descroptor.Action, descroptor.Controller, new { Area = descroptor.Area, id = Model.ID.ToString() });
                }
                else
                {
                    Html.RenderAction("Generic", "Widget", new { Area = "", id = Model.ID.ToString() });
                }
            }
            catch (Exception e)
            {
                var errList = new StringBuilder();
                var internalErr = e;
                errList.AppendLine(descroptor.Name + " - ");
                var i = 1;
                while (internalErr != null)
                {
                    errList.AppendLine(i + "." + internalErr.Message);
                    internalErr = internalErr.InnerException;
                    i++;
                }
                var defaultEmail = string.IsNullOrEmpty(descroptor.AuthorEmail) ? "csharp2002@hotmail.com" : descroptor.AuthorEmail;
                var errEle = new XElement("p", new XAttribute("class", "warn"),
                    new XElement("strong", descroptor.Name + " runtime error."),
                    new XElement("span", "Please "));
                var contactEle = new XElement("a", "Send report");
                contactEle.Add(new XAttribute("href", "mailto:" + defaultEmail + "?subject=Widget error report&body=" + errList.ToString()));
                errEle.Add(contactEle);
                errEle.Add(new XElement("span", " to author in order to fixed this bug."));
                //errEle.Add(new XElement("span", new XAttribute("class", "d-icon-plus-2"),
                    //new XAttribute("style","font-size:1.2em;float:right;"),new XAttribute("onclick","$(this).next().toggle();")));
                //errEle.Add(new XElement("span", errList.ToString(), new XAttribute("style", "display:none;float:none;")));
                
                ///TODO: Show the detail error in debug mode.
                writer.Write(errEle.OuterXml());

            }

            writer.WriteEndTag("div");

        }
    }
}