//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DNA.Xml.Solutions;
using DNA.Utility;
using DNA.Xml;

namespace DNA.Web.Data.Entity
{
    internal static class InternalXmlConvertionExtensions
    {
        internal static WebElement ToXmlElement(this Web web)
        {
            throw new NotImplementedException();
            //var template = new WebElement()
            //{
            //    Title = new LocalizableElement() { Text = "{website-title}" },
            //    Theme = "{website-theme}",
            //    Description = new LocalizableElement() { Text = "{website-desc}" },
            //    DefaultUrl = web.DefaultUrl,
            //    DefaultLanguage = web.DefaultLocale,
            //    //Copyright = web.Copyright,
            //    CssText = web.CssText,
            //    LogoImageUrl = new RefElement() { Ref = web.LogoImageUrl, Language =web.DefaultLocale },
            //    ShortcutIconUrl = web.ShortcutIconUrl,
            //    //SearchKeywords = web.SearchKeywords
            //};

            //if ((!string.IsNullOrEmpty(web.DefaultUrl)) && (web.DefaultUrl.IndexOf("/" + web.Name.ToLower() + "/") > -1))
            //    template.DefaultUrl = web.DefaultUrl.ToLower().Replace("/" + web.Name.ToLower() + "/", "/{website}/");

            //var pages = web.Pages;

            //if (pages.Count() > 0)
            //{
            //    template.PageRefs = new List<PageElement>();
            //    var topLvPages = pages.Where(p => p.ParentID == 0).ToList();

            //    foreach (var page in topLvPages)
            //    {
            //        if ((page.IsStatic) && (!page.IsShared))
            //            continue;
            //        template.PageRefs.Add(page.ToXmlElement());
            //    }
            //}
            //return template;
        }

        internal static WidgetDataElement ToXmlElement(this WidgetInstance widget)
        {
            var descriptor = widget.WidgetDescriptor;
            var widgetData = new WidgetDataElement()
            {
                Title = new LocalizableElement()
                {
                    Text = widget.Title
                },
                Link = new LinkElement()
                {
                    Source = widget.Link
                },
                IconUrl = widget.IconUrl,
                Style = new WidgetStyleElement()
                {
                    Header = new HeaderStyleElement()
                    {
                        Hidden = !widget.ShowHeader,
                        Text = widget.HeaderCssText,
                        CssClass = widget.HeaderClass,
                    },
                    Body = new StyleElement()
                    {
                        CssClass = widget.BodyClass,
                        Text = widget.BodyCssText
                    },
                    Box = new StyleElement()
                    {
                        Text = widget.CssText
                    }
                },
                //ActionName = descriptor.Action,
                Zone = widget.ZoneID,
                Sequence = widget.Pos,
                //ControllerName = descriptor.Controller,
                WidgetID = descriptor.InstalledPath
            };
            var _preferences = widget.ReadUserPreferences();
            widgetData.Preferences = new List<PreferenceElement>();
            foreach (var _pref in _preferences)
            {
                widgetData.Preferences.Add(new PreferenceElement()
                {
                    Name = (string)_pref["name"],
                    Value = (string)_pref["value"]
                });
            }
            return widgetData;
        }

        internal static PageElement ToXmlElement(this WebPage page, bool includeChilden = true, bool includeWidgets = true)
        {
            var web = page.Web;
            var _pg = new PageElement()
            {
                Title = new LocalizableElement() { Text = page.Title, },
                Description = new LocalizableElement() { Text = page.Description },
                Keywords = new LocalizableElement() { Text = page.Keywords },
                Image = new ImageElement() { Source = page.ImageUrl },
                Icon = new ImageElement() { Source = page.IconUrl },
                LinkUrl = page.LinkTo,
                Target = page.Target,
                IsStatic = page.IsStatic,
                IsShared = page.IsShared,
                AllowAnonymous = page.AllowAnonymous,
                ShowInMenu = page.ShowInMenu,
                Slug = page.Slug
            };
            if (!string.IsNullOrEmpty(page.ViewData) || !string.IsNullOrEmpty(page.ViewName))
                _pg.Layout = new LayoutElement()
                {
                    Name = page.ViewName,
                    Text = page.ViewData
                };
            //_pg.Path = _pg.Path.ToLower().Replace("/" + web.Name.ToLower() + "/", "/{website}/");

            if (includeWidgets)
            {
                if (page.Widgets.Count > 0)
                {
                    _pg.Widgets = new List<WidgetDataElement>();
                    foreach (var widget in page.Widgets)
                    {
                        if (!widget.IsStatic && widget.TrackState == (int)TrackStates.NoChange)
                            _pg.Widgets.Add(widget.ToXmlElement());
                    }
                }
            }

            if (includeChilden)
            {
                var children = web.Pages.Where(p => p.ParentID == page.ID);
                if (children.Count() > 0)
                {
                    _pg.Children = new List<PageElement>();
                    foreach (var pc in children)
                        _pg.Children.Add(pc.ToXmlElement(true));
                }
            }

            return _pg;
        }

