using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;

namespace Web.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            var ctx = new MyDbContext();
            if (!ctx.Flowers.Any())
            {
                ctx.Flowers.Add(new Flower { Name = "Daisy" });
                ctx.SaveChanges();
            }
            var flower = ctx.Flowers.First();
            return View(flower);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}