using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace BerrySync.Updater.Services
{
    public class CrawlService : ICrawlService
    {
        private readonly ILogger<CrawlService> _logger;
        private readonly IImageService _imageService;
        private readonly IOCRService _ocrService;
        private readonly IGoogleCalendarService _calendarService;

        public CrawlService(ILogger<CrawlService> logger, IImageService imageService, IOCRService ocrService, IGoogleCalendarService calendarService)
        {
            _logger = logger;
            _imageService = imageService;
            _ocrService = ocrService;
            _calendarService = calendarService;
        }

        public async Task StartCrawlAsync()
        {
            var urls = GetImages();
            var dataRegex = new Regex("[a-zA-Z0-9]+", RegexOptions.Singleline);
            _logger.LogInformation(String.Join("\n", urls));
            foreach (var url in urls)
            {
                _logger.LogDebug($"Calendar URL found: {url}");

                await DownloadCalendarAsync(url);
                await _imageService.ProcessCalendar();

                var matches = dataRegex.Matches(Path.GetFileName(new Uri(url).AbsolutePath));
                var days = await _ocrService.StartOCRAsync(matches[0].Value, Int32.Parse(matches[1].Value));
                days = await _calendarService.AddAsync(days);
                // db stuff

                Directory.Delete(Constants.WorkDir, true);
            }
        }

        public static IEnumerable<string> GetImages()
        {
            var client = new HtmlWeb();
            var regex = new Regex("https://.*?(calendar.jpg)", RegexOptions.IgnoreCase);
            return regex.Matches(client
                    .Load(Constants.Url)
                    .Text)
                .Select(x => x.Value)
                .Distinct();
        }

        public static async Task DownloadCalendarAsync(string url)
        {
            if (!Directory.Exists(Constants.WorkDir)) Directory.CreateDirectory(Constants.WorkDir);
            using var client = new HttpClient();
            using var stream = await client.GetStreamAsync(url);
            await stream.CopyToAsync(new FileStream(Constants.CalendarFile, FileMode.Create));
        }
    }
}
