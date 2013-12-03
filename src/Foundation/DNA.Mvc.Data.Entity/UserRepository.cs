//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

namespace DNA.Web.Data.Entity
{
    public class UserRepository : EntityRepository<User>, IUserRepository
    {
        public UserRepository() : base() { }

        public UserRepository(CoreDbContext dbContext) : base(dbContext) { }

        public bool Validate(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("userName");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password");

            var user = Find(userName);
            if (user == null)
                return false;
            if (user.PasswordFormat == 0)
                return user.Password.Equals(password);
            else
            {
                var encodepass = EncodePassword(password, user.PasswordFormat, user.PasswordSalt);
                return user.Password.Equals(encodepass);
            }
        }

        public override User Find(params object[] keys)
        {
            if (keys == null || (keys != null && keys[0] == null))
                return null;

            if ((keys[0].GetType().Equals(typeof(int))))
                return base.Find(keys);
            else
            {
                var userName = keys[0].ToString();
                return Find(u => u.UserName.Equals(userName));
            }
        }

        public UserCreateStatus CreateUser(string userName, string password, string email, string passwordQuestion = "",
            string passwordAnswer = "", bool isApproved = true)
        {
            var existUser = Find(userName);
            if (existUser != null)
                return UserCreateStatus.DuplicateUserName;

            if (Count(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) > 0)
                return UserCreateStatus.DuplicateEmail;

            var salt = GenerateSalt();
            var passwordFormat = 1;

            var user = new User()
            {
                UserName = userName,
                Password = passwordFormat > 0 ? EncodePassword(password, passwordFormat, salt) : password,
                PasswordSalt = salt,
                Email = email,
                PasswordFormat = passwordFormat,
                PasswordQuestion = string.IsNullOrEmpty(passwordQuestion) ? "" : passwordQuestion,
                PasswordAnswer = string.IsNullOrEmpty(passwordAnswer) ? "" : EncodePassword(passwordAnswer.Trim(), passwordFormat, salt),
                LastLoginDate = DateTime.Now,
                CreationDate = DateTime.Now,
                IsApproved = isApproved
            };

            //Add default profile
            var profile = new UserProfile()
            {
                User = user,
                UserName = userName,
                Account = userName,
                AppName = "dotnetage",
                IsDefault = true
            };

            DbSet.Add(user);

            Context.Set<UserProfile>().Add(profile);

            var guestRole = Context.Roles.FirstOrDefault(r => r.Name.Equals("guests"));
            user.Roles = new List<Role>();
            user.Roles.Add(guestRole);

            //Add default role
            //throw new Exception("Not complete");
            //Context.Set<UsersInRoles>().Add(new UsersInRoles()
            //{
            //    RoleName = "guests",
            //    UserName = userName
            //});

            if (!IsOwnContext)
                Context.SaveChanges();

            return UserCreateStatus.Success;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("userName");

            if (string.IsNullOrEmpty(oldPassword))
                throw new ArgumentException("oldPassword");

            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("newPassword");
            var user = Find(userName);

            if (user == null)
                throw new Exception("User not found:" + userName);

            if (Validate(userName, oldPassword))
            {
                if (user.PasswordFormat == 0)
                {
                    user.Password = newPassword;
                }
                else
                {
                    var encodepass = EncodePassword(newPassword, user.PasswordFormat, user.PasswordSalt);
                    user.Password = encodepass;
                }

                if (!IsOwnContext)
                    Context.SaveChanges();

                return true;
            }
            return false;
        }

        public string[] GetSuggestUserNames(string term, int maxResults)
        {
            var query = DbSet.Where(u => u.UserName.Contains(term)).Select(u => u.UserName).Take(maxResults);
            return query.ToArray();
        }

        private string _HashAlgorithm = null;

        private string GenerateSalt()
        {
            byte[] data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        private string EncodePassword(string pass, int passwordFormat, string salt)
        {
            byte[] buffer5;
            if (passwordFormat == 0)
            {
                return pass;
            }

            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Convert.FromBase64String(salt);
            byte[] inArray = null;

            if (passwordFormat == 1)
            {
                HashAlgorithm hashAlgorithm = this.GetHashAlgorithm();
                if (hashAlgorithm is KeyedHashAlgorithm)
                {
                    KeyedHashAlgorithm algorithm2 = (KeyedHashAlgorithm)hashAlgorithm;
                    if (algorithm2.Key.Length == src.Length)
                    {
                        algorithm2.Key = src;
                    }
                    else
                    {
                        byte[] buffer4;
                        if (algorithm2.Key.Length < src.Length)
                        {
                            buffer4 = new byte[algorithm2.Key.Length];
                            Buffer.BlockCopy(src, 0, buffer4, 0, buffer4.Length);
                            algorithm2.Key = buffer4;
                        }
                        else
                        {
                            int num2;
                            buffer4 = new byte[algorithm2.Key.Length];
                            for (int i = 0; i < buffer4.Length; i += num2)
                            {
                                num2 = Math.Min(src.Length, buffer4.Length - i);
                                Buffer.BlockCopy(src, 0, buffer4, i, num2);
                            }
                            algorithm2.Key = buffer4;
                        }
                    }
                    inArray = algorithm2.ComputeHash(bytes);
                }
                else
                {
                    buffer5 = new byte[src.Length + bytes.Length];
                    Buffer.BlockCopy(src, 0, buffer5, 0, src.Length);
                    Buffer.BlockCopy(bytes, 0, buffer5, src.Length, bytes.Length);
                    inArray = hashAlgorithm.ComputeHash(buffer5);
                }
            }

            //else
            //{
            //    buffer5 = new byte[src.Length + bytes.Length];
            //    Buffer.BlockCopy(src, 0, buffer5, 0, src.Length);
            //    Buffer.BlockCopy(bytes, 0, buffer5, src.Length, bytes.Length);
            //    inArray = this.EncryptPassword(buffer5);
            //}
            return Convert.ToBase64String(inArray);
        }

        private HashAlgorithm GetHashAlgorithm()
        {
            if (this._HashAlgorithm != null)
            {
                return HashAlgorithm.Create(this._HashAlgorithm);
            }

            string hashAlgorithmType = "SHA1";

            HashAlgorithm algorithm = HashAlgorithm.Create(hashAlgorithmType);
            if (algorithm == null)
            {
                throw new Exception("HashAlgException");
            }
            this._HashAlgorithm = hashAlgorithmType;
            return algorithm;
        }

        public void SetDefaultProfile(string userName, string appName, string account)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (string.IsNullOrEmpty(appName))
                throw new ArgumentNullException("appName");

            //var profile = context.Profiles.FirstOrDefault(pro => pro.AppName.Equals(appName, StringComparison.OrdinalIgnoreCase) &&
            //    pro.Account.Equals(account, StringComparison.OrdinalIgnoreCase) && pro.UserName.Equals(userName) );

            //if (profile != null)
            //{
            //    Context.Database.ExecuteSqlCommand("UPDATE dna_profiles SET IsDefault=0 WHERE UserName=N'{0}'", userName);
            //    profile.IsDefault = true;
            //}

            var profiles = Context.Profiles.Where(p => p.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            foreach (var pro in profiles)
                pro.IsDefault = pro.AppName.Equals(appName, StringComparison.OrdinalIgnoreCase) && pro.Account.Equals(account, StringComparison.OrdinalIgnoreCase);

            if (!IsOwnContext)
                Context.SaveChanges();
        }
    }
}
