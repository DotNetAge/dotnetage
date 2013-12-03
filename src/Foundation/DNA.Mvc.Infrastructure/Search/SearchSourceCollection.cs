//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web
{
    public class SearchSourceCollection : List<ISearchSource>
    {
        public SearchSourceCollection(IEnumerable<ISearchSource> sources) { this.AddRange(sources); }

        public ISearchSource this[string name]
        {
            get
            {
                return this.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
