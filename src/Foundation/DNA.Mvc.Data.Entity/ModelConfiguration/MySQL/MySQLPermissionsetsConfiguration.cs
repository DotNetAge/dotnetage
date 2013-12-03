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
    public class MySQLPermissionsetsConfiguration : EntityTypeConfiguration<PermissionSet>
    {
        public MySQLPermissionsetsConfiguration()
        {
            HasKey(p => p.ID).ToTable("dna_PermissionSets");

            HasMany(p => p.Permissions)
                .WithRequired(p => p.PermissionSet)
                .HasForeignKey(p => p.PermissionSetID);

            Property(p => p.Name).HasMaxLength(100);
            Property(p => p.Title).HasMaxLength(255);
            Property(p => p.Description).HasColumnType("LONGTEXT");
            Property(p => p.ResbaseName).HasMaxLength(255);
            Property(p => p.TitleResName).HasMaxLength(255);
        }
    }
}
