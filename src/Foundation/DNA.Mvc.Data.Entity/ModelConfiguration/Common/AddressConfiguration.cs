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
    public class AddressConfiguration : EntityTypeConfiguration<Address>
    {
        public AddressConfiguration()
        {
            HasKey(c => c.ID).ToTable("dna_addresses");
            Property(c => c.Name).HasMaxLength(50);
            Property(p => p.Street).HasMaxLength(255);
            Property(p => p.City).HasMaxLength(50);
            Property(p => p.Country).HasMaxLength(50);
            Property(p => p.Email).HasMaxLength(2048);
            Property(p => p.Mobile).HasMaxLength(50);
            Property(p => p.Fax).HasMaxLength(50);
            Property(p => p.Tel).HasMaxLength(50);
            Property(p => p.State).HasMaxLength(50);
            Property(p => p.Zip).HasMaxLength(50);
            Property(p => p.Company).HasMaxLength(100);
            Property(p => p.FirstName).HasMaxLength(50);
            Property(p => p.LastName).HasMaxLength(50);
        }
    }
}
