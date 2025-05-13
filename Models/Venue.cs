using System.ComponentModel.DataAnnotations;

namespace poe.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required]
        public string VenueName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }

}