        internal static string ToXml(this Web web)
        {
            return XmlSerializerUtility.SerializeToXml(web.ToXmlElement());
        }

        internal static string ToXml(this WidgetInstance widget)
        {
            return XmlSerializerUtility.SerializeToXml(widget.ToXmlElement());
        }

        internal static string ToXml(this WebPage page, bool includeChilden = true, bool includeWidgets = true)
        {
            return XmlSerializerUtility.SerializeToXml(page.ToXmlElement(includeChilden, includeWidgets));
        }

        //internal static void FromXml(this Web web, string xml)
        //{
        //    var template = (WebElement)XmlSerializerUtility.DeserializeFromXmlText<WebElement>(xml);
        //    web.Popuplate(template);
        //}

        //public static void FromXml(this WebPage page,string xml,out IEnumerable<IWebPage> children,out IEnumerable<IWidget> widgets)
        //{
        //    var template = (PageElement)XmlSerializerUtility.DeserializeFromXmlText<PageElement>(xml);
        //    page.Populate
        //}

        //public static void FromXml(this WebPage page, string xml,out PageElement children,out WidgetDataElement widgets )
        //{
        //    var template = (PageElement)XmlSerializerUtility.DeserializeFromXmlText<PageElement>(xml);
        //    page.Title = template.Title;
        //    page.Description = template.Description;
        //    page.Keywords = template.Keywords;
        //    page.ImageUrl = template.ImageUrl;
        //    page.IconUrl = template.IconUrl;
        //    page.LinkTo = template.LinkUrl;
        //    page.Target = template.Target;
        //    page.ViewData = template.Data;
        //    page.ViewName = template.Layout;
        //    page.AllowAnonymous = template.AllowAnonymous;
        //    page.ShowInMenu = template.ShowInMenu;
        //    page.IsShared = template.IsShared;
        //    page.IsStatic = template.IsStatic;
        //    page.Path = template.Path;
        //    children=page.Populate 
        //    //var widgets=page.Widgets;
        //}

        //public static string FromXml(this WidgetInstance widget, string xml)
        //{
        //    var widgetData = (WidgetDataElement)XmlSerializerUtility.DeserializeFromXmlText<WidgetDataElement>(xml);
        //    if (widgetData.Title != null)
        //        widget.Title = widgetData.Title.Text;

        //    widget.Link = widgetData.Link != null ? widgetData.Link.Title : "";
        //    widget.IconUrl = widgetData.IconUrl;

        //    if (widgetData.Style != null)
        //    {
        //        if (widgetData.Style.Header != null)
        //        {
        //            widget.ShowHeader = !widgetData.Style.Header.Hidden;
        //            widget.HeaderCssText = widgetData.Style.Header.Text;
        //            widget.HeaderClass = widgetData.Style.Header.CssClass;
        //        }

        //        if (widgetData.Style.Box != null)
        //            widget.CssText = widgetData.Style.Box.Text;

        //        if (widgetData.Style.Body != null)
        //        {
        //            widget.BodyCssText = widgetData.Style.Body.Text;
        //            widget.BodyClass = widgetData.Style.Body.CssClass;
        //        }
        //    }

        //    widget.ZoneID = widgetData.View;
        //    widget.ViewMode = widgetData.ViewMode;

        //    if (widgetData.Preferences != null)
        //    {
        //        var dataDict = new List<dynamic>();
        //        foreach (var pref in widgetData.Preferences)
        //        {
        //            ///TODO: The type  maybe wrong
        //            ///TODO: The readonly property not found.
        //            dataDict.Add(new
        //            {
        //                name = pref.Name,
        //                @readonly = false,
        //                value = pref.Value
        //            });
        //        }

        //        widget.Data = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(dataDict);
        //    }

        //    return widgetData.WidgetID;
        //}
    }
}
