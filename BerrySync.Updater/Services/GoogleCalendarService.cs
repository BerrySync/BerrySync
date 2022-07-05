using BerrySync.Data.Models;
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
        ILogger<GoogleCalendarService> _logger;
        CalendarService _calendar;

        public GoogleCalendarService(ILogger<GoogleCalendarService> logger)
        {
            _logger = logger;
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

        public async Task<IEnumerable<FlavorOfTheDay>> AddAsync(IEnumerable<FlavorOfTheDay> flavors)
        {
            var datesOnly = flavors
                .Select(r => r.Date);
            var e = (await GetAsync(datesOnly.Min(), datesOnly.Max())).Items;

            var allGoodList = flavors
                .Where(f => e
                    .Where(x => IsSameDay(f.Date, x.Start.Date)
                        && x.Summary == f.Flavor)
                    .Any());
            var modifyList = flavors
                .Where(f => e
                    .Where(x => IsSameDay(f.Date, x.Start.Date)
                        && x.Summary != f.Flavor)
                    .Any());
            var addList = flavors
                .Except(allGoodList)
                .Except(modifyList);

            return (await UpdateAsync(modifyList, e))
                .Union(await InsertAsync(addList));
        }

        public async Task<Events> GetAsync(DateTime start, DateTime end)
        {
            var req = _calendar.Events.List(Constants.GoogleCalendarId);
            req.TimeMin = start;
            req.TimeMax = end;
            return await req.ExecuteAsync();
        }

        public async Task<IEnumerable<FlavorOfTheDay>> UpdateAsync(IEnumerable<FlavorOfTheDay> flavors, IList<Event> events)
        {
            var output = new List<FlavorOfTheDay>();

            foreach (var f in flavors)
            {
                var eventId = events
                    .Where(x => IsSameDay(f.Date, x.Start.Date))
                    .Select(x => x.Id)
                    .First();

                var e = new Event
                {
                    Summary = f.Flavor
                };

                var req = _calendar.Events.Update(e, Constants.GoogleCalendarId, eventId);
                var response = await req.ExecuteAsync();

                f.Event = new CalendarEvent
                {
                    Date = f.Date,
                    EventId = response.Id,
                    FlavorOfTheDay = f
                };

                output.Add(f);
            }

            return output;
        }

        public async Task<IEnumerable<FlavorOfTheDay>> InsertAsync(IEnumerable<FlavorOfTheDay> flavors)
        {
            var output = new List<FlavorOfTheDay>();

            foreach (var f in flavors)
            {
                var e = new Event();

                var start = new EventDateTime();
                start.Date = FormatDate(f.Date);
                e.Start = start;

                var end = new EventDateTime();
                end.Date = FormatDate(f.Date.AddDays(1));
                e.End = end;

                e.Summary = f.Flavor;
                e.Visibility = "public";
                e.Transparency = "transparent";

                var req = _calendar.Events.Insert(e, Constants.GoogleCalendarId);
                var response = await req.ExecuteAsync();

                f.Event = new CalendarEvent()
                {
                    Date = f.Date,
                    EventId = response.Id,
                    FlavorOfTheDay = f
                };

                output.Add(f);
            }

            return output;
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
