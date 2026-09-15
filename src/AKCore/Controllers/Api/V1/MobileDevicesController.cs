using AKCore.DataModel;
using AKCore.Models.Api.V1.Devices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AKCore.Controllers.Api.V1;

[ApiController]
[Route("api/v1/devices")]
[Authorize(AuthenticationSchemes = "MobileBearer")]
public class MobileDevicesController : ControllerBase
{
    private const string FcmProvider = "fcm";
    private const string AndroidPlatform = "android";

    private readonly AKContext _db;
    private readonly UserManager<AkUser> _userManager;

    public MobileDevicesController(
        AKContext db,
        UserManager<AkUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpPut("{installationId}")]
    public async Task<IActionResult> Put(
        string installationId,
        DeviceRegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(installationId) ||
            installationId.Length > 128 ||
            string.IsNullOrWhiteSpace(request.PushToken) ||
            request.Platform != AndroidPlatform)
        {
            return BadRequest();
        }

        var userId =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        var installationDevice = await _db.MobileDevices
            .SingleOrDefaultAsync(
                x => x.InstallationId == installationId);

        var tokenDevice = await _db.MobileDevices
            .SingleOrDefaultAsync(
                x => x.PushToken == request.PushToken);

        if (installationDevice != null &&
            tokenDevice != null &&
            installationDevice.Id != tokenDevice.Id)
        {
            _db.MobileDevices.Remove(tokenDevice);
            await _db.SaveChangesAsync();
            tokenDevice = null;
        }

        var now = DateTime.UtcNow;
        var device = installationDevice ?? tokenDevice;

        if (device == null)
        {
            device = new MobileDevice
            {
                UserId = user.Id,
                InstallationId = installationId,
                PushToken = request.PushToken,
                Provider = FcmProvider,
                Platform = AndroidPlatform,
                CreatedAt = now,
                UpdatedAt = now
            };

            _db.MobileDevices.Add(device);
        }
        else
        {
            device.UserId = user.Id;
            device.InstallationId = installationId;
            device.PushToken = request.PushToken;
            device.Provider = FcmProvider;
            device.Platform = AndroidPlatform;
            device.UpdatedAt = now;
        }

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{installationId}")]
    public async Task<IActionResult> Delete(string installationId)
    {
        var userId =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var device = await _db.MobileDevices
            .SingleOrDefaultAsync(
                x => x.InstallationId == installationId &&
                     x.UserId == userId);

        if (device != null)
        {
            _db.MobileDevices.Remove(device);
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }
}
