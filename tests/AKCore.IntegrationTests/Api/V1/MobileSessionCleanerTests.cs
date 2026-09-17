using AKCore.DataModel;
using AKCore.Services.Api.V1.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.Api.V1;

public class MobileSessionCleanerTests
{
    [Fact]
    public async Task ClearOldSessions_RemovesExpiredAndOldRevokedSessions()
    {
        await using var factory = new CustomWebApplicationFactory();
        var userId = await factory.SeedMemberAndReturnIdAsync();

        await SeedSessionsAsync(factory, userId);

        var cleaner =
            factory.Services.GetRequiredService<MobileSessionCleaner>();

        await cleaner.ClearOldSessionsAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var sessions = await db.MobileSessions
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        Assert.Equal(2, sessions.Count);
        Assert.Equal(new string('f', 64), sessions[0].RefreshTokenHash);
        Assert.Equal(new string('g', 64), sessions[1].RefreshTokenHash);
    }

    [Fact]
    public async Task ClearOldSessions_RunTwice_RemainsSuccessful()
    {
        await using var factory = new CustomWebApplicationFactory();
        var userId = await factory.SeedMemberAndReturnIdAsync();

        await SeedSessionsAsync(factory, userId);

        var cleaner =
            factory.Services.GetRequiredService<MobileSessionCleaner>();

        await cleaner.ClearOldSessionsAsync();
        await cleaner.ClearOldSessionsAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        Assert.Equal(2, await db.MobileSessions.CountAsync());
    }

    private static Task SeedSessionsAsync(
        CustomWebApplicationFactory factory,
        string userId)
    {
        var now = DateTime.UtcNow;

        return factory.SeedAsync(db =>
        {
            db.MobileSessions.AddRange(
                new MobileSession
                {
                    UserId = userId,
                    RefreshTokenHash = new string('d', 64),
                    CreatedAt = now.AddDays(-40),
                    ExpiresAt = now.AddDays(-1)
                },
                new MobileSession
                {
                    UserId = userId,
                    RefreshTokenHash = new string('e', 64),
                    CreatedAt = now.AddDays(-5),
                    ExpiresAt = now.AddDays(25),
                    RevokedAt = now.AddDays(-2)
                },
                new MobileSession
                {
                    UserId = userId,
                    RefreshTokenHash = new string('f', 64),
                    CreatedAt = now.AddDays(-3),
                    ExpiresAt = now.AddDays(27),
                    RevokedAt = now.AddHours(-1)
                },
                new MobileSession
                {
                    UserId = userId,
                    RefreshTokenHash = new string('g', 64),
                    CreatedAt = now.AddHours(-1),
                    ExpiresAt = now.AddDays(30)
                });

            return Task.CompletedTask;
        });
    }
}
