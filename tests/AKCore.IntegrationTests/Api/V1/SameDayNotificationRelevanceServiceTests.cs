using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.Api.V1;

public class SameDayNotificationRelevanceServiceTests
{
    [Fact]
    public async Task GetCandidates_AttendingSignupOverridesEventType()
    {
        await using var factory = new CustomWebApplicationFactory();

        var memberId = await factory.SeedMemberAndReturnIdAsync();

        await factory.SeedAsync(db =>
        {
            db.Events.Add(new Event
            {
                Type = AkEventTypes.Evenemang,
                Name = "Registered event",
                Day = DateTime.UtcNow.Date,
                SignUps =
                [
                    new SignUp
                    {
                        Person = TestUsers.MemberUserName,
                        PersonId = memberId,
                        PersonName = "Test Member",
                        Where = AkSignupType.Halan,
                        SignupTime = DateTime.UtcNow
                    }
                ]
            });

            return Task.CompletedTask;
        });

        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService<SameDayNotificationRelevanceService>();

        var candidates =
            await service.GetCandidatesAsync(DateTime.UtcNow.Date);

        var candidate = Assert.Single(candidates);
        Assert.Equal(memberId, candidate.UserId);
    }

    [Fact]
    public async Task GetCandidates_CantComeOverridesRelevantRehearsal()
    {
        await using var factory = new CustomWebApplicationFactory();

        var memberId = await factory.SeedMemberAndReturnIdAsync();

        await factory.SeedAsync(db =>
        {
            db.Events.Add(new Event
            {
                Type = AkEventTypes.KarRep,
                Name = AkEventTypes.KarRep,
                Day = DateTime.UtcNow.Date,
                SignUps =
                [
                    new SignUp
                    {
                        Person = TestUsers.MemberUserName,
                        PersonId = memberId,
                        PersonName = "Test Member",
                        Where = AkSignupType.CantCome,
                        SignupTime = DateTime.UtcNow
                    }
                ]
            });

            return Task.CompletedTask;
        });

        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService<SameDayNotificationRelevanceService>();

        var candidates =
            await service.GetCandidatesAsync(DateTime.UtcNow.Date);

        Assert.Empty(candidates);
    }

    [Fact]
    public async Task GetCandidates_RepAndBalletRepUseMembershipRoles()
    {
        await using var factory = new CustomWebApplicationFactory();

        var orchestraId =
            await factory.SeedMemberAndReturnIdAsync("orchestra-member");

        var balletId =
            await factory.SeedMemberAndReturnIdAsync("ballet-member");

        await factory.EnsureRoleExistsAsync(AkRoles.Balett);

        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AkUser>>();

            var balletUser =
                await userManager.FindByNameAsync("ballet-member");

            Assert.NotNull(balletUser);

            var result =
                await userManager.AddToRoleAsync(
                    balletUser,
                    AkRoles.Balett);

            Assert.True(
                result.Succeeded,
                string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description)));
        }

        var rep = new Event
        {
            Type = AkEventTypes.Rep,
            Name = AkEventTypes.Rep,
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var balletRep = new Event
        {
            Type = AkEventTypes.BalettRep,
            Name = AkEventTypes.BalettRep,
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        await factory.SeedAsync(db =>
        {
            db.Events.AddRange(rep, balletRep);
            return Task.CompletedTask;
        });

        using var serviceScope = factory.Services.CreateScope();
        var service = serviceScope.ServiceProvider
            .GetRequiredService<SameDayNotificationRelevanceService>();

        var candidates =
            await service.GetCandidatesAsync(DateTime.UtcNow.Date);

        Assert.Contains(
            candidates,
            x => x.EventId == rep.Id &&
                 x.UserId == orchestraId);

        Assert.DoesNotContain(
            candidates,
            x => x.EventId == rep.Id &&
                 x.UserId == balletId);

        Assert.Contains(
            candidates,
            x => x.EventId == balletRep.Id &&
                 x.UserId == balletId);

        Assert.DoesNotContain(
            candidates,
            x => x.EventId == balletRep.Id &&
                 x.UserId == orchestraId);
    }

    [Theory]
    [InlineData(AkEventTypes.KarRep)]
    [InlineData(AkEventTypes.AthenRep)]
    [InlineData(AkEventTypes.Samlingsrep)]
    [InlineData(AkEventTypes.FikaRep)]
    public async Task GetCandidates_CommonRehearsalTypesNotifyMember(
        string eventType)
    {
        await using var factory = new CustomWebApplicationFactory();

        var memberId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = eventType,
            Name = eventType,
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        await factory.SeedAsync(db =>
        {
            db.Events.Add(evt);
            return Task.CompletedTask;
        });

        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService<SameDayNotificationRelevanceService>();

        var candidates =
            await service.GetCandidatesAsync(DateTime.UtcNow.Date);

        Assert.Contains(
            candidates,
            x => x.EventId == evt.Id &&
                 x.UserId == memberId);
    }

    [Fact]
    public async Task GetCandidates_UnrelatedOrOtherDayEventIsExcluded()
    {
        await using var factory = new CustomWebApplicationFactory();

        await factory.SeedMemberAsync();

        await factory.SeedAsync(db =>
        {
            db.Events.AddRange(
                new Event
                {
                    Type = AkEventTypes.Evenemang,
                    Name = "Unregistered event",
                    Day = DateTime.UtcNow.Date,
                    SignUps = []
                },
                new Event
                {
                    Type = AkEventTypes.KarRep,
                    Name = "Tomorrow rehearsal",
                    Day = DateTime.UtcNow.Date.AddDays(1),
                    SignUps = []
                });

            return Task.CompletedTask;
        });

        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService<SameDayNotificationRelevanceService>();

        var candidates =
            await service.GetCandidatesAsync(DateTime.UtcNow.Date);

        Assert.Empty(candidates);
    }

    [Fact]
    public async Task GetCandidates_MultiplePositiveReasonsProduceOneCandidate()
    {
        await using var factory = new CustomWebApplicationFactory();

        var memberId = await factory.SeedMemberAndReturnIdAsync();

        await factory.SeedAsync(db =>
        {
            db.Events.Add(new Event
            {
                Type = AkEventTypes.KarRep,
                Name = AkEventTypes.KarRep,
                Day = DateTime.UtcNow.Date,
                SignUps =
                [
                    new SignUp
                    {
                        Person = TestUsers.MemberUserName,
                        PersonId = memberId,
                        PersonName = "Test Member",
                        Where = AkSignupType.Direct,
                        SignupTime = DateTime.UtcNow
                    }
                ]
            });

            return Task.CompletedTask;
        });

        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService<SameDayNotificationRelevanceService>();

        var candidates =
            await service.GetCandidatesAsync(DateTime.UtcNow.Date);

        Assert.Single(candidates);
    }
}
