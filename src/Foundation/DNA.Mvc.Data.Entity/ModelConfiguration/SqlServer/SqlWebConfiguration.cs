//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace DNA.Web.Data.Entity.ModelConfiguration
{
    public class SqlWebConfiguration : EntityTypeConfiguration<Web>
    {
        public SqlWebConfiguration()
        {
            //var web = builder.Entity<Web>();
            HasKey(w => w.Id).ToTable("dna_Webs");

            HasMany<WebPage>(w => w.Pages)
                  .WithRequired(p => p.Web)
                  .HasForeignKey(p => p.WebID);

            HasMany<ContentList>(c => c.Lists)
                .WithRequired(c => c.Web)
                .HasForeignKey(c => c.WebID);

            //HasMany<Category>(c => c.Categories)
            //    .WithRequired(c => c.Web)
            //    .HasForeignKey(c => c.WebID);

            HasMany<MovedUrl>(m=>m.MovedUrls)
                .WithRequired(c=>c.Web)
                .HasForeignKey(c => c.WebID);

            Property(w => w.Name).IsRequired().HasMaxLength(1024);
            Property(w => w.Title).HasMaxLength(50);
            Property(w => w.Type).IsRequired();
            Property(w => w.Theme).HasMaxLength(255);
            Property(w => w.TimeZone).HasMaxLength(255);
            //Property(w => w.CacheDuration).IsRequired();
            Property(w => w.Created).IsRequired();
            Property(w => w.DefaultLocale).HasMaxLength(50);
            Property(w => w.Dir).HasMaxLength(10);
            Property(w => w.IsEnabled).IsRequired();
            Property(w => w.Layout).HasMaxLength(1024);
            Property(w => w.LogoImageUrl).HasMaxLength(1024);
            Property(w => w.DefaultUrl).HasMaxLength(1024);
            Property(w => w.InstalledSolutions).HasMaxLength(2048);
            Property(w => w.MasterName).HasMaxLength(1024);
            Property(w => w.Owner).HasMaxLength(256).IsRequired();
            Property(w => w.ShortcutIconUrl).HasMaxLength(1024);

            Property(w => w.LocData).HasColumnType("ntext");
            Property(w => w.Copyright).HasColumnType("ntext");
            Property(w => w.CssText).HasColumnType("ntext");
            Property(w => w.Data).HasColumnType("ntext");
            Property(w => w.Description).HasColumnType("ntext");

            //Ignore(w => w.MasterTools);
            Ignore(w => w.Properties);
            Ignore(w => w.LogoLayout);
            Ignore(w => w.TimeZoneInfo);
            Ignore(w => w.IsRoot);
            Ignore(w => w.WebFolder);
        }
    }
}
