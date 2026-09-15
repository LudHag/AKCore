using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers.Api.V1;

[ApiController]
[Route("api/v1/mobile-push/health")]
[AllowAnonymous]
public class MobilePushHealthController : ControllerBase
{
    private readonly MobilePushHealth _health;

    public MobilePushHealthController(
        MobilePushHealth health)
    {
        _health = health;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var response = new
        {
            status = _health.Status.ToString(),
            lastSuccessfulSendAt =
                _health.LastSuccessfulSendAt,
            lastFailureAt =
                _health.LastFailureAt
        };

        if (_health.Status ==
            MobilePushHealthStatus.Unhealthy)
        {
            return StatusCode(503, response);
        }

        return Ok(response);
    }
}