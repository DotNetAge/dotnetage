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
    public class SqlRecipe
    {
        public static void Register(ConfigurationRegistrar configurations)
        {
            
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

            configurations.Add(new SqlAbuseConfiguration());
            //configurations.Add(new SqlEmailQueueItemConfiguration());
            configurations.Add(new SqlUserProfileConfiguration());
            configurations.Add(new SqlWebConfiguration());
            configurations.Add(new SqlWebPageConfiguration());
            configurations.Add(new SqlWebPageVersionConfiguration());
            configurations.Add(new SqlWidgetConfiguration());
            configurations.Add(new SqlWidgetDescriptorConfiguration());
            configurations.Add(new SqlPermissionsetsConfiguration());
            configurations.Add(new SqlPermissionsConfiguration());
            configurations.Add(new SqlContentListConfiguration());
            configurations.Add(new SqlContentViewConfiguration());
            configurations.Add(new SqlContentFormConfiguration());
            configurations.Add(new SqlContentItemDataConfiguration());
            configurations.Add(new SqlCommentConfiguration());
        }
    }
}
