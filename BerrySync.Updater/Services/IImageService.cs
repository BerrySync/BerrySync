using SixLabors.ImageSharp;

namespace BerrySync.Updater.Services
{
    public interface IImageService
    {
        public Task ProcessCalendar();
        public Task CropCalendar(Rectangle cornerOffset, Rectangle entry, Rectangle offset);
    }
}
