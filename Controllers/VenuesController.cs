using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using poe.Data;
using poe.Models;

namespace poe.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VenuesController> _logger;

        public VenuesController(ApplicationDbContext context, ILogger<VenuesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Venues
        public async Task<IActionResult> Index(string searchString, string locationFilter, int? capacityFilter)
        {
            var query = _context.Venues.AsQueryable();

            // Apply filters if they exist
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(v => v.VenueName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(locationFilter))
            {
                query = query.Where(v => v.Location.Contains(locationFilter));
            }

            if (capacityFilter.HasValue)
            {
                query = query.Where(v => v.Capacity >= capacityFilter.Value);
            }

            var viewModel = new VenueSearchViewModel
            {
                Venues = await query.ToListAsync(),
                Locations = await _context.Venues.Select(v => v.Location).Distinct().ToListAsync(),
                SearchString = searchString,
                LocationFilter = locationFilter,
                CapacityFilter = capacityFilter
            };

            return View(viewModel);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.VenueId == id);

            if (venue == null) return NotFound();

            if (venue.Bookings?.Any() == true)
            {
                TempData["ErrorMessage"] = "Cannot delete venue with active bookings.";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}