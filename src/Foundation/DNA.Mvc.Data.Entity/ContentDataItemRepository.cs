//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DNA.Utility;

namespace DNA.Web.Data.Entity
{
    public class ContentDataItemRepository : EntityRepository<ContentDataItem>, IContentDataItemRepository
    {
        public ContentDataItemRepository() : base() { }

        public ContentDataItemRepository(CoreDbContext dbContext) : base(dbContext) { }

        public IEnumerable<ContentDataItem> GetViewItems(ContentView view)
        {
            if (view.Parent == null && view.ParentID == 0)
                throw new Exception("ContentType not found");

            var contentType = view.Parent != null ? view.Parent : Context.ContentLists.Find(view.ParentID);

            if (contentType == null)
                throw new Exception("ContentType not found.");
            var query = DbSet.Where(c => c.ParentID.Equals(view.ParentID)).OrderByDescending(v => v.Modified);
            return query.AsQueryable();
        }

        private string GetColumnTypeString(ContentField field)
        {
            //var sqlType = "nvarchar(max)";
            var type = field.SystemType;
            if (type == typeof(int))
                return "int";

            if (type == typeof(Guid))
                return "uniqueidentifier";

            if (type == typeof(bool))
                return "bit";

            if (type == typeof(decimal))
                return "decimal(18, 2)";

            if (type == typeof(float))
                return "float";

            if (type == typeof(double))
                return "real";

            if (type == typeof(DateTime))
                return "datetime";

            return "nvarchar(max)";
        }

        public ContentDataItem Create(int contentTypeID, object dataItem, string user, bool enableComments = false,
            bool isPublished = false, string parentID = "", int pos = 0,
            string categories = "", string tags = "")
        {
            var contentList = this.context.ContentLists.Find(contentTypeID);

            if (contentList == null)
                throw new Exception("ContentType not found.");

            var catlist = new List<Category>();

            #region Create new categories if they are not exists
            if (!string.IsNullOrEmpty(categories))
            {
                var cats = categories.Split(',');
                var testId = 0;
                if (cats.Length > 0 && !int.TryParse(cats[0], out testId))
                {
                    //we guess the categories is a name list
                    catlist = Context.Categories.Where(c => cats.Contains(c.Name) && !string.IsNullOrEmpty(c.Name) && c.WebID == contentList.WebID).ToList();
                    //var recats = exists.ToList();
                    //var newcatids = new List<int>();
                    foreach (var c in cats)
                    {
                        var catIndb = catlist.FirstOrDefault(a => a.Name.Equals(c));
                        if (catIndb == null)
                        {
                            var newcat = new Category()
                            {
                                Name = c,
                                WebID = contentList.WebID,
                                ParentID = 0,
                                Locale = contentList.Locale
                            };
                            Context.Categories.Add(newcat);
                            Context.SaveChanges();
                            catlist.Add(newcat);
                        }
                    }

                }
            }

            #endregion

            //var minDate = new DateTime(1600, 1, 1);

            var item = new ContentDataItem()
            {
                ID = Guid.NewGuid(),
                Created = DateTime.Now,
                Modifier = user,
                Locale = contentList.Locale,
                Pos = pos,
                //Published = minDate,
                //Moderated = minDate,
                //Modified = minDate,
                Owner = user,
                EnableComments = enableComments,
                Parent = contentList,
                ParentID = contentTypeID,
                RefID = Guid.Empty,
                IsPublished = isPublished,
                Categories = catlist,
                PrimaryCategoryID = catlist != null && catlist.Count > 0 ? catlist.FirstOrDefault().ID : 0,
                Tags = tags,
                RawData = GetRowXml(dataItem, contentList),
                Version = 1,
                IsCurrentVersion = true
            };

            if (isPublished)
                item.Published = DateTime.Now;

            #region Generate Slug
            var fields = contentList.ReadFields();
            var slugField = fields.FirstOrDefault(f => f.IsSlug);
            if (slugField != null)
            {
                var xdoc = XDocument.Parse(item.RawData);
                var slugElement = xdoc.Descendants(slugField.Name).FirstOrDefault();
                if (slugElement != null && !string.IsNullOrEmpty(slugElement.Value))
                {
                    item.Slug = TextUtility.Slug(slugElement.Value.ToLower());
                    var flex = item.Slug;
                    var slug = flex;
                    var j = 0;
                    while (DbSet.Count(i => i.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase) && i.ParentID == contentTypeID) > 0)
                        slug = flex + (j++).ToString();
                    item.Slug = slug;
                }
            }

