//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace DNA.Web
{
    /// <summary>
    /// The User helper class.
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Identity whether the current user is administrator
        /// </summary>
        /// <param name="user">The user object</param>
        /// <returns></returns>
        public static bool IsAdministrator(this IPrincipal user)
        {
            if (user.Identity != null)
            {
                return App.Get().Users[user.Identity.Name].IsInRole("administrators");
                //return user.IsInRole("administrators");
                // return App.Get().Roles.GetUserRoles(user.Identity.Name).Contains("administrators");
                //return (Context.DataContext.Count<UsersInRoles>(u => user.Identity.Name.Equals(u.UserName) && u.RoleName.Equals("administrators")) > 0);
                //if (user.IsInRole("administrators"))
                //  return true;
            }

            return false;
        }

        /// <summary>
        /// Identity whether current user is a web master.
        /// </summary>
        /// <param name="user">The principal object.</param>
        /// <returns></returns>
        public static bool IsWebMaster(this IPrincipal user)
        {
            return App.Get().CurrentWeb.IsOwner(new HttpContextWrapper(HttpContext.Current));
        }

        /// <summary>
        /// Get MD5 hash code by specified input value.
        /// </summary>
        /// <param name="input">The input value</param>
        /// <returns>A string contains MD5 hash result.</returns>
        public static string MD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}