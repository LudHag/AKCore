using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers
{
    [Route("AdminEvent")]
    [Authorize(Roles = "SuperNintendo")]
    public class AdminEventController : Controller
    {
        private readonly AKContext _db;
        private readonly UserManager<AkUser> _userManager;
        private static readonly CultureInfo Culture = new CultureInfo("sv");
        public AdminEventController(AKContext db, UserManager<AkUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Ändra händelser";

            return View();
        }

        [Route("EventData")]
        public ActionResult EventData(bool old, int page)
        {
            if (page == 0)
            {
                page = 1;
            }

            IQueryable<Event> eventsQuery;
            if (old)
                eventsQuery = _db.Events.OrderByDescending(x => x.Day).Where(x => x.Day < DateTime.UtcNow.Date);
            else
                eventsQuery = _db.Events.OrderBy(x => x.Day.Date).Where(x => x.Day >= DateTime.UtcNow.Date);

            var totalPages = ((eventsQuery.Count() - 1) / 20) + 1;

            var events = eventsQuery.Skip(20 * (page - 1)).Take(20).ToList();

            var model = new AdminEventModel
            {
                Events = events.Select(MapEventModel),
                TotalPages = totalPages,
                CurrentPage = page,
                Old = old
            };
            return Json(model);
        }

        private static EventViewModel MapEventModel(Event e)
        {
            var model = new EventViewModel()
            {
                Id = e.Id,
                Type = e.Type,
                Name = e.Name,
                Place = e.Place,
                Description = e.Description,
                DescriptionEng = e.DescriptionEng,
                InternalDescription = e.InternalDescription,
                InternalDescriptionEng = e.InternalDescriptionEng,
                Fika = e.Fika,
                DayDate = e.Day,
                Day = e.Day.ToString("dd MMM - yyyy", Culture),
                DayInMonth = e.Day.Day,
                HalanTime = ParseTime(e.HalanTime),
                ThereTime = ParseTime(e.ThereTime),
                StartsTime = ParseTime(e.StartsTime),
                PlayDuration = e.PlayDuration,
                Secret = e.Secret,
                Stand = e.Stand,
                Year = e.Day.Year,
                Month = e.Day.Month,
                AllowsSignUps = e.AllowsSignUps,
            };
            return model;
        }

        private static string ParseTime(TimeSpan date)
        {
            var time = date.ToString(@"hh\:mm");
            return time == "00:00" ? null : time;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<ActionResult> Edit(EventViewModel model)
        {
            if (model.Type == AkEventTypes.Spelning)
            {
                if (!AkSpeltyp.Speltyper.Contains(model.Stand))
                {
                    return Json(new { success = false, message = "Du måste välja stå eller gå." });
                }
            }

            if (model.Type != null || model.Day != null)
                if (model.Id > 0) //redigera
                {
                    var changeEvent = _db.Events.FirstOrDefault(x => x.Id == model.Id);
                    if (changeEvent == null)
                        return Json(new { success = false, message = "Misslyckades med att spara ändringen" });
                    changeEvent.Name = model.Name;
                    changeEvent.Place = model.Place ?? "";
                    changeEvent.Day = DateTime.Parse(model.Day).ConvertToSwedishTime();
                    changeEvent.HalanTime = ParseTime(model.HalanTime);
                    changeEvent.ThereTime = ParseTime(model.ThereTime);
                    changeEvent.PlayDuration = model.PlayDuration;
                    changeEvent.Stand = model.Stand;
                    changeEvent.StartsTime = ParseTime(model.StartsTime);
                    changeEvent.Fika = model.Fika;
                    changeEvent.Description = model.Description;
                    changeEvent.DescriptionEng = model.DescriptionEng;
                    changeEvent.InternalDescription = model.InternalDescription;
                    changeEvent.InternalDescriptionEng = model.InternalDescriptionEng;
                    changeEvent.Type = model.Type;
                    changeEvent.Secret = model.Secret;
                    changeEvent.AllowsSignUps = model.AllowsSignUps;
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    _db.Log.Add(new LogItem()
                    {
                        Type = AkLogTypes.Events,
                        Modified = DateTime.Now.ConvertToSwedishTime(),
                        ModifiedBy = user,
                        Comment = "Händelse med id " + model.Id + " redigeras"
                    });

                    _db.SaveChanges();
                    return Json(new { success = true, message = "Lyckades ändra händelse" });
                }
                else //skapa
                {
                    if ((model.Type == AkEventTypes.FikaRep) || (model.Type == AkEventTypes.KarRep) ||
                        (model.Type == AkEventTypes.AthenRep) ||
                        (model.Type == AkEventTypes.Rep))
                        model.Name = model.Type;

                    var newEvent = new Event
                    {
                        Name = model.Name,
                        Place = model.Place ?? "",
                        Description = model.Description,
                        DescriptionEng = model.DescriptionEng,
                        InternalDescription = model.InternalDescription,
                        InternalDescriptionEng = model.InternalDescriptionEng,
                        Day = DateTime.Parse(model.Day).ConvertToSwedishTime(),
                        Type = model.Type,
                        Fika = model.Fika,
                        HalanTime = ParseTime(model.HalanTime),
                        ThereTime = ParseTime(model.ThereTime),
                        Stand = model.Stand,
                        StartsTime = ParseTime(model.StartsTime),
                        PlayDuration = model.PlayDuration,
                        AllowsSignUps = model.AllowsSignUps,
                        Secret = model.Secret
                    };
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);

                    _db.Log.Add(new LogItem()
                    {
                        Type = AkLogTypes.Events,
                        Modified = DateTime.Now.ConvertToSwedishTime(),
                        ModifiedBy = user,
                        Comment = "Händelse med namn " + model.Name + " skapas"
                    });

                    _db.Events.Add(newEvent);
                    _db.SaveChanges();
                    return Json(new { success = true, message = "Lyckades skapa en ny händelse" });
                }
            return Json(new { success = false, message = "Misslyckades med att spara ändringen" });
        }

        private static TimeSpan ParseTime(string stringTime)
        {
            return stringTime == null ? default : TimeSpan.Parse(stringTime);
        }

        [HttpPost]
        [Route("Remove/{id:int}")]
        public async Task<ActionResult> Remove(string id)
        {
            if (!int.TryParse(id, out var eId))
                return Json(new { success = false, message = "Misslyckades med att ta bort event" });
            var e = _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
            if (e == null) return Json(new { success = false, message = "Misslyckades med att ta bort event" });

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Events,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Händelse med id " + id + " tas bort"
            });

            _db.Events.Remove(e);
            _db.SaveChanges();
            return Json(new { success = true, message = "Lyckades ta bort event" });
        }

        [Route("GetEvent/{id:int}")]
        public ActionResult GetEvent(string id)
        {
            if (!int.TryParse(id, out var eId))
                return Json(new { success = false, message = "Misslyckades med att hämta event" });
            var e = _db.Events.FirstOrDefault(x => x.Id == eId);
            if (e == null) return Json(new { success = false, message = "Misslyckades med att hämta event" });

            return Json(new { success = true, e = JsonConvert.SerializeObject(e) });
        }
    }
}