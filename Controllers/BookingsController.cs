using System;
using System.Linq;
using System.Threading.Tasks;
using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX + SEARCH + FILTER
      
        public async Task<IActionResult> Index(
            string searchString,
            bool availableOnly = false)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                 b.BookingID.ToString().Contains(searchString)
                 ||
                 (b.Event != null &&
                  b.Event.Name.Contains(searchString)));
            }

            // AVAILABLE ONLY FILTER
            if (availableOnly)
            {
                bookings = bookings.Where(b =>
                    !_context.Bookings.Any(existing =>
                        existing.BookingID != b.BookingID
                        &&
                        existing.VenueID == b.VenueID
                        &&
                        b.StartDate < existing.EndDate
                        &&
                        b.EndDate > existing.StartDate));
            }

            ViewBag.SearchString = searchString;
            ViewBag.AvailableOnly = availableOnly;

            return View(await bookings.ToListAsync());
        }

        // DETAILS

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // CREATE GET
       
        public IActionResult Create()
        {
            LoadDropdowns(null);
            return View();
        }

        // CREATE POST
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            LoadDropdowns(booking);

            if (!ModelState.IsValid)
                return View(booking);

            // DATE VALIDATION
            if (booking.EndDate <= booking.StartDate)
            {
                ModelState.AddModelError("",
                    "End date must be after start date.");

                return View(booking);
            }

            // VENUE AVAILABILITY CHECK
            bool available =
                !_context.Bookings.Any(
                    b =>
                    b.VenueID == booking.VenueID
                    &&
                    booking.StartDate < b.EndDate
                    &&
                    booking.EndDate > b.StartDate
                );

            if (!available)
            {
                ModelState.AddModelError("",
                    "This venue is already booked for those dates.");

                return View(booking);
            }

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Booking created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
     
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking =
                await _context.Bookings.FindAsync(id);

            if (booking == null) return NotFound();

            LoadDropdowns(booking);

            return View(booking);
        }

        // EDIT POST
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Booking booking)
        {
            if (id != booking.BookingID)
                return NotFound();

            LoadDropdowns(booking);

            if (!ModelState.IsValid)
                return View(booking);

            // DATE VALIDATION
            if (booking.EndDate <= booking.StartDate)
            {
                ModelState.AddModelError("",
                    "End date must be after start date.");

                return View(booking);
            }

            bool available =
                !_context.Bookings.Any(
                    b =>
                    b.BookingID != booking.BookingID
                    &&
                    b.VenueID == booking.VenueID
                    &&
                    booking.StartDate < b.EndDate
                    &&
                    booking.EndDate > b.StartDate
                );

            if (!available)
            {
                ModelState.AddModelError("",
                    "This venue is already booked.");

                return View(booking);
            }

            _context.Update(booking);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Booking updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
    
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(
                    b => b.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // DELETE POST
      
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking =
                await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // DROPDOWNS
        
        private void LoadDropdowns(Booking? booking)
        {
            ViewData["VenueID"] = new SelectList(
                _context.Venues,
                "VenueID",
                "Name",
                booking?.VenueID);

            ViewData["EventID"] = new SelectList(
                _context.Events,
                "EventID",
                "Name",
                booking?.EventID);
        }
    }
}