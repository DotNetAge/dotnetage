//  Copyright (c) 2013 Ray Liang (http://www.dotnetage.com)
//  Licensed MIT: http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace DNA.Web.Data.Entity.ModelConfiguration
{
    public class SqlEmailQueueItemConfiguration : EntityTypeConfiguration<EmailQueueItem>
    {
        public SqlEmailQueueItemConfiguration() 
        {
            HasKey(a => a.ID).ToTable("dna_emailqueues");
            Property(a => a.Domain).HasMaxLength(2048);
            Property(a => a.Subject).HasMaxLength(2048);
            Property(a => a.To).HasMaxLength(1024);
            Property(a => a.Body).HasColumnType("ntext");
        }
    }
}
