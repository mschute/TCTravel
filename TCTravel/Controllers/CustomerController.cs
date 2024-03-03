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
            
                _logger.LogInformation("Successfully retrieved Customers.");
                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetCustomers)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // GET: api/Customer/5
        // Retrieve specific customer
        //TODO Configure specific user access
        [Authorize(Roles = "SuperAdmin,Admin,Customer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    _logger.LogError($"Error, Customer {id} not found.");
                    return NotFound("The customer was not found.");
                }

                _logger.LogInformation($"Customer {id} retrieved successfully.");
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetCustomer)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // PUT: api/Customer/5
        // Update specific customer
        [Authorize(Roles = "SuperAdmin,Admin,Customer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            if (id != customer.CustomerId)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    _logger.LogError($"Error. Customer {id} not found.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"Customer {id} updated successfully!");
            return NoContent();
        }

        // POST: api/Customer
        // Create Customer
        //TODO May need to update based on specific user access
        [Authorize(Roles = "SuperAdmin,Admin,Customer")]
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Customer created successfully");
                return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
            }
            catch (Exception ex)
            {
               _logger.LogError($"{nameof(PostCustomer)} failed with error: {ex}");
               return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // DELETE: api/Customer/5
        // Delete specific customer
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    _logger.LogError($"Error. Customer {id} not found");
                    return NotFound();
                }

                _context.Customers.Remove(customer);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Customer {id} deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteCustomer)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);

        }
    }
}
