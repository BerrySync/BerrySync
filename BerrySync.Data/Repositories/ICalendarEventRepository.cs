using BerrySync.Data.Models;

namespace BerrySync.Data.Repositories
{
    public interface ICalendarEventRepository
    {
        public Task<bool> AddCalendarEventAsync(CalendarEvent calendarEvent);
        public Task<bool> AddCalendarEventRangeAsync(IEnumerable<CalendarEvent> calendarEvents);
        public Task<CalendarEvent> GetCalendarEventAsync(DateTime dateTime);
        public Task<IEnumerable<CalendarEvent>> GetCalendarEventRangeAsync(DateTime start, DateTime end);
        public Task<bool> ModifyCalenderEventAsync(CalendarEvent calendarEvent);
        public Task<bool> RemoveCalendarEventAsync(DateTime dateTime);
        public Task RemoveCalendarEventRangeAsync(DateTime start, DateTime end);
    }
}
