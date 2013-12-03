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
    public class SqlWebPageConfiguration : EntityTypeConfiguration<WebPage>
    {
        public SqlWebPageConfiguration() 
        {
            HasKey(p => p.ID).ToTable("dna_WebPages");

            //HasMany(p => p.Versions)
            //          .WithRequired(p => p.Page)
            //          .HasForeignKey(v => v.PageID);

            HasMany(p => p.Widgets)
                      .WithRequired(w => w.WebPage)
                      .HasForeignKey(w => w.PageID);

            HasMany(p => p.Roles)
                .WithMany(r => r.Pages)
                .Map(pr => {
                    pr.MapLeftKey("PageID");
                    pr.MapRightKey("RoleID");
                    pr.ToTable("dna_PagesInRoles");
                });
            //HasMany(p => p.Roles)
            //    .WithRequired(r => r.WebPage)
            //    .HasForeignKey(r => r.PageID);

            Property(p => p.Title).IsRequired().HasMaxLength(255);
            Property(p => p.ViewName).HasMaxLength(255);
            Property(p => p.Target).HasMaxLength(50);
            Property(p => p.Locale).HasMaxLength(20);
            Property(p => p.AllowAnonymous).IsRequired();
            Property(p => p.Created).IsRequired();
            Property(p => p.IsShared).IsRequired();
            Property(p => p.IsStatic).IsRequired();
            Property(p => p.Pos).IsRequired();
            Property(p => p.ShowInMenu).IsRequired();
            Property(p => p.LastModified).IsRequired();
            Property(p => p.LinkTo).HasMaxLength(1024);
            Property(p => p.Owner).IsRequired().HasMaxLength(255);
            Property(p => p.Slug).IsRequired().HasMaxLength(1024);
            //Ignore(p => p.IgnoreRouteKeys);
            Property(p => p.RouteName).HasMaxLength(256);
            Property(p => p.Dir).HasMaxLength(10);
            Property(p => p.IconUrl).HasMaxLength(1024);
            Property(p => p.ImageUrl).HasMaxLength(1024);

            Property(p => p.Keywords).HasMaxLength(256);
            Property(p => p.Description).HasMaxLength(256);
            Property(p => p.AdditionalData).HasColumnType("ntext");
            Property(p => p.CssText).HasColumnType("ntext");
            Property(p => p.UnsaveContent).HasColumnType("ntext");
            Property(p => p.ViewData).HasColumnType("ntext");
        }
    }
}
