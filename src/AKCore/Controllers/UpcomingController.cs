using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AKCore.Controllers
{
    [Route("upcoming")]
    [Route("Gigs")]
    public class UpcomingController : Controller
    {
        private readonly UserManager<AkUser> _userManager;
        private readonly AKContext _db;
        private static readonly CultureInfo Culture = new CultureInfo("sv");

        public UpcomingController(
            UserManager<AkUser> userManager, AKContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "På gång";
            return View();
        }

        [Route("UpcomingListData")]
        public async Task<ActionResult> UpcomingData()
        {
            var loggedIn = User.Identity.IsAuthenticated;
            var member = false;
            var userId = "";
            if (loggedIn)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var roles = await _userManager.GetRolesAsync(user);
                member = roles.Contains("Medlem");
                userId = user.Id;
            }

            var model = new UpcomingViewModel
            {
                Years = _db.Events
                    .Include(x => x.SignUps)
                    .Where(x => loggedIn || (x.Type == "Spelning"))
                    .Where(x => loggedIn || (!x.Secret))
                    .Where(x => x.Day >= DateTime.UtcNow.Date)
                    .OrderBy(x => x.Day).ThenBy(x => x.StartsTime)
                    .ToList()
                    .GroupBy(x => x.Day.Year)
                    .ToDictionary(x => x.Key, x => new YearList
                    {
                        Year = x.Key,
                        Months = x.Select(y => MapEventModel(y, loggedIn, userId)).GroupBy(z => z.Month).
                            ToDictionary(xx => xx.Key, xx => xx.ToList())
                    }),
                LoggedIn = loggedIn,
                Member = member,
                IcalLink = $"{Request.Scheme}://{Request.Host}/upcoming/akevents.ics"
            };

            return Json(model);
        }

        private static EventViewModel MapEventModel(Event e, bool loggedIn, string userId)
        {
            var model = loggedIn
                ? new EventViewModel()
                {
                    Id = e.Id,
                    Type = e.Type,
                    Name = e.Name,
                    Place = e.Place,
                    Description = e.Description,
                    InternalDescription = e.InternalDescription,
                    Fika = e.Fika,
                    Day = e.Day.ToString("dddd dd", Culture) + "/" + e.Day.ToString("MM", Culture),
                    DayInMonth = e.Day.Day,
                    HalanTime = ParseTime(e.HalanTime),
                    ThereTime = ParseTime(e.ThereTime),
                    StartsTime = ParseTime(e.StartsTime),
                    Stand = e.Stand,
                    Coming = e.CanCome(),
                    Year = e.Day.Year,
                    Month = e.Day.Month,
                    NotComing = e.CantCome(),
                    SignupState = e.SignUps?.FirstOrDefault(x => x.PersonId == userId)?.Where
                }
                : new EventViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Place = e.Place,
                    Type = e.Type,
                    Description = e.Description,
                    Day = e.Day.ToString("dddd dd", Culture) + "/" + e.Day.ToString("MM", Culture),
                    StartsTime = ParseTime(e.StartsTime),
                    Year = e.Day.Year,
                    Month = e.Day.Month
                };
            return model;
        }

        private static string ParseTime(TimeSpan date)
        {
            var time = date.ToString(@"hh\:mm");
            return time == "00:00" ? null : time;
        }

        [Route("akevents.ics")]
        public ActionResult Ical()
        {
            var events = _db.Events.OrderBy(x => x.Day).ThenBy(x => x.StartsTime)
                .Include(x => x.SignUps)
                .Where(x => x.Day >= DateTime.UtcNow.Date);


            var sb = new StringBuilder();
            var DateFormat = "yyyyMMddTHHmmssZ";
            var now = DateTime.Now.ToUniversalTime().ToString(DateFormat);
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:-//AkCalendar//altekamereren.org");
            sb.AppendLine("X-WR-CALDESC:Alte Kamererens eventkalender");
            sb.AppendLine("X-WR-CALNAME:AKnewcal");
            sb.AppendLine("X-WR-TIMEZONE:Europe/Stockholm");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("METHOD:PUBLISH");
            foreach (var res in events)
            {
                var dtStart = res.Day.Date;
                dtStart += res.HalanTime;

                var dtEnd = dtStart.AddHours(1);
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTART:" + dtStart.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTEND:" + dtEnd.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTSTAMP:" + now);
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("CREATED:" + now);
                sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + res.Description + "<br/>" +
                              res.InternalDescription);
                sb.AppendLine("DESCRIPTION:" + res.Description +
                              (!string.IsNullOrWhiteSpace(res.Description) ? "\\n" : "") + res.InternalDescription);
                sb.AppendLine("LAST-MODIFIED:" + now);
                sb.AppendLine("LOCATION:" + res.Place);
                sb.AppendLine("SEQUENCE:0");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + GetName(res));
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("END:VEVENT");
            }

            sb.AppendLine("END:VCALENDAR");
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "application/octet-stream", "akevents.ics");
        }

        private static string GetName(Event e)
        {
            if (e.Type == AkEventTypes.Spelning || e.Type == AkEventTypes.Fest)
            {
                return e.Name;
            }

            return e.Type;
        }

        [Route("EditSignup")]
        [Authorize(Roles = AkRoles.SuperNintendo)]
        [HttpPost]
        public ActionResult EditSignup(string eventId, string memberId, string type)
        {
            if (!int.TryParse(eventId, out var eIdInt) || string.IsNullOrWhiteSpace(type) ||
                string.IsNullOrWhiteSpace(memberId)) return Json(new {success = false, message = "Felaktig data"});
            var e = _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eIdInt);
            var member = _db.Users.FirstOrDefault(x => x.Id == memberId);
            var signUp = e.SignUps.FirstOrDefault(x => x.PersonId == member.Id);
            if (signUp != null)
            {
                signUp.Where = type;
                signUp.InstrumentName = member.Instrument;
            }
            else
            {
                e.SignUps.Add(new SignUp()
                {
                    Person = member.UserName,
                    PersonName = member.GetName(),
                    PersonId = member.Id,
                    SignupTime = DateTime.Now,
                    Where = type,
                    InstrumentName = member.Instrument
                });
            }

            _db.SaveChanges();
            return Json(new {success = true});
        }

        [Route("Event/{id:int}")]
        [Authorize(Roles = "Medlem")]
        public ActionResult Event(SignUpModel model, string id)
        {
            ViewBag.Title = "Anmälan";
            if (!int.TryParse(id, out var eId))
                return Redirect("/upcoming");

            return View(eId);
        }

        [Route("Event/EventData/{id:int}")]
        [Authorize(Roles = "Medlem")]
        public async Task<ActionResult> EventData(string id)
        {
            var model = new SignUpModel();
            if (!int.TryParse(id, out var eId))
                return Json(false);

            var spelning = _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
            if (spelning == null) return Json(false);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var nintendo = roles.Contains("SuperNintendo");
            var signup = spelning.SignUps.FirstOrDefault(x => x.PersonId == user.Id);
            if (signup != null)
            {
                model.Where = signup.Where;
                model.Car = signup.Car;
                model.Instrument = signup.Instrument;
                model.Comment = signup.Comment;
            }

            model.IsNintendo = nintendo;
            model.Event = MapEventModel(spelning, true, user.Id);
            model.Signups = spelning.SignUps.OrderBy(x=>x.InstrumentName).ThenBy(x=>x.PersonName);

            if (nintendo)
            {
                model.Members = (await _userManager.GetUsersInRoleAsync(AkRoles.Medlem)).OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName).Select(x => new MemberViewModel
                    {
                        Id = x.Id,
                        FullName = x.GetName()
                    });
            }

            return Json(model);
        }

        [Route("Signup/{id:int}")]
        [Authorize(Roles = "Medlem")]
        public async Task<ActionResult> SignUp(SignUpModel model, string id)
        {
            if (!int.TryParse(id, out var eId))
                return Json(new {success = false, message = "Felaktigt id"});
            if (string.IsNullOrWhiteSpace(model.Where))
                return Json(new
                {
                    success = false,
                    message = "Du måste välja om du kommer via hålan, direkt eller inte alls"
                });
            var spelning = _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
            if (spelning == null) return Json(new {success = false, message = "Felaktigt id"});
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var signup = spelning.SignUps.FirstOrDefault(x => x.PersonId == user.Id) ?? new SignUp();
            if (signup.Where == AkSignupType.CantCome || model.Where == AkSignupType.CantCome)
            {
                signup.SignupTime = DateTime.Now;
            }
            else
            {
                signup.SignupTime = signup.Where == null ? DateTime.Now : signup.SignupTime;
            }

            signup.Where = model.Where;
            signup.Car = model.Car;
            signup.Instrument = model.Instrument;
            signup.Comment = model.Comment;
            signup.Person = user.UserName;
            signup.PersonId = user.Id;
            signup.PersonName = user.GetName();
            signup.InstrumentName = user.Instrument;
            signup.OtherInstruments = user.OtherInstruments;
            spelning.SignUps.Add(signup);
            _db.SaveChanges();
            return Json(new {success = true});
        }
    }
}