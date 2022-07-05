namespace BerrySync.Updater
{
    public static class Constants
    {
        private static readonly string _url = "https://www.goodberrys.com/flavor-of-the-day/";
        private static readonly string _workDir = "/data/work";
        private static readonly string _calendarFile = $"{_workDir}/calendar.jpg";
        private static readonly string _googleAppSecretsFile = "/data/key.json";
        private static readonly string _googleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
        private static readonly string _googleCalendarId = Environment.GetEnvironmentVariable("GOOGLE_CALENDAR_ID");

        public static string Url { get { return _url; } }
        public static string WorkDir { get { return _workDir; } }
        public static string CalendarFile { get { return _calendarFile; } }
        public static string GoogleAppSecretsFile { get { return _googleAppSecretsFile; } }
        public static string GoogleApiKey { get { return _googleApiKey; } }
        public static string GoogleCalendarId { get { return _googleCalendarId; } }
    }
}
