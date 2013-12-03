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
    public class MySQLUserProfileConfiguration : EntityTypeConfiguration<UserProfile>
    {
        public MySQLUserProfileConfiguration()
        {
            HasKey(p => p.ID).ToTable("dna_profiles");
            Property(p => p.UserName).HasMaxLength(50);
            Property(p => p.Address).HasMaxLength(255);
            Property(p => p.Avatar).HasMaxLength(2048);
            Property(p => p.City).HasMaxLength(50);
            Property(p => p.Country).HasMaxLength(50);
            Property(p => p.DisplayName).HasMaxLength(50);
            Property(p => p.Email).HasMaxLength(2048);
            Property(p => p.FirstName).HasMaxLength(50);
            Property(p => p.Gender).HasMaxLength(10);
            Property(p => p.Language).HasMaxLength(50);
            Property(p => p.LastName).HasMaxLength(50);
            Property(p => p.Mobile).HasMaxLength(50);
            Property(p => p.Phone).HasMaxLength(50);
            Property(p => p.State).HasMaxLength(50);
            Property(p => p.Theme).HasMaxLength(50);
            Property(p => p.TimeZone).HasMaxLength(50);
            Property(p => p.ZipCode).HasMaxLength(50);
            Property(p => p.HomePage).HasMaxLength(2048);
            Property(p => p.AppName).HasMaxLength(256);
            Property(p => p.Locale).HasMaxLength(20);
            Property(p => p.Account).HasMaxLength(256);
            Property(p => p.Company).HasMaxLength(100);
            Property(p => p.Fax).HasMaxLength(50);
            Property(p => p.TaxID).HasMaxLength(100);
            Property(p => p.MiddleName).HasMaxLength(50);
            Property(p => p.CurrencyCode).HasMaxLength(10);

            Property(p => p.Signature).HasColumnType("LONGTEXT");
            Property(p => p.Data).HasColumnType("LONGTEXT");
        }
    }
}
