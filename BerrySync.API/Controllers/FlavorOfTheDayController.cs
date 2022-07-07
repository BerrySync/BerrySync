using BerrySync.Data.Models;
using BerrySync.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BerrySync.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class FlavorOfTheDayController : ControllerBase
    {
        private readonly IFlavorRepository _flavorRepository;

        public FlavorOfTheDayController(IFlavorRepository flavorRepository)
        {
            _flavorRepository = flavorRepository;
        }

        // GET: api/flavor/{date}
        [HttpGet("flavor/{date}")]
        public async Task<ActionResult<FlavorOfTheDay>> GetFlavorOfTheDay(DateTime date)
        {
            var result = await _flavorRepository.GetFlavorOfTheDayAsync(date);
            return result is null ? NotFound() : Ok(result);
        }

        // GET: api/flavor/{start}/{end}
        [HttpGet("flavor/{start}/{end}")]
        public async Task<ActionResult<IEnumerable<FlavorOfTheDay>>> GetFlavorOfTheDayRange(DateTime start, DateTime end)
        {
            if ((end - start).TotalDays > 90) return BadRequest("Request exceeded maximum date range size.");
            var result = await _flavorRepository.GetFlavorOfTheDayRangeAsync(start, end);
            return !result.Any() ? NotFound() : Ok(result);
        }

        // GET: api/dates/{flavor}/{start}/{end}
        [HttpGet("dates/{flavor}/{start}/{end}")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetDaysWithFlavor(DateTime start, DateTime end, string flavor)
        {
            if ((end - start).TotalDays > 90) return BadRequest("Request exceeded maximum date range size.");
            var result = await _flavorRepository.GetDaysWithFlavorAsync(start, end, flavor);
            return !result.Any() ? NotFound() : Ok(result);
        }

        // GET: api/dates/next/{flavor}
        [HttpGet("dates/next/{flavor}")]
        public async Task<ActionResult<DateTime>> GetNextDayWithFlavor(string flavor)
        {
            var result = await _flavorRepository.GetNextDayWithFlavorAsync(flavor);
            return result is null ? NotFound() : Ok(result);
        }

        // GET: api/dates/upcoming/{flavor}
        [HttpGet("dates/upcoming/{flavor}")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetUpcomingDaysWithFlavorAsync(string flavor)
        {
            var result = await _flavorRepository.GetUpcomingDaysWithFlavorAsync(flavor);
            return !result.Any() ? NotFound() : Ok(result);
        }
    }
}
