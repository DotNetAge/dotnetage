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
    public class MySQLWidgetConfiguration : EntityTypeConfiguration<WidgetInstance>
    {
        public MySQLWidgetConfiguration()
        {
            HasKey(w => w.ID).ToTable("dna_Widgets");
            //Property(w => w.Title).HasMaxLength(255);
            Property(w => w.Title).HasMaxLength(255);
            Property(w => w.ZoneID).HasMaxLength(255);
            Property(w => w.Link).HasMaxLength(1024);
            Property(w => w.ViewMode).HasMaxLength(50);
            Property(w => w.BodyClass).HasMaxLength(256);
            Property(w => w.Locale).HasMaxLength(20);
            Property(w => w.HeaderClass).HasMaxLength(256);
            Property(w => w.IsExpanded).IsRequired();
            Property(w => w.IsStatic).IsRequired();
            Property(w => w.Pos).IsRequired();
            Property(w => w.ShowHeader).IsRequired();

            //Property(w => w.ID).HasColumnType("VARCHAR");
            Property(w => w.CssText).HasColumnType("LONGTEXT");
            Property(w => w.IconUrl).HasColumnType("LONGTEXT");
            Property(w => w.Data).HasColumnType("LONGTEXT");
            Property(w => w.HeaderCssText).HasColumnType("LONGTEXT");
            Property(w => w.BodyCssText).HasColumnType("LONGTEXT");

            HasMany(p => p.Roles)
                         .WithMany(r => r.Widgets)
                         .Map(pr =>
                         {
                             pr.MapLeftKey("WidgetID");
                             pr.MapRightKey("RoleID");
                             pr.ToTable("dna_WidgetsInRoles");
                         });
        }
    }
}
