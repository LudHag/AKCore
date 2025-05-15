using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("upcoming")]
[Route("Gigs")]
public class UpcomingController : Controller
{
    private readonly UserManager<AkUser> _userManager;
    private readonly AKContext _db;
    private readonly TranslationsService _translationsService;
    private static readonly CultureInfo Culture = new("sv");
    private static readonly CultureInfo CultureEn = new("en-US");

    public UpcomingController(
        UserManager<AkUser> userManager, AKContext db, TranslationsService translationsService)
    {
        _userManager = userManager;
        _db = db;
        _translationsService = translationsService;
    }

    public ActionResult Index()
    {
        ViewBag.Title = "På gång";
        ViewData["Canonical"] = "https://www.altekamereren.org/upcoming";
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

        var isEnglish = _translationsService.IsEnglish();

        var model = new UpcomingViewModel
        {
            Years = _db.Events
                .Include(x => x.SignUps)
                .Where(x => loggedIn || (x.Type == "Spelning") || (x.Type == "Evenemang"))
                .Where(x => loggedIn || (!x.Secret))
                .Where(x => x.Day >= DateTime.UtcNow.Date)
                .ToList()
                .OrderBy(x => x.Day.Date).ThenBy(x => x.StartsTime != default ? x.StartsTime : x.HalanTime)
                .GroupBy(x => x.Day.Year)
                .ToDictionary(x => x.Key, x => new YearList
                {
                    Year = x.Key,
                    Months = x.Select(y => MapEventModel(y, loggedIn, userId, isEnglish)).GroupBy(z => z.Month).
                        ToDictionary(xx => xx.Key, xx => xx.ToList())
                }),
            LoggedIn = loggedIn,
            Member = member,
            IcalLink = $"{Request.Scheme}://{Request.Host}/upcoming/akevents.ics"
        };

        return Json(model);
    }

    private static EventViewModel MapEventModel(Event e, bool loggedIn, string userId, bool isEnglish)
    {
        var cultureToUse = isEnglish ? CultureEn : Culture;

        var description = (isEnglish && !string.IsNullOrWhiteSpace(e.DescriptionEng)) ? e.DescriptionEng : e.Description;
        var internalDescription = (isEnglish && !string.IsNullOrWhiteSpace(e.InternalDescriptionEng)) ? e.InternalDescriptionEng : e.InternalDescription;

        var model = loggedIn
            ? new EventViewModel()
            {
                Id = e.Id,
                Type = e.Type,
                Name = e.Name,
                Place = e.Place,
                Description = description,
                InternalDescription = internalDescription,
                Fika = e.Fika,
                FikaCollection = string.IsNullOrWhiteSpace(e.FikaCollection) ? null : e.FikaCollection.Split(',').ToList(),
                Day = e.Day.ToString("dddd dd", cultureToUse) + "/" + e.Day.ToString("MM", cultureToUse),
                DayInMonth = e.Day.Day,
                HalanTime = ParseTime(e.HalanTime),
                ThereTime = ParseTime(e.ThereTime),
                StartsTime = ParseTime(e.StartsTime),
                PlayDuration = e.PlayDuration,
                Stand = e.Stand,
                Coming = e.CanCome(),
                Year = e.Day.Year,
                Month = e.Day.Month,
                NotComing = e.CantCome(),
                SignupState = e.SignUps?.FirstOrDefault(x => x.PersonId == userId)?.Where,
                Disabled = e.Disabled
            }
            : new EventViewModel()
            {
                Id = e.Id,
                Name = e.Name,
                Place = e.Place,
                Type = e.Type,
                Description = description,
                Day = e.Day.ToString("dddd dd", cultureToUse) + "/" + e.Day.ToString("MM", cultureToUse),
                StartsTime = ParseTime(e.StartsTime),
                PlayDuration = e.PlayDuration,
                Year = e.Day.Year,
                Month = e.Day.Month
            };
        if (e.FikaCollection == null && loggedIn)
            model.FikaCollection = [e.Fika];
        return model;
    }

    private static string ParseTime(TimeSpan date)
    {
        var time = date.ToString(@"hh\:mm");
        return time == "00:00" ? null : time;
    }

