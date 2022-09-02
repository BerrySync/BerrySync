using BerrySync.Data.Models;
using BerrySync.Data.Repositories;

namespace BerrySync.Updater.Services
{
    public class ImageArchiveService : IImageArchiveService
    {
        private readonly ICalendarImageRepository _images;

        public ImageArchiveService(ICalendarImageRepository images)
        {
            _images = images;
        }

        public async Task ArchiveCalendar(int year, string month)
        {
            var filePath = $"{Constants.CalendarStorage}/{year}-{month}.jpg";
            if (!File.Exists(filePath))
            {
                File.Move(Constants.CalendarFile, filePath);
            }

            await _images.TryAddCalendarImageAsync(new CalendarImage
            {
                Year = year,
                Month = month,
                FilePath = filePath
            });
        }
    }
}
