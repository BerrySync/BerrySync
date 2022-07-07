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
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
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
        }

        public async Task<bool> AddFlavorOfTheDayRangeAsync(IEnumerable<FlavorOfTheDay> flavors)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
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
        }

        public async Task<FlavorOfTheDay> GetFlavorOfTheDayAsync(DateTime dateTime)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    return await db.Dates
                        .Include(x => x.Event)
                        .FirstOrDefaultAsync(x => x.Date == dateTime);
                }
            }
        }

        public async Task<IEnumerable<FlavorOfTheDay>> GetFlavorOfTheDayRangeAsync(DateTime start, DateTime end)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    return await db.Dates
                        .Include(x => x.Event)
                        .Where(x => x.Date >= start && x.Date <= end)
                        .ToListAsync();
                }
            }
        }

        public async Task<IEnumerable<DateTime>> GetDaysWithFlavorAsync(DateTime start, DateTime end, string flavor)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    return await db.Dates
                        .Include(x => x.Event)
                        .Where(x => x.Date >= start && x.Date <= end)
                        .Where(x => x.Flavor.ToLower() == flavor.ToLower())
                        .Select(x => x.Date)
                        .ToListAsync();
                }
            }
        }

        public async Task<DateTime?> GetNextDayWithFlavorAsync(string flavor)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    var next = await db.Dates
                        .Include(x => x.Event)
                        .Where(x => x.Flavor.ToLower() == flavor.ToLower())
                        .Select(x => x.Date)
                        .Where(x => x >= DateTime.Today)
                        .OrderBy(x => x)
                        .FirstOrDefaultAsync();
                    return (next == default) ? null : next;
                }
            }
        }

        public async Task<IEnumerable<DateTime>> GetUpcomingDaysWithFlavorAsync(string flavor)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
                {
                    _logger.LogDebug(flavor);
                    return await db.Dates
                        .Include(x => x.Event)
                        .Where(x => x.Flavor.ToLower() == flavor.ToLower())
                        .Select(x => x.Date)
                        .Where(x => x >= DateTime.Today)
                        .OrderBy(x => x)
                        .Take(5)
                        .ToListAsync();
                }
            }
        }

        public async Task<bool> ModifyFlavorOfTheDayAsync(FlavorOfTheDay flavorOfTheDay)
        {
            using (var scope = _sp.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<FlavorDbContext>())
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
}
