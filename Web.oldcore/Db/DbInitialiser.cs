using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Db
{
    using Microsoft.EntityFrameworkCore;
    using Serilog;

    public static class DbInitialiser
    {
        public static void Init()
        {
            Log.Information("Starting to init database");
            try
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
            catch (Exception e)
            {
                Log.Error(e, "Failed to init database");
                throw;
            }
            Log.Information("Finished initing database");
        }
    }
}
