//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web.ServiceModel.StateProvinceProviders
{
   // [Inject("IStateProvinceProvider")]
    public class CanadianProvincesProvider : StateProvinceProviderBase
    {
        public override string CountryCode
        {
            get { return "CA"; }
        }

        private List<StateProvince> states = null;

        public override IEnumerable<StateProvince> Items
        {
            get
            {
                if (states == null)
                {
                    states = new List<StateProvince>()
                    {
                        new StateProvince("AB","Alberta"),
                        new StateProvince("BC","British Columbia"),
                        new StateProvince("MB", "Manitoba" ),
                        new StateProvince("NB","New Brunswick"),
                        new StateProvince("NF","Newfoundland" ),
                        new StateProvince("NT","Northwest Territories"),
                        new StateProvince("NS", "Nova Scotia"),
                        new StateProvince("NU","Nunavut"),
                        new StateProvince("ON","Ontario"),
                        new StateProvince("PE", "Prince Edward Island"),
                        new StateProvince("QC","Quebec"),
                        new StateProvince("SK","Saskatchewan" ),
                        new StateProvince("YT","Yukon Territory" )
                    };

                    foreach (var s in states)
                        s.CountryCode = this.CountryCode;
                }
                return states;
            }


        }
    }
}
