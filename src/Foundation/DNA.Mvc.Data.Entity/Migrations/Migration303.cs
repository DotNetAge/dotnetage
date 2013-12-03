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
    public class Migration303 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dna_ContentLists", "IsSystem", c => c.Boolean());

            AddColumn("dna_profiles", "Fax", c => c.String(maxLength: 50));
            AddColumn("dna_profiles", "TaxID", c => c.String(maxLength:100));
            AddColumn("dna_profiles", "MiddleName", c => c.String(maxLength: 50));
            AddColumn("dna_profiles", "CurrencyCode", c => c.String(maxLength: 10));
            AddColumn("dna_profiles", "Company", c => c.String(maxLength: 100));

            CreateTable("dbo.dna_addresses", c => new
            {
                ID = c.Int(null, true),
                IsDefault = c.Boolean(false, false),
                Name = c.String(maxLength: 50),
                Street = c.String(maxLength: 255),
                FirstName=c.String(maxLength:50),
                LastName = c.String(maxLength: 50),
                Company = c.String(maxLength: 100),
                City = c.String(maxLength: 50),
                Country = c.String(maxLength: 50),
                Email = c.String(maxLength: 2048),
                Mobile = c.String(maxLength: 50),
                Tel = c.String(maxLength: 50),
                Fax = c.String(maxLength: 50),
                State = c.String(maxLength: 50),
                Zip = c.String(maxLength: 50),
                Longitude = c.Double(),
                Latitude = c.Double()
            })
                .PrimaryKey(t => new { t.ID });
        }
    }
}
