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
    public class SqlPermissionsConfiguration : EntityTypeConfiguration<Permission>
    {
        public SqlPermissionsConfiguration()
        {
            HasKey(p => p.ID).ToTable("dna_Permissions");
            HasMany(p => p.Roles)
                .WithMany(r => r.Permissions)
                .Map(pr => {
                    pr.MapLeftKey("PermID");
                    pr.MapRightKey("RoleID");
                    pr.ToTable("dna_PermsInRoles");
                });

            Property(p => p.Title).HasMaxLength(255);
            Property(p => p.Action).HasMaxLength(255);
            Property(p => p.Assembly).HasMaxLength(1024);
            Property(p => p.Controller).HasMaxLength(255);
            Property(p => p.Description).HasColumnType("ntext");
        }
    }
}
