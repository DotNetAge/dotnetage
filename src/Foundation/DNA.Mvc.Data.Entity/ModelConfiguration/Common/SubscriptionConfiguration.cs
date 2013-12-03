///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace DNA.Web.Data.Entity.ModelConfiguration
{
    public class SubscriptionConfiguration : EntityTypeConfiguration<Subscription>
    {
        public SubscriptionConfiguration()
        {
            HasKey(s => s.ID).ToTable("dna_subscriptions");
            Property(s => s.HashKey).IsRequired();
            Property(s => s.Token).IsRequired().HasMaxLength(2048);
            Property(s => s.UserName).IsRequired().HasMaxLength(50);
        }
    }
}
