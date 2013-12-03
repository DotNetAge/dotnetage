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
    public class MySQLContentViewConfiguration : EntityTypeConfiguration<ContentView>
    {
        public MySQLContentViewConfiguration()
        {
            HasKey(v => v.ID).ToTable("dna_ContentViews");
            Property(c => c.Description).HasMaxLength(2048);
            Property(c => c.Title).HasMaxLength(255);
            Property(c => c.Name).HasMaxLength(50);
            
            Property(c => c.ViewTemplate).HasMaxLength(2048);
            Property(i => i.BodyTemplateXml).HasColumnType("LONGTEXT");
            //Property(i => i.RowTemplate).HasColumnType("LONGTEXT");
            Property(i => i.EmptyTemplateXml).HasColumnType("LONGTEXT");
            Property(i => i.ScriptsXml).HasColumnType("LONGTEXT");
            Property(i => i.StyleSheetsXml).HasColumnType("LONGTEXT");
            Property(c => c.FieldRefsXml).HasColumnType("LONGTEXT");
            Property(c => c.Filter).HasMaxLength(2048);
            Property(c => c.GroupBy).HasMaxLength(2048);
            Property(c => c.Sort).HasMaxLength(2048);
            Property(c => c.ParentID).IsRequired();

//            HasRequired(c => c.Parent)
////                .HasForeignKey(c=>c.ParentID)
//                .WithMany()
//                .WillCascadeOnDelete(true);

            //Property(c => c.Style).HasMaxLength(50);
            //Property(c => c.QueryXml).HasColumnType("LONGTEXT");
            //Ignore(v => v.Fields);
        }
    }
}
