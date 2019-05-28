using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    using Db;

    public class HomeModel
    {
        public string DummyConfigValue { get; set; }

        public IReadOnlyList<Customer> Customers { get; set; }
    }
}
