using BerrySync.Data.Models;

namespace BerrySync.Data.Repositories
{
    public interface ICalendarImageRepository
    {
        public Task<bool> TryAddCalendarImageAsync(CalendarImage image);
        public Task<CalendarImage> GetCalendarImageAsync(int year, string month);
    }
}
