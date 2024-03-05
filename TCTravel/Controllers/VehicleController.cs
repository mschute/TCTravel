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
    public class VehicleController : ControllerBase
    {
        private readonly TCTravelContext _context;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(TCTravelContext context, ILogger<VehicleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Vehicle
        // Retrieve all vehicles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            try
            {
                var vehicles = await _context.Vehicles.ToListAsync();
            
                _logger.LogInformationEx("Successfully retrieved Vehicles");
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex.InnerException}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // GET: api/Vehicle/5
        // Retrieve specific vehicle
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx($"Invalid request");
                    return BadRequest(ModelState);
                }
                
                var vehicle = await _context.Vehicles.FindAsync(id);

                if (vehicle == null)
                {
                    _logger.LogErrorEx($"Vehicle {id} not found");
                    return NotFound($"Vehicle {id} not found");
                }

                _logger.LogInformationEx($"Vehicle {id} retrieved successfully");
                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // PUT: api/Vehicle/5
        // Update specific vehicle
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicle(int id, Vehicle vehicle)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
            
                if (id != vehicle.VehicleId)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest();
                }

                _context.Entry(vehicle).State = EntityState.Modified;
            
                await _context.SaveChangesAsync();
                
                _logger.LogInformationEx($"Vehicle {id} updated successfully");
                return Ok($"Vehicle {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VehicleExists(id))
                {
                    _logger.LogErrorEx($"Vehicle {id} not found");
                    return NotFound($"Vehicle {id} not found");
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

        // POST: api/Vehicle
        // Create Vehicle
        [HttpPost]
        public async Task<ActionResult<Vehicle>> PostVehicle(Vehicle vehicle)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Vehicle created successfully");
                return CreatedAtAction("GetVehicle", new { id = vehicle.VehicleId }, vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // DELETE: api/Vehicle/5
        // Delete specific vehicle
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var vehicle = await _context.Vehicles.FindAsync(id);
                
                if (vehicle == null)
                {
                    _logger.LogError($"Vehicle {id} not found");
                    return NotFound($"Vehicle {id} not found");
                }

                _context.Vehicles.Remove(vehicle);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Vehicle {id} deleted successfully");
                return Ok($"Vehicle {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
