using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Db
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyContext>
    {
        public MyContext CreateDbContext(string[] args)
        {
            var configuration = Program.Configuration;
            var connectionString = configuration.GetConnectionString("MyContext");
            var builder = new DbContextOptionsBuilder<MyContext>();
            builder.UseSqlServer(connectionString);
            return new MyContext(builder.Options);
        }
    }
}
