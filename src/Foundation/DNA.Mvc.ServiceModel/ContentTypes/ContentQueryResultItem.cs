//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a query result item. 
    /// </summary>
    public class ContentQueryResultItem
    {
        /// <summary>
        /// Gets the parent view object.
        /// </summary>
        public ContentViewDecorator View { get; private set; }

        /// <summary>
        /// Gets the parent list object.
        /// </summary>
        public ContentListDecorator List { get { return View.Parent; } }

        private ContentDataItemDecorator rawItem = null;

        /// <summary>
        /// Gets the raw data item of this result.
        /// </summary>
        public ContentDataItemDecorator RawItem
        {
            get
            {
                if (rawItem == null)
                {
                    rawItem = this.List.GetItem(this.ID);
                }
                return rawItem;
            }
        }

        /// <summary>
        /// Gets field value by specified name.
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <returns>A object value of the field.</returns>
        public object this[string fieldName]
        {
            get
            {
                if (string.IsNullOrEmpty(fieldName))
                    throw new ArgumentNullException("fieldName");

                if (Data != null)
                {
                    var dict = (IDictionary<string, object>)Data;
                    if (dict.ContainsKey(fieldName))
                        return dict[fieldName];
                }
                return null;

            }
        }

        /// <summary>
        /// Gets typed value by specified field name.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="fieldName">The field name.</param>
        /// <returns>A typed value of the field.</returns>
        public T Get<T>(string fieldName)
        {
            return (T)this[fieldName];
        }

        /// <summary>
        /// Gets the data value object that contains in data content item.
        /// </summary>
        public dynamic Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ContentQueryResultItem class.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="objectItem"></param>
        internal ContentQueryResultItem(ContentViewDecorator parent, dynamic objectItem)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (objectItem == null)
                throw new ArgumentNullException("objectItem");

            this.View = parent;
            this.Data = objectItem;
        }

        /// <summary>
        /// Gets content field object by specified name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>A content field object that defined in parent list.</returns>
        public ContentField Field(string name)
        {
            return this.List.Fields[name];
        }

        /// <summary>
        /// Get the label text by specified field name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>A string containts the field title text.</returns>
        public string Label(string name)
        {
            return Field(name).Title;
        }

        #region fields

        /// <summary>
        /// Gets data item id.
        /// </summary>
        public Guid ID { get { return (Guid)this[DataNames.ID]; } }

        /// <summary>
        /// Gets the parent data item id. If no parent found returns Guid.Empty
        /// </summary>
        public Guid ParentID { get { return (Guid)this[DataNames.ParentID]; } }

        /// <summary>
        /// Gets the data item path.
        /// </summary>
        public string Path { get { return (string)this[DataNames.Path]; } }

        /// <summary>
        /// Gets latest modifer user name.
        /// </summary>
        public string Modifier { get { return (string)this[DataNames.Modifier]; } }

        /// <summary>
        /// Gets data item owner name.
        /// </summary>
        public string Owner
        {
            get
            {
                if (this[DataNames.Owner] is DBNull)
                    return "";
                return (string)this[DataNames.Owner];
            }
        }

        /// <summary>
        /// Gets data item url name.
        /// </summary>
        public string Slug { get { return (string)this[DataNames.Slug]; } }

        /// <summary>
        /// Gets rating value.
        /// </summary>
        public double Ratings { get { return (double)this[DataNames.Ratings]; } }

        /// <summary>
        /// Gets total read count.
        /// </summary>
        public int Reads { get { return (int)this[DataNames.Reads]; } }

        /// <summary>
        /// Gets total share count.
        /// </summary>
        public int TotalShares { get { return (int)this[DataNames.TotalShares]; } }

        /// <summary>
        /// Gets total attachment count.
        /// </summary>
        public int TotalAttachs { get { return (int)this[DataNames.TotalAttachs]; } }

        /// <summary>
        /// Gets the date published.
        /// </summary>
        public DateTime Published
        {
            get
            {
                if (this[DataNames.Published] == DBNull.Value)
                    return DateTime.MinValue;

                return (DateTime)this[DataNames.Published];
            }
        }

        /// <summary>
        /// Indicates whether this data item is published.
        /// </summary>
        public bool IsPublished
        {
            get
            {
                return (bool)this[DataNames.IsPublished];
            }
        }

        /// <summary>
        /// Gets the moderate state.
        /// </summary>
        public ModerateStates ModerateState
        {
            get
            {
                return (ModerateStates)this[DataNames.State];
            }
        }

        /// <summary>
        /// Indicates whether this data item has children items.
        /// </summary>
        public bool HasChildren { get { return (bool)this[DataNames.HasChildren]; } }

        /// <summary>
        /// Gets item version number.
        /// </summary>
        public int Version { get { return (int)this[DataNames.Version]; } }

        /// <summary>
        /// Gets categories raw string.
        /// </summary>
        public string CategoriesRaw { get { return (string)this[DataNames.Categories]; } }

        /// <summary>
        /// Gets tags raw string.
        /// </summary>
        public string TagsRaw { get { return (string)this[DataNames.Tags]; } }

        /// <summary>
        /// Gets categories.
        /// </summary>
        public string[] Categories
        {
            get
            {
                var cats = this[DataNames.Categories];
                if (!(cats is DBNull) && cats != null && !string.IsNullOrEmpty((string)cats))
                    return cats.ToString().Split(',');
                return new string[0];
            }
        }

        /// <summary>
        /// Gets tags.
        /// </summary>
        public string[] Tags
        {
            get
            {
                var tags = this[DataNames.Tags];
                if (!(tags is DBNull) && tags != null && !string.IsNullOrEmpty((string)tags))
                    return tags.ToString().Split(',');
                return new string[0];
            }
        }

        /// <summary>
        /// Gets css class names for view item.
        /// </summary>
        public string CssClass
        {
            get
            {
                string itemCls = "d-view-item";
                if (this.List.IsModerated)
                {
                    if (ModerateState != ModerateStates.Approved && ModerateState != ModerateStates.Notset)
                        itemCls += " d-view-item-" + ModerateState.ToString().ToLower();
                }
                if (!this.IsPublished)
                    itemCls += " d-view-item-draft";
                return itemCls;
            }
        }

        #endregion

        #region added 3.0.3

        /// <summary>
        /// Gets the formatted string value by specified field name.
        /// </summary>
        public string GetFormattedValue(string fieldName)
        {
            return this.List.Fields[fieldName].Format(this[fieldName]);
        }

        public T GetModel<T>()
            where T : class
        {
            return List.GetItem(ID).GetModel<T>();
        }

        #endregion

        private CommentCollection comments = null;

        /// <summary>
        /// Gets the comment collection of the data item.
        /// </summary>
        public CommentCollection Comments
        {
            get
            {
                if (comments == null)
                    comments = new CommentCollection(List.Context, this.UrlComponent);
                return comments;
            }
        }

        /// <summary>
        /// Gets total comment count.
        /// </summary>
        public int TotalComments
        {
            get
            {
                return this.Comments.Count();
            }
        }

        /// <summary>
        /// Indicates whether the current user can edit this item.
        /// </summary>
        /// <param name="ctx">The http context.</param>
        /// <returns>ture if allow; otherwise , false.</returns>
        public bool AllowEdit(HttpContextBase ctx)
        {
            var editForm = this.List.EditForm;
            return editForm != null && editForm.IsAuthorized(ctx);
        }

        /// <summary>
        /// Gets the absoulte url to display data item.
        /// </summary>
        public string UrlComponent
        {
            get
            {
                var request = HttpContext.Current.Request;
                string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath;
                var d = this.Published;
                var year = d.ToString("yyyy");
                var month = d.ToString("MM");
                var days = d.ToString("dd");
                var vers = this.Version.ToString();
                var uri = new Uri(string.Format("{0}{1}/{2}/{3}/{4}/{5}/{6}/{7}.html", baseUrl, this.List.Web.Name, this.List.Locale, this.List.Name, year, month, days, this.Slug));
                return uri.GetComponents(UriComponents.Path | UriComponents.SchemeAndServer, UriFormat.Unescaped);
            }
        }

        /// <summary>
        /// Gets edit data item url.
        /// </summary>
        public string UrlForEdit
        {
            get
            {
                return this.List.GetEditItemUrl(this.Slug);
            }
        }

        /// <summary>
        ///  Gets the relative url to display data item.
        /// </summary>
        public string Url
        {
            get
            {
                var d = this.Published;
                var year = d.ToString("yyyy");
                var month = d.ToString("MM");
                var days = d.ToString("dd");
                var vers = this.Version.ToString();
                return string.Format("~/{0}/{1}/{2}/{3}/{4}/{5}/{6}.html", this.List.Web.Name, this.List.Locale, this.List.Name, year, month, days, this.Slug);

            }
        }

        /// <summary>
        /// Indicates whether the specified field value is null or an System.String.Empty string.
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <returns> true if the value is null or an empty string (""); otherwise, false.</returns>
        public bool IsNull(string fieldName)
        {
            var val = this[fieldName];
            return val == null || (val != null && string.IsNullOrEmpty(val.ToString()) || val is DBNull);
        }

        /// <summary>
        /// Get detail item query result by specified the detail list name.
        /// </summary>
        /// <param name="detailListName">The detail list name.</param>
        /// <param name="index">The one base page index value.</param>
        /// <param name="size">The pre page item size.</param>
        /// <returns>A content query result object that contains query result.</returns>
        public ContentQueryResult GetDetailQueryResult(string detailListName, int index = 0, int size = 0)
        {
            var query = new ContentQuery() { };
            var detailList = this.View.Parent.Web.Lists[detailListName];
            if (detailList == null)
                throw new ContentListNotFoundException(string.Format("{0} list not found.", detailListName));

            if (detailList.DefaultView == null)
                throw new ContentViewNotFoundException(string.Format("There is not any view define in {0} list", detailListName));

            var detailQuery = new ContentQuery()
            {
                Filter = string.Format("parentId='{0}'", this.ID.ToString())
            };

            if (index > 0)
                detailQuery.Index = index;

            if (size > 0)
                detailQuery.Size = size;

            return detailList.DefaultView.GetItems(detailQuery);
        }
    }
}
