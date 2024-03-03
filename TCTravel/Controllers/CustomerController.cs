using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCTravel.Helpers;
using TCTravel.Models;

namespace TCTravel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly TCTravelContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(TCTravelContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Customer
        // Retrieve all client companies
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers.ToListAsync();
            
                _logger.LogInformationEx("Successfully retrieved Customers");
                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // GET: api/Customer/5
        // Retrieve specific customer
        //TODO Configure specific user access
        [Authorize(Roles = "SuperAdmin,Admin,Customer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    _logger.LogErrorEx($"Customer {id} not found");
                    return NotFound($"Customer {id} not found");
                }

                _logger.LogInformationEx($"Customer {id} retrieved successfully");
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // PUT: api/Customer/5
        // Update specific customer
        [Authorize(Roles = "SuperAdmin,Admin,Customer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }

                if (id != customer.CustomerId)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest();
                }

                _context.Entry(customer).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Customer {id} updated successfully");
                return Ok($"Customer {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CustomerExists(id))
                {
                    _logger.LogErrorEx($"Customer {id} not found.");
                    return NotFound();
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

        // POST: api/Customer
        // Create Customer
        //TODO May need to update based on specific user access
        [Authorize(Roles = "SuperAdmin,Admin,Customer")]
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Customer created successfully");
                return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
            }
            catch (Exception ex)
            {
               _logger.LogErrorEx($"Failed with error: {ex}");
               return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // DELETE: api/Customer/5
        // Delete specific customer
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var customer = await _context.Customers.FindAsync(id);
                
                if (customer == null)
                {
                    _logger.LogErrorEx($"Customer {id} not found");
                    return NotFound();
                }

                _context.Customers.Remove(customer);

                await _context.SaveChangesAsync();

                _logger.LogInformationEx($"Customer {id} deleted successfully");
                return Ok($"Customer {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);

        }
    }
}
