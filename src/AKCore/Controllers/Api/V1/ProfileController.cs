using AKCore.DataModel;
using AKCore.Models;
using AKCore.Models.Api.V1.Profile;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AKCore.Controllers.Api.V1;

[ApiController]
[Route("api/v1/profile")]
[Authorize(AuthenticationSchemes = "MobileBearer")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<AkUser> _userManager;
    private readonly AKContext _db;

    public ProfileController(
        UserManager<AkUser> userManager,
        AKContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> Get()
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

        var posts = UserAdminService.ParseSlavPosterToList(
            user.SlavPoster);

        var profile = UserAdminService.MapToProfileModel(
            user,
            roles,
            posts);

        return Ok(new ProfileResponse
        {
            UserName = profile.UserName ?? "",
            Email = profile.Email ?? "",
            FirstName = profile.FirstName ?? "",
            LastName = profile.LastName ?? "",
            Phone = profile.Phone ?? "",
            Instrument = profile.Instrument ?? "",
            OtherInstruments = profile.OtherInstruments ?? [],
            Roles = profile.Roles ?? [],
            Posts = profile.Posts ?? [],
            Medal = profile.Medal ?? "",
            GivenMedal = profile.GivenMedal ?? ""
        });
    }


    [HttpPut]
    public async Task<ActionResult<ProfileResponse>> UpdateProfile(
        [FromBody] UpdateProfileRequest request)
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

        var otherInstruments = request.OtherInstruments ?? [];

        if (otherInstruments.Contains(request.Instrument))
        {
            return BadRequest(new
            {
                error = "DuplicateInstrument"
            });
        }

        var model = new ProfileModel
        {
            UserName = user.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Instrument = request.Instrument,
            OtherInstruments = otherInstruments
        };

        UserAdminService.ApplyProfileFields(
            user,
            model,
            includeAdminFields: false);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors.Select(x => new
                {
                    code = x.Code,
                    description = x.Description
                })
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var posts = UserAdminService.ParseSlavPosterToList(
            user.SlavPoster);

        var profile = UserAdminService.MapToProfileModel(
            user,
            roles,
            posts);

        return Ok(new ProfileResponse
        {
            UserName = profile.UserName ?? "",
            Email = profile.Email ?? "",
            FirstName = profile.FirstName ?? "",
            LastName = profile.LastName ?? "",
            Phone = profile.Phone ?? "",
            Instrument = profile.Instrument ?? "",
            OtherInstruments = profile.OtherInstruments ?? [],
            Roles = profile.Roles ?? [],
            Posts = profile.Posts ?? [],
            Medal = profile.Medal ?? "",
            GivenMedal = profile.GivenMedal ?? ""
        });
    }


    [HttpPut("username")]
    public async Task<IActionResult> ChangeUsername(
        [FromBody] ChangeUsernameRequest request)
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

        if (!await _userManager.CheckPasswordAsync(
                user,
                request.CurrentPassword))
        {
            return BadRequest(new
            {
                error = "InvalidCurrentPassword"
            });
        }

        if (string.IsNullOrWhiteSpace(request.NewUsername))
        {
            return BadRequest(new
            {
                error = "InvalidUsername"
            });
        }

        if (user.UserName == request.NewUsername)
        {
            return Ok(new
            {
                userName = user.UserName
            });
        }

        user.UserName = request.NewUsername;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors.Select(x => new
                {
                    code = x.Code,
                    description = x.Description
                })
            });
        }

        return Ok(new
        {
            userName = user.UserName
        });
    }


    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request)
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

        await using var transaction = _db.Database.IsRelational()
            ? await _db.Database.BeginTransactionAsync()
            : null;

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors.Select(x => new
                {
                    code = x.Code,
                    description = x.Description
                })
            });
        }

        var sessions = await _db.MobileSessions
            .Where(x => x.UserId == userId)
            .ToListAsync();

        _db.MobileSessions.RemoveRange(sessions);
        await _db.SaveChangesAsync();

        if (transaction != null)
        {
            await transaction.CommitAsync();
        }

        return NoContent();
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<ProfileStatisticsResponse>> GetStatistics(
        [FromQuery] string startDate = null,
        [FromQuery] string endDate = null)
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

        var now = DateTime.UtcNow;
        List<SignUp> signups;
        int totalGigs;

        if (startDate == null && endDate == null)
        {
            var lastYear = now.AddMonths(-12);

            signups = await _db.SignUps
                .AsNoTracking()
                .Include(x => x.Event)
                .Where(x => x.PersonId == userId)
                .Where(x => x.SignupTime > lastYear)
                .Where(x => x.Event.Day < now)
                .ToListAsync();

            totalGigs = await _db.Events
                .AsNoTracking()
                .Where(x => x.Type == AkEventTypes.Spelning)
                .Where(x => x.Day > lastYear)
                .Where(x => x.Day < now)
                .CountAsync();
        }
        else
        {
            if (!DateTime.TryParseExact(
                    startDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var start)
                || !DateTime.TryParseExact(
                    endDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var end)
                || start > end
                || end == DateTime.MaxValue.Date)
            {
                return BadRequest(new
                {
                    error = "A valid startDate and endDate are required."
                });
            }

            var endExclusive = end.AddDays(1);

            signups = await _db.SignUps
                .AsNoTracking()
                .Include(x => x.Event)
                .Where(x => x.PersonId == userId)
                .Where(x => x.Event.Day >= start)
                .Where(x => x.Event.Day < endExclusive)
                .Where(x => x.Event.Day < now.Date)
                .ToListAsync();

            totalGigs = await _db.Events
                .AsNoTracking()
                .Where(x => x.Type == AkEventTypes.Spelning)
                .Where(x => x.Day >= start)
                .Where(x => x.Day < endExclusive)
                .Where(x => x.Day < now.Date)
                .CountAsync();
        }

        return Ok(new ProfileStatisticsResponse
        {
            TotalGigs = totalGigs,
            Halan = signups.Count(x => x.Where == AkSignupType.Halan),
            Direct = signups.Count(x => x.Where == AkSignupType.Direct),
            CantCome = signups.Count(x => x.Where == AkSignupType.CantCome),
            Car = signups.Count(x => x.Car),
            Instrument = signups.Count(x => x.Instrument),
            Comment = signups.Count(x =>
                !string.IsNullOrWhiteSpace(x.Comment))
        });
    }
}
