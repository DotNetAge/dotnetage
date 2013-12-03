//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;

namespace DNA.Web.ServiceModel.StateProvinceProviders
{
    //[Inject("IStateProvinceProvider")]
    public class USAProvincesProviders : StateProvinceProviderBase
    {
        public override string CountryCode
        {
            get { return "US"; }
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
                        new StateProvince("AK","Alaska" ),
                        new StateProvince("AL","Alabama"),
                        new StateProvince("AR", "Arkansas"),
                        new StateProvince("AZ","Arizona"),
                        new StateProvince("CA","California"),
                        new StateProvince("CO", "Colorado"),
                        new StateProvince("CT", "Connecticut"),
                        new StateProvince("DC","District of Columbia" ),
                        new StateProvince("DE", "Delaware"),
                        new StateProvince("FL", "Florida"),
                        new StateProvince("GA", "Georgia"),
                        new StateProvince("HI", "Hawaii"),
                        new StateProvince("IA","Iowa"),
                        new StateProvince("ID","Idaho"),
                        new StateProvince("IL","Illinois"),
                        new StateProvince("IN", "Indiana"),
                        new StateProvince("KS","Kansas"),
                        new StateProvince("KY","Kentucky" ),
                        new StateProvince("LA","Louisiana"),
                        new StateProvince("MA","Massachusetts" ),
                        new StateProvince("MD","Maryland" ),
                        new StateProvince("ME", "Maine"),
                        new StateProvince("MI", "Michigan"),
                        new StateProvince("MN","Minnesota" ),
                        new StateProvince("MO","Missouri" ),
                        new StateProvince("MS", "Mississippi"),
                        new StateProvince("MT","Montana" ),
                        new StateProvince("NC","North Carolina"),
                        new StateProvince("ND","North Dakota" ),
                        new StateProvince("NE","Nebraska"),
                        new StateProvince("NH", "New Hampshire"),
                        new StateProvince("NJ","New Jersey"),
                        new StateProvince("NM","New Mexico"),
                        new StateProvince("NV","Nevada"),
                        new StateProvince("NY","New York" ),
                        new StateProvince("OH", "Ohio"),
                        new StateProvince("OK","Oklahoma"),
                        new StateProvince("OR","Oregon"),
                        new StateProvince("PA","Pennsylvania"),
                        new StateProvince("PR","Puerto Rico"),
                        new StateProvince("RI", "Rhode Island"),
                        new StateProvince("SC", "South Carolina"),
                        new StateProvince("SD","South Dakota"),
                        new StateProvince("TN", "Tennessee"),
                        new StateProvince("TX", "Texas"),
                        new StateProvince("UT","Utah"),
                        new StateProvince("VA", "Virginia"),
                        new StateProvince("VT","Vermont" ),
                        new StateProvince("WA","Washington" ),
                        new StateProvince("WI", "Wisconsin"),
                        new StateProvince("WV","West Virginia"),
                        new StateProvince("WY","Wyoming")
                    };


                    foreach (var s in states)
                        s.CountryCode = this.CountryCode;
                }
                return states;
            }


        }
    }
}
