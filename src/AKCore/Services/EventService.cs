using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKCore.Services;

public class EventService
{
    private readonly AKContext _db;
    private readonly AdminLogService _adminLogService;
    private static readonly CultureInfo Culture = new("sv");
    private static readonly CultureInfo CultureEn = new("en-US");
    private const int AdminPageSize = 20;

    public EventService(AKContext db, AdminLogService adminLogService)
    {
        _db = db;
        _adminLogService = adminLogService;
    }

    public AdminEventModel GetAdminEventList(bool old, int page)
    {
        if (page == 0)
        {
            page = 1;
        }

        IQueryable<Event> eventsQuery = old
            ? _db.Events.OrderByDescending(x => x.Day).Where(x => x.Day < DateTime.UtcNow.Date)
            : _db.Events.OrderBy(x => x.Day.Date).Where(x => x.Day >= DateTime.UtcNow.Date);

        var totalCount = eventsQuery.Count();
        var events = eventsQuery.Skip(AdminPageSize * (page - 1)).Take(AdminPageSize).ToList();

        return new AdminEventModel
        {
            Events = events.Select(MapEventModelForAdmin),
            TotalPages = totalCount.TotalPages(AdminPageSize),
            CurrentPage = page,
            Old = old
        };
    }

    public UpcomingViewModel BuildUpcomingViewModel(bool loggedIn, bool member, string userId, bool isEnglish)
    {
        return new UpcomingViewModel
        {
            Years = _db.Events
                .Include(x => x.SignUps)
                .Where(x => loggedIn || x.Type == "Spelning" || x.Type == "Evenemang")
                .Where(x => loggedIn || !x.Secret)
                .Where(x => x.Day >= DateTime.UtcNow.Date)
                .ToList()
                .OrderBy(x => x.Day.Date).ThenBy(x => x.StartsTime != default ? x.StartsTime : x.HalanTime)
                .GroupBy(x => x.Day.Year)
                .ToDictionary(x => x.Key, x => new YearList
                {
                    Year = x.Key,
                    Months = x.Select(y => MapEventModelForUpcoming(y, loggedIn, userId, isEnglish))
                        .GroupBy(z => z.Month)
                        .ToDictionary(xx => xx.Key, xx => xx.ToList())
                }),
            LoggedIn = loggedIn,
            Member = member
        };
    }

    public byte[] GenerateIcal(string rehearsalFilter)
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
        var dateFormat = "yyyyMMddTHHmmss";
        var now = DateTime.Now.ToUniversalTime().ToString(dateFormat);
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("PRODID:-//AkCalendar//altekamereren.org");
        sb.AppendLine("X-WR-CALDESC:Alte Kamererens eventkalender");
        sb.AppendLine("X-WR-CALNAME:AKnewcal");
        sb.AppendLine("X-WR-TIMEZONE:Europe/Stockholm");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("METHOD:PUBLISH");

