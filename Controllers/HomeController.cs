using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EventEase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            // Load all venues and events from database
            List<Venue> venues = await _context.Venues.ToListAsync();
            List<Event> eventsList = await _context.Events.ToListAsync();

            // Pass data to the view using ViewBag
            ViewBag.Venues = venues;
            ViewBag.Events = eventsList;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
