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
    public class LikeConfiguration : EntityTypeConfiguration<Like>
    {
        public LikeConfiguration()
        {
            HasKey(a => a.ID).ToTable("dna_likes");
            Property(a => a.Uri).HasMaxLength(2048);
            Property(a => a.UserName).HasMaxLength(50);
        }
    }
}
