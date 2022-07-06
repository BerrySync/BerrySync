using BerrySync.Data.Models;
using BerrySync.Data.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
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
            var datesOnly = flavors
                .Select(r => r.Date);

            var modifyList = new List<FlavorOfTheDay>();
            var addList = new List<FlavorOfTheDay>();
            foreach (var f in flavors)
            {
                var result = await _flavor.GetFlavorOfTheDayAsync(f.Date);
                if (result != null)
                {
                    var gEvent = await _calendar.Events.Get(Constants.GoogleCalendarId, result.Event.EventId).ExecuteAsync();
                    if (f.Flavor != gEvent.Summary
                        || !IsSameDay(result.Date, gEvent.Start.Date))
                    {
                        modifyList.Add(f);
                    }
                }
                else
                {
                    addList.Add(f);
                }
            }

            await InsertAsync(addList);
            await UpdateAsync(modifyList);
        }

        public async Task UpdateAsync(IEnumerable<FlavorOfTheDay> flavors)
        {
            foreach (var f in flavors)
            {
                var id = (await _event.GetCalendarEventAsync(f.Date)).EventId;

                _logger.LogDebug($"Updating Google Calendar event {id}");

                var e = await _calendar.Events.Get(Constants.GoogleCalendarId, id).ExecuteAsync();
                e.Summary = f.Flavor;
                e.Start.Date = FormatDate(f.Date);
                e.End.Date = FormatDate(f.Date.AddDays(1));

                var req = _calendar.Events.Update(e, Constants.GoogleCalendarId, id);
                var response = await req.ExecuteAsync();

                f.Event = new CalendarEvent
                {
                    Date = f.Date,
                    EventId = response.Id,
                    FlavorOfTheDay = f
                };

                await _flavor.ModifyFlavorOfTheDayAsync(f);
            }

        }

        public async Task InsertAsync(IEnumerable<FlavorOfTheDay> flavors)
        {
            foreach (var f in flavors)
            {
                var e = new Event();

                e.Start = new EventDateTime()
                {
                    Date = FormatDate(f.Date)
                };

                e.End = new EventDateTime()
                {
                    Date = FormatDate(f.Date.AddDays(1))
                };

                e.Summary = f.Flavor;
                e.Visibility = "public";
                e.Transparency = "transparent";

                var req = _calendar.Events.Insert(e, Constants.GoogleCalendarId);
                var response = await req.ExecuteAsync();

                _logger.LogDebug($"Adding Google Calendar event {response.Id}");

                f.Event = new CalendarEvent()
                {
                    Date = f.Date,
                    EventId = response.Id,
                    FlavorOfTheDay = f
                };

                await _flavor.AddFlavorOfTheDayAsync(f);
            }
        }

        public static bool IsSameDay(DateTime a, string b)
        {
            var t = Convert.ToDateTime(b);
            return a.Year == t.Year
                && a.Month == t.Month
                && a.Day == t.Day;
        }

        public static string FormatDate(DateTime date)
        {
            return $"{date:yyyy-MM-dd}";
        }
    }
}
