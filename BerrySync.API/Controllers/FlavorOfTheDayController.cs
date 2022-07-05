using BerrySync.Data.Models;
using BerrySync.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BerrySync.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlavorOfTheDayController : ControllerBase
    {
        private readonly IFlavorRepository _flavorRepository;

        public FlavorOfTheDayController(FlavorRepository flavorRepository)
        {
            _flavorRepository = flavorRepository;
        }

        [HttpGet("flavor/{date}")]
        public async Task<ActionResult<FlavorOfTheDay>> GetFlavorOfTheDay(DateTime date)
        {
            var result = await _flavorRepository.GetFlavorOfTheDayAsync(date);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("flavor/{start}/{end}")]
        public async Task<ActionResult<IEnumerable<FlavorOfTheDay>>> GetFlavorOfTheDayRange(DateTime start, DateTime end)
        {
            var result = await _flavorRepository.GetFlavorOfTheDayRangeAsync(start, end);
            return !result.Any() ? NotFound() : Ok(result);
        }

        [HttpGet("dates/{start}/{end}/{flavor}")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetDaysWithFlavor(DateTime start, DateTime end, string flavor)
        {
            var result = await _flavorRepository.GetDaysWithFlavorAsync(start, end, flavor);
            return !result.Any() ? NotFound() : Ok(result);
        }

        [HttpGet("dates/{start}/{flavor}")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetDaysWithFlavor(DateTime start, string flavor)
        {
            var result = await _flavorRepository.GetDaysWithFlavorAsync(start, flavor);
            return !result.Any() ? NotFound() : Ok(result);
        }

        [HttpGet("dates/next/{flavor}")]
        public async Task<ActionResult<DateTime>> GetNextDayWithFlavor(string flavor)
        {
            var result = await _flavorRepository.GetNextDayWithFlavorAsync(flavor);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("dates/upcoming/{flavor}")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetUpcomingDaysWithFlavorAsync(string flavor)
        {
            var result = await _flavorRepository.GetUpcomingDaysWithFlavorAsync(flavor);
            return !result.Any() ? NotFound() : Ok(result);
        }
    }
}
