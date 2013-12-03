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
    public class MySQLContentFormConfiguration : EntityTypeConfiguration<ContentForm>
    {
        public MySQLContentFormConfiguration()
        {
            HasKey(f => f.ID).ToTable("dna_ContentForms");
            Property(c => c.Description).HasMaxLength(2048);
            Property(c => c.Title).HasMaxLength(255);
            //Property(f => f.CaptionField).HasMaxLength(255);
            //Property(f => f.TemplateView).HasMaxLength(255);
            Property(f => f.BodyTemplateXml).HasColumnType("LONGTEXT");
            Property(i => i.ScriptsXml).HasColumnType("LONGTEXT");
            Property(i => i.StyleSheetsXml).HasColumnType("LONGTEXT");
            Property(i => i.FieldsXml).HasColumnType("LONGTEXT");

            //HasRequired(c=>c.Parent)
            //    .WithMany()
            //    //.HasForeignKey(c => c.ParentID)
            //    .WillCascadeOnDelete(true);
        }
    }
}
