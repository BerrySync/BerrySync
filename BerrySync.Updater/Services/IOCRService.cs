using BerrySync.Data.Models;

namespace BerrySync.Updater.Services
{
    public interface IOCRService
    {
        Task<IEnumerable<FlavorOfTheDay>> StartOCRAsync(string month, int year);
    }
}
