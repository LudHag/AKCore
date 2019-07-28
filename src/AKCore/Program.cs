using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AKCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .CaptureStartupErrors(true) // the default
                .UseSetting("detailedErrors", "true")
                .ConfigureKestrel((context, options) =>
                {
                    // Set properties and call methods on options
                });
    }
}
