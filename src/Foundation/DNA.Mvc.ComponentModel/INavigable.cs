///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web
{
    /// <summary>
    /// Defines properties of a navigable object.
    /// </summary>
    public interface INavigable
    {
        /// <summary>
        /// Retrieves the display title 
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the description to show as tooltip.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the target uri of the navigable object.
        /// </summary>
        string NavigateUrl { get; }

        /// <summary>
        /// Gets the image url that use to display the icon.
        /// </summary>
        string ImageUrl { get; }

        /// <summary>
        /// Gets the open target mode this value maybe sets window's name,"_blank","_parent","_self","_top"
        /// </summary>
        string Target { get; }

        /// <summary>
        /// Gets the additational object in the navigatable object.
        /// </summary>
        object Value { get; }
    }

}
