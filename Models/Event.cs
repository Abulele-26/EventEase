using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public string? ImageURL { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public int EventTypeID { get; set; }

        public EventType? EventType { get; set; }
    }
}