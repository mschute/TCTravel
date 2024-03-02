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
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientCompany>> GetClientCompany(int id)
        {
            try
            {
                var clientCompany = await _context.ClientCompanies.FindAsync(id);

                if (clientCompany == null)
                {
                    _logger.LogError($"Error, Client Company {id} not found.");
                    return NotFound("The client company was not found.");
                }

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
        [HttpPut("{id}")]

        public async Task<IActionResult> PutClientCompany(int id, ClientCompany clientCompany)
        {
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
        [HttpPost]
        public async Task<ActionResult<ClientCompany>> PostClientCompany(ClientCompany clientCompany)
        {
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
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteClientCompany(int id)
        {
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
