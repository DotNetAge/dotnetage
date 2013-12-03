//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Define the contact address properties
    /// </summary>
    public interface IAddress
    {
        /// <summary>
        /// Gets/Sets the contact user name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets/Sets the first name.
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        /// Gets/Sets the last name.
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        /// Gets/Sets the contact company name.
        /// </summary>
        string Company { get; set; }

        /// <summary>
        /// Gets/Sets the email address
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets/Sets the fax number.
        /// </summary>
        string Fax { get; set; }

        /// <summary>
        /// Gets/Sets the cell phone number.
        /// </summary>
        string Mobile { get; set; }

        /// <summary>
        /// Gets/Sets the country code
        /// </summary>
        string Country { get; set; }

        /// <summary>
        /// Gets/Sets the state code.
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// Gets/Sets the city name code.
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// Gets/Sets the street.
        /// </summary>
        string Street { get; set; }

        /// <summary>
        /// Gets/Sets the telephone number.
        /// </summary>
        string Tel { get; set; }

        /// <summary>
        /// Gets/Sets the zip code/ postal code number.
        /// </summary>
        string Zip { get; set; } 
        
        double Latitude { get; set; }

        double Longitude { get; set; }
    }
}
