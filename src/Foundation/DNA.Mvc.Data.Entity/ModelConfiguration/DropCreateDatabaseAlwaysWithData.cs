using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace DNA.Mvc.Data.Entity
{
    public class DropCreateDatabaseAlwaysWithData : DropCreateDatabaseAlways<CoreDbContext>
    {
        protected override void Seed(CoreDbContext context)
        {
            context.Roles.Add(new Role() { Name = "administrators", Description = "The system administrators" });
            context.Roles.Add(new Role() { Name = "guests", Description = "" });
            context.SaveChanges();
        }
    }
}
