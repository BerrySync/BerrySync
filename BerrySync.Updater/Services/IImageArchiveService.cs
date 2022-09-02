namespace BerrySync.Updater.Services
{
    public interface IImageArchiveService
    {
        public Task ArchiveCalendar(int year, string month);
    }
}
