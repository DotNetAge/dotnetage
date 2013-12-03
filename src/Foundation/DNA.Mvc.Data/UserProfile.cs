//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Dynamic;

namespace DNA.Web
{
    /// <summary>
    /// Represents a user profile.
    /// </summary>
    public class UserProfile : DynamicObject, IAddress, IPerson
    {
        public virtual int ID { get; set; }

        /// <summary>
        /// Get/Sets the user name.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets/Sets the street 
        /// </summary>
        public virtual string Address { get; set; }

        /// <summary>
        /// Gets/Sets the user photo
        /// </summary>
        public virtual string Avatar { get; set; }

        /// <summary>
        /// Gets/Sets the birthday
        /// </summary>
        public virtual DateTime? Birthday { get; set; }

        /// <summary>
        /// Gets/Sets the city name.
        /// </summary>
        public virtual string City { get; set; }

        /// <summary>
        /// Gets/Sets the country name
        /// </summary>
        public virtual string Country { get; set; }
        
        /// <summary>
        /// Gets/Sets the person's company.
        /// </summary>
        public virtual string Company { get; set; }

        /// <summary>
        /// Gets/Sets the user display name.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets/Sets the register email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets/Sets the user first name.
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Gets/Sets the gender.
        /// </summary>
        public virtual string Gender { get; set; }

        /// <summary>
        /// Gets/Sets the home page url.
        /// </summary>
        public virtual string HomePage { get; set; }

        /// <summary>
        /// Gets/Sets the language name.
        /// </summary>
        public virtual string Language { get; set; }

        /// <summary>
        /// Gets/Sets the lastname.
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Gets/Sets the latitiude of default address.
        /// </summary>
        public virtual double Latitude { get; set; }

        /// <summary>
        /// Gets/Sets longitude of default address.
        /// </summary>
        public virtual double Longitude { get; set; }

        /// <summary>
        /// Gets/Sets the mobile number.
        /// </summary>
        public virtual string Mobile { get; set; }

        /// <summary>
        /// Gets/Sets the telephone number.
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        /// Gets/Sets the user signature.
        /// </summary>
        public virtual string Signature { get; set; }

        /// <summary>
        /// Gets/Sets the contact state.
        /// </summary>
        public virtual string State { get; set; }

        /// <summary>
        /// Gets/Sets the user theme.
        /// </summary>
        public virtual string Theme { get; set; }

        /// <summary>
        /// Gets/Sets the user locale name.
        /// </summary>
        public virtual string Locale { get; set; }

        /// <summary>
        /// Gets/Sets the time zone.
        /// </summary>
        public virtual string TimeZone { get; set; }

        /// <summary>
        /// Gets/Sets the zip/postal code.
        /// </summary>
        public virtual string ZipCode { get; set; }

        /// <summary>
        /// Gets/Sets the custom data.
        /// </summary>
        public virtual string Data { get; set; }

        /// <summary>
        /// Indicates whether the profile is default.
        /// </summary>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Gets/Sets the profile's application name.
        /// </summary>
        public virtual string AppName { get; set; }

        /// <summary>
        /// Gets/Sets the user points.
        /// </summary>
        public virtual int Points { get; set; }

        /// <summary>
        /// Gets/Sets the fax number.
        /// </summary>
        public virtual string Fax { get; set; }

        /// <summary>
        /// Gets/Sets the tax id.
        /// </summary>
        public virtual string TaxID { get; set; }

        /// <summary>
        /// Gets/Sets the currency code.
        /// </summary>
        public virtual string CurrencyCode { get; set; }

        /// <summary>
        /// Gets/Sets the middle name.
        /// </summary>
        public virtual string MiddleName { get; set; }

        /// <summary>
        /// Gets/Sets the specified app account name
        /// </summary>
        public virtual string Account { get; set; }

        /// <summary>
        /// Gets/Sets the user profile url.
        /// </summary>
        public virtual string Link { get; set; }

        /// <summary>
        /// Gets/Sets the user account id.
        /// </summary>
        public virtual int UserID { get; set; }

        /// <summary>
        /// Gets/Sets the user object.
        /// </summary>
        public virtual User User { get; set; }

        #region Impletement IAddress inerface


        string IAddress.Name
        {
            get
            {
                return this.UserName;
            }
            set
            {
            }
        }

        string IAddress.Street
        {
            get
            {
                return this.Address;
            }
            set
            {
                this.Address = value;
            }
        }

        string IAddress.Tel
        {
            get
            {
                return this.Phone;
            }
            set
            {
                this.Phone = value;
            }
        }

        string IAddress.Zip
        {
            get
            {
                return this.ZipCode;
            }
            set
            {
                this.ZipCode = value;
            }
        }

        #endregion

        #region Impletement IPerson interface

        string IPerson.Photo
        {
            get
            {
                return this.Avatar;
            }
            set
            {
                this.Avatar = value;
            }
        }

        string IPerson.About
        {
            get
            {
                return this.Signature;
            }
            set
            {
                this.Signature = value;
            }
        }
        #endregion
    }

}
