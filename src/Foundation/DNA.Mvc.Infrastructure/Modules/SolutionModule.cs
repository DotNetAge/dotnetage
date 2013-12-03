//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)


namespace DNA.Web
{
    /// <summary>
    /// Represent a module that has route
    /// </summary>
    public abstract class SolutionModule:ServiceModule
    {
        /// <summary>
        /// Gets the additional namespaces register to route.
        /// </summary>
        public virtual string[] GetNamespaces() { return null; }
    }
}
