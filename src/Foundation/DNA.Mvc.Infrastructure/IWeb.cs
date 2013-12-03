//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Defines a web properties
    /// </summary>
    public interface IWeb
    {
        /// <summary>
        /// Gets the Web name
        /// </summary>
        string Name { get;}

        /// <summary>
        /// Gets the display title text 
        /// </summary>
        string Title { get;}

        /// <summary>
        /// Gets the description text
        /// </summary>
        string Description { get;}

        /// <summary>
        /// Gets the default theme name
        /// </summary>
        string Theme { get; }

        /// <summary>
        /// Gets the default language
        /// </summary>
        string DefaultLocale { get; }

        /// <summary>
        /// Gets the copyright info
        /// </summary>
        string Copyright { get; }

        /// <summary>
        /// Gets the custom css text
        /// </summary>
        string CssText { get; }

        /// <summary>
        /// Gets the logo image url
        /// </summary>
        string LogoImageUrl { get; }

        /// <summary>
        /// Gets the default url
        /// </summary>
        string DefaultUrl { get; }

        /// <summary>
        /// Gets the shortcut icon url
        /// </summary>
        string ShortcutIconUrl { get; }

        /// <summary>
        /// Gets the children pages 
        /// </summary>
        IEnumerable<IWebPage> Pages { get; }
    }
}
