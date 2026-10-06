using AKCore.DataModel;
using AKCore.Models.Api.V1.Me;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AKCore.Controllers.Api.V1;

[ApiController]
[Route("api/v1/me")]
[Authorize(AuthenticationSchemes = "MobileBearer")]
public class MeController : ControllerBase
{
    private readonly UserManager<AkUser> _userManager;

    public MeController(UserManager<AkUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<MeResponse>> Get()
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

        return Ok(new MeResponse
        {
            DisplayName = user.GetName(),
            IsMember = roles.Contains(AkRoles.Medlem),
            IsBallet = roles.Contains(AkRoles.Balett),
            AvailableInstruments = SignupService.GetInstrumentsForUser(user)
        });
    }
}
