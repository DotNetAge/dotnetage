//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using DNA.Data.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a decorator object that use to add logical methods and properties to User model. 
    /// </summary>
    public class UserDecorator : User, IIdentity, IPrincipal
    {
        #region Private

        private User Model { get; set; }
        //private string[] _roles = null;
        private IEnumerable<Permission> rolesPermissions = null;
        private int[] permHashValues = null;
        private CommentCollection comments = null;
        private IQueryable<UserProfile> followers;
        private IQueryable<UserProfile> follows;
        private IDataContext Context { get; set; }
        private IEnumerable<WebDecorator> webs = null;
        private IUnitOfWorks blobs;
        private IQueues queues;
        //private IRepository<ShortMessage> inbox = null;
        //private IRepository<ShortMessage> outbox = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of UserDecorator class  for testing.
        /// </summary>
        public UserDecorator() { }

        /// <summary>
        /// Initializes a new instance of UserDecorator class with given user model and data context object.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <param name="context">The data context object.</param>
        public UserDecorator(User user, IDataContext context)
        {
            Model = user;
            user.CopyTo(this, new string[] { "Roles" });
            Context = context;
        }

        //public new string[] Roles
        //{
        //    get
        //    {
        //        if (_roles == null)
        //            _roles = Context.Where<UsersInRoles>(u => u.UserName.Equals(UserName)).Select(u => u.RoleName).ToArray();
        //        return _roles;
        //    }
        //}

        /// <summary>
        /// Remove user from all roles.
        /// </summary>
        /// <returns>The current user decorator object.</returns>
        public UserDecorator ClearRoles()
        {
            //foreach (var m in Model.Roles)
            //Context.Delete(m);
            Model.Roles.Clear();
            Context.Update(Model);
            Context.SaveChanges();
            //_roles = null;
            return this;
        }

        /// <summary>
        /// Add user to specified roles.
        /// </summary>
        /// <param name="roles">The roles to add.</param>
        /// <returns>The current user decorator object.</returns>
        public UserDecorator AddToRoles(params string[] roles)
        {
            if (roles == null)
                throw new ArgumentNullException("roles");

            foreach (var r in roles)
            {
                if (IsInRole(r)) continue;
                AddToRole(r);
            }
            return this;
        }

        /// <summary>
        /// Add user to specified role.
        /// </summary>
        /// <param name="role">The role to add.</param>
        /// <returns>The current user decorator object.</returns>
        public UserDecorator AddToRole(string role)
        {
            if (IsInRole(role))
                throw new Exception("The role is exists");

            var _role = Context.Find<Role>(r => r.Name.Equals(role));

            if (_role == null)
                throw new Exception(string.Format("{0} role is not exists.", role));

            if (this.Model.Roles == null)
                this.Model.Roles = new List<Role>();

            this.Model.Roles.Add(_role);
            Context.Update(this.Model);
            Context.SaveChanges();

            return this;
        }

        /// <summary>
        /// Remove user from specified roles.
        /// </summary>
        /// <param name="roles">The roles to remove to.</param>
        /// <returns>The current user decorator object.</returns>
        public UserDecorator RemoveFromRoles(string[] roles)
        {
            foreach (var r in roles)
                RemoveFromRole(r);
            return this;
        }

        /// <summary>
        /// Remove user from specified role.
        /// </summary>
        /// <param name="role">The role to remove to.</param>
        /// <returns>The current user decorator object.</returns>
        public UserDecorator RemoveFromRole(string role)
        {
            if (!IsInRole(role))
                throw new Exception("User not in role");
            var _r = Model.Roles.First(r => r.Name.Equals(role));

            Model.Roles.Remove(_r);
            Context.Update(Model);
            Context.SaveChanges();

            return this;
        }

        /// <summary>
        /// Indicates whether the user has specified role.
        /// </summary>
        /// <param name="role">The role name.</param>
        /// <returns>true if the user has this role otherwrise false.</returns>
        public bool IsInRole(string role)
        {
            if (this.Model.Roles != null)
            {
                var _roles = this.Model.Roles.Select(r => r.Name);
                return _roles.Contains(role);
            } return false;
        }

        /// <summary>
        ///  Indicates whether the user has one of the specified roles.
        /// </summary>
        /// <param name="roles">The role names.</param>
        /// <returns>true if the user has one of the roles otherwrise false.</returns>
        public bool IsInRoles(string[] roles)
        {
            if (this.Model.Roles != null)
            {
                var _roles = this.Model.Roles.Select(r => r.Name);
                foreach (var r in roles)
                {
                    if (_roles.Contains(r))
                        return true;
                }


            } return false;
        }

        /// <summary>
        /// Indicates whether the user has the sepcified permission.
        /// </summary>
        /// <param name="perm">The permission object.</param>
        /// <returns>true is authorized otherwrise false.</returns>
        public bool IsAuthorized(Permission perm)
        {
            if (IsInRole("administrators"))
                return true;

            return Context.Permissions.IsAuthorized(perm, Roles.Select(r => r.Name).ToArray());
        }

        /// <summary>
        /// Indicates whether the user has the sepcified action.
        /// </summary>
        /// <typeparam name="T">The controller type</typeparam>
        /// <param name="action">The action name.</param>
        /// <returns>true is authorized otherwrise false.</returns>
        public bool IsAuthorized<T>(string action)
        {
            if (string.IsNullOrEmpty(action))
                return false;

            if (IsInRole("administrators"))
                return true;

            if ((Permissions != null) && (permHashValues != null))
            {
                var code = (typeof(T).FullName + "." + action).ToLower().GetHashCode();
                return permHashValues.Contains(code);
            }
            return false;
        }

        /// <summary>
        /// Gets the user permissions.
        /// </summary>
        public IEnumerable<Permission> Permissions
        {
            get
            {
                if ((rolesPermissions == null) && (HttpContext.Current.Request.IsAuthenticated))
                {
                    rolesPermissions = Context.Permissions.GetUserPermissions(this.UserName);
                    if ((rolesPermissions != null) && (rolesPermissions.Count() > 0))
                        permHashValues = rolesPermissions.Select(p => (p.Controller + "." + p.Action).ToLower().GetHashCode()).ToArray();
                }
                return rolesPermissions;
            }
        }

        /// <summary>
        /// Change the user password.
        /// </summary>
        /// <param name="oldpassword">The old password.</param>
        /// <param name="password">The new password.</param>
        /// <returns>true if success otherwrise false.</returns>
        public bool ChangePassword(string oldpassword, string password)
        {
            return Context.Users.ChangePassword(UserName, oldpassword, password);
        }

        /// <summary>
        /// Add comment to specified uri.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="content">The comment content.</param>
        /// <param name="isApproved">Indicates the comment can be seem by others.</param>
        /// <param name="replyTo">The command id to reply to .</param>
        /// <param name="lat">The latitude of the user's location.</param>
        /// <param name="lon">The longitude of the users's location.</param>
        /// <returns>A new comment object.</returns>
        public CommentDecorator AddComment(string uri, string content, bool isApproved = true, int replyTo = 0, float lat = 0, float lon = 0)
        {
            var Request = HttpContext.Current.Request;
            var comment = new Comment()
            {
                TargetUri = uri,
                Posted = DateTime.Now,
                Modified = DateTime.Now,
                UserName = this.UserName,
                Email = this.Email,
                HomePage = this.DefaultProfile.HomePage,
                Latitude = lat,
                Longitude = lon,
                Content = content,
                ReplyTo = replyTo,
                IP = Request.UserHostAddress,
                IsApproved = isApproved,
                UrlReferrer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : ""
            };

            Context.Add(comment);
            Context.SaveChanges();

            Comment reply = null;

            if (replyTo > 0)
            {
                reply = Context.Find<Comment>(replyTo);
                if (reply != null)
                    comment.Path = reply.Path + comment.ID.ToString() + "/";
            }
            else
                comment.Path = comment.ID.ToString() + "/";

            Context.SaveChanges();

            return new CommentDecorator(Context, comment);
        }

        /// <summary>
        /// Gets the user comments.
        /// </summary>
        public CommentCollection Comments
        {
            get
            {
                if (comments == null)
                    comments = new CommentCollection(Context, "", this.UserName);
                return comments;
            }
        }

        /// <summary>
        /// Gets the followers who the follows to current user.
        /// </summary>
        public IQueryable<UserProfile> Followers
        {
            get
            {
                if (followers == null)
                {
                    var usernames = Context.Where<Follow>(f => f.Follower.Equals(UserName)).Select(f => f.Owner).Distinct().ToArray();
                    followers = Context.Where<UserProfile>(p => usernames.Contains(p.UserName));
                }
                return followers;
            }
        }

        /// <summary>
        /// Gets user profiles who the current user follows.
        /// </summary>
        public IQueryable<UserProfile> Follows
        {
            get
            {
                if (follows == null)
                {
                    var usernames = Context.Where<Follow>(f => f.Owner.Equals(UserName)).Select(f => f.Follower).Distinct().ToArray();
                    follows = Context.Where<UserProfile>(p => usernames.Contains(p.UserName));
                }
                return follows;
            }
        }

        /// <summary>
        /// Returns the specified user is following this user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool IsFollowing(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (userName == this.UserName)
                return true;

            return Context.Count<Follow>(f => f.Owner.Equals(this.UserName) && f.Follower.Equals(userName)) > 0;
        }

        /// <summary>
        /// Follow to specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool Follow(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (userName == UserName)
                return false;

            if (IsFollowing(userName))
                return true;

            var listIds = Context.Where<ContentList>(l => l.Owner.Equals(userName)).Select(l => l.ID).Distinct().ToArray();

            foreach (var id in listIds)
            {
                Context.Add(new Follow()
                {
                    Owner = userName,
                    ListID = id,
                    Follower = this.UserName
                });
            }

            followers = null;

            return Context.SaveChanges() > 0;
        }

        public bool IsLike(string url) { throw new NotImplementedException(); }

        public bool Like(string url)
        {
            throw new NotImplementedException();
        }

        public bool Unlike(string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unfollow the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool Unfollow(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (userName == UserName)
                return false;

            Context.Delete<Follow>(f => f.Follower.Equals(this.UserName) && f.Owner.Equals(userName));
            followers = null;

            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Indicates whether the user connected to specified network application.
        /// </summary>
        /// <param name="app">The application name. e.g: facebook</param>
        /// <param name="account">The network account.</param>
        /// <returns></returns>
        public bool IsConnected(string app, string account)
        {
            return Context.Count<UserProfile>(c => c.AppName.Equals(app) && (c.UserName.Equals(this.UserName)) && c.Account.Equals(account)) > 0;
        }

        /// <summary>
        /// Add new profile to user profiles by connect to other network.
        /// </summary>
        /// <param name="profile">The user profile </param>
        public void Connect(UserProfile profile)
        {
            if (!IsConnected(profile.AppName, profile.Account))
            {
                if (!string.IsNullOrEmpty(profile.DisplayName))
                    profile.DisplayName = HttpContext.Current.Server.UrlDecode(profile.DisplayName);
                profile.UserName = this.UserName;
                Context.Add(profile);
                Context.SaveChanges();
            }
        }

        public void SetDefaultProfile(string app, string account)
        {
            Context.Users.SetDefaultProfile(this.UserName, app, account);
        }

        public UserProfile FindProfile(string app, string account)
        {
            return Context.Find<UserProfile>(c => c.AppName.Equals(app) && (c.UserName.Equals(this.UserName)) && c.Account.Equals(account));
        }

        /// <summary>
        /// Copy other application profile to current profile.
        /// </summary>
        /// <param name="app">The application name.</param>
        /// <param name="account">The application account name.</param>
        /// <returns></returns>
        public bool SyncProfile(string app, string account)
        {
            if (string.IsNullOrEmpty(app))
                throw new ArgumentNullException("app");

            if (string.IsNullOrEmpty(account))
                throw new ArgumentNullException("account");

            if (!app.Equals("dotnetage"))
            {
                var profile = FindProfile(app, account);
                var local = LocalProfile();
                profile.CopyTo(local, "User", "ID", "AppName", "IsDefault", "Account");
                Context.Update(local);
                Context.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the user profile for DotNetAge.
        /// </summary>
        /// <returns>A profile object.</returns>
        public UserProfile LocalProfile()
        {
            return this.FindProfile("dotnetage", this.UserName);
        }

        /// <summary>
        /// Gets the user page url
        /// </summary>
        public string Url
        {
            get
            {
                return "~/social/users/" + this.UserName;
            }
        }

        /// <summary>
        /// Get the user display name.
        /// </summary>
        /// <remarks>
        /// If the does not set the display name in his/her profile, this property will return the user name as display name
        /// </remarks>
        public string DisplayName
        {
            get
            {
                var dispName = this.DefaultProfile.DisplayName;
                if (string.IsNullOrEmpty(dispName))
                    dispName = this.UserName;
                return dispName;
            }
        }

        /// <summary>
        /// Gets all activity stream items for current user.
        /// </summary>
        /// <returns></returns>
        public IQueryable<ContentDataItem> ActivityItems()
        {
            //Get following lists
            var followingListIDs = Context.Where<Follow>(f => f.Follower.Equals(this.UserName)).Select(l => l.ListID).ToArray().Distinct();
            var myListIDs = Context.Where<ContentList>(c => c.Owner.Equals(this.UserName) && c.IsActivity).Select(l => l.ID).ToArray();
            var untions = new List<int>();

            if (followingListIDs != null && followingListIDs.Count() > 0)
                untions.AddRange(followingListIDs);

            if (myListIDs != null && myListIDs.Count() > 0)
                untions.AddRange(myListIDs);

            return Context.Where<ContentDataItem>(c => c.IsCurrentVersion && c.IsPublished &&
                untions.Contains(c.ParentID)).OrderByDescending(c => c.Modified).AsQueryable();
        }

        /// <summary>
        /// Save the user properties to database.
        /// </summary>
        /// <returns>true if success otherwrise false.</returns>
        public bool Save()
        {
            this.CopyTo(Model, "Profiles", "Roles");
            Context.Update(Model);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Get all abuses that reported by other peoples.
        /// </summary>
        /// <returns>A queryable abuse collection.</returns>
        public IQueryable<Abuse> GetAbuses()
        {
            return Context.Where<Abuse>(a => a.Owner.Equals(this.UserName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get all abuses reported from current user.
        /// </summary>
        /// <returns>A queryable abuse collection.</returns>
        public IQueryable<Abuse> GetReportAbuses()
        {
            return Context.Where<Abuse>(a => a.Reportor.Equals(this.UserName, StringComparison.OrdinalIgnoreCase));
        }

        #region Implement the IIdentity interface

        string IIdentity.AuthenticationType
        {
            get { return "Forms"; }
        }

        bool IIdentity.IsAuthenticated
        {
            get
            {
                //Get user cookie
                var cookieName = FormsAuthentication.FormsCookieName;
                return HttpContext.Current.Request.Cookies[cookieName] != null;
            }
        }

        string IIdentity.Name
        {
            get { return this.UserName; }
        }

        #endregion

        #region Implement the IPrincipal interface

        IIdentity IPrincipal.Identity
        {
            get { return (IIdentity)this; }
        }

        bool IPrincipal.IsInRole(string role)
        {
            return this.IsInRole(role);
        }

        #endregion

        ///// <summary>
        ///// Send message to user
        ///// </summary>
        ///// <param name="to">The receiver.</param>
        ///// <param name="subject">The email subject.</param>
        ///// <param name="body">The email body.</param>
        //public void MessageTo(UserDecorator to, string subject, string body)
        //{
        //    var message = new Message()
        //    {
        //        Subject = subject,
        //        Body = body,
        //        ContentType = "text/html",
        //        Creation = DateTime.Now
        //    };

        //    Storage.Add(new OutboxMessage(message) { To = to.UserName });
        //    Storage.SaveChanges();

        //    to.Storage.Add(new InboxMessage(message) { Sender = UserName });
        //    to.Storage.SaveChanges();
        //}


        /// <summary>
        /// Gets the webs belong to current user. 
        /// </summary>
        public IEnumerable<WebDecorator> Webs
        {
            get
            {
                if (webs == null)
                    webs = Context.Where<Web>(w => w.Owner.Equals(this.UserName)).ToList().Select(w => new WebDecorator(w, Context));
                return webs;
            }
        }

        /// <summary>
        /// Gets user default profile 
        /// </summary>
        public override UserProfile DefaultProfile
        {
            get
            {
                if (this.Profiles == null)
                    this.Profiles = Context.Where<UserProfile>(u => u.ID.Equals(this.ID)).ToList();
                return base.DefaultProfile;
            }
        }

        /// <summary>
        /// Gets the user default address object.
        /// </summary>
        public Address DefaultAddress
        {
            get
            {
                var _addr = this.DefaultProfile as IAddress;
                 var addr=_addr.ConvertTo<Address>();
                 addr.Tel = _addr.Tel;
                 addr.Zip = _addr.Zip;
                 addr.Street = _addr.Street;
                 return addr;
            }
        }

        /// <summary>
        /// Convert to user to dynamic object for json format.
        /// </summary>
        /// <returns></returns>
        public dynamic ToObject()
        {
            // dynamic actor = new ExpandoObject();
            var appPath = App.Get().Context.AppUrl.ToString();
            var profile = this.DefaultProfile;
            return new
            {
                id = this.UserName,
                url = string.Format(appPath + "profiles/{0}", this.UserName),
                homePage = profile.HomePage,
                email = string.IsNullOrEmpty(this.Email) ? profile.Email : this.Email,
                displayName = string.IsNullOrEmpty(profile.DisplayName) ? profile.UserName : profile.DisplayName,
                image = new { url = string.IsNullOrEmpty(profile.Avatar) ? (appPath + "content/images/no-avatar.gif") : profile.Avatar }
            };
        }


        /// <summary>
        /// Gets the personal document storage
        /// </summary>
        public virtual IUnitOfWorks Storage
        {
            get
            {
                if (blobs == null)
                {
                    var directory = HttpContext.Current.Server.MapPath("~/app_data/users/" + this.UserName + "/blobs");
                    if (!System.IO.Directory.Exists(directory))
                        System.IO.Directory.CreateDirectory(directory);
                    blobs = new DocumentStorage(directory);
                }
                return blobs;
            }
        }

        /// <summary>
        /// Gets the personal queue storage.
        /// </summary>
        public virtual IQueues Queues
        {
            get
            {
                if (queues == null)
                {
                    var directory = HttpContext.Current.Server.MapPath("~/app_data/users/" + this.UserName + "/queues");
                    if (!System.IO.Directory.Exists(directory))
                        System.IO.Directory.CreateDirectory(directory);
                    queues = new QueueStorage(directory);
                }
                return queues;
            }
        }

    }
}
