//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.WebPages;
using System.Xml.Linq;

namespace DNA.Web.UI
{
    /// <summary>
    /// Provides helper methods for content type system
    /// </summary>
    public static class Contents
    {
        /// <summary>
        /// Render an items tree for specified list 
        /// </summary>
        /// <param name="list">The content list object</param>
        /// <param name="current">The content data item object.</param>
        /// <param name="htmlAttributes">The html attributes object.</param>
        /// <returns></returns>
        public static HelperResult Tree(ContentListDecorator list, ContentDataItemDecorator current = null, object htmlAttributes = null)
        {
            return new HelperResult((w) =>
            {
                using (var writer = new Html32TextWriter(w))
                {
                    writer.WriteBeginTag("ul");
                    writer.WriteAttribute("data-role", "tree");
                    if (htmlAttributes != null)
                    {
                        var dict = htmlAttributes.ToDictionary();
                        foreach (var key in dict.Keys)
                        {
                            if (key.StartsWith("data_"))
                                writer.WriteAttribute(key.Replace("_", "-"), (string)dict[key]);
                            else
                                writer.WriteAttribute(key, (string)dict[key]);
                        }
                    }

                    writer.Write(Html32TextWriter.TagRightChar);
                    var items = list.GetItems(i => i.ParentItemID == Guid.Empty && i.IsPublished && i.IsCurrentVersion).OrderBy(i => i.Pos);
                    RenderChildren(writer, list, items, current);
                    writer.WriteEndTag("ul");
                }
            });
        }

        /// <summary>
        /// Render an items tree for specified view 
        /// </summary>
        /// <param name="view">The data view object to get data items .</param>
        /// <param name="parentID">The root data item id.</param>
        /// <param name="currentPath">The current data item path.</param>
        /// <param name="htmlAttributes">The html attributes for treeview element.</param>
        /// <param name="dynamicLoad">Specified the tree node load on demand.</param>
        /// <returns></returns>
        public static HelperResult Tree(ContentViewDecorator view, string parentID = "", string currentPath = "", object htmlAttributes = null, bool dynamicLoad = true)
        {
             var items = view.Items(string.Format("parentId='{0}'", string.IsNullOrEmpty(parentID) ? Guid.Empty.ToString() : parentID));
             return Tree(items, currentPath, htmlAttributes, dynamicLoad);
        }

        /// <summary>
        ///  Render an items tree by specified item query result.
        /// </summary>
        /// <param name="items">The item query result.</param>
        /// <param name="currentPath">The current data item path.</param>
        /// <param name="htmlAttributes">The html attributes for treeview element.</param>
        /// <param name="dynamicLoad">Specified the tree node load on demand.</param>
        /// <returns></returns>
        public static HelperResult Tree(ContentQueryResult items, string currentPath = "", object htmlAttributes = null, bool dynamicLoad = true)
        {
            return new HelperResult((w) =>
            {
                var root = new XElement("ul", new XAttribute("data-role", "tree"));
                root.AddHtmlAttributes(htmlAttributes);
                AddChildren(root, items, items.View, currentPath);
                w.Write(root.OuterXml());
            });
        }

