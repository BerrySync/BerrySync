using BerrySync.Data.Data;
using BerrySync.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BerrySync.Data
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataContext(this IServiceCollection services)
        {
            services.AddDbContext<FlavorDbContext>();
            services.AddSingleton<IFlavorRepository, FlavorRepository>();
            services.AddSingleton<ICalendarEventRepository, CalendarEventRepository>();

            return services;
        }

        public static IApplicationBuilder SetupDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<FlavorDbContext>().Database.EnsureCreated();
            }
            return app;
        }
    }
}
