using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AKCore.Migrations;

public class MetricsMiddleware(RequestDelegate next)
{
    private IDictionary<string, int> routeRequests = new Dictionary<string, int>();
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);


        if (
            context.Response.ContentType?.Contains("text/html") is true
            )
        {
            var path = context.Request.Path.ToString();
            if(routeRequests.TryGetValue(path, out var numberRequests))
            {
                routeRequests[path] = numberRequests + 1;
            }
            else
            {
                routeRequests[path] = 1;
            }


            Console.WriteLine("Logging "  + routeRequests[path] + " request to :" + context.Request.Path);

        }
    }
}
