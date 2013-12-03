//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web
{
    public class PackageCollection<T, TElement> : List<T>
        where T : PackageBase<TElement>
        where TElement : class
    {
        public T this[string name]
        {
            get
            {
                return this.FirstOrDefault(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
