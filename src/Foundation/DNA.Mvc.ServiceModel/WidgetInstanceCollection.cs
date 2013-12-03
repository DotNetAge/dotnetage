//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a collection of WidgetInstanceDecorators.
    /// </summary>
    public class WidgetInstanceCollection : IEnumerable<WidgetInstanceDecorator>
    {
        private IDataContext DataContext { get; set; }

        /// <summary>
        /// Gets the parent web object.
        /// </summary>
        public Web ParentWeb { get; private set; }

        /// <summary>
        /// Gets the parent web page.
        /// </summary>
        public WebPage ParentPage { get; private set; }

        /// <summary>
        ///  Initializes a new instance of the WidgetInstanceCollection class.
        /// </summary>
        /// <param name="dbContext">The data context.</param>
        /// <param name="web">The web object.</param>
        public WidgetInstanceCollection(IDataContext dbContext, Web web)
        {
            this.DataContext = dbContext;
            this.ParentWeb = web;
        }

        /// <summary>
        /// Initializes a new instance of the WidgetInstanceCollection class.
        /// </summary>
        /// <param name="dbContext">The data context.</param>
        /// <param name="parentPage">The parent web page object.</param>
        public WidgetInstanceCollection(IDataContext dbContext, WebPage parentPage)
        {
            this.DataContext = dbContext;
            this.ParentPage = parentPage;
        }

        /// <summary>
        /// Get widget instance by specified id.
        /// </summary>
        /// <param name="id">The web page id.</param>
        /// <returns></returns>
        public WidgetInstanceDecorator Get(int id)
        {
            var widget = DataContext.Find<WidgetInstance>(id);
            if (widget != null)
                return new WidgetInstanceDecorator(widget, DataContext.Widgets);
            return null;

        }

        /// <summary>
        /// Remove widget instance by specified id.
        /// </summary>
        /// <param name="id">The widget id.</param>
        /// <returns>ture if success otherwrise.</returns>
        public bool Remove(int id)
        {
            if (id == 0)
                throw new ArgumentNullException("id");
            return this.Remove(DataContext.Widgets.Find(id));
        }

        /// <summary>
        /// Remove widget instance.
        /// </summary>
        /// <param name="widget">The widget instance object.</param>
        /// <returns>ture if success otherwrise.</returns>
        public bool Remove(WidgetInstance widget)
        {
            if (widget == null)
                throw new ArgumentNullException("widget");

            DataContext.Widgets.Delete(widget);
            return DataContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Create and add widget to collection parent page by specified widget descriptor, zone id and position.
        /// </summary>
        /// <param name="descriptor">The widget descriptor</param>
        /// <param name="zoneID">The widget zone id.</param>
        /// <param name="pos">The widget position.</param>
        /// <returns>A new widget instance object.</returns>
        public WidgetInstanceDecorator Add(WidgetDescriptor descriptor, string zoneID, int pos) 
        {
            if (descriptor == null)
                throw new Exception("Widget descriptor not found.");

            var widget = DataContext.Widgets.AddWidget(descriptor, ParentPage.ID, zoneID, pos);
            DataContext.SaveChanges();
            return new WidgetInstanceDecorator(widget, DataContext.Widgets);
        }

        /// <summary>
        /// Create and add widget to collection parent page by specified widget descriptor id, zone id and position.
        /// </summary>
        /// <param name="descriptorID">The widget descriptor id.</param>
        /// <param name="zoneID">The widget zone id.</param>
        /// <param name="pos">The widget position.</param>
        /// <returns>A new widget instance object.</returns>
        public WidgetInstanceDecorator Add(int descriptorID, string zoneID, int pos)
        {
            if (ParentPage == null)
                throw new Exception("The parent web page not specified.Add widget fail!");

            var descriptor = DataContext.WidgetDescriptors.Find(descriptorID);

            if (descriptor == null)
                throw new Exception("Widget descriptor not found.");

            return Add(descriptor, zoneID, pos);
        }
        
        /// <summary>
        /// Add widget to parent page.
        /// </summary>
        /// <param name="widget">The widget instance object.</param>
        /// <returns>A new widget instance object.</returns>
        public WidgetInstanceDecorator Add(WidgetInstance widget)
        {
            if (widget.PageID == 0 && ParentPage == null)
                throw new Exception("The parent web page not specified.Add widget fail!");

            var wd = new WidgetInstanceDecorator(DataContext.Widgets.Create(widget), DataContext.Widgets);
            DataContext.SaveChanges();
            return wd;
        }

        public IEnumerator<WidgetInstanceDecorator> GetEnumerator()
        {
            if (ParentWeb != null)
            {
                var pageIDs = DataContext.Where<WebPage>(w => w.WebID == ParentWeb.Id).Select(p => p.ID).ToArray();
                if (pageIDs == null || pageIDs.Count() == 0)
                    return new List<WidgetInstanceDecorator>().GetEnumerator();
                else
                    return DataContext.Widgets.Filter(w => pageIDs.Contains(w.PageID)).ToList().Select(w => new WidgetInstanceDecorator(w, this.DataContext.Widgets)).GetEnumerator();
            }
            else
            {
                var pageIDs = DataContext.Where<WebPage>(w => w.WebID == ParentPage.WebID).Select(p => p.ID).ToArray();
                var pageQuery=DataContext.Widgets.Filter(w => w.PageID == ParentPage.ID);
                var parentPath = this.ParentPage.Path;
                var parentIDs =string.IsNullOrEmpty(parentPath) ? new int[0] : parentPath.Split('/').Select(p => int.Parse(p)).ToArray();

                if (pageIDs != null && pageIDs.Count() > 0)
                {
                    pageQuery = DataContext.Widgets.Filter(w => w.Locale.Equals(ParentPage.Locale, StringComparison.OrdinalIgnoreCase) &&
                        (w.PageID == ParentPage.ID || (w.ShowMode == 1 && pageIDs.Contains(w.PageID)) || (w.ShowMode == 2 && parentIDs.Contains(w.PageID)) ));
                }
                
                var list =pageQuery.ToList();
                if (list.Count == 0)
                    return new List<WidgetInstanceDecorator>().GetEnumerator();

                var results = list.Select(w => new WidgetInstanceDecorator(w, this.DataContext.Widgets)).ToList();
                return results.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
