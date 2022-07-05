using BerrySync.Data.Data;
using BerrySync.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BerrySync.Data.Repositories
{
    public class FlavorRepository : IFlavorRepository
    {
        private readonly IServiceProvider _sp;

        public FlavorRepository(IServiceProvider sp)
        {
            _sp = sp;
        }

        public async Task<bool> AddFlavorOfTheDayAsync(FlavorOfTheDay flavorOfTheDay)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {
                if (await db.Dates
                    .Select(x => x.Date)
                    .ContainsAsync(flavorOfTheDay.Date))
                {
                    return false;
                }

                await db.Dates.AddAsync(flavorOfTheDay);
                await db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> AddFlavorOfTheDayRangeAsync(IEnumerable<FlavorOfTheDay> flavors)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {
                if (await db.Dates
                    .Select(x => x.Date)
                    .Intersect(flavors
                        .Select(x => x.Date))
                    .AnyAsync())
                {
                    return false;
                }

                await db.Dates.AddRangeAsync(flavors);
                await db.SaveChangesAsync();
                return true;

            }
        }

        public async Task<FlavorOfTheDay> GetFlavorOfTheDayAsync(DateTime dateTime)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {
                return await db.Dates
                    .FirstOrDefaultAsync(x => x.Date == dateTime);
            }
        }

        public async Task<IEnumerable<FlavorOfTheDay>> GetFlavorOfTheDayRangeAsync(DateTime start, DateTime end)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {
                return await db.Dates
                    .Where(x => x.Date >= start && x.Date <= end)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<DateTime>> GetDaysWithFlavorAsync(DateTime start, DateTime end, string flavor)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {
                return await db.Dates
                    .Where(x => x.Date >= start && x.Date <= end)
                    .Where(x => x.Flavor == flavor)
                    .Select(x => x.Date)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<DateTime>> GetDaysWithFlavorAsync(DateTime start, string flavor)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {
                return await db.Dates
                    .Where(x => x.Date >= start)
                    .Where(x => x.Flavor == flavor)
                    .Select(x => x.Date)
                    .ToListAsync();
            }
        }

        public async Task<DateTime?> GetNextDayWithFlavorAsync(string flavor)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {
                var nextDate = await db.Dates
                    .Where(x => x.Date >= DateTime.Today)
                    .FirstOrDefaultAsync(x => x.Flavor == flavor);
                return nextDate?.Date;
            }
        }

        public async Task<IEnumerable<DateTime>> GetUpcomingDaysWithFlavorAsync(string flavor)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {
                return await db.Dates.Select(x => x.Date).Where(x => x >= DateTime.Today).ToListAsync();
            }
        }

        public async Task<bool> ModifyFlavorOfTheDayAsync(FlavorOfTheDay flavorOfTheDay)
        {
            using (var db = _sp.GetRequiredService<FlavorDbContext>())
            {

                if (!await db.Dates
                    .Select(x => x.Date)
                    .ContainsAsync(flavorOfTheDay.Date))
                {
                    return false;
                }

                db.Dates.Update(flavorOfTheDay);
                await db.SaveChangesAsync();
                return true;
            }
        }
    }
}
