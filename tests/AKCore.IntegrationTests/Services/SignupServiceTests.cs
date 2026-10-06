using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System.Reflection;

namespace AKCore.IntegrationTests.Services;

public class SignupServiceTests
{
    [Fact]
    public async Task SaveSignupAsync_WhenAnotherRequestSignsUpFirst_UpdatesTheExistingSignup()
    {
        var duplicateKey = new DuplicateKeyInterceptor();
        await using var factory = new CustomWebApplicationFactory(dbInterceptors: [duplicateKey]);

        var eventId = await factory.SeedEventAndReturnIdAsync(TestEvents.SignupSpelning());
        var userId = await factory.SeedMemberAndReturnIdAsync();

        duplicateKey.FailNextSaveAfter(() => factory.SeedAsync(async db =>
        {
            var spelning = await db.Events
                .Include(x => x.SignUps)
                .SingleAsync(x => x.Id == eventId);

            spelning.SignUps.Add(new SignUp
            {
                Person = TestUsers.MemberUserName,
                PersonId = userId,
                PersonName = "Test Member",
                Where = AkSignupType.Halan,
                SignupTime = DateTime.Now
            });
        }));

        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
            var user = await userManager.FindByNameAsync(TestUsers.MemberUserName);
            var signupService = scope.ServiceProvider.GetRequiredService<SignupService>();

            await signupService.SaveSignupAsync(
                new SignUpModel
                {
                    Where = AkSignupType.Direct,
                    Comment = "Kommer direkt"
                },
                eventId,
                user!);
        }

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AKContext>();
            var signup = await db.SignUps.SingleAsync();

            Assert.Equal(userId, signup.PersonId);
            Assert.Equal(AkSignupType.Direct, signup.Where);
            Assert.Equal("Kommer direkt", signup.Comment);
        }
    }

    /// <summary>
    /// The in memory provider does not enforce unique indexes, so the duplicate
    /// key failure a second concurrent signup would cause has to be simulated.
    /// </summary>
    private sealed class DuplicateKeyInterceptor : SaveChangesInterceptor
    {
        private Func<Task> _beforeFailing = () => Task.CompletedTask;
        private bool _shouldFail;

        public void FailNextSaveAfter(Func<Task> beforeFailing)
        {
            _beforeFailing = beforeFailing;
            _shouldFail = true;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (!_shouldFail)
            {
                return result;
            }

            _shouldFail = false;
            await _beforeFailing();

            throw new DbUpdateException("Duplicate entry", CreateDuplicateKeyException());
        }

        // MySqlException only exposes its error code through internal constructors.
        private static MySqlException CreateDuplicateKeyException() =>
            (MySqlException)Activator.CreateInstance(
                typeof(MySqlException),
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                args: [MySqlErrorCode.DuplicateKeyEntry, "Duplicate entry"],
                culture: null)!;
    }
}
