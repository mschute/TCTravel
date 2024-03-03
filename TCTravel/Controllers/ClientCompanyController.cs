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
    public class ClientCompanyController : ControllerBase
    {
        private readonly TCTravelContext _context;
        private readonly ILogger<ClientCompanyController> _logger;

        public ClientCompanyController(TCTravelContext context, ILogger<ClientCompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ClientCompany
        // Retrieve all client companies
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientCompany>>> GetClientCompanies()
        {
            try
            {
                var clientCompanies = await _context.ClientCompanies.ToListAsync();
            
                _logger.LogInformation("Successfully retrieved Client Company.");
                return clientCompanies;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetClientCompanies)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // GET: api/ClientCompany/5
        // Retrieve specific client companies
        // TODO Note: This is currently not configured to only allow specific user to update only their user information
        // TODO Will need to assign specific user role id upon sign up.
        [Authorize(Roles = "SuperAdmin,Admin,ClientCompany")]
        [HttpGet("{id}")]
        
        //TODO Need to pass AssignRoleModel for specific user restrictions
        public async Task<ActionResult<ClientCompany>> GetClientCompany(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var clientCompany = await _context.ClientCompanies.FindAsync(id);

                if (clientCompany == null)
                {
                    _logger.LogError($"Error, Client Company {id} not found.");
                    return NotFound("The client company was not found.");
                }

                //TODO Finish implementing specific user restrictions
                // if (id != model.ClientCompanyId && model.RoleName != "SuperAdmin" && model.RoleName != "Admin")
                // {
                //     _logger.LogError("User is unauthorized to get this information.");
                //     return Unauthorized(new { message = "Unauthorized" });
                // }
                
                _logger.LogInformation($"Client Company {id} retrieved successfully.");
                return clientCompany;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetClientCompany)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // PUT: api/ClientCompany/5
        // Update specific client company
        // TODO Need to configure restriction for only the user to update their own data
        [Authorize(Roles = "SuperAdmin,Admin,ClientCompany")]
        [HttpPut("{id}")]

        public async Task<IActionResult> PutClientCompany(int id, ClientCompany clientCompany)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            if (id != clientCompany.ClientCompanyId)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest();
            }

            _context.Entry(clientCompany).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!ClientCompanyExists(id))
                {
                    _logger.LogError($"Error. Client Company {id} not found.");
                    return NotFound("The client company was not found");
                }
                else
                {
                    throw;
                }
            }
            
            _logger.LogInformation($"Client Company {id} updated successfully!");
            return NoContent();
        }

        // POST: api/ClientCompany
        // Create Client Company
        //TODO May need to edit this depending on how I configure specific user access
        [Authorize(Roles = "SuperAdmin,Admin,ClientCompany")]
        [HttpPost]
        public async Task<ActionResult<ClientCompany>> PostClientCompany(ClientCompany clientCompany)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                _context.ClientCompanies.Add(clientCompany);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Client company created successfully");
                return CreatedAtAction("GetClientCompany", new { id = clientCompany.ClientCompanyId }, clientCompany);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PostClientCompany)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // DELETE: api/ClientCompany/5
        // Delete specific client company
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientCompany(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var clientCompany = await _context.ClientCompanies.FindAsync(id);

                if (clientCompany == null)
                {
                    _logger.LogError($"Error. Client Company {id} not found");
                    return NotFound("The client company was not found");
                }

                _context.ClientCompanies.Remove(clientCompany);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Client Company {id} deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteClientCompany)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
        
        private bool ClientCompanyExists(int id)
        {
            return _context.ClientCompanies.Any(e => e.ClientCompanyId == id);
        }
    }
}
