//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Represest a contact address.
    /// </summary>
    public class Address : IAddress
    {
        public Address() { }

        public Address(IAddress addr)
        {
            City = addr.City;
            Company = addr.Company;
            Country = addr.Country;
            Email = addr.Email;
            Fax = addr.Fax;
            FirstName = addr.FirstName;
            LastName = addr.LastName;
            Latitude = addr.Latitude;
            Longitude = addr.Longitude;
            Mobile = addr.Mobile;
            Name = addr.Name;
            State = addr.State;
            Street = addr.Street;
            Tel = addr.Tel;
            Zip = addr.Zip;
        }

        /// <summary>
        /// Gets/Sets ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Indicate whehter the address is default.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets/Sets the address contact name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the contact email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets/Sets the telephone.
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// Gets/Sets the cell phone number.
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets/Sets the fax number.
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Gets/Sets the zipcode / postalcode
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Gets/Sets country name.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets/Sets the state name.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets /Sets the city name.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets/Sets the street.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets/Sets the company name.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets/Sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets/Sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets/Sets longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets/Sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Indicates the address is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Country) && !string.IsNullOrEmpty(State) && !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(Street) && !string.IsNullOrEmpty(Zip);
            }
        }

        /// <summary>
        /// Indicates the address is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(FirstName) &&
                    string.IsNullOrEmpty(LastName) &&
                    string.IsNullOrEmpty(Mobile) &&
                    string.IsNullOrEmpty(Tel) &&
                    string.IsNullOrEmpty(Street) &&
                    string.IsNullOrEmpty(City) &&
                    string.IsNullOrEmpty(State) &&
                    string.IsNullOrEmpty(Country) &&
                    string.IsNullOrEmpty(Zip);
            }
        }

        public override string ToString()
        {
            //if (IsValid)
            return string.Format("{0} {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", Guard(FirstName), Guard(LastName), Guard(Mobile), Guard(Tel), Guard(Street), Guard(City), Guard(State), Guard(Country), Guard(Zip));
            //            return "";
        }

        public string ToString(string culture)
        {
            if (culture.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", Guard(FirstName), Guard(LastName), Guard(Country), Guard(State), Guard(City), Guard(Street), Guard(Zip));
            else
                return ToString();
        }

        private string Guard(string val) { return string.IsNullOrEmpty(val) ? "" : val; }
    }
}
