using AKCore.Extensions;
using AKCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Middlewares;

public class MetricsMiddleware
{
    private Dictionary<string, int> loggedInRouteRequests = [];
    private Dictionary<string, int> loggedOutRouteRequests = [];
    private int mobileInRequests = 0;
    private int mobileOutRequests = 0;
    private int desktopInRequests = 0;
    private int desktopOutRequests = 0;
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
                var userAgent = context.Request.Headers.UserAgent.ToString().ToLower();

                SavePath(GetNormalizedPath(path), userAgent, isLoggedIn);
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

    private void SavePath(string path, string userAgent, bool loggedIn)
    {
        if (IsCrawler(userAgent))
        {
            return;
        }

        var requestsStore = loggedIn ? loggedInRouteRequests : loggedOutRouteRequests;

        requestsStore[path] = requestsStore.TryGetValue(path, out var numberRequests) ? numberRequests + 1 : 1;

        var isDesktop = IsDesktop(userAgent);
        if (isDesktop)
        {
            if(loggedIn)
            {
                desktopInRequests++;
            }
            else
            {
                desktopOutRequests++;
            }
        }
        else
        {
            if(loggedIn)
            {
                mobileInRequests++;
            }
            else
            {
                mobileOutRequests++;
            }
        }
    }

    private static bool IsCrawler(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return true;
        }

        var crawlerIndicators = new[]
        {
            "bot", "crawler", "spider", "scraper",
            "curl", "wget", "python-requests", "httpclient", "go-http-client", "java/",
            "googlebot", "bingbot", "duckduckbot", "baiduspider", "yandexbot", "twitterbot", "linkedinbot",
            "embedly", "applebot"
        };

        return crawlerIndicators.Any(indicator => userAgent.Contains(indicator));
    }

    private static bool IsDesktop(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return false;
        }

        var mobileIndicators = new[]
        {
            "mobile", "android", "iphone", "ipad", "tablet"
        };

        return !mobileIndicators.Any(indicator => userAgent.Contains(indicator));
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

                    await metricsService.SaveMetrics(loggedInRouteRequests, true, mobileInRequests, desktopInRequests, nowTime);
                    await metricsService.SaveMetrics(loggedOutRouteRequests, false, mobileOutRequests, desktopOutRequests, nowTime);
                }
                loggedInRouteRequests = [];
                loggedOutRouteRequests = [];
                mobileInRequests = 0;
                mobileOutRequests = 0;
                desktopInRequests = 0;
                desktopOutRequests = 0;
            }
            catch
            {
            }
            await Task.Delay(intervalPeriod);
        }


    }
}
