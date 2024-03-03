using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Helpers;
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
            
                _logger.LogInformationEx("Successfully retrieved Client Company.");
                return Ok(clientCompanies);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
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
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var clientCompany = await _context.ClientCompanies.FindAsync(id);

                if (clientCompany == null)
                {
                    _logger.LogErrorEx($"Client Company {id} not found.");
                    return NotFound($"Client Company {id} not found.");
                }

                //TODO Finish implementing specific user restrictions
                // if (id != model.ClientCompanyId && model.RoleName != "SuperAdmin" && model.RoleName != "Admin")
                // {
                //     _logger.LogErrorEx("User is unauthorized to get this information.");
                //     return Unauthorized(new { message = "Unauthorized" });
                // }
                
                _logger.LogInformationEx($"Client Company {id} retrieved successfully");
                return clientCompany;
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // PUT: api/ClientCompany/5
        // Update specific client company
        // TODO Need to configure restriction for only the user to update their own data
        [Authorize(Roles = "SuperAdmin,Admin,ClientCompany")]
        [HttpPut("{id}")]

        public async Task<IActionResult> PutClientCompany(int id, ClientCompany clientCompany)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }

                if (id != clientCompany.ClientCompanyId)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest();
                }

                _context.Entry(clientCompany).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Client Company {id} updated successfully");
                return Ok($"Client Company {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ClientCompanyExists(id))
                {
                    _logger.LogErrorEx($"Error. Client Company {id} not found.");
                    return NotFound("The client company was not found");
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

        // POST: api/ClientCompany
        // Create Client Company
        //TODO May need to edit this depending on how I configure specific user access
        [Authorize(Roles = "SuperAdmin,Admin,ClientCompany")]
        [HttpPost]
        public async Task<ActionResult<ClientCompany>> PostClientCompany(ClientCompany clientCompany)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                _context.ClientCompanies.Add(clientCompany);
                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Client Company created successfully");
                return CreatedAtAction("GetClientCompany", new { id = clientCompany.ClientCompanyId }, clientCompany);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // DELETE: api/ClientCompany/5
        // Delete specific client company
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientCompany(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var clientCompany = await _context.ClientCompanies.FindAsync(id);

                if (clientCompany == null)
                {
                    _logger.LogErrorEx($"Client Company {id} not found");
                    return NotFound($"Client Company {id} not found");
                }

                _context.ClientCompanies.Remove(clientCompany);

                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Client Company {id} deleted successfully");
                return Ok($"Client Company {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }
        
        private bool ClientCompanyExists(int id)
        {
            return _context.ClientCompanies.Any(e => e.ClientCompanyId == id);
        }
    }
}
