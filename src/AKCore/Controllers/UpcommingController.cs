using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AKCore.Controllers
{
    [Route("Upcomming")]
    public class UpcommingController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "På gång";
            using (var db = new AKContext())
            {
                var model = new UpcommingModel();

                var events = db.Events.GroupBy(x => x.Day.Year)
                    .Select(x=>new
                        {
                            Year=x.Key,
                            Event=x.ToList().GroupBy(z=>z.Day.Month)
                        }
                ).ToList();
                return View();
            }
        }

    }
}