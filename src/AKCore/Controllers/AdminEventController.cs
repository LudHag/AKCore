using System;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace AKCore.Controllers
{
    [Route("AdminEvent")]
    [Authorize(Roles = "SuperNintendo")]
    public class AdminEventController : Controller
    {
        private readonly AKContext _db;
        private readonly UserManager<AkUser> _userManager;
        public AdminEventController(AKContext db, UserManager<AkUser> userManager)
        {
            _db = db;
            _userManager = userManager;
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

            var totalPages = ((eventsQuery.Count() - 1) / 20) + 1;

            var events = eventsQuery.Skip(20 * (pId - 1)).Take(20).ToList();

            var model = new AdminEventModel
            {
                Events = events,
                TotalPages = totalPages,
                CurrentPage = pId
            };
            return View(model);
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<ActionResult> Edit(AdminEventModel model)
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
                    changeEvent.HalanTime = model.Halan;
                    changeEvent.ThereTime = model.There;
                    changeEvent.Stand = model.Stand;
                    changeEvent.StartsTime = model.Starts;
                    changeEvent.Fika = model.Fika;
                    changeEvent.Description = model.Description;
                    changeEvent.InternalDescription = model.InternalDescription;
                    changeEvent.Type = model.Type;
                    changeEvent.Secret = model.Secret;

                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    _db.Log.Add(new LogItem()
                    {
                        Type = AkLogTypes.Events,
                        Modified = DateTime.Now,
                        ModifiedBy = user,
                        Comment = "Händelse med id " + model.Id + " redigeras"
                    });

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
                        HalanTime = model.Halan,
                        Stand = model.Stand,
                        StartsTime = model.Starts,
                        ThereTime = model.There,
                        Secret = model.Secret
                    };
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    _db.Log.Add(new LogItem()
                    {
                        Type = AkLogTypes.Events,
                        Modified = DateTime.Now,
                        ModifiedBy = user,
                        Comment = "Händelse med namn " + model.Name + " skapas"
                    });

                    _db.Events.Add(newEvent);
                    _db.SaveChanges();
                    return Json(new {success = true});
                }
            return Json(new {success = false, message = "Misslyckades med att spara ändringen"});
        }

        [HttpPost]
        [Route("Remove/{id:int}")]
        public async Task<ActionResult> Remove(string id)
        {
            if (!int.TryParse(id, out var eId))
                return Json(new { success = false, message = "Misslyckades med att ta bort event" });
            var e = _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
            if (e == null) return Json(new {success = false, message = "Misslyckades med att ta bort event"});

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Events,
                Modified = DateTime.Now,
                ModifiedBy = user,
                Comment = "Händelse med id " + id + " tas bort"
            });

            _db.Events.Remove(e);
            _db.SaveChanges();
            return Json(new {success = true});
        }

        [Route("GetEvent/{id:int}")]
        public ActionResult GetEvent(string id)
        {
            if (!int.TryParse(id, out var eId))
                return Json(new { success = false, message = "Misslyckades med att hämta event" });
            var e = _db.Events.FirstOrDefault(x => x.Id == eId);
            if (e == null) return Json(new {success = false, message = "Misslyckades med att hämta event"});

            return Json(new {success = true, e = JsonConvert.SerializeObject(e)});
        }
    }
}