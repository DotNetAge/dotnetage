//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;

namespace DNA.Web
{
    /// <summary>
    /// Defines the repository for user data.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Validate the user name and password.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user password.</param>
        /// <returns>true is validated otherwise false</returns>
        bool Validate(string userName, string password);

        /// <summary>
        /// Create user 
        /// </summary>
        /// <param name="userName">The new user name.</param>
        /// <param name="password">The new user password.</param>
        /// <param name="email">The new user email address.</param>
        /// <param name="passwordQuestion">The password question.</param>
        /// <param name="passwordAnswer">The password answer.</param>
        /// <param name="isApproved">Specified whether the user account is approved.</param>
        /// <returns>A result value of the UserCreateStatus.</returns>
        UserCreateStatus CreateUser(string userName, string password, string email, string passwordQuestion="", string passwordAnswer="", bool isApproved=true);

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="oldPassword">The user old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>true is success otherwise fail.</returns>
        bool ChangePassword(string userName, string oldPassword, string newPassword);

        /// <summary>
        /// Gets suggest user name by specified user name search term.
        /// </summary>
        /// <param name="term">The search term value.</param>
        /// <param name="maxResults">Specified max results count returns.</param>
        /// <returns>An string array contains user name search result.</returns>
        string[] GetSuggestUserNames(string term, int maxResults);

        /// <summary>
        /// Set the profile as default.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="appName">The application name.</param>
        /// <param name="account">The application account.</param>
        void SetDefaultProfile(string userName, string appName,string account);
    }

    /// <summary>
    /// Describes the result of a Create user operation.
    /// </summary>
    public enum UserCreateStatus
    {
        /// <summary>
        /// The user was successfully created.
        /// </summary>
        Success = 0,
        /// <summary>
        /// The user name was not found in the database.
        /// </summary>
        InvalidUserName = 1,
        /// <summary>
        ///  The password is not formatted correctly.
        /// </summary>
        InvalidPassword = 2,
        /// <summary>
        /// The password question is not formatted correctly.
        /// </summary>
        InvalidQuestion = 3,
        /// <summary>
        /// The password answer is not formatted correctly.
        /// </summary>
        InvalidAnswer = 4,
        /// <summary>
        /// The e-mail address is not formatted correctly.
        /// </summary>
        InvalidEmail = 5,
        /// <summary>
        /// The user name already exists in the database for the application.
        /// </summary>
        DuplicateUserName = 6,
        /// <summary>
        ///  The e-mail address already exists in the database for the application.
        /// </summary>
        DuplicateEmail = 7,
        /// <summary>
        /// The user was not created, for a reason defined by the provider.
        /// </summary>
        UserRejected = 8
    }
}
