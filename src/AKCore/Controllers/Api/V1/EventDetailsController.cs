using AKCore.DataModel;
using AKCore.Models.Api.V1.Events;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers.Api.V1;

[ApiController]
[Route("api/v1/events")]
[Authorize(AuthenticationSchemes = "MobileBearer")]
public class EventDetailsController : ControllerBase
{
    private readonly AKContext _db;
    private readonly UserManager<AkUser> _userManager;

    public EventDetailsController(
        AKContext db,
        UserManager<AkUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventDetailResponse>> Get(int id)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(AkRoles.Medlem))
        {
            return Forbid();
        }

        var evt = await _db.Events
            .AsNoTracking()
            .Include(x => x.SignUps)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (evt == null)
        {
            return NotFound();
        }

        var signups = evt.SignUps ?? [];
        var registration = signups
            .FirstOrDefault(x => x.PersonId == user.Id);

        var availableInstruments =
            SignupService.GetInstrumentsForUser(user);

        var selectedInstrument =
            registration?.InstrumentName;

        if (selectedInstrument == null &&
            availableInstruments.Count > 0)
        {
            selectedInstrument = availableInstruments[0];
        }

        var isPassed =
            evt.Day.Date < DateTime.UtcNow.Date.AddDays(-1);

        var response = new EventDetailResponse
        {
            Id = evt.Id,
            Type = evt.Type,
            Name = evt.Name,
            Place = evt.Place ?? "",
            Description = evt.Description ?? "",
            InternalDescription = evt.InternalDescription ?? "",
            Date = evt.Day.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture),
            HalanTime = FormatTime(evt.HalanTime),
            ThereTime = FormatTime(evt.ThereTime),
            StartsTime = FormatTime(evt.StartsTime),
            PlayDuration = evt.PlayDuration ?? "",
            Stand = evt.Stand ?? "",
            SignupState = registration?.Where,
            Coming = evt.CanCome(),
            NotComing = evt.CantCome(),
            Disabled = evt.Disabled,
            RegistrationAvailable =
                !evt.Disabled && !isPassed,
            Registration = new EventRegistrationSelectionResponse
            {
                Where = registration?.Where,
                Car = registration?.Car ?? false,
                Instrument = registration?.Instrument ?? true,
                Comment = registration?.Comment ?? "",
                SelectedInstrument = selectedInstrument,
                AvailableInstruments = availableInstruments
            },
            Attendees = signups
                .OrderBy(x => x.InstrumentName)
                .ThenBy(x => x.PersonName)
                .Select(x => new EventAttendeeResponse
                {
                    PersonName = x.PersonName ?? "",
                    Where = x.Where,
                    Car = x.Car,
                    Instrument = x.Instrument,
                    InstrumentName = x.InstrumentName,
                    Comment = x.Comment ?? ""
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
