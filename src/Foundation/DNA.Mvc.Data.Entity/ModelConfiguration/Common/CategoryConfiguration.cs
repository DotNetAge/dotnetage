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
    public class CategoryConfiguration : EntityTypeConfiguration<Category>
    {
        public CategoryConfiguration()
        {
            HasKey(c => c.ID).ToTable("dna_categories");
            Property(c => c.Name).HasMaxLength(100);
            Property(c => c.Description).HasMaxLength(256);
            Property(c => c.Locale).HasMaxLength(20);

            Property(c => c.Path).HasMaxLength(2048);
            Property(c => c.ImageUrl).HasMaxLength(2048);


        }
    }
}
