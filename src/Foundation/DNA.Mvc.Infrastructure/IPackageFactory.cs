//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web
{
    public interface IPackageFactory<T, TElement>
        where T : PackageBase<TElement>
        where TElement : class
    {
        T Create(string path);
    }
}
