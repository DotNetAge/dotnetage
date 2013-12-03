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
    public class MySQLContentItemDataConfiguration : EntityTypeConfiguration<ContentDataItem>
    {
        public MySQLContentItemDataConfiguration()
        {
            HasKey(i => i.ID).ToTable("dna_ContentDataItems");
            //Property(w => w.ID).HasColumnType("VARCHAR");

            Property(i => i.Owner).HasMaxLength(50);
            Property(i => i.Auditor).HasMaxLength(50);
            Property(i => i.Locale).HasMaxLength(20);
            //Property(i => i.Categories).HasMaxLength(1024);
            Property(i => i.Tags).HasMaxLength(1024);
            Property(i => i.Slug).HasMaxLength(1024);
            Property(i => i.Modifier).HasMaxLength(50);
            Property(i => i.Path).HasColumnType("LONGTEXT");
            Property(i => i.Annotation).HasColumnType("LONGTEXT");
            Property(i => i.RawData).HasColumnType("LONGTEXT");

            HasMany(i => i.Attachments)
                .WithRequired(i => i.Item)
                .HasForeignKey(i => i.ItemID);

            HasMany(c => c.Categories)
                .WithMany(d => d.ContentDataItems)
                .Map(pr =>
                {
                    pr.MapLeftKey("ContentDataItemID")
                       .MapRightKey("CategoryID")
                       .ToTable("dna_ContentDataItems_Categories");
                });
        }
    }
}
