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
    public class SqlWebPageVersionConfiguration : EntityTypeConfiguration<WebPageVersion>
    {
        public SqlWebPageVersionConfiguration() 
        {
            HasKey(p => p.ID).ToTable("dna_WebPageVersions");
            Property(p => p.Published).IsRequired();
            Property(p => p.Version).IsRequired();
            Property(p => p.Content).HasColumnType("ntext");
            Property(p => p.Remarks).HasColumnType("ntext");
        }
    }
}