            if (string.IsNullOrEmpty(item.Slug))
                item.Slug = item.ID.ToString();

            #endregion

            #region Generate Path
            item.Path = item.ID.ToString();
            if (!string.IsNullOrEmpty(parentID))
            {
                var parentUID = new Guid(parentID);
                if (parentUID != Guid.Empty)
                {
                    var parentItem = DbSet.Find(parentUID);
                    if (parentItem != null)
                    {
                        item.ParentItemID = parentUID;
                        item.Path = parentItem.Path + "/" + item.ID.ToString();
                    }
                }
            }
            #endregion

            #region Generate pos
            var q = dbSet.Where(i => i.ParentID == item.ParentID);

            if (item.ParentItemID != Guid.Empty)
                q = q.Where(i => i.ParentItemID == item.ParentItemID);

            if (q.Count() > 0)
            {
                var max = q.Max(i => i.Pos);
                item.Pos = max + 1;
            };

            #endregion

            if (contentList.IsModerated)
            {
                var isModerator = contentList.Moderators != null && contentList.Moderators.Split(',').Contains(user);

                if (contentList.Owner.Equals(item.Owner) || isModerator)
                    item.ModerateState = 2;
            }

            DbSet.Add(item);

            if (IsOwnContext)
                Context.SaveChanges();

            return item;
        }

        public ContentDataItem Update(ContentDataItem dataItem, object dataObject)
        {
            if (dataItem == null)
                throw new ArgumentNullException("dataItem");

            if (dataObject == null)
                throw new ArgumentNullException("dataObject");

            if (dataItem.ParentID == 0 && dataItem.Parent == null)
                throw new Exception("The content data item is not belong to any content type.");

            if (dataItem.ID == null || dataItem.ID == Guid.Empty)
                throw new Exception("The dataItem's ID can not be empty!");

            var contentList = dataItem.Parent != null ? dataItem.Parent : Context.ContentLists.Find(dataItem.ParentID);
            var orginialItem = DbSet.Find(dataItem.ID);

            #region When the parent item is changed

            if (orginialItem.ParentItemID != dataItem.ParentItemID)
            {
                var descendant = dbSet.Where(i => i.Path.StartsWith(orginialItem.Path) && i.ID != orginialItem.ID).ToList();
                if (dataItem.ParentItemID != Guid.Empty)
                {
                    var parentItem = dbSet.Find(dataItem.ParentItemID);
                    dataItem.Path = parentItem.Path + "/" + dataItem.ID.ToString();
                }
                else
                    dataItem.Path = dataItem.ID.ToString();

                foreach (var d in descendant)
                    d.Path = dataItem.Path + "/" + d.ID.ToString();
            }

            #endregion

            dataItem.Modified = DateTime.Now;
            //dataItem.Moderated = DateTime.MinValue; 
            //contentList.LastModified = DateTime.Now;

            #region Set Xml data
            var dict = GetValues(dataItem);
            var tmpDict = ReflectionHelper.ConvertToDictionary(dataObject);
            foreach (var key in tmpDict.Keys)
            {
                if (dict.ContainsKey(key))
                    dict[key] = tmpDict[key];
                else
                    dict.Add(key, tmpDict[key]);
            }
            var fields = contentList.ReadFields();
            dataItem.RawData = GenerateDataXml(dict, contentList.Name, fields);
            #endregion

            #region Versioning
            if (contentList.EnableVersioning)
            {
                if (orginialItem.IsPublished)
                {
                    var item = new ContentDataItem();
                    item.ID = Guid.NewGuid();
                    item.IsCurrentVersion = false;
                    item.RefID = dataItem.ID;

                    if (contentList.IsModerated)
                    {
                        if (!dataItem.IsLocked)
                        {
                            //Is Moderated list we need to lock the item
                            dataItem.CopyTo(item, "ID", "RefID", "IsCurrentVersion", "Attachments");
                            item.Version++;

                            //Cancal editing
                            Context.Entry(dataItem).Reload();

                            //Lock item 
                            dataItem.IsLocked = true;
                            dataItem.RevID = item.ID;
                            dataItem.ModerateState = 0;
                        }
                        else
                        {
                            //Update the rev item
                            var revItem = DbSet.Find(dataItem.RevID);
                            dataItem.CopyTo(revItem, "ID", "RefID", "IsCurrentVersion", "Attachments");
                            //Cancal editing
                            Context.Entry(dataItem).Reload();
                        }
                    }
                    else
                    {
                        orginialItem.CopyTo(item, "ID", "RefID", "IsCurrentVersion", "Attachments");
                        dataItem.Version++;
                    }

                    dbSet.Add(item);
                }
            }
            #endregion

            if (IsOwnContext)
                Context.SaveChanges();

            return dataItem;
        }

