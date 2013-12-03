//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web
{
    public interface IWidget
    {
        string UID { get; }

        string CssText { get; }

        string BodyClass { get; }

        string BodyCssText { get; }

        string HeaderClass { get; }

        string HeaderCssText { get; }

        bool ShowHeader { get; }
        
        bool IsStatic { get; }

        string Title { get; }

        string IconUrl { get; }

        string Link { get; }

        string Data { get; }

        int Pos { get; }

        string ZoneID { get; }

        string ShowMode { get; }

        string ViewMode { get; }
    }
}
