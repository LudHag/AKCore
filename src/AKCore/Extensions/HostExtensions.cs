using AKCore.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace AKCore.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase(this IHost host)
        {

            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AKContext>();
            var migrations = dbContext.DatabaseAccessor.GetPendingMigrations();

            if (migrations.Any())
            {
                dbContext.DatabaseAccessor.Migrate();
            }

            return host;

        }
    }
}
