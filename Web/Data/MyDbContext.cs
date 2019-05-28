using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Web.Data
{
    public class MyDbContext : DbContext
    {
        //public MyDbContext() : base()
        //{

        //}

        public IDbSet<Flower> Flowers { get; set; }

    }
}