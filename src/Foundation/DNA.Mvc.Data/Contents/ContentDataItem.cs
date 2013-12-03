//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a content data item.
    /// </summary>
    public class ContentDataItem : System.Dynamic.DynamicObject
    {
        private string rawData = "";

        /// <summary>
        /// Gets/Sets the item id.
        /// </summary>
        public virtual Guid ID { get; set; }

        /// <summary>
        /// Gets/Sets the parent list id.
        /// </summary>
        public virtual int ParentID { get; set; }

        /// <summary>
        /// Gets/Sets the parent data item id.
        /// </summary>
        public virtual Guid ParentItemID { get; set; }

        /// <summary>
        /// Gets/Sets the shared data item id.
        /// </summary>
        public virtual Guid ShareID { get; set; }

        /// <summary>
        /// Gets/Sets the privacy value.
        /// </summary>
        public virtual int Privacy { get; set; }

        /// <summary>
        /// Gets/Sets the reference dataitem id.
        /// </summary>
        public virtual Guid RefID { get; set; }

        /// <summary>
        /// Gets/Sets the data item version.
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// Indicates whether the data item is current version.
        /// </summary>
        public virtual bool IsCurrentVersion { get; set; }

        /// <summary>
        /// Gets/Sets this data item is locked.
        /// </summary>
        /// <remarks>
        /// If this property set it means this dataitem has a pending review version.
        /// </remarks>
        public virtual bool IsLocked { get; set; }

        /// <summary>
        /// Gets/Sets the reversion data item id.
        /// </summary>
        public virtual Guid RevID { get; set; }

        /// <summary>
        /// Gets/Sets the modifier user name.
        /// </summary>
        public virtual string Modifier { get; set; }

        /// <summary>
        /// Gets/Sets the annotation text.
        /// </summary>
        public virtual string Annotation { get; set; }

        /// <summary>
        /// Gets/Sets the datetime of the data item creation.
        /// </summary>
        public virtual DateTime Created { get; set; }

        /// <summary>
        /// Gets/Sets the data item latest modified time.
        /// </summary>
        public virtual DateTime? Modified { get; set; }

        /// <summary>
        /// Gets/Sets the publiched date time of the data item.
        /// </summary>
        public virtual DateTime? Published { get; set; }

        /// <summary>
        /// Gets/Sets the moderated date time of the data item.
        /// </summary>
        public virtual DateTime? Moderated { get; set; }

        /// <summary>
        /// Gets/Sets the data owner
        /// </summary>
        public virtual string Owner { get; set; }

        /// <summary>
        /// Gets/Sets the auditor user name
        /// </summary>
        public virtual string Auditor { get; set; }

        /// <summary>
        /// Gets/Sets the data item locale name.
        /// </summary>
        public virtual string Locale { get; set; }

        /// <summary>
        /// The possible values : Pending | Approved | Rejected
        /// </summary>
        public virtual int ModerateState { get; set; }

        /// <summary>
        /// Gets/Sets the raw object data string.
        /// </summary>
        public virtual string RawData
        {
            get { return rawData; }
            set { rawData = value; }
        }

        /// <summary>
        /// Gets/Sets the rating value.
        /// </summary>
        public virtual double Ratings { get; set; }

        /// <summary>
        /// Gets/Sets the read amount.
        /// </summary>
        public virtual int Reads { get; set; }

        /// <summary>
        /// Indicates whether the data item is published.
        /// </summary>
        public virtual bool IsPublished { get; set; }

        /// <summary>
        /// Indicates whether data item can add comments.
        /// </summary>
        public virtual bool EnableComments { get; set; }

        /// <summary>
        /// Gets/Sets tags
        /// </summary>
        public virtual string Tags { get; set; }

        /// <summary>
        /// Gets/Sets the url slug.
        /// </summary>
        public virtual string Slug { get; set; }

        /// <summary>
        /// Gets/Sets the data item path.
        /// </summary>
        public virtual string Path { get; set; }

        /// <summary>
        /// Gets/Sets the data item order.
        /// </summary>
        public virtual int Pos { get; set; }

        /// <summary>
        /// Gets/Sets total attachment count.
        /// </summary>
        public virtual int TotalAttachments { get; set; }

        /// <summary>
        /// Gets/Sets total vote count.
        /// </summary>
        public virtual int TotalVotes { get; set; }

        /// <summary>
        /// Gets/Sets all attachments of the data item.
        /// </summary>
        public virtual ICollection<ContentAttachment> Attachments { get; set; }

        /// <summary>
        /// Gets/Sets the primary category id.
        /// </summary>
        public int? PrimaryCategoryID { get; set; }

        /// <summary>
        /// Gets/Sets all categories of the data item.
        /// </summary>
        public virtual ICollection<Category> Categories { get; set; }

        /// <summary>
        /// Gets /Sets the parent list object.
        /// </summary>
        public virtual ContentList Parent { get; set; }

        #region schema
        internal const string TAG_NAME = "row";
        #endregion

        /// <summary>
        /// Load properties from xml.
        /// </summary>
        /// <param name="element">The XElement object</param>
        /// <param name="locale">The locale name.</param>
        public void Load(XElement element, string locale = "")
        {
            var _id = element.StrAttr(DataNames.ID);
            var ns = element.GetDefaultNamespace();
            if (!string.IsNullOrEmpty(_id))
            {
                Guid gid = Guid.Empty;
                if (Guid.TryParse(_id, out gid))
                    ID = gid;
            }

            if (this.ID == Guid.Empty || this.ID == null)
                this.ID = Guid.NewGuid();

            Privacy = element.IntAttr(DataNames.Privacy);
            Created = element.DateAttr(DataNames.Created);
            if (Created == DateTime.MinValue)
                Created = DateTime.Now;

            IsPublished = element.BoolAttr(DataNames.IsPublished);
            if (IsPublished)
            {
                Published = element.DateAttr(DataNames.Published);
                if (Published == DateTime.MinValue)
                    Published = DateTime.Now;
            }
            ModerateState = element.IntAttr(DataNames.State);
            EnableComments = element.BoolAttr(DataNames.EnableComments);
            Tags = element.StrAttr(DataNames.Tags);

            //Categories = element.StrAttr(DataNames.Categories);

            Path = element.StrAttr(DataNames.Path);
            Reads = element.IntAttr(DataNames.Reads);
            Ratings = (double)element.DecimalAttr(DataNames.Ratings);
            IsCurrentVersion = true;
            var parentID = element.StrAttr(DataNames.ParentID);
            if (!string.IsNullOrEmpty(parentID))
            {
                Guid gPID = Guid.Empty;
                if (Guid.TryParse(parentID, out gPID))
                    ParentItemID = gPID;
            }
            Pos = element.IntAttr(DataNames.Pos);
            Slug = element.StrAttr(DataNames.Slug);
            Modified = element.DateAttr(DataNames.Modified);
            RawData = element.Element(ns + "data").Value;

            if (RawData.StartsWith("data:text/xml;base64"))
            {
                RawData = element.Element(ns + "data").Value.Replace("data:text/xml;base64,", "");
                RawData = Encoding.UTF8.GetString(Convert.FromBase64String(RawData));
            }

            if (RawData.Contains(ContentList.DefaultNamespace))
                RawData = RawData.Replace(" xmlns=\"" + ContentList.DefaultNamespace + "\"", "");

            if (Modified == DateTime.MinValue)
                Modified = DateTime.Now;
        }

        /// <summary>
        /// Convert the data item to XElement object.
        /// </summary>
        /// <param name="encodeBase64">Specified whether output to base64 string.</param>
        /// <returns>A XElement object returns.</returns>
        public XElement Element(bool encodeBase64 = false)
        {
            XNamespace ns = ContentList.DefaultNamespace;
            var element = new XElement(ns + TAG_NAME,
                new XAttribute(DataNames.ID, ID),
                new XAttribute(DataNames.Privacy, Privacy),
                new XAttribute(DataNames.Created, this.Created),

                new XAttribute(DataNames.State, this.ModerateState),
                new XAttribute(DataNames.IsPublished, this.IsPublished),
                new XAttribute(DataNames.EnableComments, EnableComments),
                new XAttribute(DataNames.Slug, Slug),
                new XAttribute(DataNames.Ratings, this.Ratings),
                new XAttribute(DataNames.Reads, this.Reads),
                new XAttribute(DataNames.Path, this.Path),
                new XAttribute(DataNames.Owner, Owner),
                new XAttribute(DataNames.Pos, Pos));

            if (!string.IsNullOrEmpty(this.Tags))
                element.Add(new XAttribute(DataNames.Tags, this.Tags));

            //if (!string.IsNullOrEmpty(this.Categories))
            //    element.Add(new XAttribute(DataNames.Categories, this.Categories));

            element.Add(new XElement(ns + "data", new XCData(encodeBase64 ? ("data:text/xml;base64," + RawDataBase64) : RawData)));

            if (this.ParentItemID != null && this.ParentItemID != Guid.Empty)
                element.Add(new XAttribute(DataNames.ParentID, this.ParentItemID));

            if (this.Modified.HasValue)
                element.Add(new XAttribute(DataNames.Modified, this.Modified));

            if (this.Published.HasValue)
                element.Add(new XAttribute(DataNames.Published, this.Published));

            //#region attachs
            //if (this.HasAttachments)
            //{
            //    var attachElement = new XElement(ATTACH);
            //    foreach (var attach in this.Attachments)
            //    {
            //        attachElement.Add(new XAttribute(NAME, attach.Name),
            //            new XAttribute(HREF, attach.Uri),
            //            new XAttribute(CONTENT_TYPE, attach.ContentType),
            //            new XAttribute(EXTENSION, attach.Extension),
            //            new XAttribute(PRIVACY, attach.Privacy));
            //    }
            //}
            //#endregion

            return element;
        }

        internal string RawDataBase64
        {
            get
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(this.RawData));
            }
        }

    }
}
