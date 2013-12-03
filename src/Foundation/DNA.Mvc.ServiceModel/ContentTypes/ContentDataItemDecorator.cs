//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data.Schema;
using DNA.Web.Events;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Xml.Linq;


namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to ContentDataItem object model. 
    /// </summary>
    public class ContentDataItemDecorator : ContentDataItem
    {
        #region Private variables

        private Dictionary<string, object> values;
        private CommentCollection comments = null;
        private IEnumerable<Vote> votes = null;
        private ContentDataItemDecorator reshareFrom = null;
        private ICollection<ContentAttachment> attachs = null;
        private bool flag = false;

        #endregion

        internal ContentDataItem Model { get; set; }

        internal IDataContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the ContentDataItemDecorator class 
        /// </summary>
        /// <remarks>
        /// This constructor only use for testing.
        /// </remarks>
        public ContentDataItemDecorator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ContentDataItemDecorator class with data item object and data context.
        /// </summary>
        /// <param name="model">The ContentDataitem object.</param>
        /// <param name="context">The data context.</param>
        public ContentDataItemDecorator(ContentDataItem model, IDataContext context)
        {
            Model = model;
            model.CopyTo(this, "ContentList", "Tags", "Attachments");
            Context = context;
            this.Parent = new ContentListDecorator(context, model.Parent);
        }

        /// <summary>
        /// Initializes a new instance of the ContentDataItemDecorator class with xml data item element, content list object and data context.
        /// </summary>
        /// <param name="element">The XElement that contains dataitem object data.</param>
        /// <param name="parent">The parent list decorator object.</param>
        /// <param name="context">The data context.</param>
        public ContentDataItemDecorator(XElement element, ContentListDecorator parent, IDataContext context)
        {
            this.Model = new ContentDataItem() { ID = Guid.NewGuid(), ParentID = parent.ID };
            this.Parent = parent;
            Context = context;
            this.Load(element);
            flag = true;
        }

        /// <summary>
        /// Gets the absoulte url to display data item.
        /// </summary>
        public virtual string UrlComponent
        {
            get
            {
                var request = HttpContext.Current.Request;
                string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath;
                var d = this.Published.HasValue ? this.Published.Value : this.Created;
                var year = d.ToString("yyyy");
                var month = d.ToString("MM");
                var days = d.ToString("dd");
                var vers = this.Version.ToString();
                var uri = new Uri(string.Format("{0}{1}/{2}/{3}/{4}/{5}/{6}/{7}.html", baseUrl, this.Parent.Web.Name, this.Parent.Locale, this.Parent.Name, year, month, days, this.Slug));
                return uri.GetComponents(UriComponents.Path | UriComponents.SchemeAndServer, UriFormat.Unescaped);
            }
        }

        /// <summary>
        ///  Gets the relative url to display data item.
        /// </summary>
        public string Url
        {
            get
            {
                var d = this.Published.HasValue ? this.Published.Value : this.Created;
                var year = d.ToString("yyyy");
                var month = d.ToString("MM");
                var days = d.ToString("dd");
                var vers = this.Version.ToString();

                return string.Format("~/{0}/{1}/{2}/{3}/{4}/{5}/{6}.html", this.Parent.Web.Name, this.Parent.Locale, this.Parent.Name, year, month, days, this.Slug);

                //return string.Format("~/{0}/{1}/lists/{2}/items/{3}.html", this.Parent.Web.Name, this.Parent.Locale, this.Parent.Name, this.Slug);
            }
        }

        /// <summary>
        /// Get the raw values of this item
        /// </summary>
        public Dictionary<string, object> Values
        {
            get
            {
                if (this.values == null)
                {
                    var fields = Parent.Fields;
                    values = new Dictionary<string, object>();
                    var xobject = XElement.Parse(this.RawData);
                    if (xobject.HasElements)
                    {
                        var elements = xobject.Elements();
                        foreach (var xe in elements)
                        {
                            var field = fields[xe.Name.LocalName];
                            if (field != null)
                            {
                                var val = Convert.ChangeType(xe.Value, field.SystemType);
                                values.Add(xe.Name.LocalName, val);
                            }
                        }
                    }

                    //using (var reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(this.RawData))))
                    //{
                    //    reader.ReadStartElement(Parent.Name);
                    //    while (!reader.EOF && reader.NodeType != XmlNodeType.EndElement)
                    //    {
                    //        var fieldName = reader.Name;
                    //        var field = fields.FirstOrDefault(f => f.Name.Equals(fieldName));
                    //        if (field == null)
                    //        {
                    //            reader.Read();
                    //            continue;
                    //        }

                    //        try
                    //        {

                    //            var val = reader.ReadElementContentAs(field.SystemType, null);
                    //            this.values.Add(fieldName, val);
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            throw new XmlException(string.Format("Read Xml data error : Can not convert the \"{0}\" field from XML value to {1}", fieldName, field.SystemType.ToString()), e);
                    //        }
                    //    }
                    //}

                }
                return this.values;
            }
        }

        #region 3.0.3 added

        private dynamic model = null;

        /// <summary>
        /// Gets the expando object for data item value model.
        /// </summary>
        public dynamic DataObject
        {
            get
            {
                if (model == null)
                {
                    model = new ExpandoObject();
                    var vals = Values;
                    foreach (var key in vals.Keys)
                    {
                        ((IDictionary<String, Object>)model).Add(key, vals[key]);
                    }
                }
                return model;
            }
        }

        /// <summary>
        /// Gets the parent list name.
        /// </summary>
        public string ListName
        {
            get { return Parent.Name; }
        }

        #endregion

        /// <summary>
        /// Gets the content field value object by specified field name.
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <returns>A content field value object</returns>
        public ContentFieldValue this[string fieldName]
        {
            get
            {
                return this.Value(fieldName);
            }
        }

        /// <summary>
        /// Gets the content field value object by specified field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>A content field value object</returns>
        public ContentFieldValue Value(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("fieldName");
            var field = Parent.Fields[fieldName];
            if (field == null)
                return null;

            var fieldValue = new ContentFieldValue()
            {
                ParentList = this.Parent
            };

            if (this.Values.ContainsKey(fieldName))
            {
                if (this.Values[fieldName] != null)
                {
                    fieldValue.Raw = Convert.ChangeType(this.Values[fieldName], field.SystemType);
                }
                else
                {
                    if (field.DefaultValue != null)
                        fieldValue.Raw = field.DefaultValue;
                }
            }

            fieldValue.Field = this.Parent.Fields[fieldName];
            fieldValue.Item = this;
            return fieldValue;
        }

        /// <summary>
        /// Gets the content field collection.
        /// </summary>
        public ContentFieldCollection Fields { get { return this.Parent.Fields; } }

        /// <summary>
        /// Gets the comment colleciton object for this content item.
        /// </summary>
        public CommentCollection Comments
        {
            get
            {
                if (comments == null)
                    comments = new CommentCollection(Context, this.UrlComponent);
                return comments;
            }
        }

        /// <summary>
        /// Gets the total comments.
        /// </summary>
        public int TotalComments
        {
            get
            {
                try
                {
                    return this.Comments.Count();
                }
                catch (Exception e)
                {
                    //Where using MySQL (MAS not avalidable here will cause "There is already an open DataReader associated with this Command which must be closed first" exception)
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the tags 
        /// </summary>
        public new string[] Tags
        {
            get
            {
                if (string.IsNullOrEmpty(Model.Tags)) return new string[0];
                return Model.Tags.Split(',');
            }
        }

        /// <summary>
        /// Gets the default title value.
        /// </summary>
        /// <remarks>
        /// This method will returns the "title" field value or first text field value in row data value.
        /// </remarks>
        /// <returns>A string contains title text value.</returns>
        public string GetDefaultTitleValue()
        {
            var defField = this.Parent.GetDefaultTitleField();
            if (defField != null)
                return this.Value(defField.Name).Formatted;
            return "";
        }

        /// <summary>
        /// Gets the default description value.
        /// </summary>
        /// <returns></returns>
        public string GetDefaultDescription()
        {
            var defField = this.Parent.GetDefaultSummaryField();
            if (defField != null)
                return this.Value(defField.Name).Formatted;
            return "";
        }

        /// <summary>
        /// Gets the default image url.
        /// </summary>
        /// <remarks></remarks>
        /// <returns></returns>
        public string GetDefaultImageUrl()
        {
            var defField = this.Parent.GetDefaultImageField();
            if (defField != null)
                return (string)this.Value(defField.Name).Raw;
            return "";
        }

        /// <summary>
        /// Vote the content data item by specified user name and value.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="value">The vote value.</param>
        /// <returns>A double value of average votes.</returns>
        public double Vote(string userName, int value)
        {
            if (this.CanVote(userName))
            {
                votes = null;
                Context.Add(new Vote()
                {
                    ObjectID = this.ID.ToString(),
                    UserName = userName,
                    Value = value
                });
                Context.SaveChanges();
                this.Ratings = AverageVotes;
                Model.Ratings = this.Ratings;
                Model.TotalVotes = Votes.Count();
                this.TotalVotes = Model.TotalVotes;
                Context.Update(Model);
                Context.SaveChanges();

                //EventDispatcher.RaiseContentDataItemVoted(this, userName, value, AverageVotes);
                this.Trigger("ContentDataItemVoted", new ContentDataItemVotedEventArgs() { List = this.Parent, DataItem = this, Average = AverageVotes, UserName = userName, Votes = value });

            }
            return AverageVotes;
        }

        /// <summary>
        /// Identity the content data item whether can vote.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <returns>If can vote return true.</returns>
        public bool CanVote(string userName)
        {
            if (this.Parent.AllowVotes)
                return Votes.FirstOrDefault(v => v.UserName.Equals(userName)) == null;
            return false;
        }

        /// <summary>
        /// Gets the average votes value.
        /// </summary>
        public double AverageVotes
        {
            get
            {
                if (TotalVotes > 0)
                    return Votes.Average(v => v.Value);
                return 0;
            }
        }

        /// <summary>
        /// Gets the vote collection.
        /// </summary>
        public IEnumerable<Vote> Votes
        {
            get
            {
                if (votes == null)
                {
                    var id = this.ID.ToString();
                    votes = Context.Where<Vote>(v => v.ObjectID.Equals(id)).ToList();
                }
                return votes;
            }
        }

        /// <summary>
        /// Set categories string .
        /// </summary>
        /// <param name="categoryIdList"></param>
        public void SetCategories(string categoryIdList)
        {
            if (this.Model.Categories != null)
            {
                this.Model.Categories.Clear();
                this.PrimaryCategoryID = 0;
                this.Model.PrimaryCategoryID = 0;
                Context.SaveChanges();
            }

            if (!string.IsNullOrEmpty(categoryIdList))
            {
                var cats = categoryIdList.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
                var catLists = this.Context.Where<Category>(c => cats.Contains(c.ID)).ToList();
                this.Model.Categories = catLists;
                this.Model.PrimaryCategoryID = cats.FirstOrDefault();
                this.PrimaryCategoryID = this.Model.PrimaryCategoryID;
                Context.SaveChanges();
            }
            this.Categories = Model.Categories;
        }

        /// <summary>
        /// Set tags string.
        /// </summary>
        /// <param name="tagList"></param>
        public void SetTags(string tagList)
        {
            this.Model.Tags = tagList;
        }

        /// <summary>
        /// Get the formatted field value by specified field name
        /// </summary>
        /// <remarks>If the field not found this the method will return a empty string </remarks>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetFormattedValue(string fieldName)
        {
            var parentField = this.Fields[fieldName];
            if (parentField != null)
                return parentField.Format(this.Values[fieldName]);
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetModel<T>() where T : class
        {
            return (T)this.GetModel(typeof(T));
        }

        private object GetModelInstance(Type type)
        {
            //if (type.IsInterface) {
            //    var pb = new DefaultProxyBuilder();
            //    var proxyType=pb.CreateInterfaceProxyTypeWithoutTarget(type, null, ProxyGenerationOptions.Default);
            //    return Activator.CreateInstance(type);
            //}
            //else
            //{
            return Activator.CreateInstance(type);
            //}
        }

        private object GetModel(Type type, string prefix = "")
        {
            var typeObject = GetModelInstance(type);
            var props = type.GetProperties().Where(p => !p.IsDefined(typeof(IgnoreAttribute), true) && p.CanWrite);
            var fields = this.Parent.Fields;

            foreach (var pro in props)
            {
                try
                {
                    var fieldName = prefix + pro.Name;
                    if (pro.IsDefined(typeof(GroupAttribute), true))
                    {
                        var groupAttrs = pro.GetCustomAttributes(typeof(GroupAttribute), true);
                        GroupAttribute groupAttr = groupAttrs != null ? (GroupAttribute)groupAttrs.FirstOrDefault() : null;
                        var fieldNamePrefix = !string.IsNullOrEmpty(groupAttr.Name) ? groupAttr.Name : pro.Name;
                        var propModel = GetModel(pro.PropertyType, fieldNamePrefix);
                        pro.SetValue(typeObject, propModel, null);
                    }
                    else
                    {
                        var fieldAttrs = pro.GetCustomAttributes(typeof(FieldAttribute), true);
                        FieldAttribute fieldAttr = fieldAttrs != null ? (FieldAttribute)fieldAttrs.FirstOrDefault() : null;

                        if (fieldAttr != null && !string.IsNullOrEmpty(fieldAttr.Name))
                        {
                            fieldName = prefix + fieldAttr.Name;
                            if (fields[fieldName] == null)
                                fieldName = prefix + pro.Name;
                            else
                                fieldName = fields[fieldName].Name;
                        }

                        var field = fields[fieldName];
                        if (field != null)
                        {
                            var fieldVal = Value(fieldName);
                            object val = fieldVal.IsNull ? fieldVal.Default : fieldVal.Raw;
                            if (pro.PropertyType.IsEnum)
                                val = Enum.Parse(pro.PropertyType, val.ToString());
                            pro.SetValue(typeObject, val, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return typeObject;
        }

        /// <summary>
        /// Gets current data item is a new object (published in 3 days.)
        /// </summary>
        public bool IsNew
        {
            get
            {
                if (this.IsPublished)
                {
                    if (this.Published.HasValue)
                        return (DateTime.Now - this.Published.Value).Days <= 3;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets whether this item has attachements
        /// </summary>
        public bool HasAttachments
        {
            get
            {
                return Context.Count<ContentAttachment>(a => a.ItemID.Equals(this.ID)) > 0;
            }
        }

        /// <summary>
        /// Gets attachment collection.
        /// </summary>
        public override ICollection<ContentAttachment> Attachments
        {
            get
            {
                if (attachs == null)
                    attachs = Context.Where<ContentAttachment>(a => a.ItemID == this.ID).ToList();

                return attachs;
            }
            set
            {
                attachs = value;
            }
        }

        /// <summary>
        /// Gets parent content list.
        /// </summary>
        public virtual new ContentListDecorator Parent { get; private set; }

        /// <summary>
        /// Disable or enable comments on display form and activity form.
        /// </summary>
        /// <param name="value"></param>
        public void DisableComments(bool value = true)
        {
            Context.ContentDataItems.DisableComments(this.ID, value);
        }

        /// <summary>
        /// Approve the content data item by specified auditor name.
        /// </summary>
        /// <param name="auditor">The auditor user name.</param>
        public void Approve(string auditor) { Audit(auditor, ModerateStates.Approved); }

        /// <summary>
        /// Reject the content data item by specified auditor name.
        /// </summary>
        /// <param name="auditor">The auditor user name.</param>
        public void Reject(string auditor) { Audit(auditor, ModerateStates.Rejected); }

        /// <summary>
        /// Audit the content data item by specified state
        /// </summary>
        /// <param name="auditor">The audior user name.</param>
        /// <param name="state">The moderate states.</param>
        private void Audit(string auditor, ModerateStates state)
        {
            Context.ContentDataItems.Audit(this.ID, auditor, (int)state);
        }

        /// <summary>
        /// Rollback the previous version.
        /// </summary>
        public void Rollback()
        {
            if (!IsCurrentVersion)
            {
                var currentVer = Context.Find<ContentDataItem>(this.RefID);
                this.Model.CopyTo(currentVer, "ID", "RefID", "Attachments");
                currentVer.IsCurrentVersion = true;
                Context.Delete(this.Model);
                Context.SaveChanges();
                Context.Delete<ContentDataItem>(c => !c.IsCurrentVersion && c.Version > currentVer.Version && c.RefID.Equals(currentVer.ID));
                Context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the version content data items.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ContentDataItemDecorator> Versions()
        {
            var refId = this.ID;
            if (this.RefID != Guid.Empty)
                refId = this.RefID;
            var items = Context.Where<ContentDataItem>(i => i.RefID.Equals(refId) || i.ID.Equals(refId)).OrderByDescending(i => i.Version).ToList();
            return items.Select(i => new ContentDataItemDecorator(i, Context)).ToList();
        }

        /// <summary>
        /// Remove the attachement files by specified attachment ids.
        /// </summary>
        /// <param name="attachmentIDs">The attachment id array.</param>
        /// <param name="service">The INetDriveService object.</param>
        public void DetachFiles(int[] attachmentIDs, INetDriveService service)
        {
            if (attachmentIDs == null)
                throw new ArgumentNullException("attachmentIDs");

            if (service == null)
                throw new ArgumentNullException("service");

            var attachs = Context.Where<ContentAttachment>(c => attachmentIDs.Contains(c.ID));
            foreach (var attach in attachs)
            {
                service.Delete(new Uri(attach.Uri));
                Context.Delete(attach);
            }

            Context.SaveChanges();
        }

        /// <summary>
        /// Add attachment files.
        /// </summary>
        /// <param name="files">The http file collection.</param>
        /// <param name="service">The INetDriveService object.</param>
        public void AttachFiles(HttpFileCollectionBase files, INetDriveService service)
        {
            if (files.Count == 0) return;
            if (service == null)
                throw new ArgumentNullException("service");

            var attchUri = Parent.AttachmentsPath;
            if (!service.Exists(attchUri))
                service.CreatePath(attchUri);

            var itemUri = new Uri(attchUri.ToString() + "/" + this.ID.ToString());
            if (!service.Exists(itemUri))
                service.CreatePath(itemUri);

            var itemPath = service.MapPath(itemUri);

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(file.FileName);
                    file.SaveAs(itemPath + (!itemPath.EndsWith("\\") ? "\\" : "") + fileName);
                    var webFile = new WebResourceInfo(itemUri.ToString() + (!itemPath.EndsWith("\\") ? "\\" : "") + fileName);
                    var attach = new ContentAttachment()
                    {
                        ContentType = webFile.ContentType,
                        Extension = webFile.Extension,
                        ItemID = this.ID,
                        Size = file.ContentLength,
                        Name = fileName,
                        Uri = webFile.Url.ToString()
                    };
                    Context.Add(attach);
                }
            }
            Context.SaveChanges();

            Model.TotalAttachments = Context.Count<ContentAttachment>(c => c.ItemID.Equals(this.ID));
            this.TotalAttachments = Model.TotalAttachments;

            Context.SaveChanges();
        }

        public ContentDataItemDecorator ReshareTo(ContentList to = null, string annotation = "")
        {
            if (this.Parent.AllowResharing)
            {
                var list = to == null ? this.Parent : to;
                if (to != null)
                {
                    if (!to.BaseType.Equals(this.Parent.BaseType))
                        throw new Exception("The data item only allow share between same type list.");
                }

                var reshareItem = new ContentDataItem();
                Model.CopyTo(reshareItem, "ID", "RefID", "ShareID", "ParentItemID", "ParentID", "TotalAttachments", "TotalVotes", "ContentList", "Attachments");
                reshareItem.ID = Guid.NewGuid();
                reshareItem.ShareID = this.ID;
                reshareItem.Modified = DateTime.Now;
                reshareItem.Modifier = list.Owner;
                reshareItem.Annotation = annotation;

                //reshareItem.Published = DateTime.Now;
                //reshareItem.Created = DateTime.Now;

                reshareItem.Ratings = 0;
                reshareItem.Reads = 0;
                reshareItem.IsCurrentVersion = true;
                reshareItem.ModerateState = (int)ModerateStates.Approved;
                reshareItem.ParentID = list.ID;
                reshareItem.Parent = list;

                reshareItem.Path = reshareItem.ID.ToString();

                //reshareItem.Owner = list.Owner;

                Context.Add(reshareItem);
                Context.SaveChanges();
                return new ContentDataItemDecorator(reshareItem, Context);
            }

            throw new ResharingNotAllowException();
        }

        public IQueryable<UserProfile> GetResharePeoples()
        {
            if (this.Parent.AllowResharing)
            {
                var peopleNames = Reshares().Select(r => r.Owner).Distinct().ToArray();
                return Context.Where<UserProfile>(p => peopleNames.Contains(p.UserName));
            }
            return null;
        }

        public ContentDataItemDecorator ReshareForm
        {
            get
            {
                if (this.ShareID != Guid.Empty)
                {
                    if (reshareFrom == null)
                    {
                        var ri = Context.Find<ContentDataItem>(this.ShareID);
                        if (ri != null)
                        {
                            reshareFrom = new ContentDataItemDecorator(ri, Context);
                        }
                    }
                    return reshareFrom;
                }
                return null;
            }
        }

        /// <summary>
        /// Set raw data to content data item and  save changes.
        /// </summary>
        /// <param name="data">The data object that has the schema same as list fields</param>
        /// <example>
        /// <para>This example demonstrate how update the raw data value in content data item.</para>
        /// <code language="cs">
        /// var list=App.Get().CurrentWeb.Lists["products"];
        /// var item=list.GetItem("iphone");
        /// item.Save(new {
        ///   code="po-iphone-5",
        ///   price=350.99
        /// });
        /// </code>
        /// </example>
        /// <returns>If success returns true.</returns>
        public bool Save(object data)
        {
            var forms = data as NameValueCollection;
            this.values = null;

            this.CopyTo(Model, "ContentList", "Attachments", "Categories", "Tags");

            if (forms != null)
                Context.ContentDataItems.Update(Model, Parent.ConvertToDict(forms));
            else
                Context.ContentDataItems.Update(Model, Parent.MapTypedData(data));

            var count = Context.SaveChanges();
            Model.CopyTo(this, "ContentList", "Attachments", "Categories", "Tags");

            //this.Parent.ClearCache();

            //EventDispatcher.RaiseContentDataItemUpdated(this);
            this.Trigger("ContentDataItemUpdated", new ContentDataItemEventArgs() { List = this.Parent, DataItem = this });
            return count > 0;
        }

        /// <summary>
        /// Save changes to database.
        /// </summary>
        /// <returns>If success returns true.</returns>
        public bool Save()
        {
            if (flag)
            {
                flag = false;
                Context.Add(Model);
                Model.CopyTo(this, "ContentList", "Tags", "Categories");
                var saved = this.Context.SaveChanges() > 0;
                //EventDispatcher.RaiseContentDataItemUpdated(this);
                this.Trigger(EventNames.ContentListCreated, new ContentDataItemEventArgs() { List = this.Parent, DataItem = this });
                //this.Parent.ClearCache();
                return saved;
            }
            else
            {
                this.values = null;
                this.CopyTo(Model, "ContentList", "Attachments", "Categories", "Tags");
                this.Context.ContentDataItems.Update(Model);
                var result = this.Context.SaveChanges() > 0;
                //this.Parent.ClearCache();
                //EventDispatcher.RaiseContentDataItemUpdated(this);
                this.Trigger(EventNames.ContentDataItemUpdated, new ContentDataItemEventArgs() { List = this.Parent, DataItem = this });
                return result;
            }
        }

        /// <summary>
        /// Increase read count,
        /// </summary>
        public void Read()
        {
            Context.ContentDataItems.Read(this.ID);
        }

        /// <summary>
        /// Move the content data item to other parent item and new position.
        /// </summary>
        /// <param name="parentID">The parent item id.</param>
        /// <param name="pos">The item position.</param>
        public void MoveTo(Guid parentID, int pos)
        {
            if (parentID == this.ID)
                throw new Exception("Can not move the item under itself.");

            Context.ContentDataItems.Move(parentID, this.ID, pos);
            Context.SaveChanges();

            this.Pos = pos;
            this.ParentItemID = parentID;
            //Parent.ClearCache();
            this.Trigger("ContentDataItemUpdated", new ContentDataItemEventArgs() { List = this.Parent, DataItem = this });
        }

        /// <summary>
        /// Convert the content data item to dynamic object.
        /// </summary>
        /// <returns>An expando object defined in open activity object standard.</returns>
        public dynamic ToObject()
        {
            dynamic activity = new ExpandoObject();
            var vals = this.Values;
            var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
            var request = HttpContext.Current.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority;

            dynamic actor = new ExpandoObject();

            #region base properties
            actor.id = this.Modifier;
            actor.url = baseUrl + "/profiles/" + this.Modifier;
            actor.displayName = this.Modifier;

            activity.id = this.ID;
            activity.parentId = this.ParentItemID;
            activity.published = this.Published;
            activity.updated = this.Modified;
            activity.version = this.Version;
            //activity.tags = this.Model.Tags;
            activity.slug = this.Model.Slug;
            activity.locale = this.Locale;
            //activity.categories = !string.IsNullOrEmpty(this.Model.Categories) ? this.Model.Categories.Split(',') : null;
            activity.tags = !string.IsNullOrEmpty(this.Model.Tags) ? this.Model.Tags.Split(',') : null;
            activity.pos = this.Pos;
            activity.reads = this.Reads;
            activity.actor = actor;
            activity.annotation = this.Annotation;
            activity.verb = this.ShareID != Guid.Empty ? "share" : "post";
            activity.url = this.UrlComponent;

            #endregion

            #region object
            dynamic _obj = new ExpandoObject();
            dynamic owner = new ExpandoObject();

            owner.id = this.Owner;
            owner.url = baseUrl + "/profiles/" + this.Owner;
            owner.displayName = this.Owner;
            _obj.actor = owner;

            var j = 0;
            var title = "";

            foreach (var key in Values.Keys)
            {
                if (j == 0)
                {
                    title = Values[key].ToString();
                    j++;
                }
                ((IDictionary<String, Object>)_obj).Add(key, Values[key]);
            }
            #endregion

            var objUrl = UrlComponent.ToString();
            _obj.objectType = this.Parent.BaseType;
            _obj.objectTypeID = this.ParentID;
            _obj.id = ShareID != Guid.Empty ? ShareID.ToString() : this.ID.ToString();

            ContentList originalList = null;
            ContentDataItem originalItem = null;

            if (ShareID != Guid.Empty)
            {
                originalItem = Context.Find<ContentDataItem>(this.ShareID);
                originalList = originalItem.ParentID.Equals(this.ParentID) ? this.Parent.Model : Context.Find<ContentList>(originalItem.ParentID);
                var originalWeb = originalItem.ParentID.Equals(this.ParentID) ? this.Parent.Web.Model : Context.Find<Web>(originalList.WebID);
                objUrl = (new Uri(string.Format("{0}/{1}/lists/{2}/items/{3}.html", baseUrl, originalWeb.Name, originalList.Name, originalItem.Slug))).ToString();
            }

            _obj.url = objUrl;

            #region additional properties
            activity.links = new
            {
                //view = this.UrlComponent,
                edit = Url.Content(this.Parent.GetEditItemUrl(this)),
                del = Url.Content("~/api/contents/deleteItem/" + this.ParentID.ToString())
            };

            activity.title = title;

            #endregion

            #region categories
            if (Categories != null && Categories.Count() > 0)
            {
                activity.categories = Categories.Select(c => new
                {
                    id = c.ID,
                    parentId = c.ParentID,
                    name = c.Name,
                    desc = c.Description
                });
            }
            //dynamic categories = new ExpandoObject();
            #endregion

            #region comments
            dynamic replies = new ExpandoObject();
            replies.totalItems = ShareID != Guid.Empty ? Context.Count<Comment>(c => c.TargetUri.Equals(objUrl, StringComparison.OrdinalIgnoreCase)) : this.TotalComments;
            replies.disabled = originalItem != null ? !originalItem.EnableComments : !this.Model.EnableComments;
            replies.link = Url.Content(string.Format("~/api/comments?url={0}", objUrl));
            #endregion

            #region ratings
            dynamic ratings = new ExpandoObject();
            ratings.totalItems = originalItem != null ? originalItem.TotalVotes : this.TotalVotes;
            ratings.value = originalItem != null ? originalItem.Ratings : this.Ratings;
            ratings.disabled = originalList != null ? !originalList.AllowVotes : !this.Parent.AllowVotes;

            if (originalList != null)
                ratings.url = Url.Content(string.Format("~/api/contents/vote/{0}?list={1}", ShareID.ToString(), originalList.Name));
            else
            {
                if (this.Parent != null)
                    ratings.url = Url.Content(string.Format("~/api/contents/vote/{0}?list={1}", this.ID.ToString(), this.Parent.Name));
            }
            #endregion

            #region attachments

            dynamic attachs = new ExpandoObject();
            attachs.totalItems = originalItem != null ? originalItem.TotalAttachments : this.TotalAttachments;
            attachs.disabled = originalList != null ? !originalList.AllowAttachments : !this.Parent.AllowAttachments;

            if (originalItem != null)
            {
                attachs.items = Context.Where<ContentAttachment>(c => c.ItemID.Equals(originalItem.ID)).ToList().Select(attach => new
                {
                    type = attach.ContentType,
                    url = attach.Uri,
                    name = attach.Name,
                    id = attach.ID,
                    size = attach.Size
                }).ToArray();
            }
            else
            {
                if (this.Parent != null && this.Attachments != null)
                {
                    if (this.Parent.AllowAttachments)
                    {
                        attachs.items = Attachments.Select(attach => new
                        {
                            type = attach.ContentType,
                            url = attach.Uri,
                            name = attach.Name,
                            id = attach.ID,
                            size = attach.Size
                        }).ToList().ToArray();
                    }
                    else
                    {
                        attachs.items = new object[0];
                    }
                }
            }
            _obj.attachments = attachs;

            #endregion

            dynamic reshares = new ExpandoObject();
            reshares.totalItems = 0;
            reshares.link = Url.Content("~/api/contents/shares/" + (ShareID != Guid.Empty ? this.ShareID.ToString() : this.ID.ToString()));
            reshares.disabled = !this.Parent.AllowResharing;

            if (ShareID != Guid.Empty)
                reshares.totalItems = Context.Count<ContentDataItem>(i => i.ShareID == this.ShareID);
            else
                if (this.Parent != null && this.Parent.AllowResharing)
                    reshares.totalItems = Reshares().Count();


            _obj.reshares = reshares;
            _obj.replies = replies;
            _obj.ratings = ratings;

            activity.@object = _obj;

            return activity;
        }

        /// <summary>
        /// Gets the children content data items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<ContentDataItem> Children()
        {
            return Context.Where<ContentDataItem>(i => i.ParentItemID == this.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IQueryable<ContentDataItem> Reshares()
        {
            return Context.Where<ContentDataItem>(i => i.ShareID == this.ID);
        }

        /// <summary>
        /// Gets descendant data items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<ContentDataItem> Descendant()
        {
            return Context.Where<ContentDataItem>(i => i.Path.StartsWith(this.Path) && i.ID != this.ID);
        }

        /// <summary>
        /// Get parent content data items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<ContentDataItem> Parents()
        {
            if (this.ParentID > 0)
            {
                var idsFromPath = this.Path.Split('/').Where(p => !string.IsNullOrEmpty(p)).Select(c => new Guid(c)).ToArray();
                return Context.ContentDataItems.Filter(p => idsFromPath.Contains(p.ID) && p.ID != this.ID);
            }
            return null;
        }

        /// <summary>
        /// Gets the sibling content data items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<ContentDataItem> Siblings()
        {
            if (this.ParentItemID != Guid.Empty)
                return Context.ContentDataItems.Filter(p => p.Path.Equals(this.Path));
            else
                return Context.ContentDataItems.Filter(p => p.ParentID == this.ParentID && this.IsPublished);
        }

        /// <summary>
        /// Identity the current user whether the data content item is owner.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns></returns>
        public bool IsOwner(HttpContextBase context)
        {
            if (context.Request.IsAuthenticated)
                return this.IsOwner(context.User.Identity.Name);
            return false;
        }

        /// <summary>
        /// Identity the current user whether the data content item is owner.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <returns></returns>
        public bool IsOwner(string userName)
        {
            return userName.Equals(this.Owner);
        }

        #region 3.0.3 added for detail view

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
            var detailList = this.Parent.Web.Lists[detailListName];
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

        #endregion
    }
}