        foreach (var res in events)
        {
            var description = SanitizeIcalDescription(res.Description);
            var internalDesc = SanitizeIcalDescription(res.InternalDescription);

            string dtStartStr, dtEndStr;
            if (res.HalanTime == TimeSpan.Zero)
            {
                dtStartStr = "DTSTART;VALUE=DATE:" + res.Day.ToString("yyyyMMdd");
                dtEndStr = "DTEND;VALUE=DATE:" + res.Day.AddDays(1).ToString("yyyyMMdd");
            }
            else
            {
                var dtStart = res.Day.Date + res.HalanTime;
                var dtEnd = dtStart.AddHours(1);
                dtStartStr = "DTSTART:" + dtStart.ToString(dateFormat);
                dtEndStr = "DTEND:" + dtEnd.ToString(dateFormat);
            }

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine(dtStartStr);
            sb.AppendLine(dtEndStr);
            sb.AppendLine("DTSTAMP:" + now);
            sb.AppendLine("UID:" + Guid.NewGuid());
            sb.AppendLine("CREATED:" + now);
            sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + description + "<br/>" + internalDesc);
            sb.AppendLine("DESCRIPTION:" + description +
                          (!string.IsNullOrWhiteSpace(description) ? "\\n" : "") + internalDesc);
            sb.AppendLine("LAST-MODIFIED:" + now);
            sb.AppendLine("LOCATION:" + res.Place);
            sb.AppendLine("SEQUENCE:0");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("SUMMARY:" + GetEventSummaryName(res));
            sb.AppendLine("TRANSP:OPAQUE");
            sb.AppendLine("END:VEVENT");
        }

        sb.AppendLine("END:VCALENDAR");
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<ServiceResult> SaveEventAsync(EventViewModel model, string userName)
    {
        if (model.Type == AkEventTypes.Spelning && !AkSpeltyp.Speltyper.Contains(model.Stand))
        {
            return ServiceResult.Fail("Du måste välja stå eller gå.");
        }

        if (model.Type == null && model.Day == null)
        {
            return ServiceResult.Fail("Misslyckades med att spara ändringen");
        }

        if (model.Id > 0)
        {
            var changeEvent = _db.Events.FirstOrDefault(x => x.Id == model.Id);
            if (changeEvent == null)
            {
                return ServiceResult.Fail("Misslyckades med att spara ändringen");
            }

            ApplyEventModel(changeEvent, model);
            await _adminLogService.LogAction(AkLogTypes.Events, userName,
                "Händelse med id " + model.Id + " redigeras");
            await _db.SaveChangesAsync();
            return ServiceResult.Ok("Lyckades ändra händelse");
        }

        if (AkEventTypes.RepEventTypes.Contains(model.Type))
        {
            model.Name = model.Type;
        }

        var newEvent = CreateEventFromModel(model);
        _db.Events.Add(newEvent);
        await _adminLogService.LogAction(AkLogTypes.Events, userName,
            "Händelse med namn " + model.Name + " skapas");
        await _db.SaveChangesAsync();
        return ServiceResult.Ok("Lyckades skapa en ny händelse");
    }

    public async Task<ServiceResult> RemoveEventAsync(int eventId, string userName)
    {
        var e = await _db.Events.Include(x => x.SignUps).FirstOrDefaultAsync(x => x.Id == eventId);
        if (e == null)
        {
            return ServiceResult.Fail("Misslyckades med att ta bort event");
        }

        await _adminLogService.LogAction(AkLogTypes.Events, userName,
            "Händelse med id " + eventId + " tas bort");
        _db.Events.Remove(e);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok("Lyckades ta bort event");
    }

    public Event GetEventById(int eventId) =>
        _db.Events.FirstOrDefault(x => x.Id == eventId);

    public static EventViewModel MapEventModelForAdmin(Event e)
    {
        var model = new EventViewModel
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
            FikaCollection = string.IsNullOrWhiteSpace(e.FikaCollection) ? [] : e.FikaCollection.Split(',').ToList(),
            DayDate = e.Day,
            Day = e.Day.ToString("dd MMM - yyyy", Culture),
            DayInMonth = e.Day.Day,
            HalanTime = FormatTimeSpan(e.HalanTime),
            ThereTime = FormatTimeSpan(e.ThereTime),
            StartsTime = FormatTimeSpan(e.StartsTime),
            PlayDuration = e.PlayDuration,
            Secret = e.Secret,
            Stand = e.Stand,
            Year = e.Day.Year,
            Month = e.Day.Month,
            Disabled = e.Disabled,
        };
        if (e.FikaCollection == null)
        {
            model.FikaCollection = [e.Fika];
        }

        return model;
    }

    public static EventViewModel MapEventModelForUpcoming(Event e, bool loggedIn, string userId, bool isEnglish)
    {
        var cultureToUse = isEnglish ? CultureEn : Culture;
        var description = isEnglish && !string.IsNullOrWhiteSpace(e.DescriptionEng) ? e.DescriptionEng : e.Description;
        var internalDescription = isEnglish && !string.IsNullOrWhiteSpace(e.InternalDescriptionEng)
            ? e.InternalDescriptionEng
            : e.InternalDescription;

        var model = loggedIn
            ? new EventViewModel
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
                HalanTime = FormatTimeSpan(e.HalanTime),
                ThereTime = FormatTimeSpan(e.ThereTime),
                StartsTime = FormatTimeSpan(e.StartsTime),
                PlayDuration = e.PlayDuration,
                Stand = e.Stand,
                Coming = e.CanCome(),
                Year = e.Day.Year,
                Month = e.Day.Month,
                NotComing = e.CantCome(),
                SignupState = e.SignUps?.FirstOrDefault(x => x.PersonId == userId)?.Where,
                Disabled = e.Disabled
            }
            : new EventViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Place = e.Place,
                Type = e.Type,
                Description = description,
                Day = e.Day.ToString("dddd dd", cultureToUse) + "/" + e.Day.ToString("MM", cultureToUse),
                StartsTime = FormatTimeSpan(e.StartsTime),
                PlayDuration = e.PlayDuration,
                Year = e.Day.Year,
                Month = e.Day.Month
            };

        if (e.FikaCollection == null && loggedIn)
        {
            model.FikaCollection = [e.Fika];
        }

        return model;
    }

    public static string FormatTimeSpan(TimeSpan date)
    {
        var time = date.ToString(@"hh\:mm");
        return time == "00:00" ? null : time;
    }

    public static TimeSpan ParseTimeString(string stringTime) =>
        stringTime == null ? default : TimeSpan.Parse(stringTime);

    private static void ApplyEventModel(Event changeEvent, EventViewModel model)
    {
        changeEvent.Name = model.Name;
        changeEvent.Place = model.Place ?? "";
        changeEvent.Day = DateTime.Parse(model.Day).ConvertToSwedishTime();
        changeEvent.HalanTime = ParseTimeString(model.HalanTime);
        changeEvent.ThereTime = ParseTimeString(model.ThereTime);
        changeEvent.PlayDuration = model.PlayDuration;
        changeEvent.Stand = model.Stand;
        changeEvent.StartsTime = ParseTimeString(model.StartsTime);
        changeEvent.Fika = model.Fika;
        changeEvent.FikaCollection = model.FikaCollection == null ? "" : string.Join(",", model.FikaCollection);
        changeEvent.Description = model.Description;
        changeEvent.DescriptionEng = model.DescriptionEng;
        changeEvent.InternalDescription = model.InternalDescription;
        changeEvent.InternalDescriptionEng = model.InternalDescriptionEng;
        changeEvent.Type = model.Type;
        changeEvent.Secret = model.Secret;
        changeEvent.Disabled = model.Disabled;
    }

    private static Event CreateEventFromModel(EventViewModel model) => new()
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
        FikaCollection = model.FikaCollection == null ? "" : string.Join(",", model.FikaCollection),
        HalanTime = ParseTimeString(model.HalanTime),
        ThereTime = ParseTimeString(model.ThereTime),
        Stand = model.Stand,
        StartsTime = ParseTimeString(model.StartsTime),
        PlayDuration = model.PlayDuration,
        Disabled = model.Disabled,
        Secret = model.Secret
    };

    private static string SanitizeIcalDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return string.Empty;
        }

        return description.Replace("\n", @" ").Replace("\r", @" ");
    }

    private static string GetEventSummaryName(Event e)
    {
        if (e.Type is AkEventTypes.Spelning or AkEventTypes.Fest or AkEventTypes.Evenemang)
        {
            return e.Name;
        }

        return e.Type;
    }
}
