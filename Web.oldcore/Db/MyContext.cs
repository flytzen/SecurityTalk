using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Db
{
    using Microsoft.EntityFrameworkCore;
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
