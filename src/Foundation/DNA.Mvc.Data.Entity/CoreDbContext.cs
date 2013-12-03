//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using DNA.Web.Data.Entity.ModelConfiguration;

namespace DNA.Web.Data
{
    /// <summary>
    /// This context only for microsoft sql database series
    /// </summary>
    public class CoreDbContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder builder)
        {
            if (string.IsNullOrEmpty(Recipe) || Recipe.Equals("sql", StringComparison.OrdinalIgnoreCase))
                SqlRecipe.Register(builder.Configurations);
            else
            {
                if (Recipe.Equals("mysql", StringComparison.OrdinalIgnoreCase))
                    MySQLRecipe.Register(builder.Configurations);
            }
        }

        public string Recipe { get; set; }

        #region Ctors
        
        public CoreDbContext()
            : base("DNADB")
        {

        }

        public CoreDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public CoreDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        public CoreDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext) { }

        #endregion

        public DbSet<Web> Webs { get; set; }

        public DbSet<WebPage> WebPages { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<PermissionSet> PermissionSets { get; set; }

        public DbSet<WidgetInstance> Widgets { get; set; }

        public DbSet<WidgetDescriptor> WidgetDescriptors { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserProfile> Profiles { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Abuse> Abuses { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<WebPageVersion> WebPageVersions { get; set; }

        public DbSet<ContentList> ContentLists { get; set; }

        public DbSet<ContentView> ContentViews { get; set; }

        public DbSet<ContentAction> ContentActions { get; set; }

        public DbSet<ContentForm> ContentForms { get; set; }

        public DbSet<ContentDataItem> ContentData { get; set; }

        //public DbSet<EmailQueueItem> EmailQueueItems { get; set; }

        //public DbSet<ShortMessage> ShotMessages { get; set; }

        public DbSet<Vote> Votes { get; set; }

        public DbSet<Follow> Follows { get; set; }
    }

}
