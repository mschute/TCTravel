using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Helpers;
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
        //[Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            try
            {
                var bookings = await _context.Bookings.ToListAsync();
            
                _logger.LogInformationEx("Successfully retrieved Bookings");
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // GET: api/Booking/5
        // Retrieve specific bookings
        [Authorize(Roles = "SuperAdmin,Admin,ClientCompany,Customer,Driver")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx($"Invalid request");
                    return BadRequest(ModelState);
                }
                
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    _logger.LogErrorEx($"Error, booking {id} not found.");
                    return NotFound();
                }

                _logger.LogInformationEx($"Booking {id} retrieved successfully.");
                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // PUT: api/Booking/5
        // Update specific booking
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }

                if (id != booking.BookingId)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest();
                }

                _context.Entry(booking).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                
                _logger.LogInformationEx($"Booking {id} updated successfully");
                return Ok($"Booking {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookingExists(id))
                {
                    _logger.LogErrorEx($"Booking {id} not found");
                    return NotFound($"Booking {id} not found");
                }
                
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // POST: api/Booking
        // Create booking
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Booking created successfully");
                return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // DELETE: api/Booking/5
        // Delete specific booking
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    _logger.LogErrorEx($"Booking {id} not found");
                    return NotFound($"Booking {id} not found");
                }

                _context.Bookings.Remove(booking);
                
                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Booking {id} deleted successfully");
                return Ok($"Booking {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
