using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================================
        // INDEX + SEARCH + FILTERING
        // ===================================================
        public async Task<IActionResult> Index(
            string searchString,
            int? eventTypeId,
            DateTime? startDate,
            DateTime? endDate)
        {
            var events = _context.Events
                .Include(e => e.EventType)
                .AsQueryable();

            // SEARCH BY EVENT NAME
            if (!string.IsNullOrEmpty(searchString))
            {
                events = events.Where(e =>
                    e.Name.Contains(searchString));
            }

            // EVENT TYPE FILTER
            if (eventTypeId.HasValue)
            {
                events = events.Where(e =>
                    e.EventTypeID == eventTypeId);
            }

            // START DATE FILTER
            if (startDate.HasValue)
            {
                events = events.Where(e =>
                    e.StartDate >= startDate);
            }

            // END DATE FILTER
            if (endDate.HasValue)
            {
                events = events.Where(e =>
                    e.EndDate <= endDate);
            }

            // DROPDOWN DATA
            ViewBag.EventTypeID =
                new SelectList(
                    _context.EventTypes,
                    "EventTypeID",
                    "TypeName");

            return View(await events.ToListAsync());
        }

        // ===================================================
        // DETAILS
        // ===================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // ===================================================
        // CREATE GET
        // ===================================================
        public IActionResult Create()
        {
            ViewData["EventTypeID"] =
                new SelectList(
                    _context.EventTypes,
                    "EventTypeID",
                    "TypeName");

            return View();
        }

        // ===================================================
        // CREATE POST
        // ===================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Event created successfully.";

                return RedirectToAction(nameof(Index));
            }

            ViewData["EventTypeID"] =
                new SelectList(
                    _context.EventTypes,
                    "EventTypeID",
                    "TypeName",
                    @event.EventTypeID);

            return View(@event);
        }

        // ===================================================
        // EDIT GET
        // ===================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event =
                await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            ViewData["EventTypeID"] =
                new SelectList(
                    _context.EventTypes,
                    "EventTypeID",
                    "TypeName",
                    @event.EventTypeID);

            return View(@event);
        }

        // ===================================================
        // EDIT POST
        // ===================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Event @event)
        {
            if (id != @event.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] =
                        "Event updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["EventTypeID"] =
                new SelectList(
                    _context.EventTypes,
                    "EventTypeID",
                    "TypeName",
                    @event.EventTypeID);

            return View(@event);
        }

        // ===================================================
        // DELETE GET
        // ===================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // ===================================================
        // DELETE POST
        // ===================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event =
                await _context.Events.FindAsync(id);

            bool hasBookings =
                await _context.Bookings
                .AnyAsync(b => b.EventID == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] =
                    "Cannot delete event because it has active bookings.";

                return RedirectToAction(nameof(Index));
            }

            if (@event != null)
            {
                _context.Events.Remove(@event);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Event deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================================================
        // EXISTS
        // ===================================================
        private bool EventExists(int id)
        {
            return _context.Events.Any(e =>
                e.EventID == id);
        }
    }
}