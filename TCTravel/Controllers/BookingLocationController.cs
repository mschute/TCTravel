using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Models;

// Adjusted Many-to-Many Controller using this reference: https://nxk.io/2019/04/01/dealing-with-composite-primary-keys-and-entityframework-scaffolded-controllers/

namespace TCTravel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingLocationController : ControllerBase
    {
        private readonly TCTravelContext _context;
        private readonly ILogger<BookingLocationController> _logger;

        public BookingLocationController(TCTravelContext context, ILogger<BookingLocationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/BookingLocation
        // Retrieve booking locations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingLocation>>> GetBookingLocations()
        {
            try
            {
                var bookingLocation = await _context.BookingLocations.ToListAsync();

                _logger.LogInformation("Successfully retrieved booking location");
                return bookingLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetBookingLocations)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // GET: api/BookingLocation/bookingId/locationId
        // Retrieve specific booking location
        [HttpGet("{bookingId}/{locationId}")]
        public async Task<ActionResult<BookingLocation>> GetBookingLocation([FromRoute] int bookingId, [FromRoute] int locationId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Error. Invalid request. Please try again.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var bookingLocation = await _context.BookingLocations.FindAsync(bookingId, locationId);

                if (bookingLocation == null)
                {
                    _logger.LogError($"Error, location {locationId} for booking {bookingId} not found.");
                    return NotFound();
                }

                return Ok(bookingLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetBookingLocation)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // PUT: api/BookingLocation/bookingId/locationId
        // Update specific BookingLocation
        [HttpPut("{bookingId}/{locationId}")]
        public async Task<IActionResult> PutBookingLocation([FromRoute] int bookingId, [FromRoute] int locationId, [FromBody] BookingLocation bookingLocation)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Error. Invalid request. Please try again.");
                return BadRequest(ModelState);
            }
            
            if (bookingId != bookingLocation.BookingId && locationId != bookingLocation.LocationId)
            {
                _logger.LogError($"Error. Invalid request. Please try again.");
                return BadRequest();
            }

            _context.Entry(bookingLocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingLocationExists(bookingId, locationId))
                {
                    _logger.LogError($"Error. Location {locationId} and Booking {bookingId} not found.");
                    return NotFound("The location for that booking was not found.");
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"Location {locationId} for Booking {bookingId} was not found.");
            return NoContent();
        }

        // POST: api/BookingLocation
        [HttpPost]
        public async Task<ActionResult<BookingLocation>> PostBookingLocation([FromBody] BookingLocation bookingLocation)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }

            _context.BookingLocations.Add(bookingLocation);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookingLocationExists(bookingLocation.BookingId, bookingLocation.LocationId))
                {
                    var statusConflict = new StatusCodeResult(StatusCodes.Status409Conflict);
                    _logger.LogError($"{nameof(PostBookingLocation)} failed with error: {statusConflict}");
                    return statusConflict;
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookingLocation", new { bookingId = bookingLocation.BookingId ,
                locationId = bookingLocation.LocationId }, bookingLocation);
        }

        // DELETE: api/BookingLocation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookingLocation([FromRoute] int bookingId, [FromRoute] int locationId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }

            try
            {
                var bookingLocation = await _context.BookingLocations.FindAsync(bookingId, locationId);
                if (bookingLocation == null)
                {
                    _logger.LogError("Error. BookingLocation not found.");
                    return NotFound();
                }

                _context.BookingLocations.Remove(bookingLocation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("BookingLocation deleted successfully!");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteBookingLocation)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        private bool BookingLocationExists(int bookingId, int locationId)
        {
            return _context.BookingLocations.Any(e => e.BookingId == bookingId && e.LocationId == locationId);
        }
    }
}
