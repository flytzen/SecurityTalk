namespace ClientSideEncryption.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using DbDemo;

    internal sealed class Configuration : DbMigrationsConfiguration<ClientSideEncryption.DbDemo.MyContext>
    {
        public Configuration()
        {
            // This won't actually work with column encryption
            AutomaticMigrationsEnabled = true; 
        }

        protected override void Seed(ClientSideEncryption.DbDemo.MyContext context)
        {

        }
    }
}
