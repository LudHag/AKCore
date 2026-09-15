using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Models.Api.V1.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AKCore.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();
    private readonly bool _useTestAuthentication;
    private const string MobileAuthSigningKey =
    "0123456789abcdef0123456789abcdef";

    public CustomWebApplicationFactory(
        bool useTestAuthentication = true)
    {
        _useTestAuthentication = useTestAuthentication;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            RemoveDbContextRegistrations(services);

            services.AddDbContext<AKContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            services.Configure<MobileAuthOptions>(options =>
            {
                options.Issuer = "AKCore.Tests";
                options.Audience = "AlteKamerer.Tests";
                options.SigningKey = MobileAuthSigningKey;
                options.AccessTokenMinutes = 15;
                options.RefreshTokenDays = 30;
            });

            services.PostConfigure<JwtBearerOptions>(
                "MobileBearer",
                options =>
                {
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = "AKCore.Tests",
                            ValidateAudience = true,
                            ValidAudience = "AlteKamerer.Tests",
                            ValidateLifetime = true,
                            RequireExpirationTime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(MobileAuthSigningKey)),
                            ClockSkew = TimeSpan.FromMinutes(1)
                        };
                });

            if (_useTestAuthentication)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    _ => { });
            }
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

    public Task SeedAdminAsync() =>
        SeedUserWithRoleAsync(
            TestUsers.AdminUserName,
            AkRoles.SuperNintendo,
            "Admin",
            "User");

    public Task SeedEditorAsync() =>
        SeedUserWithRoleAsync(
            TestUsers.EditorUserName,
            AkRoles.Editor,
            "Editor",
            "User");

    public Task SeedMemberAsync(
        string userName = TestUsers.MemberUserName,
        string instrument = "Flöjt") =>
        SeedUserWithRoleAsync(userName, AkRoles.Medlem, "Test", "Member", instrument);

    public async Task<string> SeedMemberAndReturnIdAsync(
        string userName = TestUsers.MemberUserName,
        string instrument = "Flöjt")
    {
        await SeedMemberAsync(userName, instrument);
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var user = await userManager.FindByNameAsync(userName);
        return user!.Id;
    }

    public async Task EnsureRoleExistsAsync(string roleName)
    {
        using var scope = Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private async Task SeedUserWithRoleAsync(
        string userName,
        string roleName,
        string firstName,
        string lastName,
        string instrument = "Flöjt")
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        if (await userManager.FindByNameAsync(userName) != null)
        {
            return;
        }

        var user = new AkUser
        {
            UserName = userName,
            FirstName = firstName,
            LastName = lastName,
            Instrument = instrument
        };

        var result = await userManager.CreateAsync(user, TestUsers.DefaultPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, roleName);
    }

    public HttpClient CreateClientWithHttpsBaseAddress()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }
}
