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
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(ApplicationDbContext context, ILogger<BookingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["IdSort"] = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewData["EventSort"] = sortOrder == "event_asc" ? "event_desc" : "event_asc";
            ViewData["DateSort"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["VenueSort"] = sortOrder == "venue_asc" ? "venue_desc" : "venue_asc";
            ViewData["BookingDateSort"] = sortOrder == "booking_asc" ? "booking_desc" : "booking_asc";
            ViewData["CurrentFilter"] = searchString;

            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.Event.EventName.Contains(searchString) ||
                    b.Venue.VenueName.Contains(searchString));
            }

            bookings = sortOrder switch
            {
                "id_asc" => bookings.OrderBy(b => b.BookingId),
                "id_desc" => bookings.OrderByDescending(b => b.BookingId),
                "event_asc" => bookings.OrderBy(b => b.Event.EventName),
                "event_desc" => bookings.OrderByDescending(b => b.Event.EventName),
                "date_asc" => bookings.OrderBy(b => b.Event.EventDate),
                "date_desc" => bookings.OrderByDescending(b => b.Event.EventDate),
                "venue_asc" => bookings.OrderBy(b => b.Venue.VenueName),
                "venue_desc" => bookings.OrderByDescending(b => b.Venue.VenueName),
                "booking_asc" => bookings.OrderBy(b => b.BookingDate),
                "booking_desc" => bookings.OrderByDescending(b => b.BookingDate),
                _ => bookings.OrderByDescending(b => b.BookingDate),
            };

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,VenueId")] Booking booking)
        {
            try
            {
                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.EventId == booking.EventId && b.VenueId == booking.VenueId);

                if (existingBooking != null)
                {
                    ModelState.AddModelError("", "This venue is already booked for the selected event.");
                }

                if (ModelState.IsValid)
                {
                    booking.BookingDate = DateTime.Now;
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                ModelState.AddModelError("", "An error occurred while creating the booking.");
            }

            await PopulateDropdowns(booking.EventId, booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Booking deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int? eventId = null, int? venueId = null)
        {
            ViewData["EventId"] = new SelectList(
                await _context.Events.Where(e => e.EventDate > DateTime.Now).ToListAsync(),
                "EventId", "EventName", eventId);

            ViewData["VenueId"] = new SelectList(
                await _context.Venues.ToListAsync(),
                "VenueId", "VenueName", venueId);
        }
    }
}
