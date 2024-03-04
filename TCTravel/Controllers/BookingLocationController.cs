using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Helpers;
using TCTravel.Models;

// Many-to-Many Controller Reference: https://nxk.io/2019/04/01/dealing-with-composite-primary-keys-and-entityframework-scaffolded-controllers/

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
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingLocation>>> GetBookingLocations()
        {
            try
            {
                var bookingLocation = await _context.BookingLocations.ToListAsync();

                _logger.LogInformationEx("Successfully retrieved BookingLocation");
                return bookingLocation;
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // GET: api/BookingLocation/bookingId/locationId
        // Retrieve specific booking location
        [Authorize(Roles = "SuperAdmin,Admin,ClientCompany,Customer,Driver")]
        [HttpGet("{bookingId}/{locationId}")]
        public async Task<ActionResult<BookingLocation>> GetBookingLocation([FromRoute] int bookingId, [FromRoute] int locationId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx($"Invalid request");
                    return BadRequest(ModelState);
                }
                
                var bookingLocation = await _context.BookingLocations.FindAsync(bookingId, locationId);

                if (bookingLocation == null)
                {
                    _logger.LogErrorEx($"Booking {bookingId} / Location {locationId} not found");
                    return NotFound($"Booking {bookingId} / Location {locationId} not found");
                }
                
                _logger.LogInformationEx($"Booking {bookingId} / Location {locationId} retrieved successfully");
                return Ok(bookingLocation);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // POST: api/BookingLocation
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public async Task<ActionResult<BookingLocation>> PostBookingLocation([FromBody] BookingLocation bookingLocation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }

                _context.BookingLocations.Add(bookingLocation);

                await _context.SaveChangesAsync();
                
                return CreatedAtAction("GetBookingLocation", new { bookingId = bookingLocation.BookingId ,
                    locationId = bookingLocation.LocationId }, bookingLocation);
            }
            catch (DbUpdateException ex)
            {
                if (BookingLocationExists(bookingLocation.BookingId, bookingLocation.LocationId))
                {
                    var statusConflict = new StatusCodeResult(StatusCodes.Status409Conflict);
                    _logger.LogError($"Failed with error: {statusConflict}");
                    return statusConflict;
                }
                
                _logger.LogError($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }
        
        // PUT: api/BookingLocation/bookingId/locationId
        // Update specific BookingLocation
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("{bookingId}/{locationId}")]
        public async Task<IActionResult> PutBookingLocation([FromRoute] int bookingId, [FromRoute] int locationId, [FromBody] BookingLocation bookingLocation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }

                if (bookingId != bookingLocation.BookingId && locationId != bookingLocation.LocationId)
                {
                    _logger.LogErrorEx($"Invalid request");
                    return BadRequest();
                }

                _context.Entry(bookingLocation).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                
                _logger.LogInformationEx($"Booking {bookingId} / Location {locationId} updated successfully");
                return Ok($"Booking {bookingId} / Location {locationId} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookingLocationExists(bookingId, locationId))
                {
                    _logger.LogErrorEx($"Booking {bookingId} / Location {locationId} not found");
                    return NotFound($"Booking {bookingId} / Location {locationId} not found");
                }
                
                _logger.LogError($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // DELETE: api/BookingLocation/5
        // Delete specific booking
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteBookingLocation([FromRoute] int bookingId, [FromRoute] int locationId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var bookingLocation = await _context.BookingLocations.FindAsync(bookingId, locationId);
                
                if (bookingLocation == null)
                {
                    _logger.LogErrorEx($"Booking {bookingId} / Location {locationId} not found");
                    return NotFound($"Booking {bookingId} / Location {locationId} not found");
                }

                _context.BookingLocations.Remove(bookingLocation);
                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Booking {bookingId} / Location {locationId} deleted successfully");
                return Ok($"Booking {bookingId} / Location {locationId} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        private bool BookingLocationExists(int bookingId, int locationId)
        {
            return _context.BookingLocations.Any(e => e.BookingId == bookingId && e.LocationId == locationId);
        }
    }
}
