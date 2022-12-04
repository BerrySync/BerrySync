using BerrySync.Data.Models;
using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace BerrySync.Updater.Services
{
    public class OCRService : IOCRService
    {
        private readonly ILogger<OCRService> _logger;
        private readonly ImageAnnotatorClient _vision;

        public OCRService(ILogger<OCRService> logger)
        {
            _logger = logger;
            _vision = ImageAnnotatorClient.Create();
        }

        public async Task<IEnumerable<FlavorOfTheDay>> StartOCRAsync(string month, int year)
        {
            var days = new List<FlavorOfTheDay>();

            foreach (var fileName in Directory.EnumerateFiles($"{Constants.WorkDir}/crop"))
            {
                var img = Image.FromFile(fileName);
                var result = (await _vision.DetectTextAsync(img)).FirstOrDefault()?.Description.Replace('\n', ' ');
                if (result is not null && !result.ToLower().Contains("closed"))
                {
                    var date = Regex.Match(result, @"\d+").Value;

                    if (!string.IsNullOrEmpty(date))
                    {
                        days.Add(new FlavorOfTheDay
                        {
                            Date = Convert.ToDateTime($"{month} {date}, {year}"),
                            Flavor = result.Replace($"{date}", "").Trim()
                        });
                    }
                }
            }

            return days;
        }
    }
}
