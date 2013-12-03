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
    public class MySQLActionConfiguration : EntityTypeConfiguration<ContentAction>
    {
        public MySQLActionConfiguration()
        {
            HasKey(c => c.ID).ToTable("dna_contentactions");
            Property(c => c.Title).HasMaxLength(2048);
            Property(f => f.Description).HasMaxLength(2048);
            Property(f => f.ParametersXml).HasColumnType("LONGTEXT");
        }
    }
}
