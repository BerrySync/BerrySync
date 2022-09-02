using BerrySync.Data.Models;
using BerrySync.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BerrySync.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class CalendarImageController : ControllerBase
    {
        private readonly ICalendarImageRepository _imageRepository;

        public CalendarImageController(ICalendarImageRepository calendarImageRepository)
        {
            _imageRepository = calendarImageRepository;
        }

        // GET: api/calendar/{year}/{month}
        [HttpGet("calendar/{year}/{month}")]
        public async Task<ActionResult<CalendarImage>> GetCalendarImage(int year, string month)
        {
            var result = await _imageRepository.GetCalendarImageAsync(year, month);
            return result is null ? NotFound() : Redirect($"/static/calendars/{year}-{result.Month}.jpg");
        }
    }
}
