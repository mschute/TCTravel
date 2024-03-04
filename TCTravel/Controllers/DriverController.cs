using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Helpers;
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
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            try
            {
                var drivers = await _context.Drivers.ToListAsync();
            
                _logger.LogInformationEx("Successfully retrieved Drivers");
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // GET: api/Driver/5
        // Retrieve specific driver
        [Authorize(Roles = "SuperAdmin,Admin,Driver")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var driver = await _context.Drivers.FindAsync(id);

                if (driver == null)
                {
                    _logger.LogErrorEx($"Driver {id} not found");
                    return NotFound();
                }

                _logger.LogInformationEx($"Driver {id} retrieved successfully");
                return Ok(driver);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // PUT: api/Driver/5
        // Update specific driver
        [Authorize(Roles = "SuperAdmin,Admin,Driver")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriver(int id, Driver driver)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }

                if (id != driver.DriverId)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest();
                }

                _context.Entry(driver).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                
                _logger.LogInformationEx($"Driver {id} updated successfully");
                return Ok($"Driver {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DriverExists(id))
                {
                    _logger.LogErrorEx($"Driver {id} not found");
                    return NotFound($"Driver {id} not found");
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

        // POST: api/Driver
        // Create Driver
        [Authorize(Roles = "SuperAdmin,Admin,Driver")]
        [HttpPost]
        public async Task<ActionResult<Driver>> PostDriver(Driver driver)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Driver created successfully");
                return CreatedAtAction("GetDriver", new { id = driver.DriverId }, driver);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // DELETE: api/Driver/5
        // Delete specific driver
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var driver = await _context.Drivers.FindAsync(id);
                if (driver == null)
                {
                    _logger.LogErrorEx($"Driver {id} not found");
                    return NotFound($"Driver {id} not found");
                }

                _context.Drivers.Remove(driver);
            
                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Driver {id} deleted successfully");
                return Ok($"Driver {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.DriverId == id);

        }
    }
}
