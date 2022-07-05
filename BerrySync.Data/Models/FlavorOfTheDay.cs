using System.ComponentModel.DataAnnotations;

namespace BerrySync.Data.Models
{
    public class FlavorOfTheDay
    {
        [Key]
        public DateTime Date { get; set; }
        public string Flavor { get; set; }

        public CalendarEvent Event { get; set; }
    }
}