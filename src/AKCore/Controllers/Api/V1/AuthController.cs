using AKCore.DataModel;
using AKCore.Models.Api.V1.Auth;
using AKCore.Services.Api.V1.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers.Api.V1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AkUser> _userManager;
    private readonly AKContext _db;
    private readonly MobileTokenService _mobileTokenService;

    public AuthController(
        UserManager<AkUser> userManager,
        AKContext db,
        MobileTokenService mobileTokenService)
    {
        _userManager = userManager;
        _db = db;
        _mobileTokenService = mobileTokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null ||
            !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        var now = DateTime.UtcNow;
        ClearOldSessions(now);

        var accessToken =
            _mobileTokenService.CreateAccessToken(user);

        var refreshToken =
            _mobileTokenService.CreateRefreshToken();

        var session = new MobileSession
        {
            UserId = user.Id,
            RefreshTokenHash =
                _mobileTokenService.HashRefreshToken(refreshToken),
            CreatedAt = now,
            ExpiresAt =
                _mobileTokenService.GetRefreshTokenExpiry()
        };

        _db.MobileSessions.Add(session);
        await _db.SaveChangesAsync();

        return Ok(new LoginResponse
        {
            Authenticated = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var refreshTokenHash =
            _mobileTokenService.HashRefreshToken(request.RefreshToken);

        var session = await _db.MobileSessions
            .Include(x => x.User)
            .SingleOrDefaultAsync(
                x => x.RefreshTokenHash == refreshTokenHash);

        var now = DateTime.UtcNow;

        if (session == null ||
            session.RevokedAt != null ||
            session.ExpiresAt <= now)
        {
            return Unauthorized(new
            {
                message = "Invalid refresh token."
            });
        }

        ClearOldSessions(now);

        session.RevokedAt = now;

        var accessToken =
            _mobileTokenService.CreateAccessToken(session.User);

        var refreshToken =
            _mobileTokenService.CreateRefreshToken();

        var replacementSession = new MobileSession
        {
            UserId = session.UserId,
            RefreshTokenHash =
                _mobileTokenService.HashRefreshToken(refreshToken),
            CreatedAt = now,
            ExpiresAt =
                _mobileTokenService.GetRefreshTokenExpiry()
        };

        _db.MobileSessions.Add(replacementSession);
        await _db.SaveChangesAsync();

        return Ok(new LoginResponse
        {
            Authenticated = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        var refreshTokenHash =
            _mobileTokenService.HashRefreshToken(request.RefreshToken);

        var session = await _db.MobileSessions
            .SingleOrDefaultAsync(
                x => x.RefreshTokenHash == refreshTokenHash);

        if (session != null && session.RevokedAt == null)
        {
            session.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    private void ClearOldSessions(DateTime now)
    {
        var revokedCutoff = now.AddDays(-30);

        _db.MobileSessions.RemoveRange(
            _db.MobileSessions.Where(x =>
                x.ExpiresAt <= now ||
                (x.RevokedAt != null &&
                x.RevokedAt <= revokedCutoff)));
    }

}
