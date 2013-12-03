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
    public class MySQLWidgetDescriptorConfiguration : EntityTypeConfiguration<WidgetDescriptor>
    {
        public MySQLWidgetDescriptorConfiguration()
        {
            HasKey(d => d.ID).ToTable("dna_WidgetDescriptors");
            HasMany(d => d.Widgets)
                .WithRequired(w => w.WidgetDescriptor)
                .HasForeignKey(w => w.DescriptorID);

            Property(d => d.UID).HasMaxLength(2048);
            Property(d => d.RSAKey).HasMaxLength(1024);
            Property(d => d.Name).HasMaxLength(1024);
            Property(d => d.DefaultLocale).HasMaxLength(30);
            Property(d => d.Locales).HasMaxLength(2048);
            Property(d => d.Action).HasMaxLength(255);
            Property(d => d.Area).HasMaxLength(1024);
            Property(d => d.Controller).HasMaxLength(255);
            Property(d => d.IconUrl).HasMaxLength(1024);
            Property(d => d.InstalledPath).HasMaxLength(1024);
            Property(d => d.Title).HasMaxLength(50);
            Property(d => d.Version).HasMaxLength(50);
            Property(d => d.ContentType).HasMaxLength(50);
            Property(d => d.ContentUrl).HasMaxLength(2048);
            Property(d => d.ContentDirection).HasMaxLength(5);
            Property(d => d.ContentType).HasMaxLength(50);
            Property(w => w.ViewModes).HasMaxLength(100);
            Property(d => d.Encoding).HasMaxLength(10);
            Property(d => d.Author).HasMaxLength(50);
            Property(d => d.AuthorEmail).HasMaxLength(1024);
            Property(d => d.AuthorHomePage).HasMaxLength(1024);

            Property(d => d.ContentText).HasColumnType("LONGTEXT");
            Property(d => d.Defaults).HasColumnType("LONGTEXT");
            Property(d => d.ConfigXml).HasColumnType("LONGTEXT");
            Property(d => d.Description).HasColumnType("LONGTEXT");

            Ignore(d => d.Properties);
            Ignore(d => d.ControllerShortName);

            HasMany(p => p.Roles)
                 .WithMany(r => r.Descriptors)
                 .Map(pr =>
                 {
                     pr.MapLeftKey("DescriptorID");
                     pr.MapRightKey("RoleID");
                     pr.ToTable("dna_WidgetDescriptorsInRoles");
                 });

        }
    }
}
