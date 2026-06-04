using System;

namespace EventEase.Models
{
    public class BookingSummaryViewModel
    {
        public int BookingID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string VenueName { get; set; } = string.Empty;

        public string VenueLocation { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}