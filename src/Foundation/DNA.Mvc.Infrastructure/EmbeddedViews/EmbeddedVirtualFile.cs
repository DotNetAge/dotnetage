//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace DNA.Web.EmbeddedViews
{
    /// <summary>
    /// Represents a file object in a resource space.
    /// </summary>
    public class EmbeddedVirtualFile : VirtualFile
    {
        private readonly EmbeddedViewMetadata _embeddedViewMetadata;

        /// <summary>
        /// Initializes a new instance of EmbeddedVirtualFile class.
        /// </summary>
        /// <param name="embeddedViewMetadata">The embedded view meta data object.</param>
        /// <param name="virtualPath">The virtual path to the resource represented by this instance.</param>
        public EmbeddedVirtualFile(EmbeddedViewMetadata embeddedViewMetadata, string virtualPath)
            : base(virtualPath)
        {
            if (embeddedViewMetadata == null)
                throw new ArgumentNullException("embeddedViewMetadata");

            this._embeddedViewMetadata = embeddedViewMetadata;
        }

        /// <summary>
        /// Overrided. When overridden in a derived class, returns a read-only stream to the virtual resource.
        /// </summary>
        /// <returns>A read-only stream to the virtual file.</returns>
        public override Stream Open()
        {
            Assembly assembly = GetResourceAssembly();
            return assembly == null ? null : assembly.GetManifestResourceStream(_embeddedViewMetadata.Name);
        }

        /// <summary>
        /// Get the assembly object that contains resource.
        /// </summary>
        /// <returns>A assembly contains view resources.</returns>
        protected virtual Assembly GetResourceAssembly()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => string.Equals(assembly.FullName, _embeddedViewMetadata.AssemblyFullName, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
