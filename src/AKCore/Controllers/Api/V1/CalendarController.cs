using AKCore.DataModel;
using AKCore.Models.Api.V1.Calendar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers.Api.V1;

[ApiController]
[Route("api/v1/calendar")]
[Authorize(AuthenticationSchemes = "MobileBearer")]
public class CalendarController : ControllerBase
{
    private readonly AKContext _db;

    public CalendarController(AKContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<CalendarResponse>> Get()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var events = await _db.Events
            .AsNoTracking()
            .Include(x => x.SignUps)
            .Where(x => x.Day >= DateTime.UtcNow.Date)
            .ToListAsync();

        var response = new CalendarResponse
        {
            Events = events
                .OrderBy(x => x.Day.Date)
                .ThenBy(x => x.StartsTime != default
                    ? x.StartsTime
                    : x.HalanTime)
                .Select(x => new CalendarEventResponse
                {
                    Id = x.Id,
                    Type = x.Type,
                    Name = x.Name,
                    Place = x.Place ?? "",
                    Description = x.Description ?? "",
                    InternalDescription = x.InternalDescription ?? "",
                    Date = x.Day.ToString(
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture),
                    HalanTime = FormatTime(x.HalanTime),
                    ThereTime = FormatTime(x.ThereTime),
                    StartsTime = FormatTime(x.StartsTime),
                    PlayDuration = x.PlayDuration ?? "",
                    Stand = x.Stand ?? "",
                    SignupState = x.SignUps
                        .FirstOrDefault(signup =>
                            signup.PersonId == userId)
                        ?.Where,
                    Coming = x.CanCome(),
                    NotComing = x.CantCome(),
                    Disabled = x.Disabled
                })
                .ToList()
        };

        return Ok(response);
    }

    private static string FormatTime(TimeSpan time)
    {
        return time.ToString(@"hh\:mm");
    }
}