        /// <summary>
        /// Add children nodes to specified parent element.
        /// </summary>
        /// <param name="parent">The parent xml element.</param>
        /// <param name="results">The item query result.</param>
        /// <param name="view">The view object</param>
        /// <param name="currentPath">The current data item path.</param>
        /// <param name="dynamicLoad">Specified the tree node load on demand.</param>
        public static void AddChildren(XElement parent, ContentQueryResult results, ContentViewDecorator view, string currentPath, bool dynamicLoad = true)
        {
            var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
            var fieldName = view.Parent.GetDefaultTitleField().Name;
            foreach (var item in results)
            {
                var hasChildren = item.HasChildren;

                var element = new XElement("li");
                parent.Add(element);

                if (hasChildren)
                    element.Add(new XAttribute("class", "d-node d-node-hasChildren"));
                else
                    element.Add(new XAttribute("class", "d-node"));

                var isInPath = false;

                if (!string.IsNullOrEmpty(currentPath) && !string.IsNullOrEmpty(item.Path) && currentPath.StartsWith(item.Path))
                {
                    isInPath = true;
                    if (hasChildren)
                        element.Add(new XAttribute("data-expanded", "true"));

                    if (item.Path==currentPath)
                    element.Add(new XAttribute("data-selected", "true"));
                }

                element.Add(new XAttribute("data-id", item.ID.ToString()));

                if (item.ParentID != Guid.Empty)
                    element.Add(new XAttribute("data-parentid", item.ParentID.ToString()));
                var linkElement = new XElement("a", new XAttribute("href", item.UrlComponent), item[fieldName]);
                element.Add(linkElement);

                if (hasChildren)
                {
                    if (!isInPath && dynamicLoad)
                    {
                        var urlformat = "~/api/{0}/contents/items?name={1}&slug={2}&parentId={3}";
                        var popupUrl = Url.Content(string.Format(urlformat, view.Parent.Web.Name, view.Parent.Name, view.Name, item.ID.ToString()));
                        element.Add(new XAttribute("data-popupurl", popupUrl));
                    }
                    else
                    {
                        var childrenElement = new XElement("ul");
                        element.Add(childrenElement);
                        AddChildren(childrenElement, view.Items(string.Format("parentId='{0}'", item.ID.ToString())), view, currentPath);
                    }
                }
            }
        }

        private static void RenderChildren(Html32TextWriter writer, ContentListDecorator list, IEnumerable<ContentDataItem> items, ContentDataItem current)
        {
            var app = App.Get();
            var fieldName = list.GetDefaultTitleField().Name;
            var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
            foreach (var item in items)
            {
                var itemWrapper = app.Wrap(item);
                writer.WriteBeginTag("li");
                var _class = "d-node";
                var children = itemWrapper.Children();
                var childrenCount = children.Count();
                if (childrenCount > 0)
                    _class += " d-node-hasChildren";

                //writer.WriteAttribute("class", "d-node d-node-hasChildren");


                var isInPath = false;
                if (current != null && !string.IsNullOrEmpty(current.Path) && !string.IsNullOrEmpty(item.Path) && current.Path.StartsWith(item.Path))
                {
                    isInPath = true;
                    if (childrenCount > 0)
                        writer.WriteAttribute("data-expanded", "true");

                }
                writer.WriteAttribute("data-id", itemWrapper.ID.ToString());

                if (current != null && itemWrapper.ID.Equals(current.ID))
                    writer.WriteAttribute("data-selected", "true");

                if (itemWrapper.ParentItemID != Guid.Empty)
                    writer.WriteAttribute("data-parentid", itemWrapper.ParentItemID.ToString());

                if (childrenCount > 0)
                {
                    if (!isInPath)
                    {
                        var urlformat = "~/api/contents/items?name={0}&slug={1}&parentId={2}";
                        var popupUrl = Url.Content(string.Format(urlformat, list.Name, list.Views.Default.Name, itemWrapper.ID.ToString()));
                        writer.WriteAttribute("data-popupurl", popupUrl);
                    }
                }
                writer.WriteAttribute("class", _class);
                writer.Write(Html32TextWriter.TagRightChar);

                writer.WriteBeginTag("a");
                writer.WriteAttribute("href", itemWrapper.UrlComponent);
                writer.Write(Html32TextWriter.TagRightChar);
                writer.Write(itemWrapper.Value(fieldName).Raw);
                writer.WriteEndTag("a");

                if (childrenCount > 0)
                {
                    if (isInPath)
                    {
                        writer.WriteFullBeginTag("ul");
                        RenderChildren(writer, list, children, current);
                        writer.WriteEndTag("ul");
                    }
                }

                writer.WriteEndTag("li");
            }
        }
    }
}
