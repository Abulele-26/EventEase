using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required(ErrorMessage = "Please select a venue")]
        public int VenueID { get; set; }

        [Required(ErrorMessage = "Please select an event")]
        public int EventID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Navigation 
        [ForeignKey("VenueID")]
        public virtual Venue? Venue { get; set; } 

        [ForeignKey("EventID")]
        public virtual Event? Event { get; set; } 
    }
}