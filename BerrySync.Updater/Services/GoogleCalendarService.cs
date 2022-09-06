using BerrySync.Data.Models;
using BerrySync.Data.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;

namespace BerrySync.Updater.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly ILogger<GoogleCalendarService> _logger;
        private readonly IFlavorRepository _flavor;
        private readonly ICalendarEventRepository _event;
        private readonly CalendarService _calendar;

        public GoogleCalendarService(ILogger<GoogleCalendarService> logger, IFlavorRepository flavor, ICalendarEventRepository events)
        {
            _logger = logger;
            _flavor = flavor;
            _event = events;

            using (var stream = new FileStream(Constants.GoogleAppSecretsFile, FileMode.Open, FileAccess.Read))
            {
                string[] scopes = { CalendarService.Scope.CalendarEvents };
                _calendar = new CalendarService(new BaseClientService.Initializer()
                {
                    ApplicationName = "BerrySync.Updater",
                    HttpClientInitializer = GoogleCredential
                        .FromStream(stream)
                        .CreateScoped(scopes)
                });
            }
        }

        public async Task AddAsync(IEnumerable<FlavorOfTheDay> flavors)
        {
            foreach (var f in flavors)
            {
                var e = await _event.GetCalendarEventAsync(f.Date);

                if (e is null)
                {
                    await InsertAsync(f);
                }
                else if ((await _flavor.GetFlavorOfTheDayAsync(f.Date)).Flavor != f.Flavor)
                {
                    await UpdateAsync(f, e.EventId);
                }
            }
        }

        private async Task InsertAsync(FlavorOfTheDay flavor)
        {
            var response = await _calendar.Events
                .Insert(CreateEvent(flavor), Constants.GoogleCalendarId)
                .ExecuteAsync();

            _logger.LogDebug($"Adding Google Calendar event {response.Id}");

            flavor.Event = new CalendarEvent()
            {
                Date = flavor.Date,
                EventId = response.Id,
                FlavorOfTheDay = flavor
            };

            await _flavor.AddFlavorOfTheDayAsync(flavor);
        }

        private async Task UpdateAsync(FlavorOfTheDay flavor, string e)
        {
            var response = await _calendar.Events
                .Update(CreateEvent(flavor), Constants.GoogleCalendarId, e)
                .ExecuteAsync();

            _logger.LogDebug($"Updating Google Calendar event {response.Id}");

            flavor.Event = new CalendarEvent()
            {
                Date = flavor.Date,
                EventId = e,
                FlavorOfTheDay = flavor
            };

            await _flavor.ModifyFlavorOfTheDayAsync(flavor);
        }

        private static Event CreateEvent(FlavorOfTheDay f)
        {
            return new Event()
            {
                Start = new EventDateTime()
                {
                    Date = FormatDate(f.Date)
                },
                End = new EventDateTime()
                {
                    Date = FormatDate(f.Date.AddDays(1))
                },
                Summary = f.Flavor,
                Visibility = "public",
                Transparency = "transparent"
            };
        }

        private static string FormatDate(DateTime date)
        {
            return $"{date:yyyy-MM-dd}";
        }
    }
}
