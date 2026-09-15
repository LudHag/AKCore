using AKCore.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.DataModel;

public class MobileSessionTests
{
    [Fact]
    public async Task MobileSession_CanBePersistedForExistingUser()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var createdAt = DateTime.UtcNow;
        var expiresAt = createdAt.AddDays(30);

        await factory.SeedAsync(db =>
        {
            db.MobileSessions.Add(new MobileSession
            {
                UserId = userId,
                RefreshTokenHash = new string('a', 64),
                CreatedAt = createdAt,
                ExpiresAt = expiresAt
            });

            return Task.CompletedTask;
        });

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var session = await db.MobileSessions
            .Include(x => x.User)
            .SingleAsync();

        Assert.Equal(userId, session.UserId);
        Assert.Equal(new string('a', 64), session.RefreshTokenHash);
        Assert.Equal(createdAt, session.CreatedAt);
        Assert.Equal(expiresAt, session.ExpiresAt);
        Assert.Null(session.RevokedAt);
        Assert.NotNull(session.User);
        Assert.Equal(userId, session.User.Id);
    }

    [Fact]
    public async Task MobileSession_CanBeRevoked()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        await factory.SeedAsync(db =>
        {
            db.MobileSessions.Add(new MobileSession
            {
                UserId = userId,
                RefreshTokenHash = new string('b', 64),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });

            return Task.CompletedTask;
        });

        DateTime revokedAt;

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AKContext>();
            var session = await db.MobileSessions.SingleAsync();

            revokedAt = DateTime.UtcNow;
            session.RevokedAt = revokedAt;

            await db.SaveChangesAsync();
        }

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AKContext>();
            var session = await db.MobileSessions.SingleAsync();

            Assert.Equal(revokedAt, session.RevokedAt);
        }
    }
}
