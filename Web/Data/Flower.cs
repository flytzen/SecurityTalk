using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Data
{
    public class Flower
    {
        public int ID { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }
    }
}