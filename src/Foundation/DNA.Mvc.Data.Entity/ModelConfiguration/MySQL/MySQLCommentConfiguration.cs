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
    public class MySQLCommentConfiguration : EntityTypeConfiguration<Comment>
    {
        public MySQLCommentConfiguration()
        {
            HasKey(c => c.ID).ToTable("dna_comments");
            Property(c => c.Email).HasMaxLength(2048);
            Property(c => c.UserName).HasMaxLength(50);
            Property(c => c.UrlReferrer).HasMaxLength(2048);
            Property(c => c.IP).HasMaxLength(50);
            Property(c => c.Address).HasMaxLength(255);
            Property(c => c.HomePage).HasMaxLength(1024);
            Property(f => f.Content).HasColumnType("LONGTEXT");
        }
    }
}
