//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace DNA.Web.Data.Entity.ModelConfiguration
{
    public class SqlAbuseConfiguration : EntityTypeConfiguration<Abuse>
    {
        public SqlAbuseConfiguration()
        {
            HasKey(a => a.ID).ToTable("dna_abuses");
            Property(a => a.Uri).HasMaxLength(2048);
            Property(a => a.Owner).HasMaxLength(50);
            Property(a => a.Reportor).HasMaxLength(50);
            Property(a => a.ObjectType).HasMaxLength(256);
            Property(f => f.Content).HasColumnType("ntext");
        }
    }

}
