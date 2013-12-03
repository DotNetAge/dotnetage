//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    public interface IWebPage
    {
        string Description { get; }

        string IconUrl { get; }

        string ImageUrl { get; }

        bool IsShared { get; }

        bool IsStatic { get; }

        string Keywords { get; }

        string LinkTo { get; }

        bool NoFollow { get; }

        string Slug { get; }

        int Pos { get; }

        bool ShowInMenu { get; }

        bool ShowInSitemap { get; }

        bool AllowAnonymous { get; }

        string Target { get; }

        string Title { get; }

        string ViewData { get; }

        string ViewName { get; }

        string Locale { get; }

        string Dir { get; }

        int ViewMode { get; }

        string StartupScripts { get; }

        string StyleSheets { get;}

        string CssText { get;  }

        string Scripts { get; }

        IEnumerable<IWidget> Widgets { get; }

        IEnumerable<IWebPage> Children { get; }
    }
}
