using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using poe.Data;
using poe.Models;

namespace poe.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventsController> _logger;

        public EventsController(ApplicationDbContext context, ILogger<EventsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchString, string dateFilter)
        {
            var query = _context.Events.Include(e => e.Venue).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => e.EventName.Contains(searchString)
                                      || e.Description.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(dateFilter) && DateTime.TryParse(dateFilter, out var filterDate))
            {
                query = query.Where(e => e.EventDate.Date == filterDate.Date);
            }

            var viewModel = new EventSearchViewModel
            {
                Events = await query.OrderBy(e => e.EventDate).ToListAsync(),
                SearchString = searchString,
                DateFilter = dateFilter
            };

            return View(viewModel);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (@event.EventDate < DateTime.Now)
            {
                ModelState.AddModelError("EventDate", "Event date must be in the future.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Bookings) // Include bookings to check for dependencies
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            if (@event.Bookings?.Any() == true)
            {
                TempData["ErrorMessage"] = "Cannot delete event with active bookings. Delete the bookings first.";
                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Bookings) // Include bookings to check for dependencies
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            if (@event.Bookings?.Any() == true)
            {
                TempData["ErrorMessage"] = "Cannot delete event with active bookings. Delete the bookings first.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting event");
                TempData["ErrorMessage"] = "An error occurred while deleting the event. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
