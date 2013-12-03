//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web.Events
{
    public class EventNames
    {
        #region Content events

        public const string ContentDataItemCreated = "ContentDataItemCreated";
        public const string ContentDataItemDeleted = "ContentDataItemDeleted";
        public const string ContentListCreated = "ContentListCreated";
        public const string ContentListDeleted = "ContentListDeleted";
        public const string ContentDataItemUpdated = "ContentDataItemUpdated";
        public const string ContentDataItemVoted = "ContentDataItemVoted";
        public const string UnfollowContentList = "UnfollowContentList";
        public const string FollowContentList = "FollowContentList";

        #endregion

        #region NetDrive events

        public const string FileDeleted = "FileDeleted";
        public const string PathDeleted = "PathDeleted";
        public const string FileSaved = "FileSaved";
        public const string FileRenamed = "FileRenamed";
        public const string PathRenamed = "PathRenamed";
        public const string PathCreated = "PathCreated";

        #endregion

        #region Account events
        /// <summary>
        /// Raise on user login
        /// </summary>
        /// <remarks>
        /// The event argument is LogOnModel
        /// </remarks>
        public const string Login = "Login";
        public const string LogOff = "LogOff";
        public const string Register = "Register";
        public const string PasswordChanged = "PasswordChanged";

        #endregion

        #region WebPage events

        public const string WebPageCreated = "WebPageCreated";
        public const string WebPageDeleted = "WebPageDeleted";
        public const string WebPageUrlChanged = "WebPageUrlChanged";
        public const string WebPageUpdated = "WebPageUpdated";

        #endregion

        public const string ThemeUninstalled = "ThemeUninstalled";
        public const string WidgetUninstalled = "WidgetUninstalled";
        public const string WidgetRegistered = "WidgetRegistered";
        public const string WidgetUnregistered = "WidgetUnregistered";
    }
}
