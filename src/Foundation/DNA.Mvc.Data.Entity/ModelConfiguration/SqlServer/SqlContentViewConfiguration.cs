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
    public class SqlContentViewConfiguration : EntityTypeConfiguration<ContentView>
    {
        public SqlContentViewConfiguration()
        {
            HasKey(v => v.ID).ToTable("dna_ContentViews");
            Property(c => c.Description).HasMaxLength(2048);
            Property(c => c.Title).HasMaxLength(255);
            Property(c => c.Name).HasMaxLength(50);
            Property(c => c.ViewTemplate).HasMaxLength(2048);
//            Property(c => c.Icon).HasMaxLength("ntext");
            //Property(i => i.RowTemplate).HasColumnType("ntext");
            Property(i => i.BodyTemplateXml).HasColumnType("ntext");
            Property(i => i.EmptyTemplateXml).HasColumnType("ntext");
            Property(i => i.ScriptsXml).HasColumnType("ntext");
            Property(i => i.StyleSheetsXml).HasColumnType("ntext");
            Property(c => c.FieldRefsXml).HasColumnType("ntext");
            Property(c => c.Filter).HasMaxLength(2048);
            Property(c => c.GroupBy).HasMaxLength(2048);
            Property(c => c.Sort).HasMaxLength(2048);
            //Property(c => c.Style).HasMaxLength(50);
            //Ignore(v => v.Fields);
        }
    }
}
