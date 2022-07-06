using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BerrySync.Data.Models
{
    public class CalendarEvent
    {
        [Key]
        public DateTime Date { get; set; }
        public string EventId { get; set; }

        public FlavorOfTheDay FlavorOfTheDay { get; set; }
    }
}
