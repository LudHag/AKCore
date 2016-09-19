using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("Upcomming")]
    public class UpcommingController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "På gång";
            var loggedIn = User.Identity.IsAuthenticated;
            ViewBag.loggedIn = loggedIn;
            using (var db = new AKContext())
            {
                var model = new UpcommingModel
                {
                    Events = db.Events.OrderBy(x => x.Day).Where(x=> loggedIn || x.Type == "Spelning").GroupBy(x => x.Day.Year).ToList()
                };

                return View(model);
            }
        }
    }
}