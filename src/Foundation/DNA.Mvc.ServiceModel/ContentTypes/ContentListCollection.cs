//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a collection of ContentList
    /// </summary>
    public class ContentListCollection : IEnumerable<ContentListDecorator>
    {
        private Web Parent { get; set; }

        private IDataContext DataContext { get; set; }

        /// <summary>
        /// Gets the content list by sepcified id.
        /// </summary>
        /// <param name="id">The content list id.</param>
        /// <returns></returns>
        public ContentListDecorator this[int id]
        {
            get
            {
                var list = DataContext.Find<ContentList>(id);
                if (list != null)
                {
                    if (list.WebID.Equals(Parent.Id))
                        return new ContentListDecorator(DataContext, list);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the content list by specified name.
        /// </summary>
        /// <param name="name">The content list name.</param>
        /// <returns></returns>
        public ContentListDecorator this[string name]
        {
            get
            {
                var list = DataContext.Find<ContentList>(c => c.WebID == Parent.Id && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (list != null)
                    return new ContentListDecorator(DataContext, list);
                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ContentListColleciton class with data context and parent web object.
        /// </summary>
        /// <param name="dbContext">The data context object.</param>
        /// <param name="parent">The parent web object.</param>
        public ContentListCollection(IDataContext dbContext, Web parent)
        {
            this.DataContext = dbContext;
            this.Parent = parent;
        }

        /// <summary>
        /// Remove the content list from collection and save changes to database.
        /// </summary>
        /// <param name="list">The content list object.</param>
        /// <returns></returns>
        public bool Remove(ContentList list)
        {
            return Remove(list.ID);
        }

        /// <summary>
        /// Remove the content list form collection by specified id.
        /// </summary>
        /// <param name="id">The list id.</param>
        /// <returns></returns>
        public bool Remove(int id)
        {
            var target = DataContext.Find<ContentList>(id);
            var list = target.Name;

            var webname = target.Web.Name;
            var targetWrapper = new ContentListDecorator(DataContext, target);
            //var itemFolderUrl = new Uri(targetWrapper.AttachmentsPath.ToString() + "/" + id.ToString());

            //Delete pages
            var slugPath = "lists/" + list;
            var culture = target.Locale;

            DataContext.Delete<ContentList>(c => c.ID == id && c.WebID == Parent.Id);
            var result = DataContext.SaveChanges() > 0;

            DataContext.Delete<WebPage>(p => p.Locale.Equals(culture, StringComparison.OrdinalIgnoreCase) && p.Slug.StartsWith(slugPath));

            result = DataContext.SaveChanges() > 0;

            var netdrive = App.GetService<INetDriveService>();

            if (netdrive.Exists(targetWrapper.DefaultListPath))
                netdrive.Delete(targetWrapper.DefaultListPath);

            App.Get().CurrentWeb.ClearCache();

            //EventDispatcher.RaiseContentListDeleted(target.Web, id, list, culture);
            App.Trigger("ContentListDeleted", new ContentListDeletedEventArgs() { ListID = id, ListName = list, Locale = culture, Website = target.Web });

            return result;
        }

        public IEnumerator<ContentListDecorator> GetEnumerator()
        {
            return DataContext.Where<ContentList>(c => c.WebID == Parent.Id).ToList().Select(w => new ContentListDecorator(this.DataContext, w)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
