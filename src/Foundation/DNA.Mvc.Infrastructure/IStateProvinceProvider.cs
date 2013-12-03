//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// The state provinces provider
    /// </summary>
    [Inject]
    public interface IStateProvinceProvider
    {
        /// <summary>
        /// Gets the three letters country code
        /// </summary>
        string CountryCode { get; }

        /// <summary>
        /// Gets all state/province items.
        /// </summary>
        IEnumerable<StateProvince> Items { get; }
    }
}
