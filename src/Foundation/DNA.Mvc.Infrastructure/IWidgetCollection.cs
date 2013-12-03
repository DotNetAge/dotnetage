//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    public interface IWidgetCollection : IEnumerable<WidgetData>
    {
        void Register<TModule>(string name, string title, string description = "", object preferences = null, string view = "index.cshtml", string category = "utilities") where TModule : class;
    }
}
