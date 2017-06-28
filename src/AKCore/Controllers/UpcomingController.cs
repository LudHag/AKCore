using System;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace AKCore.Controllers
{
    [Route("Upcoming")]
    public class UpcomingController : Controller
    {
        private readonly UserManager<AkUser> _userManager;
        private readonly IHostingEnvironment _hostingEnv;

        public UpcomingController(
            UserManager<AkUser> userManager, IHostingEnvironment env)
        {
            _userManager = userManager;
            _hostingEnv = env;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "På gång";
            var loggedIn = User.Identity.IsAuthenticated;
            using (var db = new AKContext(_hostingEnv))
            {
                var model = new UpcomingModel
                {
                    Events = db.Events.OrderBy(x => x.Day).ThenBy(x => x.Starts)
                        .Include(x=>x.SignUps)
                        .Where(x => loggedIn || (x.Type == "Spelning"))
                        .Where(x => x.Day >= DateTime.UtcNow.Date)
                        .GroupBy(x => x.Day.Year).ToList(),
                    LoggedIn = loggedIn,
                    ICalLink = $"{Request.Scheme}://{Request.Host}/upcoming/akevents.ics"
                };

                return View(model);
            }
        }
        [Route("akevents.ics")]
        [Authorize]
        public ActionResult Ical()
        {
            using (var db = new AKContext(_hostingEnv))
            {
                var events = db.Events.OrderBy(x => x.Day).ThenBy(x => x.Starts)
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
                    var dtStart = res.Day.Date + res.Starts.TimeOfDay;
                    var dtEnd = dtStart.AddHours(1);
                    sb.AppendLine("BEGIN:VEVENT");
                    sb.AppendLine("DTSTART:" + dtStart.ToUniversalTime().ToString(DateFormat));
                    sb.AppendLine("DTEND:" + dtEnd.ToUniversalTime().ToString(DateFormat));
                    sb.AppendLine("DTSTAMP:" + now);
                    sb.AppendLine("UID:" + Guid.NewGuid());
                    sb.AppendLine("CREATED:" + now);
                    sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + res.Description+ "<br/>" + res.InternalDescription);
                    sb.AppendLine("DESCRIPTION:" + res.Description + "\n" + res.InternalDescription);
                    sb.AppendLine("LAST-MODIFIED:" + now);
                    sb.AppendLine("LOCATION:" + res.Place);
                    sb.AppendLine("SEQUENCE:0");
                    sb.AppendLine("STATUS:CONFIRMED");
                    sb.AppendLine("SUMMARY:" + res.Name);
                    sb.AppendLine("TRANSP:OPAQUE");
                    sb.AppendLine("END:VEVENT");
                }
                sb.AppendLine("END:VCALENDAR");
                var bytes=Encoding.ASCII.GetBytes(sb.ToString());
                return File(bytes, "application/octet-stream", "akevents.ics");
            }
        }

        [Route("Event/{id:int}")]
        [Authorize]
        public async Task<ActionResult> Event(SignUpModel model, string id)
        {
            ViewBag.Title = "Anmälan";
            if (!int.TryParse(id, out int eId))
                return Redirect("/Upcoming");
            using (var db = new AKContext(_hostingEnv))
            {
                var spelning = db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
                if (spelning == null) return Redirect("/Upcoming");
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var roles = await _userManager.GetRolesAsync(user);
                var nintendo = roles.Contains("SuperNintendo");
                var signup = spelning.SignUps.FirstOrDefault(x => x.Person == user.UserName);
                if (signup!=null)
                {
                    model.Where = signup.Where;
                    model.Car = signup.Car;
                    model.Instrument = signup.Instrument;
                    model.Comment = signup.Comment;
                }
                model.IsNintendo = nintendo;
                model.Event = spelning;

                return View(model);
            }
        }

        [Route("Signup/{id:int}")]
        [Authorize]
        public async Task<ActionResult> SignUp(SignUpModel model, string id)
        {
            if (!int.TryParse(id, out int eId))
                return Json(new { success = false, message = "Felaktigt id" });
            if (string.IsNullOrWhiteSpace(model.Where))
                return Json(new { success = false, message = "Du måste välja om du kommer via hålan, direkt eller inte alls" });
            using (var db = new AKContext(_hostingEnv))
            {
                var spelning = db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
                if (spelning == null) return Json(new {success = false, message = "Felaktigt id"});
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var signup = spelning.SignUps.FirstOrDefault(x => x.Person == user.UserName) ?? new SignUp();
                if (signup.Where == AkSignupType.CantCome || model.Where == AkSignupType.CantCome)
                {
                    signup.SignupTime = DateTime.Now;
                }
                else { 
                    signup.SignupTime = signup.Where == null ? DateTime.Now : signup.SignupTime;
                }
                signup.Where = model.Where;
                signup.Car = model.Car;
                signup.Instrument = model.Instrument;
                signup.Comment = model.Comment;
                signup.Person = user.UserName;
                signup.PersonName = user.GetName();
                signup.InstrumentName = user.Instrument;
                signup.OtherInstruments = user.OtherInstruments;
                spelning.SignUps.Add(signup);
                db.SaveChanges();
                return Json(new {success = true});
            }
        }
    }
}
