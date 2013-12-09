//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to UserProfile object model. 
    /// </summary>
    public class ProfileDecorator : UserProfile
    {
        private ICollection<Address> addresses;
        private UserProfile _model;

        public IDataContext DataContext { get; private set; }

        public UserProfile Model
        {
            get { return _model; }
            set
            {
                _model = value;
                _model.CopyTo(this, "User");
            }
        }

        public ProfileDecorator() { }

        /// <summary>
        /// Initializes a new instance of the ProfileDecorator class with data context object and user profile object.
        /// </summary>
        /// <param name="dbContext">The data context object.</param>
        /// <param name="model">The user profile model.</param>
        public ProfileDecorator(IDataContext dbContext, UserProfile model)
        {
            this.DataContext = dbContext;
            this.Model = model;
        }

        /// <summary>
        /// Save profile properties to database.
        /// </summary>
        public void Save()
        {
            this.CopyTo(this.Model, "User");
            DataContext.Update(this.Model);
            DataContext.SaveChanges();
            addresses = null;
        }

        /// <summary>
        /// Gets the person object.
        /// </summary>
        public virtual IPerson Person
        {
            get
            {
                return this.Model as IPerson;
            }
        }

        /// <summary>
        /// Gets the default address object.
        /// </summary>
        public virtual IAddress DefaultAddress
        {
            get
            {
                return this.Model as IAddress;
            }
        }

        /// <summary>
        /// Gets the address object by specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IAddress GetAddress(int id)
        {
            return DataContext.Find<Address>(a => a.ID.Equals(id) && a.Name.Equals(this.UserName));
        }

        /// <summary>
        /// Gets the user addresses
        /// </summary>
        public virtual ICollection<Address> Addresses
        {
            get
            {
                if (addresses == null)
                {
                    var _addresses = DataContext.Where<Address>(a => a.Name.Equals(this.UserName)).ToList();
                    var _defAddr=this.DefaultAddress;
                    var defAddr = _defAddr.ConvertTo<Address>();
                    defAddr.ID = 0;

                    defAddr.Tel = _defAddr.Tel;
                    defAddr.Street = _defAddr.Street;
                    defAddr.Zip = _defAddr.Zip;

                    _addresses.Insert(0, defAddr);
                    addresses = _addresses;
                }
                return addresses;
            }
        }
    }
}
