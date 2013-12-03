//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;

namespace DNA.Web.Data.Entity.ModelConfiguration
{
    public class MySQLRecipe
    {
        public static void Register(ConfigurationRegistrar configurations)
        {
            //var mysqlmigration = new MySQLDbMigrationsConfiguration();
            
            configurations.Add(new SubscriptionConfiguration());
            configurations.Add(new UserConfiguration());
            configurations.Add(new RoleConfiguration());
            configurations.Add(new ContentAttachmentConfiguration());
            configurations.Add(new CategoryConfiguration());
            configurations.Add(new TagConfiguration());
            configurations.Add(new VoteConfiguration());
            configurations.Add(new FollowConfiguration());
            configurations.Add(new LikeConfiguration());
            configurations.Add(new MovedUrlConfiguration());

            configurations.Add(new MySQLAbuseConfiguration());
            //configurations.Add(new MySQLEmailQueueItemConfiguration());
            configurations.Add(new MySQLUserProfileConfiguration());
            configurations.Add(new MySQLWebConfiguration());
            configurations.Add(new MySQLWebPageConfiguration());
            configurations.Add(new MySQLWebPageVersionConfiguration());
            configurations.Add(new MySQLWidgetConfiguration());
            configurations.Add(new MySQLWidgetDescriptorConfiguration());
            configurations.Add(new MySQLPermissionsetsConfiguration());
            configurations.Add(new MySQLPermissionsConfiguration());
            configurations.Add(new MySQLContentListConfiguration());
            configurations.Add(new MySQLContentViewConfiguration());
            configurations.Add(new MySQLContentFormConfiguration());
            configurations.Add(new MySQLContentItemDataConfiguration());
            configurations.Add(new MySQLCommentConfiguration());
        }
    }
    //public class MySQLDbMigrationsConfiguration : System.Data.Entity.Migrations.DbMigrationsConfiguration<CoreDbContext>
    //{
    //    public MySQLDbMigrationsConfiguration()
    //    {
    //        this.AutomaticMigrationsEnabled = false;
    //    }
    //}

}
