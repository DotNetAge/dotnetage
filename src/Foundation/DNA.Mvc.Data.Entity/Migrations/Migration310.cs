///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;

namespace DNA.Web.Data.Entity.Migrations
{
    public class Migration310 : DbMigration
    {
        public override void Up()
        {
            DropTable("dna_messages");
            DropTable("dna_emailqueues");
            AddColumn("dna_contentactions", "AssemblyQualifiedName", c => c.String(maxLength: 1024));
            AddColumn("dna_contentactions", "Order", c => c.Int());
        }
    }
}
