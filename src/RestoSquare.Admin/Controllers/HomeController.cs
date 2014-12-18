using System;
using System.Linq;
using System.Web.Mvc;

using RestoSquare.Data;

namespace RestoSquare.Admin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string name, int skip = 0)
        {
            using (var ctx = new RestoContext())
            {
                ViewBag.Name = name;
                ViewBag.Count = ctx.Restaurants.Count();
                ViewBag.Showing = String.Format("Showing {0} to {1} of {2}.", skip + 1, skip + 50, ViewBag.Count);
                if (skip > 0)
                    ViewBag.PreviousPage = Url.Action("Index", new { name, skip = skip - 50 });
                ViewBag.NextPage = Url.Action("Index", new { name, skip = skip + 50 });

                if (!String.IsNullOrEmpty(name))
                    return View(ctx.Restaurants.Where(r => r.Name.Contains(name)).OrderByDescending(r => r.Budget).Skip(skip).Take(50).ToList());
                return View(ctx.Restaurants.OrderByDescending(r => r.Budget).Skip(skip).Take(50).ToList());
            }
        }
    }
}