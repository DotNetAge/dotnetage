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
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            HasKey(u => u.ID).ToTable("dna_Users");
            Property(u => u.UserName).HasMaxLength(50);
            Property(u => u.Email).HasMaxLength(1024);
            Property(u => u.LastLoginIP).HasMaxLength(100);
            Property(u => u.Password).HasMaxLength(50);
            Property(u => u.PasswordQuestion).HasMaxLength(255);
            Property(u => u.PasswordAnswer).HasMaxLength(255);
            Property(u => u.PasswordSalt).HasMaxLength(128);
            Property(u => u.DefaultWeb).HasMaxLength(255);
            HasMany(u => u.Profiles)
                .WithRequired(p => p.User)
                .HasForeignKey(p => p.UserID);

            HasMany(r => r.Roles)
                .WithMany(u => u.Users)
                .Map(ur =>
                {
                    ur.MapLeftKey("UserID");
                    ur.MapRightKey("RoleID");
                    ur.ToTable("dna_UsersInRoles");
                });

        }
    }
}
