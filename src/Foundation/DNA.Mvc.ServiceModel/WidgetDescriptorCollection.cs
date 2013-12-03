//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a collection of WidgetDescriptors.
    /// </summary>
    public class WidgetDescriptorCollection : IEnumerable<WidgetDescriptorDecorator>
    {
        private IDataContext DataContext { get; set; }
        private int[] accessableIDs = null;
        private string path = null;

        internal WidgetDescriptorCollection(IDataContext dbContext, int[] ids = null, string category = null)
        {
            this.DataContext = dbContext;
            accessableIDs = ids;
            path = category;
        }

        /// <summary>
        /// Remove WidgetDescriptor from collection and save to database by specified installPath
        /// </summary>
        /// <param name="installedPath">The installedPath format : {category name}\{name} </param>
        public void Remove(string installedPath)
        {
            var model = DataContext.Find<WidgetDescriptor>(w => w.InstalledPath.Equals(installedPath));
            if (model == null)
                throw new WidgetDescriptorNotFoundException();
            DataContext.Delete(model);
            DataContext.SaveChanges();
        }

        /// <summary>
        /// Remove WidgetDescriptor from collection and save to database by specified id.
        /// </summary>
        /// <param name="id">The widget descritpor id.</param>
        public void Remove(int id)
        {
            DataContext.Delete<WidgetDescriptor>(w => w.ID.Equals(id));
            DataContext.SaveChanges();
        }

        /// <summary>
        /// Gets widget descriptor by specified installedPath
        /// </summary>
        /// <param name="installedPath">The installedPath format : {category name}\{name} </param>
        /// <returns>A WidgetDescriptorDecorator instance that wrapps that widget descriptor model.</returns>
        public WidgetDescriptorDecorator this[string installedPath]
        {
            get
            {
                var model = DataContext.Find<WidgetDescriptor>(w => w.InstalledPath.Equals(installedPath));
                if (model == null)
                    return null;
                return new WidgetDescriptorDecorator(model, this.DataContext);
            }
        }

        /// <summary>
        /// Find widget descriptor by specified id.
        /// </summary>
        /// <param name="id">The widget descriptor id.</param>
        /// <returns>A WidgetDescriptorDecorator instance that wrapps that widget descriptor model.</returns>
        public WidgetDescriptorDecorator Find(int id)
        {
            if (accessableIDs != null)
            {
                if (!accessableIDs.Contains(id))
                    return null;
            }

            var descriptor = DataContext.WidgetDescriptors.Find(id);
            return new WidgetDescriptorDecorator(descriptor, DataContext);
        }

        public IEnumerator<WidgetDescriptorDecorator> GetEnumerator()
        {
            if (accessableIDs == null && !string.IsNullOrEmpty(path))
                return DataContext.WidgetDescriptors.WithInPath(path).Select(w => new WidgetDescriptorDecorator(w, this.DataContext)).GetEnumerator();

            if (accessableIDs != null && string.IsNullOrEmpty(path))
                return DataContext.WidgetDescriptors.Filter(w => accessableIDs.Contains(w.ID)).Select(w => new WidgetDescriptorDecorator(w, this.DataContext)).GetEnumerator();

            return DataContext.WidgetDescriptors.WithInPath(path).Where(w => accessableIDs.Contains(w.ID)).Select(w => new WidgetDescriptorDecorator(w, this.DataContext)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
