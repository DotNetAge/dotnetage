//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Events;
using System;

namespace DNA.Web.ServiceModel.Observers
{
    /// <summary>
    /// Represents an observer class use to handle user register event and send confirm email.
    /// </summary>
    [BindTo(EventNames.Register)]
    public class SendRegisterEmail : HttpObserverBase<RegisterModel>
    {
        public override void Process(object sender, RegisterModel e)
        {
            var app = App.Get();
            var user = app.Users[e.UserName];
            user.VaildToken = Guid.NewGuid().ToString();
            var uri = app.Context.AppUrl.ToString() + "account/ConfirmEmail/" + user.VaildToken;

            Mails.Send(e.UserName, "Congratulations", "sys_register", new
            {
                dispName = user.DisplayName,
                user = user.UserName,
                password = user.Password,
                confirmUrl = uri,
                web = app.Webs["home"].Title
            });
        }
    }
}
