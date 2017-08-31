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
        private readonly AKContext _db;
        public AdminEventController(AKContext db)
        {
            _db = db;
        }
        public ActionResult Index(string Future, string page)
        {
            ViewBag.Title = "Profil";
            IQueryable<Event> eventsQuery;
            if (!string.IsNullOrWhiteSpace(Future) && (Future == "Gamla"))
                eventsQuery = _db.Events.OrderByDescending(x => x.Day).Where(x => x.Day < DateTime.UtcNow.Date);
            else
                eventsQuery = _db.Events.OrderBy(x => x.Day).Where(x => x.Day >= DateTime.UtcNow.Date);

            var pId = 1;
            if (int.TryParse(page, out var tpId))
            {
                pId = tpId;
            }

            eventsQuery = eventsQuery.Skip(20 * (pId - 1)).Take(20);



            var model = new AdminEventModel {Events = eventsQuery.ToList()};
            return View(model);
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
            if (model.Type != null)
                if (model.Id > 0) //redigera
                {
                    var changeEvent = _db.Events.FirstOrDefault(x => x.Id == model.Id);
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
                    _db.SaveChanges();
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
                    _db.Events.Add(newEvent);
                    _db.SaveChanges();
                    return Json(new {success = true});
                }
            return Json(new {success = false, message = "Misslyckades med att spara ändringen"});
        }

        [HttpPost]
        [Route("Remove/{id:int}")]
        public ActionResult Remove(string id)
        {
            if (!int.TryParse(id, out int eId))
                return Json(new { success = false, message = "Misslyckades med att ta bort event" });
            var e = _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
            if (e == null) return Json(new {success = false, message = "Misslyckades med att ta bort event"});

            _db.Events.Remove(e);
            _db.SaveChanges();
            return Json(new {success = true});
        }

        [Route("GetEvent/{id:int}")]
        public ActionResult GetEvent(string id)
        {
            if (!int.TryParse(id, out int eId))
                return Json(new { success = false, message = "Misslyckades med att hämta event" });
            var e = _db.Events.FirstOrDefault(x => x.Id == eId);
            if (e == null) return Json(new {success = false, message = "Misslyckades med att hämta event"});

            return Json(new {success = true, e = JsonConvert.SerializeObject(e)});
        }
    }
}