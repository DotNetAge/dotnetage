//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;

namespace DNA.Web.Events
{
    public class ContentListEventArgs
    {
        /// <summary>
        /// Gets/Sets the created ContentList object.
        /// </summary>
        public ContentListDecorator List { get; set; }
    }

    public class ContentDataItemEventArgs : ContentListEventArgs
    {
        /// <summary>
        /// Gets the new ContentDataItem object.
        /// </summary>
        public ContentDataItemDecorator DataItem { get; set; }
    }

    public class ContentDataItemDeletedEventArgs : ContentListEventArgs
    {
        /// <summary>
        /// Gets the deleted ContentDataItem id.
        /// </summary>
        public Guid ItemID { get; set; }

        /// <summary>
        /// Gets the deleted ContentDataItem data path.
        /// </summary>
        public string DataItemPath { get; set; }

        /// <summary>
        /// Gets the deleted ContentDataItem parent list object.
        /// </summary>
        public ContentListDecorator List { get; set; }
    }

    public class ContentDataItemVotedEventArgs : ContentDataItemEventArgs
    {
        /// <summary>
        /// Gets the user vote value.
        /// </summary>
        public int Votes { get; set; }

        /// <summary>
        /// Gets the average value.
        /// </summary>
        public double Average { get; set; }

        /// <summary>
        /// Gets the vote userName.
        /// </summary>
        public string UserName { get; set; }

    }

    public class ContentListDeletedEventArgs
    {
        /// <summary>
        /// Gets the deleted list name.
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// Gets the deleted list id.
        /// </summary>
        public int ListID { get; set; }

        /// <summary>
        /// Gets the deleted list locale name.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets the deleted list parent web.
        /// </summary>
        public Web Website { get; set; }
    }

    public class ContentFollowEventArgs
    {
        public string Follower { get; set; }

        public string FollowTo { get; set; }

        public string ListName { get; set; }

        public string Website { get; set; }

        public bool IsFollow { get; set; }
    }
}
