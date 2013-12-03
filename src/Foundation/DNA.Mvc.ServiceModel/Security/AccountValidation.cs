//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.Security
{
    public static class AccountValidation
    {
        public static string ErrorCodeToString(UserCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case UserCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";//"用户名已存在。请另输入一个用户名。"

                case UserCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";
                // "已存在与该电子邮件地址对应的用户名。请另输入一个电子邮件地址。"
                case UserCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";
                //提供的密码无效。请输入有效的密码值。
                case UserCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";
                // "提供的电子邮件地址无效。请检查该值并重试。"
                case UserCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";
                //"提供的密码取回答案无效。请检查该值并重试。"
                case UserCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";
                //"提供的密码取回问题无效。请检查该值并重试。"
                case UserCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";
                ////"提供的用户名无效。请检查该值并重试。"
                //case MembershipCreateStatus.ProviderError:
                //    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                ////"身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。"
                //case MembershipCreateStatus.UserRejected:
                //    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                ////"已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。"
                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                //"发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。"
            }
        }
    }
}
