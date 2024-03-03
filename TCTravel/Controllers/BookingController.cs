using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Models;

namespace TCTravel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly TCTravelContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(TCTravelContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Booking
        // Retrieve all bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            try
            {
                var bookings = await _context.Bookings.ToListAsync();
            
                _logger.LogInformation("Successfully retrieved booking.");
                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetBookings)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // GET: api/Booking/5
        // Retrieve specific bookings
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Error. Invalid data. Please try again.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    _logger.LogError($"Error, booking {id} not found.");
                    return NotFound();
                }

                _logger.LogInformation($"Booking {id} retrieved successfully.");
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetBooking)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // PUT: api/Booking/5
        // Update specific booking
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            if (id != booking.BookingId)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    _logger.LogError($"Error. Booking {id} not found.");
                    return NotFound(" The Booking was not found");
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"Booking {id} updated successfully!");
            return NoContent();
        }

        // POST: api/Booking
        // Create booking
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Booking created successfully!");
                return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PostBooking)} failed with error: {ex.Message}");
                return StatusCode(500, $"An unexpected error occurred. Please try again.");
            }
        }

        // DELETE: api/Booking/5
        // Delete specific booking
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    _logger.LogError($"Error. Booking {id} not found");
                    return NotFound("The booking was not found.");
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Booking {id} deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteBooking)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
