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
                context.Response.ContentType?.Contains("text/html") is true
                )
            {
                var isLoggedIn = context.User.Identity != null && context.User.Identity.IsAuthenticated;

                var path = context.Request.Path.ToString();
                SavePath(path, isLoggedIn);
            }
        }
        catch
        {
        }
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

                    await metricsService.SaveMetrics(loggedInRouteRequests, true);
                    await metricsService.SaveMetrics(loggedOutRouteRequests, false);
                }
                loggedInRouteRequests = [];
            }
            catch
            {
            }
            await Task.Delay(intervalPeriod);
        }


    }
}
