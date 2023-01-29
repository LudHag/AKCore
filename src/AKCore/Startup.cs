﻿using AKCore.DataModel;
using AKCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AKCore
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfigurationRoot Configuration { get; }

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
            services.AddTransient<PageService>();
            services.AddTransient<AdminLogService>();

            services.AddIdentity<AkUser, IdentityRole>()
                .AddEntityFrameworkStores<AKContext>()
                .AddDefaultTokenProviders();

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
}