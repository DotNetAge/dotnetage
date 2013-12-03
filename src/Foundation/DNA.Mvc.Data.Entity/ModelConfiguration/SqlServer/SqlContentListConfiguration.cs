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
    public class SqlContentListConfiguration : EntityTypeConfiguration<ContentList>
    {
        public SqlContentListConfiguration()
        {
            HasKey(c => c.ID).ToTable("dna_ContentLists");
            Property(c => c.Description).HasMaxLength(2048);
            Property(c => c.ItemType).HasMaxLength(2048);
            Property(c => c.Name).HasMaxLength(255);
            Property(c => c.Title).HasMaxLength(255);
            Property(c => c.Owner).HasMaxLength(50);
            Property(c => c.FieldsXml).HasColumnType("ntext");
            Property(c => c.Moderators).HasColumnType("ntext");
            Property(c => c.ImageUrl).HasColumnType("ntext");
            //Property(c => c.ActivityDispTemplate).HasColumnType("ntext");
            Property(c => c.BaseType).HasMaxLength(255);
            Property(c => c.Master).HasMaxLength(255);
            Property(c => c.Version).HasMaxLength(50);
            Property(c => c.Locale).HasMaxLength(20);
            Ignore(c => c.DetailForm);
            Ignore(c => c.EditForm);
            Ignore(c => c.NewForm);
            Ignore(c => c.Roles);

            HasMany(i => i.Items)
                .WithRequired(i => i.Parent)
                .HasForeignKey(i => i.ParentID);

            HasMany(p => p.Views)
                .WithRequired(v => v.Parent)
                .HasForeignKey(v => v.ParentID);

            HasMany(p => p.Forms)
    .WithRequired(v => v.Parent)
    .HasForeignKey(v => v.ParentID);

            HasMany(p => p.Actions)
                .WithRequired(v => v.Parent)
                .HasForeignKey(v => v.ParentID);

            //HasMany(c => c.Roles)
            //    .WithMany(r => r.Lists)
            //    .Map(cr =>
            //    {
            //        cr.MapLeftKey("ListID");
            //        cr.MapRightKey("RoleID");
            //        cr.ToTable("dna_ListsInRoles");
            //    });

            HasMany(c => c.Followers)
                .WithRequired(c => c.List)
                .HasForeignKey(c => c.ListID);
        }
    }
}
