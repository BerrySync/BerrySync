using BerrySync.Data.Models;

namespace BerrySync.Updater.Services
{
    public interface IGoogleCalendarService
    {
        public Task AddAsync(IEnumerable<FlavorOfTheDay> flavors);
    }
}
