using System;
using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
                    Events = db.Events.OrderBy(x => x.Day)
                        .Where(x=> loggedIn || x.Type == "Spelning")
                        .Where(x => x.Day >= DateTime.UtcNow.Date)
                        .GroupBy(x => x.Day.Year).ToList()
                };

                return View(model);
            }
        }
        [Route("Event/{id:int}")]
        [Authorize]
        public ActionResult Event(SignUpModel model, string id)
        {
            var eId = 0;
            if (!int.TryParse(id, out eId))
            {
                return Redirect("/Upcomming");
            }
            using (var db = new AKContext())
            {
                var spelning =db.Events.Include(x=>x.SignUps).FirstOrDefault(x => x.Id == eId);
                if (spelning == null) return Redirect("/Upcomming");

                model.Event = spelning;

                return View(model);
            }
        }
    }
}