        public void Clear(int listID)
        {
            if (listID <= 0)
                throw new ArgumentOutOfRangeException("The content list id must be great then 0.");
            this.Context.Database.ExecuteSqlCommand("DELETE dna_ContentDataItems WHERE ParentID={0}", listID);
        }

        public override void Delete(ContentDataItem TObject)
        {
            Delete(c => c.Path.StartsWith(TObject.Path));
            if (IsOwnContext)
                Context.SaveChanges();
        }

        private Dictionary<string, object> GetValues(ContentDataItem dataItem)
        {
            //var decorator = new InternalContentListWrapper(dataItem.Parent);
            var fields = dataItem.Parent.ReadFields();
            var dict = new Dictionary<string, object>();
            using (var reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(dataItem.RawData))))
            {
                reader.ReadStartElement(dataItem.Parent.Name);
                while (!reader.EOF && reader.NodeType != XmlNodeType.EndElement)
                {
                    var fieldName = reader.Name;
                    var field = fields.FirstOrDefault(f => f.Name.Equals(fieldName));
                    var val = field == null ? reader.ReadElementContentAsString() : reader.ReadElementContentAs(field.SystemType, null);
                    dict.Add(fieldName, val);
                }
            }
            return dict;
        }

        private string GetRowXml(object dataItem, ContentList contentType, bool includeDefaults = true)
        {
            var valDict = dataItem.ToDictionary();
            var fields = contentType.ReadFields();
            if (includeDefaults)
            {

                foreach (var f in fields)
                {
                    var defVal = f.DefaultValue;
                    if (valDict.ContainsKey(f.Name))
                    {
                        if (defVal != null)
                        {
                            if (!valDict.ContainsKey(f.Name))
                                valDict.Add(f.Name, defVal);
                            else
                            {
                                if (valDict[f.Name] == null)
                                    valDict[f.Name] = defVal;
                            }
                        }
                    }
                }
            }

            return GenerateDataXml(valDict, contentType.Name, fields);
        }

        private string GenerateDataXml(IDictionary<string, object> dict, string name, IEnumerable<ContentField> fields)
        {
            var element = new XElement(name);
            foreach (var key in dict.Keys)
            {
                var val = dict[key];
                //var field = fields.FirstOrDefault(f => f.Name.Equals(key));
                //if (field != null)
                //{
                // //   val = field.ToXmlValue(dict[key]);
                //    if (field.FieldType==(int)ContentFieldTypes.Note || field.FieldType==(int)ContentFieldTypes.Image)
                //        element.Add(new XElement(key,new XCData((string)val)));
                //}
                //else
                element.Add(new XElement(key, val));
            }

            return element.OuterXml();
            //var xmlBuilder = new StringBuilder();
            //xmlBuilder.AppendFormat("<{0}>", name);
            //foreach (var key in dict.Keys)
            //{
            //    try
            //    {
            //        var field = fields.FirstOrDefault(f => f.Name.Equals(key));

            //        if (field == null)
            //            throw new ContentFieldNotFoundException(key);

            //        var val = field.ToXmlValue(dict[key]);
            //        xmlBuilder.AppendFormat("<{0}>{1}</{0}>", key, val);
            //    }
            //    catch (Exception e)
            //    {
            //        throw new Exception(string.Format("GenerateDataXml Error : Get {0} field value error.", key), e);
            //    }
            //}

            //return xmlBuilder.AppendFormat("</{0}>", name).ToString();
        }

        public void DisableComments(Guid id, bool value)
        {
            if (id == null || id == Guid.Empty)
                throw new ArgumentNullException("id");

            var item = Find(id);
            if (item == null)
                throw new Exception("DataItem not found");

            item.EnableComments = !value;

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public void Audit(Guid id, string auditor, int state)
        {
            if (id == null || id == Guid.Empty)
                throw new ArgumentNullException("id");

            var item = Find(id);

            if (item == null)
                throw new Exception("DataItem not found");

            var contentType = Context.ContentLists.Find(item.ParentID);

            if (contentType.EnableVersioning && state == 2)
            {
                if (item.IsLocked)
                {
                    //Unlocked the rev
                    var rev = dbSet.Find(item.RevID);
                    rev.CopyTo(item, "ID", "RevID", "Attachments");
                    item.IsLocked = false;
                    item.IsCurrentVersion = true;
                    item.RevID = Guid.Empty;
                    item.Version++;
                }
            }

            item.Auditor = auditor;
            item.ModerateState = state;
            item.Moderated = DateTime.Now;

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public void Publish(Guid id)
        {
            if (id == null || id == Guid.Empty)
                throw new ArgumentNullException("id");

            var item = Find(id);
            if (item == null)
                throw new Exception("DataItem not found");

            item.IsPublished = true;
            item.Published = DateTime.Now;

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public void Read(Guid id)
        {
            if (id == null || id == Guid.Empty)
                throw new ArgumentNullException("id");

            var item = Find(id);
            if (item == null)
                throw new Exception("DataItem not found");

            item.Reads++;

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public void Move(Guid parentID, Guid id, int pos)
        {
            var item = Find(id);

            if (item == null)
                throw new ObjectNotFoundException(id.ToString());

            var items = DbSet.Where(p => p.ParentID == item.ParentID);

            #region reindex
            var indexedItems = items.Where(p => p.ParentItemID == parentID).OrderBy(p => p.Pos).ToList();
            for (int i = 0; i < indexedItems.Count; i++)
            {
                indexedItems[i].Pos = i;
                if (indexedItems[i].ID == item.ID)
                    item.Pos = i;
            }

            #endregion

            var seqCollection = indexedItems.Select(p => p.Pos);

            if (seqCollection.Count() == 0)
            {
                item.Pos = 0;
                item.ParentItemID = parentID;

                #region Generate Path

                item.Path = item.ID.ToString();
                if (parentID != null && parentID != Guid.Empty)
                {
                    var parentItem = DbSet.Find(parentID);
                    if (parentItem != null)
                    {
                        item.ParentItemID = parentID;
                        item.Path = parentItem.Path + "/" + item.ID.ToString();
                    }
                }

                #endregion

                if (IsOwnContext)
                    Context.SaveChanges();

                return;
            }

            int upperBound = seqCollection.Max();
            int lowerBound = seqCollection.Min();

            if (upperBound == 0 && lowerBound == 0)
                upperBound = seqCollection.Count();

            int _from = item.Pos;
            int _to = pos;

            if (_to > upperBound)
                _to = upperBound;
            else
            {
                if (_to < lowerBound)
                    _to = lowerBound;
            }

            //1.Move up
            if (_from > _to)
            {
                indexedItems.Where(p => p.ParentItemID == parentID && p.Pos >= _to && p.Pos < _from)
                    .OrderBy(p => pos)
                    .AsParallel()
                    .ForAll(p => p.Pos++);

            }

            //2.Move down
            if (_from < _to)
            {
                indexedItems.Where(p => p.ParentItemID == parentID && p.Pos > _from && p.Pos <= _to)
                   .OrderBy(p => pos)
                   .AsParallel()
                   .ForAll(p => p.Pos--);
            }

            item.Pos = _to;
            item.ParentItemID = parentID;
            item.Path = item.ID.ToString();
            if (parentID != null && parentID != Guid.Empty)
            {
                var parentItem = DbSet.Find(parentID);
                if (parentItem != null)
                {
                    item.ParentItemID = parentID;
                    item.Path = parentItem.Path + "/" + item.ID.ToString();
                }
            }
            if (IsOwnContext) Context.SaveChanges();
        }

        //public override ContentDataItem Find(params object[] keys)
        //{
        //    var id = (Guid)keys[0];
        //    return this.DbSet.Include("Categories").Where(c => c.ID.Equals(id)).FirstOrDefault();
        //    //return base.Find(keys);
        //}

        //public override IQueryable<ContentDataItem> Filter(System.Linq.Expressions.Expression<Func<ContentDataItem, bool>> predicate)
        //{
        //    return this.DbSet.Include("Categories").Where(predicate);
        //    //return base.Filter(predicate);
        //}
    }
}
