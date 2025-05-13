using System;
using System.ComponentModel.DataAnnotations;

namespace poe.ViewModels
{
    public class BookingViewModel
    {
        // Booking Information
        public int BookingId { get; set; }

        [Display(Name = "Booking Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingDate { get; set; }

        // Event Information
        [Display(Name = "Event")]
        public string EventName { get; set; }

        [Display(Name = "Event Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime EventDate { get; set; }

        public string EventDescription { get; set; }

        // Venue Information
        [Display(Name = "Venue")]
        public string VenueName { get; set; }

        [Display(Name = "Location")]
        public string VenueLocation { get; set; }

        [Display(Name = "Capacity")]
        public int VenueCapacity { get; set; }
    }
}