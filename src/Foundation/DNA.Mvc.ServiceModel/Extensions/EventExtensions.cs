//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web.Mvc;

namespace DNA.Web.ServiceModel
{
    public static class EventExtensions
    {
        /// <summary>
        /// Trigger an event
        /// </summary>
        /// <param name="ctrl">The controller object.</param>
        /// <param name="eventName">The event name.</param>
        /// <param name="eventArg">The event argument object.</param>
        public static void Trigger(this Controller ctrl, string eventName, object eventArg = null)
        {
            App.Trigger(eventName, ctrl, eventArg);
        }

        /// <summary>
        /// Trigger an event
        /// </summary>
        /// <param name="ctrl">The content list object.</param>
        /// <param name="eventName">The event name.</param>
        /// <param name="eventArg">The event argument object.</param>
        public static void Trigger(this ContentListDecorator list, string eventName, object eventArg = null)
        {
            App.Trigger(eventName, list, eventArg);
        }

        public static void Trigger(this INetDriveService netdrive, string eventName, object eventArg = null)
        {
            App.Trigger(eventName, netdrive, eventArg);
        }

        public static void Trigger(this ContentDataItemDecorator item, string eventName, object eventArg = null)
        {
            App.Trigger(eventName, item, eventArg);
        }
    }
}
