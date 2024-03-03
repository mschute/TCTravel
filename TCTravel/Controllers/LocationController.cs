using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Models;

namespace TCTravel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            
                _logger.LogInformation("Successfully retrieved Locations.");
                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetLocations)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // GET: api/Location/5
        // Retrieve specific location
        [HttpGet("{id}")]

        public async Task<ActionResult<Location>> GetLocation(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var location = await _context.Locations.FindAsync(id);

                if (location == null)
                {
                    _logger.LogError($"Error, Location {id} not found.");
                    return NotFound();
                }

                _logger.LogInformation($"Location {id} retrieved successfully.");
                return location;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetLocation)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // PUT: api/Location/5
        // Update specific location
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation(int id, Location location)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            if (id != location.LocationId)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest();
            }

            _context.Entry(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
                {
                    _logger.LogError($"Error. Driver {id} not found.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"Location {id} updated successfully!");
            return NoContent();
        }

        // POST: api/Location
        // Create Location
        [HttpPost]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Location created successfully!");
                return CreatedAtAction("GetLocation", new { id = location.LocationId }, location);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PostLocation)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // DELETE: api/Location/5
        // Delete specific location
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var location = await _context.Locations.FindAsync(id);
                if (location == null)
                {
                    _logger.LogError($"Error. Location {id} not found.");
                    return NotFound("The location was not found.");
                }

                _context.Locations.Remove(location);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Location {id} deleted successfully!");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteLocation)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationId == id);
        }
    }
}
