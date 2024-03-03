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
    public class DriverController : ControllerBase
    {
        private readonly TCTravelContext _context;
        private readonly ILogger<DriverController> _logger;

        public DriverController(TCTravelContext context, ILogger<DriverController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Driver
        // Retrieve all Drivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            try
            {
                var drivers = await _context.Drivers.ToListAsync();
            
                _logger.LogInformation("Successfully retrieved Drivers.");
                return drivers;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetDrivers)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // GET: api/Driver/5
        // Retrieve specific driver
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var driver = await _context.Drivers.FindAsync(id);

                if (driver == null)
                {
                    _logger.LogError($"Error, Driver {id} not found.");
                    return NotFound();
                }

                _logger.LogInformation($"Driver {id} retrieved successfully.");
                return driver;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetDriver)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // PUT: api/Driver/5
        // Update specific driver
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriver(int id, Driver driver)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            if (id != driver.DriverId)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest();
            }

            _context.Entry(driver).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriverExists(id))
                {
                    _logger.LogError($"Error. Driver {id} not found.");
                    return NotFound("The driver was not found.");
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"Driver {id} updated successfully!");
            return NoContent();
        }

        // POST: api/Driver
        // Create Driver
        [HttpPost]
        public async Task<ActionResult<Driver>> PostDriver(Driver driver)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Driver created successfully!");
                return CreatedAtAction("GetDriver", new { id = driver.DriverId }, driver);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PostDriver)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // DELETE: api/Driver/5
        // Delete specific driver
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var driver = await _context.Drivers.FindAsync(id);
                if (driver == null)
                {
                    _logger.LogError($"Error. Driver {id} not found.");
                    return NotFound("The driver was not found.");
                }

                _context.Drivers.Remove(driver);
            
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Driver {id} deleted successfully!");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteDriver)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.DriverId == id);

        }
    }
}
