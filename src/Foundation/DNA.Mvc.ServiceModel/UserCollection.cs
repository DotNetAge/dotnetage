//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Security;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a collection of UserDecorators.
    /// </summary>
    public class UserCollection : IEnumerable<UserDecorator>
    {
        private IDataContext DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of  the UserCollection class using given data context object.
        /// </summary>
        /// <param name="context">The data context.</param>
        public UserCollection(IDataContext context)
        {
            DataContext = context;
        }

        /// <summary>
        /// Gets user by specified user name.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <returns>A user decorator that wraps the user model.</returns>
        public UserDecorator this[string username]
        {
            get
            {
                var user = DataContext.Users.Find(username);
                if (user != null)
                    return new UserDecorator(user, DataContext);
                return null;
            }
        }

        /// <summary>
        /// Find user by specified email address.
        /// </summary>
        /// <param name="email">The email to find.</param>
        /// <returns>A user decorator that wraps the user model.</returns>
        public UserDecorator FindByEmail(string email)
        {
            var user = DataContext.Users.Find(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user != null)
                return new UserDecorator(user, DataContext);
            return null;
        }

        /// <summary>
        /// Sign in with specified user name and password.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="persistent">Perisistent user cookie.</param>
        /// <returns>true if success otherwrise returns false.</returns>
        public bool SignIn(string userName, string password, bool persistent)
        {
            if (Validate(userName, password))
            {
                WebCache.Remove("_Identity_Cache");
                FormsAuthentication.SetAuthCookie(userName, persistent);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sign out current user.
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
            WebCache.Remove("_Identity_Cache");
        }

        /// <summary>
        /// Indicates whether the specified user exists.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>true if exists otherwrise false.</returns>
        public bool IsExists(string userName)
        {
            return this[userName] != null;
        }

        /// <summary>
        /// Create new user account.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password</param>
        /// <param name="email">The reigster email.</param>
        /// <param name="passwordQuestion">The password question.</param>
        /// <param name="passwordAnswer">The password security answer.</param>
        /// <param name="isApproved">Indictes new account is approved.</param>
        /// <returns>if user created returns UserCreateStatus.Success.</returns>
        public UserCreateStatus Create(string userName, string password, string email, string passwordQuestion = "", string passwordAnswer = "", bool isApproved = true)
        {
            return DataContext.Users.CreateUser(userName, password, email, passwordQuestion, passwordAnswer, isApproved);
        }

        /// <summary>
        /// Validate user by specified name and password.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns>true if valid otherwrise false.</returns>
        public bool Validate(string userName, string password)
        {
            return DataContext.Users.Validate(userName, password);
        }

        public IEnumerator<UserDecorator> GetEnumerator()
        {
            return DataContext.Users.All().ToList().Select(u => new UserDecorator(u, DataContext)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
