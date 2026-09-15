using AKCore.Clients;
using AKCore.DataModel;
using AKCore.Middlewares;
using AKCore.Models;
using AKCore.Models.Api.V1.Auth;
using AKCore.Services;
using AKCore.Services.Api.V1.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace AKCore;

public class Startup
{
    public Startup(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
            .AddJsonFile("manifest.json", true, true)
            .AddEnvironmentVariables();
        Configuration = builder.Build();
    }
    public IConfigurationRoot Configuration { get; }

    private static AssetsModel CreateAssetsModel(IConfiguration configuration)
    {
        var assetsSection = configuration.GetSection("assets");
        var assetsDictionary = new Dictionary<string, AssetModel>();

        foreach (var assetSection in assetsSection.GetChildren())
        {
            var assetName = assetSection.Key;
            var entrypoint = assetSection["entrypoint"] ?? "";
            var js = assetSection.GetSection("js").Get<string[]>() ?? [];
            var css = assetSection.GetSection("css").Get<string[]>() ?? [];

            assetsDictionary[assetName] = new AssetModel(entrypoint, js, css);
        }

        return new AssetsModel(assetsDictionary);
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddDbContext<AKContext>(options => options.UseMySql(Configuration["DbConnectionString"], new MySqlServerVersion(new Version(5, 6, 0))));

        services.AddRouting();
        services.AddDistributedMemoryCache();

#if DEBUG
#else
        services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            options.Filters.Add(new Microsoft.AspNetCore.Mvc.RequireHttpsAttribute());
        });
#endif
        services.AddControllersWithViews().AddNewtonsoftJson();
        services.AddSession();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddSingleton(new AkSettings()
        {
            CDN = Configuration["akcdn"]
        });
        services.AddTransient<SitemapService>();
        services.AddTransient<PageService>();
        services.AddTransient<AlbumService>();
        services.AddTransient<AdminLogService>();
        services.AddTransient<MediaService>();
        services.AddTransient<RecruitService>();
        services.AddTransient<MailBoxService>();
        services.AddTransient<EventService>();
        services.AddTransient<SignupService>();
        services.AddTransient<UserAdminService>();
        services.AddTransient<MenuService>();
        services.AddScoped<TranslationsService>();
        services.AddScoped<MetricsService>();
        services.AddScoped<UsageService>();
        services.AddSingleton<UsageCollector>();
        services.AddSingleton<SameDayNotificationRunner>();

        var apiSecret = Configuration["OpenApiSecret"];
        services.AddTransient(x => new OpenApiClient(apiSecret ?? ""));

        var assetsModel = CreateAssetsModel(Configuration);
        services.AddSingleton(assetsModel);

        services.AddIdentity<AkUser, IdentityRole>()
            .AddEntityFrameworkStores<AKContext>()
            .AddDefaultTokenProviders();

        var mobileAuthOptions = new MobileAuthOptions();
        Configuration
            .GetSection(MobileAuthOptions.SectionName)
            .Bind(mobileAuthOptions);

        services.Configure<MobilePushOptions>(
            Configuration.GetSection(MobilePushOptions.SectionName));

        services.AddSingleton(_ =>
        {
            var projectId =
                Configuration[$"{MobilePushOptions.SectionName}:ProjectId"];

            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new InvalidOperationException(
                    "MobilePush project ID is not configured.");
            }

            return FirebaseApp.Create(
                new AppOptions
                {
                    ProjectId = projectId,
                    Credential = GoogleCredential.GetApplicationDefault()
                });
        });

        services.Configure<MobileAuthOptions>(
            Configuration.GetSection(MobileAuthOptions.SectionName));

        services.AddTransient<MobileTokenService>();
        services.AddTransient<MobileNotificationDeliveryService>();
        services.AddTransient<SameDayNotificationRelevanceService>();
        services.AddTransient<MobileNotificationService>();
        services.AddTransient<SameDayNotificationProcessor>();
        services.AddTransient<IFcmNotificationSender, FcmNotificationSender>();

        services.AddAuthentication()
            .AddJwtBearer("MobileBearer", options =>
            {
                options.MapInboundClaims = false;

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = mobileAuthOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = mobileAuthOptions.Audience,

                    ValidateLifetime = true,
                    RequireExpirationTime = true,

                    ValidateIssuerSigningKey = true,

                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                if (!string.IsNullOrWhiteSpace(mobileAuthOptions.SigningKey))
                {
                    tokenValidationParameters.IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                mobileAuthOptions.SigningKey));
                }

                options.TokenValidationParameters =
                    tokenValidationParameters;
            });

        services.ConfigureApplicationCookie(options => options.LoginPath = "/");

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Start hourly usage flush loop
        app.ApplicationServices.GetRequiredService<UsageCollector>();

        if (!env.IsEnvironment("Testing"))
        {
            app.ApplicationServices.GetRequiredService<SameDayNotificationRunner>();
        }

        app.UseStaticFiles();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Page/Error");
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseSession();
        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();


        app.UseMiddleware<MetricsMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapControllerRoute(
                "StartPage",
                "",
                new { controller = "Page", action = "Page" });
            endpoints.MapControllerRoute(
                "Page",
                "{slug}",
                new { controller = "Page", action = "Page" });


        });



        app.Use(async (context, next) =>
        {
            await next.Invoke();
            if (
                context.Request.Path.ToString().Contains("hire")
                )
            {
                Console.WriteLine(context.Response.ContentType);

            }
        });


        if (env.IsDevelopment())
        {
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "./";
                spa.Options.DevServerPort = 5173;
                // Doesnt actually run react but simply runs npm script and awaits console to write "Starting the development server"
                spa.UseReactDevelopmentServer(npmScript: "start");
            });
        }

    }
}
