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
    public class Migration2013721 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dna_categories", "Path", c => c.String(true, 2048));
            AddColumn("dna_categories", "ImageUrl", c => c.String(true, 2048));

            DropColumn("dna_ContentDataItems", "Categories");
            AddColumn("dna_ContentDataItems", "PrimaryCategoryID", c => c.Int(true));

            DropForeignKey("dbo.dna_categories", new[] { "WebID" }, "dbo.dna_webs");

            CreateTable(
                "dbo.dna_ContentDataItems_Categories",
                c => new
                {
                    ContentDataItemID = c.Guid(nullable: false),
                    CategoryID = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.ContentDataItemID, t.CategoryID })
                .ForeignKey("dbo.dna_ContentDataItems", t => t.ContentDataItemID, cascadeDelete: true)
                .ForeignKey("dbo.dna_categories", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.ContentDataItemID);
        }
    }
}
