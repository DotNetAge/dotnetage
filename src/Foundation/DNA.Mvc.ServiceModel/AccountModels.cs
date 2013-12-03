//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DNA.Web
{
    //[PropertiesMustMatch("NewPassword", "ConfirmPassword", 
    //    ErrorMessageResourceName = "RegisterModel_PasswordMustBeMatch",        
    //    ErrorMessageResourceType = typeof(Resources.language))]
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Old password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.language), ErrorMessageResourceName = "LogOnModel_UserNameRequried")]
        [DisplayName("User name")]
        public string UserName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.language), ErrorMessageResourceName = "LogOnModel_PasswordRequried")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }

    //[PropertiesMustMatch("Password", "ConfirmPassword", 
    //    ErrorMessageResourceName = "RegisterModel_PasswordMustBeMatch", 
    //    ErrorMessageResourceType = typeof(Resources.language))]
    public class RegisterModel
    {
        //[Required(AllowEmptyStrings = false, 
        //    ErrorMessageResourceType = typeof(Resources.language), 
        //    ErrorMessageResourceName = "LogOnModel_UserNameRequried")]
        //[StringLength(20, MinimumLength = 4, 
        //    ErrorMessageResourceName = "RegisterModel_UserNameLength", 
        //    ErrorMessageResourceType = typeof(Resources.language))]
       // [DisplayName("User name")]
        public string UserName { get; set; }

        //[Required]
        //[DataType(DataType.EmailAddress,
        //    ErrorMessageResourceType = typeof(Resources.language),
        //    ErrorMessageResourceName = "RegisterModel_EMailIncorrect")]
        //[DisplayName("Email")]
        public string Email { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.language), 
        //    ErrorMessageResourceName = "LogOnModel_PasswordRequried")]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[DisplayName("Password")]
        public string Password { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.language), 
        //    ErrorMessageResourceName = "RegisterModel_ConfirmPasswordRequired")]
        //[DataType(DataType.Password)]
        //[DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }

}
