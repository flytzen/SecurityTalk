namespace ClientSideEncryption.DbDemo
{
    using System.Data.Entity;

    public class MyContext : DbContext
    {
        public MyContext(string nameOrConnectionString) : base(nameOrConnectionString)
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
