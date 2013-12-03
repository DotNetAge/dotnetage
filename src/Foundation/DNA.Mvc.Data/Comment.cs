//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Represents a comment 
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Gets/Sets the command id.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets comment id which reply to.
        /// </summary>
        public int ReplyTo { get; set; }

        /// <summary>
        /// Gets/Sets the comment path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets/Sets the target comment uri
        /// </summary>
        public string TargetUri { get; set; }

        /// <summary>
        /// Gets/Sets comment body.
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// Get/Sets the user email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets/Sets the IP address on comment creation.
        /// </summary>
        public virtual string IP { get; set; }

        /// <summary>
        /// Gets/Sets the datetime when comment posted.
        /// </summary>
        public virtual DateTime Posted { get; set; }

        /// <summary>
        /// Gets/Sets the latest modified date time.
        /// </summary>
        public virtual DateTime Modified { get; set; }

        /// <summary>
        /// Gets/Sets the user name of the comment owner.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets/Sets the home page url.
        /// </summary>
        public virtual string HomePage { get; set; }

        /// <summary>
        /// Indicates whether the comment is create from ping back.
        /// </summary>
        public virtual bool IsPingback { get; set; }

        /// <summary>
        /// Indicates the comment is trackback post.
        /// </summary>
        public virtual bool IsTrackback { get; set; }

        /// <summary>
        /// Indicates whether the comment is approved.
        /// </summary>
        public virtual bool IsApproved { get; set; }

        /// <summary>
        /// Gets/Sets the refer url.
        /// </summary>
        public virtual string UrlReferrer { get; set; }

        /// <summary>
        /// Gets/Sets longitude.
        /// </summary>
        public virtual double Longitude { get; set; }

        /// <summary>
        /// Gets/Sets latiude.
        /// </summary>
        public virtual double Latitude { get; set; }

        /// <summary>
        /// Gets/Sets the poster address.
        /// </summary>
        public virtual string Address { get; set; }
    }
}
