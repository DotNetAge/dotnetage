//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Text;
using DNA.Web.ServiceModel.Syndication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to ContentView object model. 
    /// </summary>
    /// <remarks>
    /// The list view defines how to filter,sort, paging, group and how to render data items.
    /// </remarks>
    /// <example>
    /// <para>This example shows how to get and render the default view for "Product" list.</para>
    /// <code language="aspx">
    /// @{
    /// var list=App.Get().CurrentWeb.Lists["products"];
    /// Html.RenderViewResults(list.DefaultView);
    /// }
    /// </code>
    /// </example>
    public class ContentViewDecorator : ContentView
    {
        private ContentFieldRefCollection viewFields = null;
        private string[] roles;

        internal ContentView Model { get; set; }

        internal IDataContext Context { get; set; }

        /// <summary>
        /// Gets the parent list object.
        /// </summary>
        public new ContentListDecorator Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a view field collection.
        /// </summary>
        public ContentFieldRefCollection FieldRefs
        {
            get
            {
                if (viewFields == null)
                    viewFields = new ContentFieldRefCollection(Context, this);
                return viewFields;
            }
        }

        /// <summary>
        /// Gets the access roles of this view.
        /// </summary>
        public new string[] Roles
        {
            get
            {
                if (roles == null)
                {
                    if (!string.IsNullOrEmpty(Model.Roles))
                    {
                        roles = Model.Roles.Split(',');
                    }
                    else
                        roles = new string[0];
                }
                return roles;
            }
            set
            {
                roles = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ContentViewDecorator class with view model and data context.
        /// </summary>
        /// <param name="model">The content view model.</param>
        /// <param name="dataContext">The data context object.</param>
        public ContentViewDecorator(ContentView model, IDataContext dataContext)
        {
            Model = model;
            model.CopyTo(this, "Parent", "FieldRefs", "Roles");
            Context = dataContext;
            this.Parent = new ContentListDecorator(dataContext, model.Parent);
        }

        /// <summary>
        /// Set ordered view fields.
        /// </summary>
        /// <param name="fields">The ordered field names.</param>
        public void SetViewFields(params string[] fields)
        {
            var tmpFields = FieldRefs.ToList();
            this.FieldRefs.Clear();

            foreach (var field in fields)
            {
                var fieldRef = tmpFields.FirstOrDefault(e => e.Name.Equals(field));
                if (fieldRef == null)
                    fieldRef = new ContentFieldRef(this, this.Parent.Fields[field]);
                this.FieldRefs.Add(fieldRef);
            }

            this.Model.FieldRefsXml = FieldRefs.Element().OuterXml();
            this.FieldRefsXml = this.Model.FieldRefsXml;

            Context.SaveChanges();

            this.Refresh();
        }

        /// <summary>
        /// The Internal fields of the Views.
        /// </summary>
        public static string[] InternalFields = new string[] {
            DataNames.ID,DataNames.ParentID,DataNames.Privacy,
            DataNames.Created,DataNames.Modified,DataNames.Published,
            DataNames.State,DataNames.IsPublished,DataNames.EnableComments,
            DataNames.Slug,DataNames.Path,DataNames.Categories,DataNames.Tags,
            DataNames.Owner,DataNames.Modifier,DataNames.Ratings,DataNames.Reads,
            DataNames.TotalAttachs,DataNames.TotalVotes
        };

        /// <summary>
        /// Gets the default ContentQuery object of this view.
        /// </summary>
        public ContentQuery DefaultQuery
        {
            get
            {
                return new ContentQuery()
                {
                    //Filter = this.Filter,
                    Size = this.PageSize,
                    Sort = this.Sort,
                    GroupBy = this.GroupBy,
                    Index = this.PageIndex == 0 ? 1 : PageIndex
                };
            }
        }

        /// <summary>
        /// Computes the sum of specified field values.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns> The sum of the values in view results</returns>
        public decimal Sum(string fieldName)
        {
            var query = DefaultQuery;
            query.Index = 0;
            query.Size = 0;

            return Items(query).Element().Descendants("row").Where(e => e.Name.Equals(fieldName) && e.Value != null).Select(e => decimal.Parse(e.Value)).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of specified field values.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>The average of the values in view results.</returns>
        public decimal Avg(string fieldName)
        {
            var query = DefaultQuery;
            query.Index = 0;
            query.Size = 0;
            return Items(query).Element()
                                            .Descendants("row")
                                            .Where(e => e.Name.Equals(fieldName) && e.Value != null)
                                            .Select(e => decimal.Parse(e.Value))
                                            .Average();
        }

        private DataTable cacheTable = null;

        internal void ClearCache() { }

        private string CacheKey
        {
            get
            {
                return "_cache_list" + this.Parent.ID + "view_" + this.Name;
            }
        }

        private DataTable LoadFromCache()
        {
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                var key = this.CacheKey;
                var cache = HttpContext.Current.Cache;
                if (cache[CacheKey] != null)
                {
                    return (DataTable)cache[CacheKey];
                }
            }
            return null;
        }

        private void AddToCache(DataTable table)
        {
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                var cache = HttpContext.Current.Cache;
                if (cache[CacheKey] == null)
                {
                    var viewDataFile = App.Get().NetDrive.MapPath(new Uri(string.Format(Parent.DefaultListPath.ToString() + "cache/view_{0}.xml", this.Name)));
                    var dependency = new CacheDependency(viewDataFile);
                    cache.Add(CacheKey, table, dependency, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
            }
        }

        /// <summary>
        /// Gets the data table that contains all data rows
        /// </summary>
        /// <returns></returns>
        private DataTable GetDataTable()
        {
            //if (cacheTable == null)
            //    cacheTable = LoadFromCache();

            if (cacheTable != null)
            {
                cacheTable.DefaultView.Sort = "";
                cacheTable.DefaultView.RowFilter = "";
                return cacheTable;
            }

            var netdrive = App.Get().NetDrive;
            var listPath = Parent.DefaultListPath.ToString();
            var dataPath = netdrive.MapPath(new Uri(listPath + "cache/"));

            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            var viewDataFile = netdrive.MapPath(new Uri(string.Format(listPath + "cache/view_{0}.xml", this.Name)));
            var schema = netdrive.MapPath(new Uri(string.Format(listPath + "cache/schema_{0}.xml", this.Name)));

            var dt = new DataTable();
            dt.TableName = this.Name;

            if (!File.Exists(viewDataFile))
            {
                //Generate the view data file
                var items = this.Parent.EnableVersioning ? Context.Where<ContentDataItem>(c => c.ParentID.Equals(this.ParentID) && c.IsCurrentVersion).OrderByDescending(v => v.Modified).ToList() :
                    Context.Where<ContentDataItem>(c => c.ParentID.Equals(this.ParentID)).OrderByDescending(v => v.Modified).ToList();
                var idColumn = new DataColumn(DataNames.ID, typeof(Guid));

                #region add columns

                dt.Columns.Add(idColumn);
                dt.Columns.Add(DataNames.ParentID, typeof(Guid));
                dt.Columns.Add(DataNames.Privacy, typeof(int));
                dt.Columns.Add(DataNames.Created, typeof(DateTime));
                dt.Columns.Add(DataNames.Modified, typeof(DateTime));
                dt.Columns.Add(DataNames.Published, typeof(DateTime));
                dt.Columns.Add(DataNames.Pos, typeof(int));
                dt.Columns.Add(DataNames.State, typeof(int));
                dt.Columns.Add(DataNames.IsPublished, typeof(bool));
                dt.Columns.Add(DataNames.EnableComments, typeof(bool));
                dt.Columns.Add(DataNames.Slug, typeof(string));
                dt.Columns.Add(DataNames.Path, typeof(string));
                dt.Columns.Add(DataNames.Tags, typeof(string));
                dt.Columns.Add(DataNames.Categories, typeof(string));
                dt.Columns.Add(DataNames.Owner, typeof(string));
                dt.Columns.Add(DataNames.Modifier, typeof(string));
                dt.Columns.Add(DataNames.Ratings, typeof(double));
                dt.Columns.Add(DataNames.Reads, typeof(int));
                dt.Columns.Add(DataNames.TotalAttachs, typeof(int));
                dt.Columns.Add(DataNames.TotalVotes, typeof(int));
                dt.Columns.Add(DataNames.TotalComms, typeof(int));
                dt.Columns.Add(DataNames.TotalShares, typeof(int));
                dt.Columns.Add(DataNames.Version, typeof(int));
                dt.Columns.Add(DataNames.HasChildren, typeof(bool));
                dt.PrimaryKey = new DataColumn[] { idColumn };

                #endregion

                var vfs = this.FieldRefs.Count == 0 ? this.Parent.Fields.Select(f => f).ToList() : this.FieldRefs.Select(f => f.Field).ToList();

                foreach (var f in vfs)
                    dt.Columns.Add(f.Name, f.SystemType);

                foreach (var item in items)
                {
                    var itemWrapper = new ContentDataItemDecorator(item, Context);
                    int[] cats = null;

                    if (item.Categories != null)
                        cats = item.Categories.Select(c => c.ID).ToArray();

                    #region add new row
                    var row = dt.NewRow();
                    row[DataNames.ID] = item.ID;
                    row[DataNames.ParentID] = item.ParentItemID;
                    row[DataNames.Privacy] = item.Privacy;
                    row[DataNames.Created] = item.Created;
                    row[DataNames.Pos] = item.Pos;
                    if (item.Modified.HasValue)
                        row[DataNames.Modified] = item.Modified;

                    if (item.Published.HasValue)
                        row[DataNames.Published] = item.Published;

                    row[DataNames.IsPublished] = item.IsPublished;
                    row[DataNames.Modifier] = item.Modifier;
                    row[DataNames.State] = item.ModerateState;
                    row[DataNames.Tags] = item.Tags;
                    row[DataNames.Slug] = item.Slug;
                    row[DataNames.Path] = item.Path;
                    row[DataNames.Categories] = cats != null ? (string.Join(",", Context.Where<Category>(c => cats.Contains(c.ID)).Select(c => c.Name).ToArray())) : "";
                    row[DataNames.EnableComments] = item.EnableComments;
                    row[DataNames.Owner] = item.Owner;
                    row[DataNames.Ratings] = item.Ratings;
                    row[DataNames.Reads] = item.Reads;
                    row[DataNames.TotalAttachs] = item.TotalAttachments;
                    row[DataNames.TotalVotes] = item.TotalVotes;
                    row[DataNames.TotalComms] = itemWrapper.TotalComments;
                    row[DataNames.TotalShares] = Parent.AllowResharing ? itemWrapper.Reshares().Count() : 0;
                    row[DataNames.Version] = itemWrapper.Version;
                    row[DataNames.HasChildren] = Parent.IsHierarchy ? itemWrapper.Children().Count() > 0 : false;

                    foreach (var v in vfs)
                    {
                        var raw = itemWrapper.Value(v.Name).Raw;
                        if (raw == null)
                            row[v.Name] = DBNull.Value;
                        else
                            row[v.Name] = raw;
                    }
                    dt.Rows.Add(row);

                    #endregion

                }

                dt.AcceptChanges();
                dt.DefaultView.Sort = this.Sort;
                dt.DefaultView.RowFilter = FormatFilter(this.Filter);

                //At first we need to apply the first filter for this table
                cacheTable = dt.DefaultView.ToTable();
                cacheTable.PrimaryKey = new DataColumn[] { cacheTable.Columns[DataNames.ID] };
                cacheTable.WriteXml(viewDataFile);
                cacheTable.WriteXmlSchema(schema, true);
            }
            else
            {
                dt.ReadXmlSchema(schema);
                dt.ReadXml(viewDataFile);
                cacheTable = dt;
            }
            //AddToCache(dt);
            return cacheTable;
        }

        private string FormatFilter(string filter)
        {
            var formattedFilter = filter;
            //Support current user filter 
            //eg Owner = {user}
            if (filter.Contains("{user}"))
            {
                var userName = HttpContext.Current.Request.IsAuthenticated ? HttpContext.Current.User.Identity.Name : "''";
                formattedFilter = filter.Replace("{user}", "'" + userName + "'");
            }

            if (filter.Contains("{now}"))
                formattedFilter = filter.Replace("{now}", string.Format("#{0}#", DateTime.Now));

            if (filter.Contains("{today}"))
                formattedFilter = filter.Replace("{today}", string.Format("#{0}#", DateTime.Today));

            return formattedFilter;
        }

        /// <summary>
        /// Find item by id
        /// </summary>
        /// <param name="id">The data item id.</param>
        /// <returns>A content query result item.</returns>
        public ContentQueryResultItem GetItem(Guid id)
        {
            var results = this.Items(string.Format("id='{0}'", id.ToString()));
            return results.FirstOrDefault();
        }

        #region Obsolete Items methods
        /// <summary>
        /// Gets the items in query result by specified query parameters.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="sort">The sort expression.</param>
        /// <param name="index">The paging index.</param>
        /// <param name="size">The paging size.</param>
        /// <returns>A content query results that contains result items.</returns>
        [Obsolete("Please use the GetItems to instead.")]
        public ContentQueryResult Items(string filter, string sort = "", int index = 1, int size = 0)
        {
            return Items(new ContentQuery() { Sort = sort, Filter = filter, Index = index, Size = size });
        }

        /// <summary>
        /// Gets the items in query result by specified query object
        /// </summary>
        /// <param name="query">The content query</param>
        /// <returns>A content query results that contains result items.</returns>
        [Obsolete("Please use the GetItems to instead.")]
        public ContentQueryResult Items(ContentQuery query = null)
        {
            var dt = GetDataTable();
            var dataView = dt.DefaultView;
            dataView.AllowDelete = false;
            dataView.AllowEdit = false;
            dataView.AllowNew = false;
            dataView.ApplyDefaultSort = false;

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Sort))
                    dataView.Sort = query.Sort;

                if (!string.IsNullOrEmpty(query.Filter))
                    dataView.RowFilter = FormatFilter(query.Filter);
            }
            else
            {
                query = this.DefaultQuery;
                dataView.Sort = query.Sort;
            }

            query.Total = dataView.Count;
            var resultItems = new List<ContentQueryResultItem>();

            if (query.Size == 0 && this.AllowPaging)
                query.Size = this.PageSize;

            var counter = dataView.Count;

            #region groupby

            //dataView.AsQueryable()
            //var selQuery=from r in dataView
            //           group r by r.f
            #endregion

            if (query.Size > 0)
                counter = query.Index <= 1 ? query.Size : ((query.Index - 1) * query.Size + query.Size);

            var skip = query.Index <= 1 ? 0 : (query.Index - 1) * query.Size;

            for (int i = skip; i < counter; i++)
            {
                if (i >= dataView.Count)
                    break;

                var rowView = dataView[i];
                dynamic rowData = new ExpandoObject();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    var col = dt.Columns[j];
                    ((IDictionary<String, Object>)rowData).Add(col.ColumnName, rowView[col.ColumnName]);
                }
                resultItems.Add(new ContentQueryResultItem(this, rowData));
            }

            return new ContentQueryResult()
            {
                List = this.Parent,
                Items = resultItems,
                View = this,
                Query = query
            };
        }

        /// <summary>
        /// Get items query from current http context.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <remarks>
        /// This method will be auto generate the filter for current user.
        /// </remarks>
        /// <returns>A content query results that contains result items.</returns>
        [Obsolete("Please use the GetItems to instead.")]
        public ContentQueryResult Items(HttpContextBase context)
        {
            var request = context.Request;
            var list = this.Parent;
            var _query = new ContentQuery(request);
            var _filter = "";
            var routeData = request.RequestContext.RouteData.Values;

            if (routeData.ContainsKey("tags") || routeData.ContainsKey("achrives"))
            {
                if (routeData.ContainsKey("achrives"))
                {
                    var year = (int)routeData["year"];
                    var month = (int)routeData["month"];
                    _filter = string.Format("published >=#{0}-{1}-01# AND Published <=#{0}-{1}-30#", year, month);
                }
                else
                {
                    var tag = routeData["tag"];
                    _filter = "tags LIKE '%" + tag + "%'";
                }
            }
            else
            {
                if (list.IsModerated)
                {
                    var ext = "";
                    if (Parent.IsHierarchy && string.IsNullOrEmpty(context.Request.QueryString["parentId"]))
                        ext = " AND parentId='" + Guid.Empty.ToString() + "'";

                    if (list.IsOwner(context) || (request.IsAuthenticated && list.IsModerator(context.User.Identity.Name)))
                        //Here is Moderator
                        _filter = "(" + DataNames.IsPublished + "=True And " + DataNames.Owner + "<>'" + context.User.Identity.Name + "')" +
                            " Or " + DataNames.Owner + "='" + context.User.Identity.Name + "'";
                    else
                        _filter = "(" + DataNames.IsPublished + "=True" +
                            " And " + DataNames.State + "=" + ((int)ModerateStates.Approved).ToString() +
                            ") Or " + DataNames.Owner + "='" + context.User.Identity.Name + "'";

                    if (!string.IsNullOrEmpty(ext))
                        _filter = "(" + _filter + ") " + ext;

                }
                else
                {
                    if (Parent.IsHierarchy && string.IsNullOrEmpty(context.Request.QueryString["parentId"]))
                        _filter = DataNames.IsPublished + "=True" + " AND parentId='" + Guid.Empty.ToString() + "'";
                    else
                        _filter = DataNames.IsPublished + "=True";
                }
            }


            if (string.IsNullOrEmpty(_query.Filter) && !string.IsNullOrEmpty(_filter))
                _query.Filter = _filter;

            if (string.IsNullOrEmpty(_query.Sort) && !string.IsNullOrEmpty(this.DefaultQuery.Sort))
                _query.Sort = this.DefaultQuery.Sort;

            return Items(_query);
        }

        #endregion

        #region 3.0.3 added

        public ContentQueryResult GetItems(HttpContextBase context) { return this.Items(context); }

        public ContentQueryResult GetItems(ContentQuery query = null) { return this.Items(query); }

        public ContentQueryResult GetItems(Action<ContentQuery> builder)
        {
            var query = new ContentQuery();
            if (builder != null)
                builder.Invoke(query);
            return this.GetItems(query);
        }

        public ContentQueryResult GetItems(string filter, string sort = "", int index = 1, int size = 0) { return this.Items(filter, sort, index, size); }

        public ContentQueryResult GetUserItems(string userName = "")
        {
            if (string.IsNullOrEmpty(userName))
                userName = HttpContext.Current.User.Identity.Name;

            var query = new ContentQuery();
            query.Eq(query.SysFieldNames.Owner, userName);

            return Items(query);
        }

        #endregion

        /// <summary>
        /// Gets the view page url.
        /// </summary>
        public string Url
        {
            get
            {
                if (this.NoPage)
                    return "";
                else
                    return string.Format("~/{0}/{1}/lists/{2}/views/{3}.html", this.Parent.Web.Name, this.Parent.Locale, this.Parent.Name, this.Name);
            }
        }

        /// <summary>
        /// Gets the rss feed url.
        /// </summary>
        public Uri RssUri
        {
            get
            {
                return new Uri(string.Format("{0}{1}/{2}/lists/{3}/rss/{4}.xml", App.Get().Context.AppUrl.ToString(), this.Parent.Web.Name, this.Parent.Locale, this.Parent.Name, this.Name));
            }
        }

        /// <summary>
        /// Gets the atom feed url.
        /// </summary>
        public Uri AtomUri
        {
            get
            {
                return new Uri(string.Format("{0}{1}/{2}/lists/{3}/atom/{4}.xml", App.Get().Context.AppUrl.ToString(), this.Parent.Web.Name, this.Parent.Locale, this.Parent.Name, this.Name));
            }
        }

        /// <summary>
        /// Gets the settings url.
        /// </summary>
        public string SettingUrl
        {
            get
            {
                return string.Format("~/dashboard/{0}/{1}/{2}/views/{3}", this.Parent.Web.Name, this.Parent.Locale, this.Parent.Name, this.Name);
            }
        }

        /// <summary>
        /// Save all changes to database.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            this.CopyTo(Model, "Parent", "FieldRefs", "Items", "Roles");
            Model.FieldRefsXml = this.FieldRefs.ToString();
            if (this.roles != null && this.roles.Length > 0)
                Model.Roles = string.Join(",", this.roles);
            else
                Model.Roles = "";
            Refresh();
            Context.Update(Model);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Gets the first visible text field as title field
        /// </summary>
        /// <returns></returns>
        public ContentField GetDefaultTitleField()
        {
            ContentField firstField = null;
            var tt = (int)ContentFieldTypes.Text;
            if (this.FieldRefs.Count == 0)
            {
                firstField = Parent.Fields.FirstOrDefault(f => f.FieldType == tt && !f.IsHidden);
            }
            else
            {
                var vf = FieldRefs.FirstOrDefault(f => f.FieldType == tt && !f.IsHidden);
                if (vf != null)
                    return Parent.Fields[vf.Name];
            }
            return firstField;
        }

        /// <summary>
        /// Convert the view to a json object
        /// </summary>
        /// <returns></returns>
        public dynamic ToObject()
        {
            return new
            {
                id = this.ID,
                name = this.Name,
                title = string.IsNullOrEmpty(this.Title) ? this.Name : this.Title,
                desc = this.Description,
                url = this.Url
            };
        }

        /// <summary>
        /// Indicates whether the view enable ajax load feature.
        /// </summary>
        public bool IsClientView
        {
            get
            {
                var body = this.GetBodyTemplate();
                return body != null && body.IsClientTemplate;
            }
        }

        /// <summary>
        /// Clear and refresh the cache data
        /// </summary>
        /// <returns></returns>
        public void Refresh()
        {
            var netdrive = App.Get().NetDrive;
            var listPath = Parent.DefaultListPath.ToString();
            var dataPath = netdrive.MapPath(new Uri(listPath + "cache/"));

            if (Directory.Exists(dataPath))
            {
                var viewDataFile = netdrive.MapPath(new Uri(string.Format(listPath + "cache/view_{0}.xml", this.Name)));
                var schema = netdrive.MapPath(new Uri(string.Format(listPath + "cache/schema_{0}.xml", this.Name)));
                if (File.Exists(viewDataFile))
                    File.Delete(viewDataFile);
                if (File.Exists(schema))
                    File.Delete(schema);
            }
        }

        /// <summary>
        /// Gets the parent web page object.
        /// </summary>
        public WebPage Page
        {
            get
            {
                // var slugFormatted = string.Format("lists/{0}/views/{1}", this.Parent.Name, this.Name);
                var page = Context.WebPages.Find(p => p.WebID == this.Parent.WebID &&
                    p.Locale.Equals(this.Parent.Locale, StringComparison.OrdinalIgnoreCase) &&
                    p.Slug.Equals(this.Name, StringComparison.OrdinalIgnoreCase));

                if (page != null)
                    return new WebPageDecorator(page, Context);

                return null;

            }
        }

        /// <summary>
        /// Convert the view object to XElement.
        /// </summary>
        /// <returns></returns>
        public override XElement Element()
        {
            var element = base.Element();
            element.Add(this.FieldRefs.Element());
            return element;
        }

        /// <summary>
        /// Indicates whether this view can generate syndication feeds.
        /// </summary>
        public bool HasFeed
        {
            get
            {
                return this.FieldRefs.FirstOrDefault(f => !string.IsNullOrEmpty(f.ToFeedItemField)) != null;
                //var feedTitleField = ;
                //var feedDescField = this.FieldRefs.FirstOrDefault(f => !string.IsNullOrEmpty(f.ToFeedItemField));
                //if (feedDescField != null && feedDescField != null)
                //    return true;
                //return this.FieldRefs["title"] != null && this.FieldRefs["description"] != null;
            }
        }

        /// <summary>
        /// Generate the feed object.
        /// </summary>
        /// <returns></returns>
        public SyndicationFeed Feed()
        {
            var items = this.Items();
            var feedItems = new List<SyndicationItem>();
            var owners = items.Select(i => i.Owner).Distinct();
            var authors = Context.Where<UserProfile>(u => owners.Contains(u.UserName));
            var userUriFormat = App.Get().Context.AppUrl.ToString() + "{0}";

            var feedTitleField = this.FieldRefs.FirstOrDefault(f => !string.IsNullOrEmpty(f.ToFeedItemField) && f.ToFeedItemField.Equals("title"));
            var feedDescField = this.FieldRefs.FirstOrDefault(f => !string.IsNullOrEmpty(f.ToFeedItemField) && f.ToFeedItemField.Equals("description"));

            var feedThumbField = this.FieldRefs.FirstOrDefault(f => !string.IsNullOrEmpty(f.ToFeedItemField) && f.ToFeedItemField.Equals("image"));
            var feedImageField = this.FieldRefs.FirstOrDefault(f => !string.IsNullOrEmpty(f.ToFeedItemField) && f.ToFeedItemField.Equals("thumbnail"));
            var feedVideoField = this.FieldRefs.FirstOrDefault(f => !string.IsNullOrEmpty(f.ToFeedItemField) && f.ToFeedItemField.Equals("video"));

            var titleFieldName = "title";
            var descFieldName = "description";
            var thumbFieldName = "";
            var videoFieldName = "";
            var imageFieldName = "";

            if (feedTitleField != null)
                titleFieldName = feedTitleField.Name;

            if (feedDescField != null)
                descFieldName = feedDescField.Name;

            if (feedThumbField != null)
                thumbFieldName = feedThumbField.Name;

            if (feedVideoField != null)
                videoFieldName = feedThumbField.Name;

            if (feedImageField != null)
                imageFieldName = feedThumbField.Name;

            foreach (var item in items)
            {
                var title = !item.IsNull(titleFieldName) ? (string)item[titleFieldName] : "";
                var desc = !item.IsNull(descFieldName) ? (string)item[descFieldName] : "";
                var subTitle = "";
                var imgUrl = !item.IsNull(imageFieldName) ? (string)item[imageFieldName] : "";

                if (string.IsNullOrEmpty(title))
                    continue;

                if (!string.IsNullOrEmpty(desc))
                {
                    desc = TextEngine.Text(desc);
                    if (desc.Length > 300)
                        subTitle = desc.Substring(0, 300) + "...";
                }

                var feedItem = new ItunesItem(title, desc, new Uri(item.UrlComponent), item.ID.ToString(), new DateTimeOffset(item.Published));

                if (!object.ReferenceEquals(item.Tags, null) && item.Tags.Length > 0)
                    feedItem.Keywords = string.Join(",", item.Tags);

                if (!string.IsNullOrEmpty(imgUrl))
                    feedItem.ImageUrl = imgUrl;

                //feedItem.ElementExtensions.Add(new SyndicationElementExtension(
                var cats = item.Categories;
                if (cats != null && cats.Length > 0)
                {
                    foreach (var c in cats)
                    {
                        feedItem.Categories.Add(new SyndicationCategory(c));
                    }
                }
                var owner = authors.FirstOrDefault(a => a.UserName.Equals(item.Owner));
                feedItem.Subtitle = subTitle;
                feedItem.Author = owner.DisplayName;
                feedItem.Authors.Add(new SyndicationPerson(owner.Email, owner.DisplayName, string.Format(userUriFormat, owner.UserName)));
                feedItems.Add(feedItem);
            }

            var feed = new SyndicationFeed(feedItems);
            feed.Title = new TextSyndicationContent(this.Title);
            feed.Description = new TextSyndicationContent(this.Description);
            feed.BaseUri = new Uri(this.Url.Replace("~", App.Get().Context.AppUrl.ToString()));
            feed.Generator = "DotNetAge";
            feed.Language = this.Parent.Locale;
            if (this.Generated.HasValue)
                feed.LastUpdatedTime = new DateTimeOffset(this.Generated.Value);

            return feed;
        }

        /// <summary>
        /// Save the query item result to xml for rss format.
        /// </summary>
        /// <returns></returns>
        public string ToRss()
        {
            var sb = new StringBuilder();
            using (var strWriter = new StringWriter(sb))
            {
                using (var writer = new XmlTextWriter(strWriter))
                {
                    this.Feed().SaveAsRss20(writer);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Save the query item result to xml for atom format
        /// </summary>
        /// <returns></returns>
        public string ToAtom()
        {
            var sb = new StringBuilder();
            using (var strWriter = new StringWriter(sb))
            {
                using (var writer = new XmlTextWriter(strWriter))
                {
                    this.Feed().SaveAsAtom10(writer);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Indicates whether current user is authorized to get the view data items.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>true if authorized otherwrise false.</returns>
        public bool IsAuthorized(HttpContextBase context)
        {
            if (AllowAnonymous)
                return true;

            if (context.Request.IsAuthenticated)
            {
                if (context.User.Identity.Name.Equals(this.Parent.Owner))
                    return true;

                if (Roles != null && Roles.Length > 0)
                {
                    foreach (var role in this.Roles)
                    {
                        if (App.Get().User.IsInRole(role))
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
