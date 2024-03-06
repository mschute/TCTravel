using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Helpers;
using TCTravel.Models;

namespace TCTravel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Only SuperAdmin and Admin authorised to manage this controller
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class LocationController : ControllerBase
    {
        private readonly TCTravelContext _context;
        private readonly ILogger<DriverController> _logger;

        public LocationController(TCTravelContext context, ILogger<DriverController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Location
        // Retrieve all locations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            try
            {
                var locations = await _context.Locations.ToListAsync();
            
                _logger.LogInformationEx("Successfully retrieved Locations");
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // GET: api/Location/5
        // Retrieve specific location
        [HttpGet("{id}")]

        public async Task<ActionResult<Location>> GetLocation(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var location = await _context.Locations.FindAsync(id);

                if (location == null)
                {
                    _logger.LogErrorEx($"Location {id} not found");
                    return NotFound($"Location {id} not found");
                }

                _logger.LogInformationEx($"Location {id} retrieved successfully");
                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // PUT: api/Location/5
        // Update specific location
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation(int id, Location location)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }

                if (id != location.LocationId)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest();
                }

                _context.Entry(location).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                
                _logger.LogInformationEx($"Location {id} updated successfully");
                return Ok($"Location {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
                {
                    _logger.LogErrorEx($"Driver {id} not found");
                    return NotFound();
                }
                
                _logger.LogErrorEx($"Vehicle {id} not found");
                return NotFound($"Vehicle {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Vehicle {id} not found");
                return NotFound($"Vehicle {id} not found");
            }
        }

        // POST: api/Location
        // Create Location
        [HttpPost]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();

                _logger.LogInformationEx("Location created successfully");
                return CreatedAtAction("GetLocation", new { id = location.LocationId }, location);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // DELETE: api/Location/5
        // Delete specific location
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var location = await _context.Locations.FindAsync(id);
                
                if (location == null)
                {
                    _logger.LogErrorEx($"Location {id} not found");
                    return NotFound($"Location {id} not found");
                }

                _context.Locations.Remove(location);

                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Location {id} deleted successfully");
                return Ok($"Location {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationId == id);
        }
    }
}
