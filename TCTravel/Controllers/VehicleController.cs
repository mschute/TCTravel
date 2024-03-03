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
            
                _logger.LogInformation("Successfully retrieved Vehicles.");
                return vehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetVehicles)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // GET: api/Vehicle/5
        // Retrieve specific vehicle
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(id);

                if (vehicle == null)
                {
                    _logger.LogError($"Error, Vehicle {id} not found.");
                    return NotFound("The vehicle was not found.");
                }

                _logger.LogInformation($"Vehicle {id} retrieved successfully!");
                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetVehicle)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // PUT: api/Vehicle/5
        // Update specific vehicle
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicle(int id, Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            if (id != vehicle.VehicleId)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest();
            }

            _context.Entry(vehicle).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(id))
                {
                    _logger.LogError($"Error. Vehicle {id} not found.");
                    return NotFound("The vehicle was not found.");
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"Vehicle {id} updated successfully!");
            return NoContent();
        }

        // POST: api/Vehicle
        // Create Vehicle
        [HttpPost]
        public async Task<ActionResult<Vehicle>> PostVehicle(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Vehicle created successfully!");
                return CreatedAtAction("GetVehicle", new { id = vehicle.VehicleId }, vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PostVehicle)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // DELETE: api/Vehicle/5
        // Delete specific vehicle
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(id);
                if (vehicle == null)
                {
                    _logger.LogError($"Error. Vehicle {id} not found.");
                    return NotFound("The location was not found.");
                }

                _context.Vehicles.Remove(vehicle);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Vehicle {id} deleted successfully!");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteVehicle)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
