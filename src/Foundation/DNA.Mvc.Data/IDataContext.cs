//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using System;

namespace DNA.Web
{
    /// <summary>
    /// Defines the global data context interface.
    /// </summary>
    public interface IDataContext:IUnitOfWorks,IDisposable
    {
        /// <summary>
        /// Gets the web page repository.
        /// </summary>
        IWebPageRepository WebPages { get;  }

        /// <summary>
        /// Gets the widget repository.
        /// </summary>
        IWidgetRepository Widgets { get;  }

        /// <summary>
        /// Gets the widget descriptor repository.
        /// </summary>
        IWidgetDescriptorRepository WidgetDescriptors { get;  }

        /// <summary>
        /// Gets the content data item repository.
        /// </summary>
        IContentDataItemRepository ContentDataItems { get; }

        /// <summary>
        /// Gets the permission repository.
        /// </summary>
        IPermissionRepository Permissions { get; }

        /// <summary>
        /// Gets the user repository.
        /// </summary>
        IUserRepository Users { get; }
    }
}
