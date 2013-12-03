//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to comment object model. 
    /// </summary>
    public class CommentDecorator : Comment
    {
        internal IDataContext DataContext { get; private set; }

        internal Comment Model { get; set; }

        /// <summary>
        /// Initializes a new instance of the CommentDecorator class with data context and comment model.
        /// </summary>
        /// <param name="dbContext">The data context</param>
        /// <param name="model">The comment model</param>
        public CommentDecorator(IDataContext dbContext, Comment model)
        {
            DataContext = dbContext;
            model.CopyTo(this);
            Model = model;
        }

        /// <summary>
        /// Get total reply count.
        /// </summary>
        public virtual int TotalReplies
        {
            get
            {
                return DataContext.Count<Comment>(c => c.Path.StartsWith(this.Path) && c.ID!=this.ID);
            }
        }

        /// <summary>
        /// Conver comment to dynamic object.
        /// </summary>
        /// <param name="profiles">The profile collection</param>
        /// <param name="appPath">The application path.</param>
        /// <returns></returns>
        public dynamic ToObject(IEnumerable<UserProfile> profiles = null, string appPath = "/")
        {
            var profile = profiles == null ?
                DataContext.Find<UserProfile>(u => u.UserName.Equals(this.UserName)) :
                profiles.FirstOrDefault(p => p.UserName.Equals(this.UserName));

            dynamic json = new ExpandoObject();
            json.id = this.ID;
            json.published = this.Posted;
            json.updated = this.Modified;
            json.location = new { lat = this.Latitude, lon = this.Longitude };
            json.approved = this.IsApproved;
            json.pingback = this.IsPingback;
            json.trackback = this.IsTrackback;
            json.refer = this.UrlReferrer;
            json.link = string.Format(appPath + "comments/{0}", this.ID);
            dynamic replies = new ExpandoObject();
            replies.url = string.Format(appPath + "api/comments/replies/{0}", this.ID);
            replies.totalItems = this.TotalReplies;

            json.replies = replies;

            dynamic actor = new ExpandoObject();
            actor.id = this.UserName;
            actor.url = string.Format(appPath + "profiles/{0}", this.UserName);
            actor.homePage = string.IsNullOrEmpty(this.HomePage) ? profile.HomePage : this.HomePage;
            actor.email = string.IsNullOrEmpty(this.Email) ? profile.Email : this.Email;
            actor.displayName = string.IsNullOrEmpty(profile.DisplayName) ? profile.UserName : profile.DisplayName;
            actor.image = new { url = string.IsNullOrEmpty(profile.Avatar) ? (appPath + "content/images/no-avatar.gif") : profile.Avatar };

            json.actor = actor;
            json.@object = new
            {
                objectType = "comment",
                content = this.Content,
            };
            return json;
        }
    }
}
