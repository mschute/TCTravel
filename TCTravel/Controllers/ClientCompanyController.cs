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

        public ClientCompanyController(TCTravelContext context)
        {
            _context = context;
        }

        // GET: api/ClientCompany
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientCompany>>> GetClientCompanies()
        {
            return await _context.ClientCompanies.ToListAsync();
        }

        // GET: api/ClientCompany/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientCompany>> GetClientCompany(int id)
        {
            var clientCompany = await _context.ClientCompanies.FindAsync(id);

            if (clientCompany == null)
            {
                return NotFound();
            }

            return clientCompany;
        }

        // PUT: api/ClientCompany/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClientCompany(int id, ClientCompany clientCompany)
        {
            if (id != clientCompany.ClientCompanyId)
            {
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ClientCompany
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClientCompany>> PostClientCompany(ClientCompany clientCompany)
        {
            _context.ClientCompanies.Add(clientCompany);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClientCompany", new { id = clientCompany.ClientCompanyId }, clientCompany);
        }

        // DELETE: api/ClientCompany/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientCompany(int id)
        {
            var clientCompany = await _context.ClientCompanies.FindAsync(id);
            if (clientCompany == null)
            {
                return NotFound();
            }

            _context.ClientCompanies.Remove(clientCompany);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientCompanyExists(int id)
        {
            return _context.ClientCompanies.Any(e => e.ClientCompanyId == id);
        }
    }
}
