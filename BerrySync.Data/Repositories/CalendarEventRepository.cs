using BerrySync.Data.Data;
using BerrySync.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BerrySync.Data.Repositories
{
    public class CalendarEventRepository : ICalendarEventRepository
    {
        private readonly IServiceProvider _sp;

        public CalendarEventRepository(IServiceProvider sp)
        {
            _sp = sp;
        }

        public async Task<bool> AddCalendarEventAsync(CalendarEvent calendarEvent)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    if (await DbContainsDate(db, calendarEvent))
                    {
                        return false;
                    }
                    await db.CalendarEvents.AddAsync(calendarEvent);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<bool> AddCalendarEventRangeAsync(IEnumerable<CalendarEvent> calendarEvents)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    if (await db.CalendarEvents
                        .Select(x => x.Date)
                        .Intersect(calendarEvents
                            .Select(x => x.Date))
                        .AnyAsync())
                    {
                        return false;
                    }

                    await db.CalendarEvents.AddRangeAsync(calendarEvents);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<CalendarEvent> GetCalendarEventAsync(DateTime dateTime)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    return await db.CalendarEvents
                        .Include(x => x.FlavorOfTheDay)
                        .FirstOrDefaultAsync(x => x.Date == dateTime);
                }
            }
        }

        public async Task<IEnumerable<CalendarEvent>> GetCalendarEventRangeAsync(DateTime start, DateTime end)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    return await db.CalendarEvents
                        .Include(x => x.FlavorOfTheDay)
                        .Where(x => x.Date >= start && x.Date <= end)
                        .ToListAsync();
                }
            }
        }

        public async Task<bool> ModifyCalenderEventAsync(CalendarEvent calendarEvent)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    if (!await DbContainsDate(db, calendarEvent))
                    {
                        return false;
                    }

                    db.CalendarEvents.Update(calendarEvent);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<bool> RemoveCalendarEventAsync(DateTime dateTime)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    var calendarEvent = await GetCalendarEventAsync(dateTime);

                    if (calendarEvent is null)
                    {
                        return false;
                    }

                    db.CalendarEvents.Remove(calendarEvent);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task RemoveCalendarEventRangeAsync(DateTime start, DateTime end)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    db.CalendarEvents.RemoveRange(await GetCalendarEventRangeAsync(start, end));
                    await db.SaveChangesAsync();
                }
            }
        }

        private async Task<bool> DbContainsDate(FlavorDbContext db, CalendarEvent calendarEvent)
        {
            return await db.CalendarEvents
                .Select(x => x.Date)
                .ContainsAsync(calendarEvent.Date);
        }
    }
}
