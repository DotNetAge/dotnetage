//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using DNA.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// The helper class provides methods for create menus
    /// </summary>
    public class Sitemaps : HelperPage
    {
        #region private methods

        private static void RenderNode(HtmlTextWriter writer, WebPage p, IEnumerable<WebPage> pages, bool withAttrs = true, bool showImg = true, WebPage curPage = null, int dataType = 1)
        {
            //var urlHelper = new UrlHelper(Context.Request);
            var path = string.Format("~/{0}/{1}/{2}.html", p.Web.Name, p.Locale, p.Slug);
            writer.WriteBeginTag("li");

            if (!object.ReferenceEquals(curPage, null) && !object.ReferenceEquals(p.Path, null) && !object.ReferenceEquals(curPage.Path, null))
            {
                var parentPath = p.Path + "/";
                if (curPage.Path.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) || p.ID == curPage.ID)
                    writer.WriteAttribute("class", "d-state-active");
            }


            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag("a");
            writer.WriteAttribute("data-id", p.ID.ToString());

            if (withAttrs)
            {
                writer.WriteAttribute("data-parentId", p.ParentID.ToString());
                writer.WriteAttribute("data-in-menu", p.ShowInMenu.ToString().ToLower());
                writer.WriteAttribute("data-in-sitemap", p.ShowInSitemap.ToString().ToLower());
                writer.WriteAttribute("data-static", p.IsStatic.ToString().ToLower());
                writer.WriteAttribute("data-shared", p.IsShared.ToString().ToLower());
                writer.WriteAttribute("data-title", p.Title);
                writer.WriteAttribute("data-desc", p.Description);
                //writer.WriteAttribute("data-slug", p.Slug.ToLower());
                writer.WriteAttribute("data-path", Href(path));
                writer.WriteAttribute("data-anonymous", p.AllowAnonymous.ToString().ToLower());
            }
            else
            {
                if (p.NoFollow)
                    writer.WriteAttribute("rel", "nofollow");
                else
                {
                    if (!string.IsNullOrEmpty(Request.Browser.Browser) && Request.Browser.Browser.Equals("chrome",StringComparison.OrdinalIgnoreCase))
                        writer.WriteAttribute("rel", "preload");
                    else
                        writer.WriteAttribute("rel", "prefetch");
                }

                if (!string.IsNullOrEmpty(p.LinkTo))
                    writer.WriteAttribute("href", p.LinkTo);
                else
                    writer.WriteAttribute("href", Href(path));
            }
            writer.Write(HtmlTextWriter.TagRightChar);

            if (showImg)
            {
                if (!string.IsNullOrEmpty(p.IconUrl))
                {
                    writer.WriteBeginTag("img");
                    writer.WriteAttribute("src", Href(p.IconUrl));
                    writer.WriteAttribute("alt", p.Title);
                    writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                }
            }

            writer.WriteEncodedText(GE.GetContent(p.Title));
            writer.WriteEndTag("a");
            if (dataType == 1)
            {
                var children = pages.Where(page => page.ParentID == p.ID).OrderBy(page => page.Pos).ToList();
                if (children.Count > 0)
                {
                    writer.WriteFullBeginTag("ul");
                    foreach (var child in children)
                    {
                        RenderNode(writer, child, pages, withAttrs, showImg);
                    }
                    writer.WriteEndTag("ul");
                }
            }
            writer.WriteEndTag("li");
        }

        private static Dictionary<int, string[]> roleTable = null;

        private static void PreloadRoleTable()
        {
            //int[] pIDs = AppModel.Get().CurrentWeb.Pages.Where(p => !p.AllowAnonymous).Select(p => p.ID).ToArray();
            //var roles = AppModel.Get().DataContext.WebPages.GetRoles(pIDs);
            var pages = AppModel.Get().CurrentWeb.Pages.Where(p => !p.AllowAnonymous);
            roleTable = new Dictionary<int, string[]>();
            foreach (var p in pages)
            {
                var roles = p.Roles;
                if (roles != null && roles.Length > 0)
                    roleTable.Add(p.ID, p.Roles);
            }

            //var roleGroups = roles.GroupBy(r => r.PageID);
            //foreach (var roleGroup in roleGroups)
            //{
            //    roleTable.Add(roleGroup.Key, roleGroup.Select(r => r.RoleName).ToArray());
            //}
        }

        private static bool IsAccessibleToUser(WebPage page)
        {
            //if (node.Equals(RootNode)) return true;
            //var page = GetPageFromKey(GetIDFromKey(node.Key));

            if (page.AllowAnonymous)
                return true;

            if (Request.IsAuthenticated)
            {
                if (roleTable == null)
                    PreloadRoleTable();
                if (roleTable.ContainsKey(page.ID))
                {
                    var roles = roleTable[page.ID];
                    foreach (var role in roles)
                    {
                        if (AppModel.Get().User.IsInRole(role))
                            return true;
                    }
                }
                //page.Roles.Contains(AppModel.Get().User.ro
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Render HTML ui element for web page structure.
        /// </summary>
        /// <param name="web">The parent web instance.</param>
        /// <param name="withAttributes"></param>
        /// <param name="showimg">Whether show image on the left of the node item.</param>
        /// <param name="ismenu">Show as menu</param>
        /// <param name="issitemap">Show as sitemap</param>
        /// <param name="htmlAttributes">The html attributes for output element.</param>
        /// <param name="dataType">The menu item filter:1:all pages. 2:root pages (the top level pages.) 3:show children pages</param>
        /// <returns></returns>
        public static HelperResult GetHtml(Web web = null, bool withAttributes = false, bool showimg = false, bool ismenu = false, bool issitemap = false, object htmlAttributes = null, int dataType = 1)
        {
            var _web = new WebDecorator(web == null ? AppModel.Get().Context.Web : web, AppModel.Get().DataContext);
            var locale = CurrentPage.Culture;// _web.DefaultLocale;
            var currentPage = AppModel.Get().CurrentPage;
            var currentID = currentPage != null ? currentPage.ID : 0;

            return new HelperResult(w =>
            {
                using (var writer = new HtmlTextWriter(w))
                {
                    var pages = _web.CachingPages.Where(p => p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase));
                    //IEnumerable<WebPage> pages = null;

                    if (ismenu)
                        pages = pages.Where(p => p.ShowInMenu);

                    if (issitemap)
                        pages = pages.Where(p => p.ShowInSitemap);

                    pages = pages.OrderBy(p => p.Pos).ToList();

                    var topPages = dataType == 3 ? pages.Where(p => p.ParentID == currentPage.ID) : pages.Where(p => p.ParentID == 0);

                    if (dataType == 4 && currentPage != null)
                    {
                        if (currentPage.ParentID == 0)
                            topPages = pages.Where(p => p.ParentID == currentPage.ID);
                        else
                            topPages = pages.Where(p => !string.IsNullOrEmpty(p.Path) && p.Path.Equals(currentPage.Path, StringComparison.OrdinalIgnoreCase));
                    }

                    if (topPages.Count() == 0)
                        return;

                    writer.WriteBeginTag("ul");
                    if (htmlAttributes != null)
                    {
                        var attrs = ObjectHelper.ConvertObjectToDictionary(htmlAttributes);
                        foreach (var attr in attrs)
                        {
                            var key = attr.Key;
                            if (key.StartsWith("data_"))
                                key = key.Replace("_", "-");

                            writer.WriteAttribute(key, attr.Value != null ? attr.Value.ToString() : "");
                        }
                    }
                    writer.Write(HtmlTextWriter.TagRightChar);

                    if (dataType == 3)
                    {
                        if (currentPage == null)
                        {
                            writer.WriteEndTag("ul");
                            return;
                        }
                        //pages = _web.Pages.Where(p => p.ParentID == currentPage.ID);
                    }

                    foreach (var p in topPages)
                    {
                        if (ismenu || issitemap)
                        {
                            if (!IsAccessibleToUser(p))
                                continue;
                        }

                        RenderNode(writer, p, pages, withAttributes, showimg, currentPage, dataType);
                    }

                    writer.WriteEndTag("ul");
                }
            });
        }

        /// <summary>
        /// Render the main menu element for current web
        /// </summary>
        /// <returns></returns>
        public static HelperResult Menu()
        {
            return GetHtml(htmlAttributes: new { data_role = "menubar" }, ismenu: true, showimg: true, withAttributes: false);
        }

        /// <summary>
        /// Render inline path navigation elements for current page.
        /// </summary>
        /// <param name="htmlAttributes">The html attributes for output element.</param>
        /// <param name="showCurrent">Show current states</param>
        /// <param name="showRoot">Show the root node of the path.</param>
        /// <returns></returns>
        public static HelperResult RenderPagePath(object htmlAttributes = null, bool showCurrent = true, bool showRoot = true)
        {
            return new HelperResult(writer =>
            {
                var curPage = AppModel.Get().CurrentPage;
                if (curPage != null)
                {
                    var parents = curPage.Parents().Where(p => !p.IsShared).OrderBy(p => p.Path).ToList().Select(n =>
                    {
                        var wrapper = AppModel.Get().Wrap(n);
                        var element = new XElement("li", new XElement("a", wrapper.Title, new XAttribute("href", wrapper.Url)));

                        if (!string.IsNullOrEmpty(wrapper.Description))
                            element.Add(new XAttribute("title", wrapper.Description));

                        return element;
                    });

                    var ul = new XElement("ul");

                    if (showRoot)
                    {
                        ul.Add(new XElement("li", new XElement("a", AppModel.Get().CurrentWeb.Title,
                           new XAttribute("href", UrlHelper.GenerateContentUrl(AppModel.Get().CurrentWeb.Url, Context))
                          )));
                    }


                    if (htmlAttributes != null)
                    {
                        var attrs = htmlAttributes.ToDictionary();
                        foreach (var attr in attrs.Keys)
                        {
                            ul.Add(new XAttribute(attr.Replace("_", "-"), attrs[attr]));
                        }
                    }

                    if (parents.Count() > 0)
                        ul.Add(parents);

                    if (showCurrent && !curPage.IsShared)
                    {
                        ul.Add(new XElement("li", new XElement("a", curPage.Title,
                            new XAttribute("href", "javascript:void(0);")
                           ), new XAttribute("class", "d-link d-state-active")));
                    }

                    var ctx = AppModel.Get().Context;
                    if (ctx.View != null)
                    {
                        ul.Add(new XElement("li", new XElement("a", ctx.View.Title,
                           new XAttribute("href", UrlHelper.GenerateContentUrl(ctx.View.Url, Context)))));
                    }

                    //Render context form , item and views
                    if (ctx.DataItem != null)
                    {
                        var list = ctx.DataItem.Parent;
                        if (list.IsHierarchy)
                        {
                            var parentItems = ctx.DataItem.Parents();
                            if (parentItems != null)
                            {
                                var itemParentEles = parentItems.ToList()
                                                                                          .Select(p =>
                                                                                          {
                                                                                              var itemWrapper = AppModel.Get().Wrap(p);
                                                                                              var title = itemWrapper.GetDefaultTitleValue();
                                                                                              var element = new XElement("li", new XElement("a", title,
                                                                                                  new XAttribute("href",
                                                                                                      UrlHelper.GenerateContentUrl(itemWrapper.Url, Context))));
                                                                                              return element;
                                                                                          });
                                if (itemParentEles.Count() > 0)
                                    ul.Add(itemParentEles);
                            }
                        }


                        ul.Add(new XElement("li",
                            new XElement("a", ctx.DataItem.GetDefaultTitleValue(),
                                new XAttribute("href", DNA.Utility.UrlUtility.CreateUrlHelper().Action("Catalog", "Contents", new { Area = "", name = list.Name, website = list.Web.Name, locale = list.Locale, id = ctx.DataItem.ID })),
                                new XAttribute("data-rel", "panel"),
                                new XAttribute("data-panel-fill", "true"),
                                new XAttribute("data-panel-display", "push"),
                                new XAttribute("data-panel-title", ctx.DataItem.Parent.Title)
                                ),
                                new XAttribute("class", "d-state-active")));
                    }


                    if (ul.HasElements)
                        writer.Write(ul.OuterXml());

                }
            });
        }

        /// <summary>
        /// Render the sitemenu element that on the top of the web page.
        /// </summary>
        /// <param name="htmlAttributes">The html attributes for output element.</param>
        /// <returns></returns>
        public static HelperResult SiteMenu(object htmlAttributes = null)
        {
            var _web = AppModel.Get().Webs["home"];
            var locale = CurrentPage.Culture;
            var curPage = AppModel.Get().CurrentPage;
            var currentID = curPage != null ? curPage.ID : 0;

            return new HelperResult(w =>
             {
                 using (var writer = new HtmlTextWriter(w))
                 {
                     var pages = _web.Pages.Where(p => p.ShowInMenu && p.ParentID == 0 && p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase)).OrderBy(p => p.Pos).ToList();
                     writer.WriteBeginTag("ul");
                     if (htmlAttributes != null)
                     {
                         var attrs = ObjectHelper.ConvertObjectToDictionary(htmlAttributes);
                         foreach (var attr in attrs)
                         {
                             var key = attr.Key;
                             if (key.StartsWith("data_"))
                                 key = key.Replace("_", "-");

                             writer.WriteAttribute(key, attr.Value != null ? attr.Value.ToString() : "");
                         }
                     }
                     writer.Write(HtmlTextWriter.TagRightChar);

                     foreach (var p in pages)
                     {
                         if (!IsAccessibleToUser(p)) continue;

                         #region render node
                         var path = string.Format("~/{0}/{1}/{2}.html", p.Web.Name, p.Locale, p.Slug);
                         writer.WriteBeginTag("li");

                         if (curPage != null)
                         {
                             var parentPath = p.Path + "/";
                             if (!string.IsNullOrEmpty(curPage.Path) && (curPage.Path.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) || p.ID == curPage.ID))
                                 writer.WriteAttribute("class", "d-state-active");
                         }

                         writer.Write(HtmlTextWriter.TagRightChar);
                         writer.WriteBeginTag("a");
                         writer.WriteAttribute("data-id", p.ID.ToString());

                         if (p.NoFollow)
                             writer.WriteAttribute("rel", "nofollow");
                         else
                         {
                             if (!string.IsNullOrEmpty(Request.Browser.Browser) && Request.Browser.Browser.Equals("chrome", StringComparison.OrdinalIgnoreCase))
                                 writer.WriteAttribute("rel", "preload");
                             else
                                 writer.WriteAttribute("rel", "prefetch");
                         }

                         if (!string.IsNullOrEmpty(p.LinkTo))
                             writer.WriteAttribute("href", p.LinkTo);
                         else
                             writer.WriteAttribute("href", Href(path));

                         writer.Write(HtmlTextWriter.TagRightChar);

                         writer.WriteEncodedText(GE.GetContent(p.Title));
                         writer.WriteEndTag("a");
                         writer.WriteEndTag("li");
                         #endregion
                     }

                     writer.WriteEndTag("ul");
                 }
             });
        }

    }

}