    private static string SanitizeIcalDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return string.Empty;
        }

        var sanitizedDesc = description
            .Replace("\n", @" ")  
            .Replace("\r", @" ");

        return sanitizedDesc;
    }

    [Route("akevents.ics")]
    public ActionResult Ical(string rehearsalFilter)
    {
        var events = _db.Events.OrderBy(x => x.Day.Date).ThenBy(x => x.StartsTime)
            .Include(x => x.SignUps)
            .Where(x => x.Day >= DateTime.UtcNow.Date);

        if (rehearsalFilter == "orchestra")
        {
            events = events.Where(x => x.Type != AkEventTypes.BalettRep);
        }
        else if (rehearsalFilter == "ballet")
        {
            events = events.Where(x => x.Type != AkEventTypes.Rep);
        }


        var sb = new StringBuilder();
        var DateFormat = "yyyyMMddTHHmmss";
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

            var description = SanitizeIcalDescription(res.Description);
            var internalDesc = SanitizeIcalDescription(res.InternalDescription);
            var dtEnd = dtStart.AddHours(1);
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + dtStart.ToString(DateFormat));
            sb.AppendLine("DTEND:" + dtEnd.ToString(DateFormat));
            sb.AppendLine("DTSTAMP:" + now);
            sb.AppendLine("UID:" + Guid.NewGuid());
            sb.AppendLine("CREATED:" + now);
            sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + description + "<br/>" +
                          internalDesc);
            sb.AppendLine("DESCRIPTION:" + description +
                          (!string.IsNullOrWhiteSpace(description) ? "\\n" : "") + internalDesc);
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
        if (e.Type is AkEventTypes.Spelning or AkEventTypes.Fest)
        {
            return e.Name;
        }

        return e.Type;
    }

    [Route("EditSignup")]
    [Authorize(Roles = AkRoles.SuperNintendo)]
    [HttpPost]
    public ActionResult EditSignup(string eventId, string memberId, string type, bool instrument, bool car)
    {
        if (!int.TryParse(eventId, out var eIdInt) || string.IsNullOrWhiteSpace(type) ||
            string.IsNullOrWhiteSpace(memberId)) return Json(new { success = false, message = "Felaktig data" });
        var e = _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eIdInt);
        var member = _db.Users.FirstOrDefault(x => x.Id == memberId);
        var signUp = e.SignUps.FirstOrDefault(x => x.PersonId == member.Id);
        if (signUp != null)
        {
            signUp.Where = type;
            signUp.InstrumentName = member.Instrument;
            signUp.Instrument = instrument;
            signUp.Car = car;
        }
        else
        {
            e.SignUps.Add(new SignUp()
            {
                Person = member.UserName,
                PersonName = member.GetName(),
                PersonId = member.Id,
                SignupTime = DateTime.Now.ConvertToSwedishTime(),
                Where = type,
                InstrumentName = member.Instrument
            });
        }

        _db.SaveChanges();
        return Json(new { success = true });
    }

    [Route("Event/{id:int}")]
    [Authorize(Roles = "Medlem")]
    public ActionResult Event(string id)
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
        var signup = spelning.SignUps.Select(x=>x.CopySignupWithoutEvent()).FirstOrDefault(x => x.PersonId == user.Id);
        if (signup != null)
        {
            model.Where = signup.Where;
            model.Car = signup.Car;
            model.Instrument = signup.Instrument;
            model.Comment = signup.Comment;
        }

        model.IsNintendo = nintendo;
        var isEnglish = _translationsService.IsEnglish();
        model.Event = MapEventModel(spelning, true, user.Id, isEnglish);
        var signups = spelning.SignUps.Select(x => x.CopySignupWithoutEvent()).OrderBy(x => x.InstrumentName).ThenBy(x => x.PersonName);

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
            return Json(new { success = false, message = "Felaktigt id" });
        if (string.IsNullOrWhiteSpace(model.Where))
            return Json(new
            {
                success = false,
                message = "Du måste välja om du kommer via hålan, direkt eller inte alls"
            });
        var spelning = _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
        if (spelning == null) return Json(new { success = false, message = "Felaktigt id" });
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
        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "Anmälan uppdaterad" });
    }
}