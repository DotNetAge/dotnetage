//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace DNA.Web.EmbeddedViews
{
    /// <summary>
    /// Represents a view path provider use to resolve embedded view file.
    /// </summary>
    public class EmbeddedViewPathProvider : VirtualPathProvider
    {
        private readonly EmbeddedViewTable _embeddedViews;

        /// <summary>
        /// Initializes a new instance of EmbeddedViewPathProvider class. 
        /// </summary>
        /// <param name="embeddedViews"></param>
        public EmbeddedViewPathProvider(EmbeddedViewTable embeddedViews)
        {
            if (embeddedViews == null)
                throw new ArgumentNullException("embeddedViews");

            this._embeddedViews = embeddedViews;
        }

        /// <summary>
        /// Overrided. Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <param name="virtualPath"> The path to the virtual file.</param>
        /// <returns> true if the file exists in the virtual file system; otherwise, false.</returns>
        public override bool FileExists(string virtualPath)
        {
            return (IsEmbeddedView(virtualPath) || Previous.FileExists(virtualPath));
            //return (ResourceFileExists(virtualPath) || Previous.FileExists(virtualPath));
        }

        /// <summary>
        /// Overrided.Gets a virtual file from the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <returns>A descendent of the System.Web.Hosting.VirtualFile class that represents a file in the virtual file system.</returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsEmbeddedView(virtualPath))
            {
                string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);
                //var fullyQualifiedViewName = virtualPathAppRelative.Substring(virtualPathAppRelative.LastIndexOf("/") + 1, virtualPathAppRelative.Length - 1 - virtualPathAppRelative.LastIndexOf("/"));
                
                var fullyQualifiedViewName = virtualPathAppRelative.Replace("~", "").Replace("/", ".");
                var embeddedViewMetadata = _embeddedViews.FindEmbeddedView(fullyQualifiedViewName);
                return new EmbeddedVirtualFile(embeddedViewMetadata, virtualPath);
            }

            return Previous.GetFile(virtualPath);

        }

        /// <summary>
        /// Overrided.Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <param name="virtualPathDependencies"> An array of paths to other resources required by the primary virtual resource.</param>
        /// <param name="utcStart"> The UTC time at which the virtual resources were read.</param>
        /// <returns>A System.Web.Caching.CacheDependency object for the specified virtual resources.</returns>
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return IsEmbeddedView(virtualPath) ? null : Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        private bool IsEmbeddedView(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
                return false;

            string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);
            
            if (!virtualPathAppRelative.StartsWith("~/Views/", StringComparison.InvariantCultureIgnoreCase) &&
                !virtualPathAppRelative.StartsWith("~/Widgets/", StringComparison.InvariantCultureIgnoreCase))
                return false;
            
            //"~/Views/Seller/Order.cshtml" ->"Views.Seller.Order.cshtml"
            var fullyQualifiedViewName = virtualPathAppRelative.Replace("~", "").Replace("/", ".");

            //var fullyQualifiedViewName = virtualPathAppRelative.Substring(virtualPathAppRelative.LastIndexOf("/") + 1, virtualPathAppRelative.Length - 1 - virtualPathAppRelative.LastIndexOf("/"));

            bool isEmbedded = _embeddedViews.ContainsEmbeddedView(fullyQualifiedViewName);
            return isEmbedded;

        }
    }
}
