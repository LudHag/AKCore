using AKCore.Extensions;
using AKCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AKCore.Middlewares;

public class MetricsMiddleware
{
    private Dictionary<string, int> loggedInRouteRequests = [];
    private Dictionary<string, int> loggedOutRouteRequests = [];
    private readonly RequestDelegate next;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly Task intervalTask;
    public MetricsMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        this.next = next;
        this.serviceScopeFactory = serviceScopeFactory;
        intervalTask = SetupInterval();
    }
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        try
        {
            if (
                context.Response.ContentType?.Contains("text/html") is true &&
                context.Response.StatusCode < 300 &&
                !context.Request.Path.ToString().Contains('.')
                )
            {
                var isLoggedIn = context.User.Identity != null && context.User.Identity.IsAuthenticated;

                var path = context.Request.Path.ToString();
                SavePath(GetNormalizedPath(path), isLoggedIn);
            }
        }
        catch
        {
        }
    }

    private static string GetNormalizedPath(string path)
    {
        if (path == "/")
        {
            return path;
        }

        var lowercase = path.ToLower();

        if (lowercase.StartsWith("/upcoming/event"))
        {
            return "/upcomming/event";
        }

        if (lowercase.StartsWith("/edit/page"))
        {
            return "/edit/page";
        }

        if (lowercase.StartsWith("/edit/page"))
        {
            return "/edit/page";
        }

        return lowercase.TrimEnd('/');
    }

    private void SavePath(string path, bool loggedIn)
    {
        var requestsStore = loggedIn ? loggedInRouteRequests : loggedOutRouteRequests;

        requestsStore[path] = requestsStore.TryGetValue(path, out var numberRequests) ? numberRequests + 1 : 1;
    }

    private async Task SetupInterval()
    {
        var intervalPeriod = TimeSpan.FromHours(1);

        while (true)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var metricsService = scope.ServiceProvider.GetRequiredService<MetricsService>();
                    var nowTime = DateTime.Now.ConvertToSwedishTime();

                    await metricsService.SaveMetrics(loggedInRouteRequests, true, nowTime);
                    await metricsService.SaveMetrics(loggedOutRouteRequests, false, nowTime);
                }
                loggedInRouteRequests = [];
                loggedOutRouteRequests = [];
            }
            catch
            {
            }
            await Task.Delay(intervalPeriod);
        }


    }
}
