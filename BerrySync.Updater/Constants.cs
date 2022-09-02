namespace BerrySync.Updater
{
    public static class Constants
    {
        public const string Url = "https://www.goodberrys.com/flavor-of-the-day/";
        public const string WorkDir = "/data/work";
        public const string CalendarFile = $"{WorkDir}/calendar.jpg";
        public const string CalendarStorage = "/data/calendars";
        public const string GoogleAppSecretsFile = "/data/key.json";

        public static string? GoogleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
        public static string? GoogleCalendarId = Environment.GetEnvironmentVariable("GOOGLE_CALENDAR_ID");
    }
}
