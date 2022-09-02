using System.ComponentModel.DataAnnotations;

namespace BerrySync.Data.Models
{
    public class CalendarImage
    {
        [Key]
        public int Id { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string FilePath { get; set; }
    }
}
