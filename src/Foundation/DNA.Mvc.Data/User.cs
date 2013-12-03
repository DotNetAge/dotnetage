//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a user use to instead the Membership object.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets/Sets the ID.
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// Gets/Sets the user name.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets/Sets the user register email address.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets/Sets user password.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets/Sets the password salt.
        /// </summary>
        public virtual string PasswordSalt { get; set; }

        /// <summary>
        /// Gets/Sets the password format
        /// </summary>
        public virtual int PasswordFormat { get; set; }

        /// <summary>
        /// Gets/Sets the password security question.
        /// </summary>
        public virtual string PasswordQuestion { get; set; }

        /// <summary>
        /// Gets/Sets the password security answer.
        /// </summary>
        public virtual string PasswordAnswer { get; set; }

        /// <summary>
        /// Gets/Sets the default web site name.
        /// </summary>
        public virtual string DefaultWeb { get; set; }

        /// <summary>
        /// Indicates whether the user account is apprvoed.
        /// </summary>
        public virtual bool IsApproved { get; set; }

        /// <summary>
        /// Indicates whether the user email is valid.
        /// </summary>
        public virtual bool IsVaildMail { get; set; }

        /// <summary>
        /// Gets/Sets the user latest login IP address
        /// </summary>
        public virtual string LastLoginIP { get; set; }

        /// <summary>
        /// Gets/Sets the vaild token 
        /// </summary>
        public virtual string VaildToken { get; set; }

        /// <summary>
        /// Gets/Sets the retrieval token.
        /// </summary>
        public virtual string RetrievalToken { get; set; }

        /// <summary>
        /// Gets/Sets the exprie date of the token.
        /// </summary>
        public virtual DateTime? TokenExpired { get; set; }

        /// <summary>
        /// Gets/Sets the last login date.
        /// </summary>
        public virtual DateTime LastLoginDate { get; set; }

        /// <summary>
        /// Gets/Sets the user account creation date.
        /// </summary>
        public virtual DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets/Sets the netdrive quota.
        /// </summary>
        public virtual int NetDriveQuota { get; set; }

        /// <summary>
        /// Gets/Sets the user profiles.
        /// </summary>
        public virtual ICollection<UserProfile> Profiles { get; set; }

        /// <summary>
        /// Gets/Sets the user access roles.
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }

        private UserProfile defaultProfile = null;

        /// <summary>
        /// Gets/Sets the user default profile object.
        /// </summary>
        public virtual UserProfile DefaultProfile
        {
            get
            {
                if (defaultProfile == null)
                {
                    if (Profiles != null)
                        defaultProfile = Profiles.FirstOrDefault(p => p.IsDefault);
                }

                return defaultProfile;
            }
        }

    }
}
