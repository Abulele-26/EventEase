using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }

        // Stored in database (image URL from Azure Blob)
        public string? ImageURL { get; set; }

        // Used ONLY for file upload (NOT stored in database)
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}