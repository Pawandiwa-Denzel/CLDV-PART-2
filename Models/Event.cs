using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace poe.Models
{
    public class Event
    {
        [Key]
        [Display(Name = "Event ID")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Event name must be between 3-100 characters")]
        [Display(Name = "Event Name")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date & Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [FutureDate(ErrorMessage = "Event date must be in the future")]
        public DateTime EventDate { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Venue")]
        [ForeignKey("Venue")]
        public int? VenueId { get; set; }
        public Venue? Venue { get; set; }

    
       
        public ICollection<Booking>? Bookings { get; set; }
    }

    // Add this custom validation attribute (create new file or add at bottom)
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.Now;
            }
            return false;
        }
    }

    // Optional: Add if you want event categories
    public enum EventType
    {
        Conference,
        Wedding,
        Concert,
        Exhibition,
        Corporate,
        Private
    }
}