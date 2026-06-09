using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            RemoveDbContextRegistrations(services);

            services.AddDbContext<AKContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName,
                _ => { });
        });
    }

    private static void RemoveDbContextRegistrations(IServiceCollection services)
    {
        var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AKContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(IDbContextOptionsConfiguration<AKContext>))
            .ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }

    public Task SeedAsync(Event evt) =>
        SeedAsync(db =>
        {
            db.Events.Add(evt);
            return Task.CompletedTask;
        });

    public async Task<int> SeedEventAndReturnIdAsync(Event evt)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        db.Events.Add(evt);
        await db.SaveChangesAsync();
        return evt.Id;
    }

    public async Task SeedAsync(Func<AKContext, Task> seed)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        await seed(db);
        await db.SaveChangesAsync();
    }

    public async Task SeedMemberAsync(
        string userName = TestUsers.MemberUserName,
        string instrument = "Flöjt")
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(AkRoles.Medlem))
        {
            await roleManager.CreateAsync(new IdentityRole(AkRoles.Medlem));
        }

        if (await userManager.FindByNameAsync(userName) != null)
        {
            return;
        }

        var user = new AkUser
        {
            UserName = userName,
            FirstName = "Test",
            LastName = "Member",
            Instrument = instrument
        };

        var result = await userManager.CreateAsync(user, "TestPassword1!");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, AkRoles.Medlem);
    }

    public HttpClient CreateClientWithHttpsBaseAddress()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }
}
