//  Copyright (c) 2013 Ray Liang (http://www.dotnetage.com)
//  Licensed MIT: http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace DNA.Web.Data.Entity.ModelConfiguration
{
    public class SqlShortMessageConfiguration : EntityTypeConfiguration<ShortMessage>
    {
        public SqlShortMessageConfiguration()
        {
            HasKey(a => a.ID).ToTable("dna_messages");
            Property(a => a.To).HasMaxLength(2048);
            Property(a => a.From).HasMaxLength(2048);
            Property(a => a.Body).HasColumnType("ntext");
        }
    }
}
