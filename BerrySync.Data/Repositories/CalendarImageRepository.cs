using BerrySync.Data.Data;
using BerrySync.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BerrySync.Data.Repositories
{
    public class CalendarImageRepository : ICalendarImageRepository
    {
        private readonly IServiceProvider _sp;

        public CalendarImageRepository(IServiceProvider sp)
        {
            _sp = sp;
        }

        public async Task<bool> TryAddCalendarImageAsync(CalendarImage image)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    if (await GetImageAsync(db, image.Year, image.Month) is not null)
                    {
                        return false;
                    }

                    await db.CalendarImages.AddAsync(image);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<CalendarImage> GetCalendarImageAsync(int year, string month)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    return await GetImageAsync(db, year, month);
                }
            }
        }

        private static async Task<CalendarImage> GetImageAsync(FlavorDbContext db, int year, string month)
        {
            return await db.CalendarImages
                .Where(i => i.Year == year
                    && i.Month.ToLower() == month.ToLower())
                .FirstOrDefaultAsync();
        }
    }
}
