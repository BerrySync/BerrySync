using BerrySync.Data;
using BerrySync.Updater.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BerrySync.Updater
{
    public static class UpdaterExtensions
    {
        public static IServiceCollection AddUpdater(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataContext(configuration);

            services.AddHostedService<HostedCrawlService>();
            services.AddSingleton<ICrawlService, CrawlService>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IOCRService, OCRService>();
            services.AddSingleton<IGoogleCalendarService, GoogleCalendarService>();

            return services;
        }
    }
}