using BerrySync.Data.Models;

namespace BerrySync.Data.Repositories
{
    public interface IFlavorRepository
    {
        public Task<bool> AddFlavorOfTheDayAsync(FlavorOfTheDay flavorOfTheDay);
        public Task<bool> AddFlavorOfTheDayRangeAsync(IEnumerable<FlavorOfTheDay> flavors);
        public Task<FlavorOfTheDay> GetFlavorOfTheDayAsync(DateTime dateTime);
        public Task<IEnumerable<FlavorOfTheDay>> GetFlavorOfTheDayRangeAsync(DateTime start, DateTime end);
        public Task<IEnumerable<DateTime>> GetDaysWithFlavorAsync(DateTime start, DateTime end, string flavor);
        public Task<IEnumerable<DateTime>> GetDaysWithFlavorAsync(DateTime start, string flavor);
        public Task<DateTime?> GetNextDayWithFlavorAsync(string flavor);
        public Task<IEnumerable<DateTime>> GetUpcomingDaysWithFlavorAsync(string flavor);
        public Task<bool> ModifyFlavorOfTheDayAsync(FlavorOfTheDay flavorOfTheDay);
    }
}
