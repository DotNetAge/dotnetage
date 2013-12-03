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
    public class ContentAttachmentConfiguration : EntityTypeConfiguration<ContentAttachment>
    {
        public ContentAttachmentConfiguration()
        {
            HasKey(a => a.ID).ToTable("dna_ContentAttachments");
            Property(a => a.Uri).HasMaxLength(2048);
            Property(a => a.Name).HasMaxLength(255);
            Property(a => a.Extension).HasMaxLength(10);
            Property(a => a.ContentType).HasMaxLength(50);
        }
    }
}
