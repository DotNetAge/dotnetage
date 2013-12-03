//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using System;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Defines a content data item repository interface
    /// </summary>
    public interface IContentDataItemRepository : IRepository<ContentDataItem>
    {
        /// <summary>
        /// Get data items by specified content view object.
        /// </summary>
        /// <param name="view">The content view object.</param>
        /// <returns>returns a content data item collection object</returns>
        IEnumerable<ContentDataItem> GetViewItems(ContentView view);

        /// <summary>
        /// Create a new data item instance.
        /// </summary>
        /// <param name="contentTypeID">The content type id.</param>
        /// <param name="dataItem">The data item object.</param>
        /// <param name="user">The user name who to create this item.</param>
        /// <param name="enableComments">Specified whether this item allow comments.</param>
        /// <param name="isPublished">Specified whether publish the data item after save</param>
        /// <param name="parentID">Specified the parent data item id.</param>
        /// <param name="pos">Specified the data item order of siblings.</param>
        /// <param name="categories">Specified the categories of the data item.</param>
        /// <param name="tags">Specified the tags of the data item.</param>
        /// <returns>returns a new content data item instance.</returns>
        ContentDataItem Create(int contentTypeID, object dataItem, string user, bool enableComments = false, bool isPublished = false, string parentID = "", int pos = 0, string categories = "", string tags = "");

        /// <summary>
        /// Update data item.
        /// </summary>
        /// <param name="item">The ContentDataItem object to be updated.</param>
        /// <param name="dataObject">The data object update to ContentDataItem object.</param>
        /// <returns>returns content dataitem contains new values</returns>
        ContentDataItem Update(ContentDataItem item, object dataObject);

        /// <summary>
        /// Clear data items by specified list id.
        /// </summary>
        /// <param name="listID">The list id.</param>
        void Clear(int listID);

        /// <summary>
        /// Move the data item to new position.
        /// </summary>
        /// <param name="parentID">The new parent data item id.</param>
        /// <param name="id">The data item id.</param>
        /// <param name="pos">The new position order value of siblings.</param>
        void Move(Guid parentID, Guid id, int pos);

        /// <summary>
        /// Disable / Enable comments by specified data item id.
        /// </summary>
        /// <param name="id">The data item id.</param>
        /// <param name="value">Set to ture to enable comments otherwire disable.</param>
        void DisableComments(Guid id, bool value);

        /// <summary>
        /// Publish data item by specified id.
        /// </summary>
        /// <param name="id"></param>
        void Publish(Guid id);

        /// <summary>
        /// Incase the read referece count by specified data item id.
        /// </summary>
        /// <param name="id">The data item id.</param>
        void Read(Guid id);

        /// <summary>
        /// Audit data item by specified data item id.
        /// </summary>
        /// <param name="id">The data item id.</param>
        /// <param name="auditor">The auditor user name.</param>
        /// <param name="state">The moderate state.</param>
        void Audit(Guid id, string auditor, int state);
    }
}
