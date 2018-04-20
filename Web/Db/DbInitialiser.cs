using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Db
{
    using Microsoft.EntityFrameworkCore;

    public static class DbInitialiser
    {
        public static void Init()
        {
            using (var context = new DesignTimeDbContextFactory().CreateDbContext(new string[0]))
            {
                context.Database.Migrate();
                if (!context.Customers.Any())
                {
                    context.Customers.Add(new Customer() {Name = "Ajax inc"});
                    context.Customers.Add(new Customer() {Name = "Big Evil Corp"});
                    context.SaveChanges();
                }
            }
        }
    }
}
