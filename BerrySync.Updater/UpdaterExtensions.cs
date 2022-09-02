using BerrySync.Data;
using BerrySync.Updater.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BerrySync.Updater
{
    public static class UpdaterExtensions
    {
        public static IServiceCollection AddUpdater(this IServiceCollection services)
        {
            services.AddDataContext();

            services.AddHostedService<HostedCrawlService>();
            services.AddSingleton<ICrawlService, CrawlService>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IImageArchiveService, ImageArchiveService>();
            services.AddSingleton<IOCRService, OCRService>();
            services.AddSingleton<IGoogleCalendarService, GoogleCalendarService>();

            return services;
        }

        public static IApplicationBuilder CreateDirStructure(this IApplicationBuilder app)
        {
            Directory.CreateDirectory(Constants.CalendarStorage);
            return app;
        }
    }
}