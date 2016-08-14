using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("AdminEvent")]
    [Authorize(Roles = "SuperNintendo")]
    public class AdminEventController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Profil";
            using (var db = new AKContext())
            {
                var model = new AdminEventModel {Events = db.Events.ToList()};
                return View(model);
            }
        }

        [HttpPost]
        [Route("Edit")]
        public ActionResult Edit(AdminEventModel model)
        {
            using (var db = new AKContext())
            {
                if (model.Name != null && model.Type != null)
                {
                    if (model.Id > 0) //editera
                    {
                    }
                    else //skapa
                    {
                        var newEvent = new Event
                        {
                            Name = model.Name,
                            Description = model.Description,
                            Day = model.Day,
                            Type = model.Type,
                            Halan = model.Halan,
                            Stand = model.Stand,
                            Starts = model.Starts,
                            There = model.There
                        };
                        db.Events.Add(newEvent);
                        db.SaveChanges();
                        return Json(new {success = true});
                    }
                }
                return Json(new {success = false, message = "Misslyckades med att spara ändringen"});
            }
        }
    }
}