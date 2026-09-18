using AKCore.DataModel;
using AKCore.Models;
using AKCore.Models.Api.V1.Events;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AKCore.Controllers.Api.V1;

[ApiController]
[Route("api/v1/events/{id:int}/registration")]
[Authorize(AuthenticationSchemes = "MobileBearer")]
public class EventRegistrationController : ControllerBase
{
    private readonly UserManager<AkUser> _userManager;
    private readonly SignupService _signupService;

    public EventRegistrationController(
        UserManager<AkUser> userManager,
        SignupService signupService)
    {
        _userManager = userManager;
        _signupService = signupService;
    }

    [HttpPut]
    [HttpPost]
    public async Task<IActionResult> Put(
        int id,
        EventRegistrationRequest request)
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

        var model = new SignUpModel
        {
            Where = request.Where,
            Car = request.Car,
            Instrument = request.Instrument,
            Comment = request.Comment,
            SelectedInstrument = request.SelectedInstrument
        };

        try
        {
            await _signupService.SaveSignupAsync(model, id, user);
        }
        catch (AkValidationError error)
        {
            if (error.Message == "InvalidId")
            {
                return NotFound();
            }

            return BadRequest();
        }

        return NoContent();
    }
}
