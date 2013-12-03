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
    public class SqlContentFormConfiguration : EntityTypeConfiguration<ContentForm>
    {
        public SqlContentFormConfiguration()
        {
            HasKey(f => f.ID).ToTable("dna_ContentForms");
            Property(c => c.Description).HasMaxLength(2048);
            Property(c => c.Title).HasMaxLength(255);
            Property(f => f.BodyTemplateXml).HasColumnType("ntext");
            Property(i => i.FieldsXml).HasColumnType("ntext");
            Property(i => i.ScriptsXml).HasColumnType("ntext");
            Property(i => i.StyleSheetsXml).HasColumnType("ntext");
        }
    }
}
