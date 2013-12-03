//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web.ServiceModel.StateProvinceProviders
{
    public abstract class StateProvinceProviderBase:IStateProvinceProvider
    {
        public virtual string ResourceClass { get; set; }

        public abstract string CountryCode { get; }

        public abstract IEnumerable<StateProvince> Items { get; }
    }
}
