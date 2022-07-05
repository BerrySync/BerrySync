using BerrySync.Data.Models;
using Google.Apis.Calendar.v3.Data;

namespace BerrySync.Updater.Services
{
    public interface IGoogleCalendarService
    {
        public Task<IEnumerable<FlavorOfTheDay>> AddAsync(IEnumerable<FlavorOfTheDay> flavors);
        public Task<Events> GetAsync(DateTime start, DateTime end);
        public Task<IEnumerable<FlavorOfTheDay>> UpdateAsync(IEnumerable<FlavorOfTheDay> flavors, IList<Event> events);
        public Task<IEnumerable<FlavorOfTheDay>> InsertAsync(IEnumerable<FlavorOfTheDay> flavors);
    }
}
