using BerrySync.Data.Models;
using Google.Apis.Calendar.v3.Data;

namespace BerrySync.Updater.Services
{
    public interface IGoogleCalendarService
    {
        public Task AddAsync(IEnumerable<FlavorOfTheDay> flavors);
        public Task UpdateAsync(IEnumerable<FlavorOfTheDay> flavors);
        public Task InsertAsync(IEnumerable<FlavorOfTheDay> flavors);
    }
}
