//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using DNA.Web.Contents;
using DNA.Web.Events;
using DNA.Utility;
using System.Reflection;
using DNA.Data;
using DNA.Data.Schema;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to ContentList object model. 
    /// </summary>
    public class ContentListDecorator : ContentList
    {
        #region Private var

        internal ContentList Model { get; set; }
        internal IDataContext Context { get; set; }
        private ContentFieldCollection fields = null;
        private WebDecorator web = null;
        private ContentViewCollection views = null;
        private ContentFormCollection forms = null;
        private ContentFormDecorator newForm = null;
        private ContentFormDecorator editForm = null;
        private ContentFormDecorator dispForm = null;
        private ContentFormDecorator activityForm = null;
        private IQueryable<UserProfile> followers = null;
        private IEnumerable<ContentItemArchive> archives = null;
        private PersonalListHelper myHelper = null;

        #endregion

        ///// <summary>
        ///// Gets personal list helper for current user.
        ///// </summary>
        //public PersonalListHelper My
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Request.IsAuthenticated)
        //        {
        //            if (myHelper == null)
        //            {
        //                myHelper = new PersonalListHelper(this);
        //            }
        //            return myHelper;
        //        }
        //        return null;
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of  the ContentListDecorator class.
        /// </summary>
        /// <remarks>
        /// This constructor only use for testing.
        /// </remarks>
        public ContentListDecorator() { }

        /// <summary>
        /// Initializes a new instance of  the ContentListDecorator class with data context and content list object.
        /// </summary>
        /// <param name="context">The data context object.</param>
        /// <param name="list">The content list object.</param>
        public ContentListDecorator(IDataContext context, ContentList list)
        {
            Model = list;
            Context = context;
            list.CopyTo(this, "Views", "Items", "Web", "Roles", "Forms", "Followers");
        }

        /// <summary>
        /// Save content list chages to database.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            this.CopyTo(Model, "Views", "Items", "Web", "Roles", "Forms", "Followers");
            Context.Update(Model);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Set field orders
        /// </summary>
        /// <param name="orders">The ordered field names.</param>
        public void SetFieldOrders(string[] orders)
        {
            var tmpFields = this.Fields.ToList();
            this.Fields.Clear();
            foreach (var o in orders)
                this.Fields.Add(tmpFields.FirstOrDefault(f => f.Name.Equals(o)));

            foreach (var t in tmpFields)
            {
                var testField = this.Fields.FirstOrDefault(f => f.Name.Equals(t.Name));
                if (testField == null)
                    this.Fields.Add(testField);
            }
            this.SaveSchema();
        }

        /// <summary>
        /// Save the fields schema to database.
        /// </summary>
        /// <returns></returns>
        public bool SaveSchema()
        {
            Model.SaveFields(this.Fields);
            this.FieldsXml = Model.FieldsXml;
            ClearCache();
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Gets the parent web object.
        /// </summary>
        public virtual new WebDecorator Web
        {
            get
            {
                if (web == null)
                    web = new WebDecorator(Context.Find<Web>(this.WebID), Context);
                return web;
            }
        }

        /// <summary>
        /// Gets the field collection.
        /// </summary>
        public ContentFieldCollection Fields
        {
            get
            {
                if (fields == null)
                    fields = new ContentFieldCollection(Context, this);
                return fields;
            }
        }

        /// <summary>
        /// Return all items of this list
        /// </summary>
        public new IQueryable<ContentDataItem> Items
        {
            get
            {
                return Context.Where<ContentDataItem>(i => i.ParentID == this.ID);
            }
        }

        /// <summary>
        /// Gets pending data item count.
        /// </summary>
        public int PendingItemCount
        {
            get { return Items.Count(i => i.ModerateState == 0 && i.IsCurrentVersion); }
        }

        /// <summary>
        /// Gets unpublish data item count.
        /// </summary>
        public int UnpublishItemCount
        {
            get
            {
                return Items.Count(i => !i.IsPublished);
            }
        }

        /// <summary>
        /// Gets the content list setting url.
        /// </summary>
        public string SettingUrl
        {
            get
            {
                return string.Format("~/dashboard/{0}/{1}/lists/{2}", this.Web.Name, this.Locale, this.Name);
            }
        }

        /// <summary>
        /// Gets total data items count.
        /// </summary>
        public int TotalItems
        {
            get { return Items.Where(i => i.IsCurrentVersion).Count(); }
        }

        /// <summary>
        /// Gets the content view collection.
        /// </summary>
        public new ContentViewCollection Views
        {
            get
            {
                if (this.views == null)
                {
                    this.views = new ContentViewCollection(Context, this.Model);
                }
                return this.views;
            }
        }

        /// <summary>
        /// Gets the content form collection.
        /// </summary>
        public new ContentFormCollection Forms
        {
            get
            {
                if (this.forms == null)
                    this.forms = new ContentFormCollection(Context, this.Model);
                return this.forms;
            }
        }

        /// <summary>
        /// Gets the default edit form object that use to edit existing data item.
        /// </summary>
        public new ContentFormDecorator EditForm
        {
            get
            {
                if (editForm == null)
                {
                    var t = (int)ContentFormTypes.Edit;
                    var _form = Context.Find<ContentForm>(f => f.ParentID == this.ID && f.FormType == t);
                    if (_form != null)
                        editForm = new ContentFormDecorator(Context, _form);
                }
                return editForm;
            }
        }

        /// <summary>
        /// Gets the default new form object that use to create new data item.
        /// </summary>
        public new ContentFormDecorator NewForm
        {
            get
            {
                if (newForm == null)
                {
                    var t = (int)ContentFormTypes.New;
                    var _form = Context.Find<ContentForm>(f => f.ParentID == this.ID && f.FormType == t);
                    if (_form != null)
                        newForm = new ContentFormDecorator(Context, _form);
                }
                return newForm;
            }
        }

        /// <summary>
        /// Gets the default detail form object that use to display the existing data item detail.
        /// </summary>
        public new ContentFormDecorator DetailForm
        {
            get
            {
                if (dispForm == null)
                {
                    var t = (int)ContentFormTypes.Display;
                    var _form = Context.Find<ContentForm>(f => f.ParentID == this.ID && f.FormType == t);
                    if (_form != null)
                        dispForm = new ContentFormDecorator(Context, _form);
                }
                return dispForm;
            }
        }

        /// <summary>
        /// Gets the default activity form that use to show in activity stream.
        /// </summary>
        public new ContentFormDecorator ActivityForm
        {
            get
            {
                if (activityForm == null)
                {
                    var t = (int)ContentFormTypes.Activity;
                    var _form = Context.Find<ContentForm>(f => f.ParentID == this.ID && f.FormType == t);
                    if (_form != null)
                        activityForm = new ContentFormDecorator(Context, _form);
                }
                return activityForm;
            }
        }

        /// <summary>
        /// Gets the user profile collection of the follow users.
        /// </summary>
        public new IQueryable<UserProfile> Followers
        {
            get
            {
                if (followers == null)
                {
                    var followerNames = Context.Where<Follow>(f => f.ListID.Equals(this.ID)).Select(u => u.Follower).ToArray();
                    followers = Context.Where<UserProfile>(p => followerNames.Contains(p.UserName));
                }
                return followers;
            }
        }

        /// <summary>
        /// Gets the default view object that use to display the data items.
        /// </summary>
        public ContentViewDecorator DefaultView
        {
            get
            {
                return Views.Default;
            }
        }

        /// <summary>
        /// Gets the archive collection of current list.
        /// </summary>
        public IEnumerable<ContentItemArchive> Archives
        {
            get
            {
                if (archives == null)
                {
                    var _archives = Context.Where<ContentDataItem>(c => c.IsPublished && c.ParentID == this.ID && c.Locale.Equals(this.Locale)).ToList()
                                                           .Select(a => new ContentItemArchive(this)
                                                           {
                                                               Year = a.Published.Value.Year,
                                                               Month = a.Published.Value.Month
                                                           })
                                                           .ToList();
                    archives = new List<ContentItemArchive>();

                    foreach (var a in _archives)
                    {
                        var _arch = archives.FirstOrDefault(aa => aa.Year.Equals(a.Year) && aa.Month.Equals(a.Month));
                        if (_arch == null)
                            ((List<ContentItemArchive>)archives).Add(a);
                        else
                            _arch.Count++;
                    }
                }
                return archives.OrderBy(a => a.Year).ThenBy(a => a.Month).ToList();
            }
        }

        private IEnumerable<ContentItemTag> _tags = null;

        /// <summary>
        /// Gets the tag colleciton of current list.
        /// </summary>
        public IEnumerable<ContentItemTag> Tags
        {
            get
            {
                if (_tags == null)
                {
                    var tags = new List<ContentItemTag>();
                    var tagStr = Context.Where<ContentDataItem>(a => !string.IsNullOrEmpty(a.Tags) && a.ParentID == this.ID).Select(a => a.Tags).ToArray();
                    string fullTags = "";

                    if (tagStr.Length > 0)
                        fullTags = string.Join(",", tagStr);

                    var tagArgs = fullTags.Split(new char[] { ',' });
                    var total = tagStr.Length;

                    foreach (var tag in tagArgs)
                    {
                        var _tag = tags.FirstOrDefault(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase));
                        if (_tag == null)
                            tags.Add(new ContentItemTag(this) { Name = tag, Count = 1, TotalTags = total });
                        else
                            _tag.Count++;
                    }

                    _tags = tags;

                }
                return _tags;
            }
        }

        /// <summary>
        /// Clear all view index file
        /// </summary>
        public void ClearCache()
        {
            try
            {
                var netdrive = App.Get().NetDrive;
                var cacheUri = new Uri(this.DefaultListPath.ToString() + "/cache");
                if (netdrive.Exists(cacheUri))
                    netdrive.Delete(cacheUri);
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Identity the specified user name whether following the list data items.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <returns>Returns true if following.</returns>
        public bool IsFollowing(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (userName == Owner)
                return true;

            return Context.Count<Follow>(f => f.Follower.Equals(userName)) > 0;
        }

        /// <summary>
        /// Follow list content changes by specified user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns></returns>
        public bool Follow(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (userName == Owner)
                return false;

            if (IsFollowing(userName))
                return true;

            Context.Add(new Follow()
            {
                Owner = this.Owner,
                ListID = this.ID,
                Follower = userName
            });

            followers = null;
            var result = Context.SaveChanges() > 0;
            //new FollowContentEvent(this, userName, true).Raise();
            this.Trigger("FollowContentList", new ContentFollowEventArgs()
            {
                ListName = this.Name,
                Website = this.Web.Name,
                Follower = Owner,
                FollowTo = userName
            });
            return result;
        }

        /// <summary>
        /// Unfollow list content changes by specified user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns></returns>
        public bool Unfollow(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (userName == Owner)
                return false;

            Context.Delete<Follow>(f => f.Follower.Equals(userName) && f.ListID == this.ID);
            followers = null;

            var result = Context.SaveChanges() > 0;

            if (result)
                this.Trigger("UnfollowContentList", new ContentFollowEventArgs()
                {
                    ListName = this.Name,
                    Website = this.Web.Name,
                    Follower = Owner,
                    FollowTo = userName
                });
            //new FollowContentEvent(this, userName, false).Raise();

            return result;
        }

        /// <summary>
        /// Gets the content package which create the list.
        /// </summary>
        public ContentPackage Package
        {
            get
            {
                return App.Get().ContentTypes[this.BaseType];
            }
        }

        /// <summary>
        /// Create a dynamic object from list schema and popuplate the field values from specified item value array
        /// </summary>
        /// <returns></returns>
        public dynamic CreateItemData(object[] values)
        {
            dynamic result = new System.Dynamic.ExpandoObject();
            for (var i = 0; i < Fields.Count; i++)
            {
                var val = values[i];
                ((IDictionary<String, Object>)result).Add(Fields[i].Name, val);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <para>This example demonstrate how add data to content list.</para>
        /// <para>1.Create a new "Product" content list and named "products".</para>
        /// <para>2.Run the code below to add new product item object.</para>
        /// <code language="cs">
        /// var list=App.Get().CurrentWeb.Lists["products"];
        /// list.NewItem(new {
        /// code="po-iphone-5",
        /// title="iphone5",
        /// price=399 },User.Identity.Name,isPublished:true);
        /// </code>
        /// </example>
        /// <param name="data"></param>
        /// <param name="owner"></param>
        /// <param name="enableComments"></param>
        /// <param name="isPublished"></param>
        /// <param name="parentID"></param>
        /// <param name="pos"></param>
        /// <param name="locale"></param>
        /// <param name="categories"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public ContentDataItemDecorator NewItem(object data, string owner, bool enableComments = false,
              bool isPublished = false, string parentID = "", int pos = 0, string locale = "en-US", string categories = "", string tags = "")
        {
            var forms = data as NameValueCollection;
            ContentDataItem item = null;
            try
            {
                if (forms != null)
                    item = Context.ContentDataItems.Create(this.ID, ConvertToDict(forms), owner,
                        enableComments, isPublished, parentID, pos, categories, tags);
                else
                {
                    item = Context.ContentDataItems.Create(this.ID, MapTypedData(data), owner, enableComments, isPublished, parentID, pos, categories, tags);
                }
                Context.SaveChanges();
            }
            catch (ContentFieldNotFoundException fieldNotFoundExcpeiton)
            {
                throw new ObjectMetaNotMatchException(string.Format("New data item fail! The new data object property \"{0}\" can not map to list fields.", fieldNotFoundExcpeiton.FieldName));
            }
            catch (Exception e)
            {
                throw new Exception("New data item fail!" + e.Message, e);
            }
            var result = new ContentDataItemDecorator(item, Context);
            //ClearCache();

            this.Trigger(EventNames.ContentDataItemCreated, new ContentDataItemEventArgs()
            {
                DataItem = result,
                List = this
            });
            // EventDispatcher.RaiseContentDataItemCreated(result);
            return result;
        }

        #region 3.0.3 added - Refactoring

        public ContentDataItemDecorator Insert(object data, string owner = "", bool enableComments = false,
              bool isPublished = true, string parentID = "", int pos = 0, string locale = "en-US", string categories = "", string tags = "")
        {
            return this.NewItem(data, (!string.IsNullOrEmpty(owner) ? owner : HttpContext.Current.User.Identity.Name), enableComments, isPublished, parentID, pos, locale, categories, tags);
        }

        /// <summary>
        /// Clear the data items in this list
        /// </summary>
        public void Clear()
        {
            this.Context.ContentDataItems.Clear(this.ID);
            this.ClearCache();
        }

        /// <summary>
        /// Get default user items in default view
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>A content query result that contains data items.</returns>
        public ContentQueryResult GetUserItemsResult(string userName = "")
        {
            return this.DefaultView.GetUserItems(userName);
        }

        public IEnumerable<T> GetModels<T>(Expression<Func<ContentDataItem, bool>> predicate)
            where T : class
        {
            var result = this.GetItems(predicate).ToList();
            return result.Select(i => (new ContentDataItemDecorator(i, Context)).GetModel<T>()).ToList();
        }

        internal object MapTypedData(object data)
        {
            var dataType = data.GetType();
            var needConvert = dataType.GetProperties().Count(i => i.IsDefined(typeof(GroupAttribute), true) || i.IsDefined(typeof(FieldAttribute), true)) > 0;
            if (needConvert)
            {
                var dict = new Dictionary<string, object>();
                var props = dataType.GetProperties().Where(p => !p.IsDefined(typeof(IgnoreAttribute), true)).ToList();

                #region For strongly type  (added 3.0.3)
                foreach (var pro in props)
                {
                    var val = pro.GetValue(data, null);
                    if (val == null)
                        continue;
                    ///TODO:Ignore colllection
                    if (pro.IsDefined(typeof(IgnoreAttribute), true))
                        continue;

                    ///TODO: Stringly the enum

                    var fieldName = pro.Name;
                    if (pro.IsDefined(typeof(GroupAttribute), true))
                    {
                        var prefix = GetGroupName(pro);
                        ///TODO: Check whether has HasElementType
                        var groupProps = pro.PropertyType.GetProperties().Where(p => !p.IsDefined(typeof(IgnoreAttribute), true)).ToList();

                        foreach (var gp in groupProps)
                        {
                            var _fieldName = prefix + gp.Name;
                            var _val = gp.GetValue(val, null);
                            if (_val != null)
                                dict.Add(_fieldName, _val);
                        }

                        continue;
                    }
                    else
                    {
                        fieldName = GetAliasName(pro);
                    }
                    dict.Add(fieldName, val);

                }

                return dict;
                #endregion
            }
            else
            {
                return data;
                //Context.ContentDataItems.Update(Model, data);
            }
        }

        #region type helper methods

        private T GetCustomAttribute<T>(PropertyInfo property) where T : class
        {
            return property.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }

        private string GetGroupName(PropertyInfo property)
        {
            var groupAttr = GetCustomAttribute<GroupAttribute>(property);
            var prefix = property.Name;

            if (groupAttr != null && !string.IsNullOrEmpty(groupAttr.Name))
                prefix = groupAttr.Name;

            return prefix;
        }

        private string GetAliasName(PropertyInfo property)
        {
            if (property.IsDefined(typeof(FieldAttribute), true))
            {
                var fieldAttr = GetCustomAttribute<FieldAttribute>(property);
                //pro.GetCustomAttributes(typeof(FieldAttribute), true).FirstOrDefault() as FieldAttribute;
                if (!string.IsNullOrEmpty(fieldAttr.Name))
                    return fieldAttr.Name;
            }
            return property.Name;
        }

        #endregion

        //public ContentDataItemDecorator Find(dynamic exprBuilder) {
        //// list.Find(b=>{ b.Field("title").Eq("1025").And().Field("price").Lt(80); });
        //    throw new NotImplementedException();
        //}

        #endregion

        /// <summary>
        /// Find ContentDataItem by specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>A content data item decorator object wraps the content data item.</returns>
        public ContentDataItemDecorator Find(Expression<Func<ContentDataItem, bool>> predicate)
        {
            if (predicate != null)
            {

                var item = Context.ContentDataItems.Filter(l => l.ParentID.Equals(this.ID)).FirstOrDefault(predicate);
                if (item != null)
                    return new ContentDataItemDecorator(item, Context);
            }
            return null;
        }

        /// <summary>
        /// Get data item by specified item id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContentDataItemDecorator GetItem(Guid id)
        {
            var item = Context.Find<ContentDataItem>(i => i.ParentID == this.ID && i.ID == id);
            if (item != null)
                return new ContentDataItemDecorator(item, Context);
            return null;
        }

        /// <summary>
        /// Get data item by specified item slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public ContentDataItemDecorator GetItem(string slug, int ver = 0)
        {
            ContentDataItem item = null;
            if (EnableVersioning)
            {
                if (ver == 0)
                    item = Context.Find<ContentDataItem>(i => i.ParentID == this.ID && i.Slug == slug && i.IsCurrentVersion);
                else
                    item = Context.Find<ContentDataItem>(i => i.ParentID == this.ID && i.Slug == slug && i.Version.Equals(ver));
            }
            else
            {
                item = Context.Find<ContentDataItem>(i => i.ParentID == this.ID && i.Slug == slug && i.IsCurrentVersion);
            }

            if (item != null)
                return new ContentDataItemDecorator(item, Context);

            return null;
        }

        /// <summary>
        /// Gets data items by specified expression.
        /// </summary>
        /// <param name="predicate">The predicate to filter the data items.</param>
        /// <returns>A queryable colleciton that contains content data items</returns>
        public IQueryable<ContentDataItem> GetItems(Expression<Func<ContentDataItem, bool>> predicate)
        {
            return Context.Where<ContentDataItem>(c => c.ParentID == this.ID).Where(predicate);
        }

        /// <summary>
        /// Delete the content data item by specified content data item.
        /// </summary>
        /// <param name="id">The content data item id.</param>
        /// <returns></returns>
        public bool DeleteItem(Guid id)
        {
            if (id == Guid.Empty || id == null)
                throw new ArgumentNullException("id");

            var item = Context.Find<ContentDataItem>(i => i.ParentID == this.ID && i.ID == id);
            if (item == null)
                throw new ContentDataItemNotFoundException();

            var itemID = item.ID;
            var itemwrapper = new ContentDataItemDecorator(item, Context);
            var modal = itemwrapper.ToObject();
            var path = item.Path;

            Context.ContentDataItems.Delete(item);

            var result = Context.SaveChanges() > 0;

            //Delete web folders
            var netdrive = App.GetService<INetDriveService>();
            var itemFolderUrl = new Uri(AttachmentsPath.ToString() + "/" + id.ToString());
            if (netdrive.Exists(itemFolderUrl))
                netdrive.Delete(itemFolderUrl);
            //ClearCache();

            this.Trigger("ContentDataItemDeleted", new ContentDataItemDeletedEventArgs()
            {
                List = this,
                DataItemPath = path,
                ItemID = id
            });

            //EventDispatcher.RaiseContentDataItemDeleted(id, path, this);
            return result;
        }

        /// <summary>
        /// Gets whether the user is the moderator by specified user name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool IsModerator(string userName)
        {
            if (!string.IsNullOrEmpty(Moderators))
            {
                var moderators = Moderators.Split(',');
                return moderators.Contains(userName);
            }
            return false;
        }

        /// <summary>
        /// Gets current user whether is list owner.
        /// </summary>
        /// <param name="context">The http context</param>
        /// <returns></returns>
        public bool IsOwner(HttpContextBase context)
        {
            if (context.Request.IsAuthenticated)
                return this.IsOwner(context.User.Identity.Name);
            return false;
        }

        /// <summary>
        /// Gets specified user whether is list owner.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <returns></returns>
        public bool IsOwner(string userName)
        {
            return userName.Equals(this.Owner);
        }

        /// <summary>
        /// Gets the path contains the lists files
        /// </summary>
        public Uri DefaultListPath
        {
            get
            {
                return new Uri(String.Format("{0}/webshared/{1}/lists/{2}/", ApplicationPath, Web.Name, this.ID));
            }
        }

        /// <summary>
        /// Gets the attachement url of the NetDrive.
        /// </summary>
        public Uri AttachmentsPath
        {
            get
            {
                return new Uri(String.Format("{0}/webshared/{1}/lists/{2}/attachs/", ApplicationPath, Web.Name, this.ID));
            }
        }

        internal string ApplicationPath
        {
            get
            {
                var request = HttpContext.Current.Request;
                string url = request.Url.Scheme + "://" + request.Url.Authority;
                if (!request.ApplicationPath.Equals("/"))
                    url += request.ApplicationPath;
                return url;
            }
        }

        /// <summary>
        /// Gets the default view url
        /// </summary>
        public string DefaultUrl
        {
            get
            {
                return Views.Count() > 0 ? Views.Default.Url : "";
            }
        }

        /// <summary>
        /// Get edit form url by specified content data item.
        /// </summary>
        /// <param name="item">The content data item.</param>
        /// <returns>A string that contains edit item url.</returns>
        public string GetEditItemUrl(ContentDataItem item)
        {
            if (item != null)
                return this.GetEditItemUrl(item.Slug);
            else
                return "";
        }

        /// <summary>
        /// Gets edit data item url by specified data item name.
        /// </summary>
        /// <param name="slug">The data item name.</param>
        /// <returns></returns>
        public string GetEditItemUrl(string slug)
        {
            if (EditForm != null)
                return EditForm.Url(slug);
            else
                return "";
        }

        /// <summary>
        /// Gets new data item url.
        /// </summary>
        /// <returns>A string that contains create data item url.</returns>
        public string GetNewItemUrl()
        {
            if (NewForm != null)
                return NewForm.Url();
            else
                return "";
        }

        /// <summary>
        /// Gets the default title field object.
        /// </summary>
        /// <remarks>
        /// The field named "title" is the default title field. if "title" field not exists will be returns the first TextField as default title field.
        /// </remarks>
        /// <returns></returns>
        public ContentField GetDefaultTitleField()
        {
            var firstField = Fields["title"];

            if (firstField == null)
            {
                var fieldType = (int)ContentFieldTypes.Text;
                firstField = Fields.FirstOrDefault(f => f.FieldType == fieldType);
            }

            return firstField;
        }

        /// <summary>
        /// Gets the default description field object.
        /// </summary>
        /// <remarks>The method will find the field named "description","summary","body" as default description field.</remarks>
        /// <returns></returns>
        public ContentField GetDefaultSummaryField()
        {
            var firstField = Fields["description"];

            if (firstField == null)
                firstField = Fields["summary"];

            if (firstField == null)
                firstField = Fields["body"];

            if (firstField == null)
            {
                var fieldType = (int)ContentFieldTypes.Note;
                firstField = Fields.FirstOrDefault(f => f.FieldType == fieldType);
            }

            return firstField;
        }

        /// <summary>
        /// Gets the default image field object.
        /// </summary>
        /// <remarks>
        /// The method the first image field
        /// </remarks>
        /// <returns></returns>
        public ContentField GetDefaultImageField()
        {
            var firstField = Fields["photo"];

            if (firstField == null)
                firstField = Fields["thumb"];

            if (firstField == null)
                firstField = Fields["image"];

            if (firstField == null)
            {
                var fieldType = (int)ContentFieldTypes.Image;
                firstField = Fields.FirstOrDefault(f => f.FieldType == fieldType);
            }

            return firstField;
        }

        internal Dictionary<string, object> ConvertToDict(NameValueCollection forms)
        {
            var values = new Dictionary<string, object>();
            var fields = Fields;
            foreach (string key in forms.Keys)
            {
                try
                {
                    var field = fields[key];
                    if (field != null)
                    {
                        var strVal = forms[key];
                        if (field.FieldType == (int)ContentFieldTypes.Image)
                        {
                            if (strVal.Length > 2048)
                            {
                                //Save the field value to file
                            }
                        }
                        object val = Convert.ChangeType(strVal, field.SystemType);
                        values.Add(key, val);
                    }
                }
                catch (Exception e)
                {
                    continue;
                    //throw new InvalidCastException("key " + key + " to " + fields[key].SystemType);
                }
            }
            return values;
        }

        //public void CreateListPages()
        //{
        //    #region view pages
        //    //Roles: The default view must be the top view
        //    //The form views must be under the top view
        //    #endregion

        //    #region form pages

        //    #endregion
        //}

        ///// <summary>
        ///// Returns the categories which use in items under this list
        ///// </summary>
        //public IEnumerable<Category> Categories
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //        //var catGroups = Items.Where(i => !string.IsNullOrEmpty(i.Categories)).Select(i => i.Categories);
        //        //var allCatString = string.Join(",", catGroups);
        //        //var unquieCatIDs = allCatString.Split(',').ToArray().Select(a => int.Parse(a)).Distinct();
        //        //return Context.Where<Category>(c => unquieCatIDs.Contains(c.ID));
        //    }
        //}

        /// <summary>
        /// Get the relative url of the list package path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ResolveUrl(string path)
        {
            return this.Package.ResolveUri(path);
        }

        /// <summary>
        /// Export the list settings and data items to xml document
        /// </summary>
        /// <returns></returns>
        //public string ToXml()
        //{
        //    var listElement = XmlSerializerUtility.DeserializeFromXmlText<ContentListElement>(ConfigXml);
        //    listElement.AllowAttachments = this.AllowAttachments;
        //    listElement.AllowCategoriesAndTags = this.AllowCategoriesAndTags;
        //    listElement.AllowComments = this.AllowComments;
        //    listElement.AllowResharing = this.AllowResharing;
        //    listElement.AllowVotes = this.AllowVotes;
        //    listElement.BaseType = this.BaseType;
        //    listElement.DefaultLocale = this.Locale;
        //    listElement.Description = new Xml.LocalizableElement() { Language = this.Locale, Text = this.Description };
        //    listElement.Title = new Xml.LocalizableElement() { Language = this.Locale, Text = this.Title };
        //    listElement.Name = this.Name;
        //    listElement.EnableVersioning = this.EnableVersioning;
        //    listElement.IsActivity = this.IsActivity;
        //    listElement.IsHierarchy = this.IsHierarchy;
        //    listElement.IsModerated = this.IsModerated;
        //    listElement.IsSingle = this.IsSingle;

        //    #region views
        //    //Views
        //    if (this.Views != null)
        //    {
        //        listElement.Views = new List<ContentViewElement>();
        //        foreach (var view in this.Views)
        //        {
        //            var viewEle = new ContentViewElement()
        //            {
        //                AccessRoles = (view.Roles != null && view.Roles.Length > 0) ? string.Join(",", view.Roles) : null,
        //                Title = string.IsNullOrEmpty(view.Title) ? null : new Xml.LocalizableElement() { Text = view.Title, Language = this.Locale },
        //                Description = string.IsNullOrEmpty(view.Description) ? null : new Xml.LocalizableElement() { Text = view.Description, Language = this.Locale },
        //                IsDefault = view.IsDefault,
        //                Icon = view.Icon,
        //                Name = view.Name,
        //                Paging = new PagingElement() { Allow = view.AllowPaging, Index = view.PageIndex, Size = view.PageSize },
        //                Runat = view.IsClientView ? "client" : "server",
        //                HideToolbar = view.HideToolbar,
        //                IsHierarchy = view.IsHierarchy,
        //                RowTemplate = string.IsNullOrEmpty(view.RowTemplate) ? null : new Xml.LocalizableElement() { Text = view.RowTemplate, Language = this.Locale }
        //            };

        //            listElement.Views.Add(viewEle);

        //            //if (!string.IsNullOrEmpty(view.StyleSheet))
        //            //    viewEle.StyleSheet = new ContentResourceElement()
        //            //    {
        //            //        Language = this.Locale
        //            //    };
        //        }
        //    }
        //    #endregion

        //    #region forms
        //    //Forms
        //    if (this.Forms != null)
        //    {
        //        listElement.Forms = new List<ContentFormElement>();
        //        foreach (var form in this.Forms)
        //        {
        //            var formEle = new ContentFormElement()
        //            {
        //                AccessRoles = (form.Roles != null && form.Roles.Length > 0) ? string.Join(",", form.Roles) : "",
        //                Ajax = form.IsAjax,
        //                CaptionField = string.IsNullOrEmpty(form.CaptionField) ? null : form.CaptionField,
        //                Description = string.IsNullOrEmpty(form.Description) ? null : new Xml.LocalizableElement() { Language = this.Locale, Text = form.Description },
        //                Title = string.IsNullOrEmpty(form.Title) ? null : new Xml.LocalizableElement() { Language = this.Locale, Text = form.Title },
        //                FormType = (ContentFormTypes)form.FormType,
        //                HideCaption = form.HideCaption,
        //                ShowAuthor = form.ShowAuthor,
        //                TemplateFile = form.TemplateView
        //            };

        //            listElement.Forms.Add(formEle);
        //        }
        //    }
        //    #endregion

        //    #region data
        //    //Data
        //    var allItems = this.Items;
        //    listElement.Rows = new List<ContentRowElement>();
        //    foreach (var row in allItems)
        //    {
        //        var r = new ContentRowElement()
        //        {
        //            Created = row.Created,
        //            Data = string.Format("<![CDATA[{0}]]>", row.RawData),
        //            ID = row.ID.ToString(),
        //            ParentID = row.ParentID.ToString(),
        //            IsPublished = row.IsPublished,
        //            Language = row.Locale,
        //            ModerateState = row.ModerateState,
        //            Modified = row.Modified,
        //            Pos = row.Pos,
        //            Privacy = row.Privacy,
        //            Published = row.Published,
        //            Slug = row.Slug,
        //            Path = row.Path,
        //            Categories = string.IsNullOrEmpty(row.Categories) ? null : row.Categories,
        //            Tags = string.IsNullOrEmpty(row.Tags) ? null : row.Tags
        //        };

        //        if (row.Attachments != null && row.Attachments.Count > 0)
        //        {
        //            r.Attachs = new List<ContentAttachElement>() { };
        //            foreach (var attach in r.Attachs)
        //            {
        //                r.Attachs.Add(new ContentAttachElement()
        //                {
        //                    Name = attach.Name,
        //                    Extension = attach.Extension,
        //                    Href = attach.Href,
        //                    Privacy = attach.Privacy,
        //                    ContentType = attach.ContentType
        //                });

        //                ///TODO: Convert the data to base64 string and save to xml
        //            }
        //        }


        //        listElement.Rows.Add(r);
        //    }
        //    #endregion

        //    return XmlSerializerUtility.SerializeToXml(listElement);
        //}

        //public XElement ToXElement()
        //{
        //    var listElement = new XElement("contentType",
        //        new XAttribute("xmlns", ContentList.DefaultNamespace),
        //        new XAttribute(ACTIVITY, IsActivity),
        //        new XAttribute(DEFAULT_LOCALE, this.Locale),
        //        new XAttribute(NAME, this.Name),
        //        new XAttribute(BASE, this.BaseType));

        //    if (AllowCategoriesAndTags)
        //        listElement.Add(new XAttribute(ALLOW_CATE_TAGS, this.AllowCategoriesAndTags));

        //    if (IsHierarchy)
        //        listElement.Add(new XAttribute(HIERARCHY, this.IsHierarchy));

        //    if (IsModerated)
        //        listElement.Add(new XAttribute(MODERATED, this.IsModerated));

        //    if (IsSingle)
        //        listElement.Add(new XAttribute(SINGLE, this.IsSingle));

        //    if (AllowResharing)
        //        listElement.Add(new XAttribute(ALLOW_RESHARING, this.AllowResharing));

        //    if (this.EnableVersioning)
        //        listElement.Add(new XAttribute(VERSIONING, this.EnableVersioning));

        //    if (this.AllowAttachments)
        //        listElement.Add(new XAttribute(ALLOW_ATTACHS, this.AllowAttachments));

        //    if (this.AllowComments)
        //        listElement.Add(new XAttribute(ALLOW_COMMENTS, this.AllowComments));

        //    if (this.AllowVotes)
        //        listElement.Add(new XAttribute(ALLOW_VOTES, this.AllowVotes));

        //    var titleElement = new XElement(TITLE);
        //    titleElement.SetValue(this.Title);

        //    var descElement = new XElement(DESC);
        //    descElement.SetValue(this.Description);

        //    listElement.Add(this.Fields.ToXElement());
        //    listElement.Add(this.Views.ToXElement());
        //    listElement.Add(this.Forms.ToXElement());

        //    if (this.TotalItems > 0)
        //    {
        //        var rowsElement = new XElement("rows");
        //        var allItems = this.Items.ToList();
        //        foreach (var item in allItems)
        //        {
        //            var itemDecorator = new ContentDataItemDecorator(item, this.Context);
        //            rowsElement.Add(itemDecorator.ToXElement());
        //        }

        //        listElement.Add(rowsElement);
        //    }

        //    return listElement;
        //}

        /// <summary>
        /// Gets all data rows in xml format
        /// </summary>
        /// <returns></returns>
        public string ExportDataXml()
        {
            var rowsElement = new XElement("rows");
            if (this.TotalItems > 0)
            {
                var allItems = this.Items.ToList();
                foreach (var item in allItems)
                {
                    var itemDecorator = new ContentDataItemDecorator(item, this.Context);
                    rowsElement.Add(itemDecorator.Element());
                }
            }
            return rowsElement.OuterXml();
        }

        /// <summary>
        /// Import data rows from xml text
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public int ImportDataXml(string xml)
        {
            var dataElement = XElement.Parse(xml);
            var rowElements = dataElement.Descendants("row");
            foreach (var row in rowElements)
                Context.Add(new ContentDataItemDecorator(row, this, Context).Model);
            return Context.SaveChanges();
        }

        /// <summary>
        /// Identity the field whether exists by specified field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns></returns>
        public bool HasField(string fieldName)
        {
            return Fields[fieldName] != null;
        }

        /// <summary>
        /// Create new data item by specified xml element.
        /// </summary>
        /// <param name="element">The XElement that contains data item.</param>
        /// <returns></returns>
        public ContentDataItemDecorator New(XElement element)
        {
            var item = new ContentDataItemDecorator(element, this, Context);
            item.Save();
            //ClearCache();
            return item;
        }

        public XElement ElementWithBase64Data()
        {
            var listElement = base.Element();
            if (listElement.Element("views") == null)
                listElement.Add(Views.Element());
            if (listElement.Element("forms") == null)
                listElement.Add(Forms.Element());
            if (listElement.Element("rows") == null && this.TotalItems > 0)
            {
                XNamespace ns = ContentList.DefaultNamespace;
                var items = Items.Where(c => c.IsCurrentVersion).ToList();
                listElement.Add(new XElement(ns + "rows", items.Select(i => i.Element(true))));
            }
            return listElement;
        }

        public override XElement Element()
        {
            var listElement = base.Element();
            if (listElement.Element("views") == null)
                listElement.Add(Views.Element());
            if (listElement.Element("forms") == null)
                listElement.Add(Forms.Element());
            if (listElement.Element("rows") == null && this.TotalItems > 0)
            {
                XNamespace ns = ContentList.DefaultNamespace;
                var items = Items.Where(c => c.IsCurrentVersion).ToList();
                listElement.Add(new XElement(ns + "rows", items.Select(i => i.Element())));
            }
            return listElement;
        }

        /// <summary>
        /// Convert the list to xml string.
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            return this.Element().OuterXml();
        }

        /// <summary>
        /// Save the content list to specified xml file.
        /// </summary>
        /// <param name="file">The xml file name.</param>
        public void Save(string file)
        {
            this.Element().Save(file);
        }

        /// <summary>
        /// Gets the unique view name for current list.
        /// </summary>
        /// <returns></returns>
        public string GenerateViewName()
        {
            var names = new List<string>();
            var newName = "view1";
            if (Views != null)
            {
                names = Views.Select(v => v.Name).ToList();
                var slug = newName;
                var flex = slug;
                var j = 0;
                while (names.Contains(slug))
                    slug = flex + (++j).ToString();
                newName = slug;
            }
            return newName;
        }

        /// <summary>
        /// Gets the access roles of the list.
        /// </summary>
        public new string[] Roles
        {
            get
            {
                if (!string.IsNullOrEmpty(base.Roles))
                {
                    return base.Roles.Split(',');
                }
                return new string[0];
            }
        }

        ///// <summary>
        ///// Identitify the specified user is allow access current list.
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public bool IsInRoles(DNA.Web.User user) 
        //{ 

        //}

        /// <summary>
        /// Create a new view of current list.
        /// </summary>
        /// <param name="name">The view name</param>
        /// <param name="title">The view title.</param>
        /// <param name="allowPaging">Specified the view allow paging.</param>
        /// <param name="defaultPageSize">The paging size of the view.</param>
        /// <param name="isDefault">Set the view to default.</param>
        /// <param name="viewFields">The view field reference names.</param>
        /// <param name="sort">The sort expression</param>
        /// <param name="filter">The filter expression.</param>
        /// <returns>A new content view object.</returns>
        public ContentViewDecorator CreateView(string name, string title, bool allowPaging = true, int defaultPageSize = 20, bool isDefault = false, string[] viewFields = null, string sort = "", string filter = "")
        {
            var view = new ContentView()
            {
                ParentID = this.ID,
                Parent = this.Model,
                AllowPaging = allowPaging,
                PageSize = defaultPageSize,
                IsDefault = isDefault,
                Title = title,
                Filter = filter,
                Sort = sort,
                Name = name
            };

            var names = new List<string>();

            foreach (var v in Views)
            {
                var slug = v.Name;
                var flex = slug;
                var j = 0;
                while (names.Contains(slug))
                    slug = flex + (++j).ToString();
                names.Add(slug);

                if (v.Name != slug)
                    v.Name = slug;
            }

            if (viewFields == null || viewFields.Count() == 0)
                viewFields = this.Fields.Select(f => f.Name).ToArray();

            var element = new XElement("fields");
            foreach (var vf in viewFields)
                element.Add(new XElement("fieldRef", new XAttribute("name", vf)));
            view.FieldRefsXml = element.OuterXml();


            if (isDefault)
            {
                foreach (var v in this.Model.Views)
                    v.IsDefault = false;
            }

            Context.Add(view);
            Context.SaveChanges();

            return new ContentViewDecorator(view, Context);
        }



    }
}
