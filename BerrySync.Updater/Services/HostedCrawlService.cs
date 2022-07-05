using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BerrySync.Updater.Services
{
    public sealed class HostedCrawlService : IHostedService, IAsyncDisposable
    {
        private readonly ILogger<HostedCrawlService> _logger;
        private readonly ICrawlService _crawlService;
        private Timer? _timer;

        public HostedCrawlService(ILogger<HostedCrawlService> logger, ICrawlService crawlService)
        {
            _logger = logger;
            _crawlService = crawlService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            _logger.LogInformation($"{nameof(HostedCrawlService)} is running.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation($"{nameof(HostedCrawlService)} is stopping.");
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_timer is IAsyncDisposable timer)
            {
                await timer.DisposeAsync();
            }

            _timer = null;
        }

        private void DoWork(object? state)
        {
            _crawlService.StartCrawlAsync().Wait();
        }
    }
}
