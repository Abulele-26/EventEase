using System;
using System.Linq;
using System.Threading.Tasks;
using EventEase.Data;
using EventEase.Models;
using EventEase.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public VenuesController(
            ApplicationDbContext context,
            BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            try
            {
                // IMPORTANT: show validation errors instead of silent failure
                if (!ModelState.IsValid)
                {
                    return View(venue);
                }

                // IMAGE UPLOAD (Azurite / Blob Storage)
                if (venue.ImageFile != null)
                {
                    venue.ImageURL = await _blobService.UploadFileAsync(venue.ImageFile);
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Venue created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating venue: " + ex.Message);
                return View(venue);
            }
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueID)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(venue);
                }

                if (venue.ImageFile != null)
                {
                    venue.ImageURL = await _blobService.UploadFileAsync(venue.ImageFile);
                }

                _context.Update(venue);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Venue updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenueExists(venue.VenueID))
                    return NotFound();

                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating venue: " + ex.Message);
                return View(venue);
            }
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasBookings = await _context.Bookings
                .AnyAsync(b => b.VenueID == id);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete venue with active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FindAsync(id);

            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Venue deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueID == id);
        }
    }
}