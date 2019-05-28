using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace Web.Data
{
    public class Initialiser : CreateDatabaseIfNotExists<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            context.Flowers.AddOrUpdate(f => f.Name, new Flower { Name = "Daisy" });
            context.SaveChanges();
            base.Seed(context);
        }
    }
}