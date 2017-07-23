using System;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace AKCore.Controllers
{
    [Route("AdminEvent")]
    [Authorize(Roles = "SuperNintendo")]
    public class AdminEventController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public AdminEventController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public ActionResult Index(string Future)
        {
            ViewBag.Title = "Profil";
            using (var db = new AKContext(_hostingEnvironment))
            {
                IQueryable<Event> eventsQuery;
                if (!string.IsNullOrWhiteSpace(Future) && (Future == "Gamla"))
                    eventsQuery = db.Events.OrderByDescending(x => x.Day).Where(x => x.Day < DateTime.UtcNow.Date);
                else
                    eventsQuery = db.Events.OrderBy(x => x.Day).Where(x => x.Day >= DateTime.UtcNow.Date);


                var model = new AdminEventModel {Events = eventsQuery.ToList()};
                return View(model);
            }
        }

        [HttpPost]
        [Route("Edit")]
        public ActionResult Edit(AdminEventModel model)
        {
            if(model.Type == AkEventTypes.Spelning)
            {
                if (!AkSpeltyp.Speltyper.Contains(model.Stand)){
                    return Json(new { success = false, message = "Du måste välja stå eller gå." });
                }
            }
            using (var db = new AKContext(_hostingEnvironment))
            {
                if (model.Type != null)
                    if (model.Id > 0) //redigera
                    {
                        var changeEvent = db.Events.FirstOrDefault(x => x.Id == model.Id);
                        if (changeEvent == null)
                            return Json(new {success = false, message = "Misslyckades med att spara ändringen"});
                        changeEvent.Name = model.Name;
                        changeEvent.Place = model.Place ?? "";
                        changeEvent.Day = model.Day;
                        changeEvent.Halan = model.Halan;
                        changeEvent.There = model.There;
                        changeEvent.Stand = model.Stand;
                        changeEvent.Starts = model.Starts;
                        changeEvent.Fika = model.Fika;
                        changeEvent.Description = model.Description;
                        changeEvent.InternalDescription = model.InternalDescription;
                        changeEvent.Type = model.Type;
                        changeEvent.Secret = model.Secret;
                        db.SaveChanges();
                        return Json(new {success = true});
                    }
                    else //skapa
                    {
                        if ((model.Type == AkEventTypes.FikaRep) || (model.Type == AkEventTypes.KarRep) ||
                            (model.Type == AkEventTypes.Rep))
                            model.Name = model.Type;

                        var newEvent = new Event
                        {
                            Name = model.Name,
                            Place = model.Place ?? "",
                            Description = model.Description,
                            InternalDescription = model.InternalDescription,
                            Day = model.Day,
                            Type = model.Type,
                            Fika = model.Fika,
                            Halan = model.Halan,
                            Stand = model.Stand,
                            Starts = model.Starts,
                            There = model.There,
                            Secret = model.Secret
                    };
                        db.Events.Add(newEvent);
                        db.SaveChanges();
                        return Json(new {success = true});
                    }
                return Json(new {success = false, message = "Misslyckades med att spara ändringen"});
            }
        }

        [HttpPost]
        [Route("Remove/{id:int}")]
        public ActionResult Remove(string id)
        {
            var eId = 0;
            if (!int.TryParse(id, out eId))
                return Json(new {success = false, message = "Misslyckades med att ta bort event"});
            using (var db = new AKContext(_hostingEnvironment))
            {
                var e = db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
                if (e == null) return Json(new {success = false, message = "Misslyckades med att ta bort event"});

                db.Events.Remove(e);
                db.SaveChanges();
                return Json(new {success = true});
            }
        }

        [Route("GetEvent/{id:int}")]
        public ActionResult GetEvent(string id)
        {
            var eId = 0;
            if (!int.TryParse(id, out eId))
                return Json(new {success = false, message = "Misslyckades med att hämta event"});
            using (var db = new AKContext(_hostingEnvironment))
            {
                var e = db.Events.FirstOrDefault(x => x.Id == eId);
                if (e == null) return Json(new {success = false, message = "Misslyckades med att hämta event"});

                return Json(new {success = true, e = JsonConvert.SerializeObject(e)});
            }
        }
    }
}