using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models;

namespace ERP_BI_Operations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApinvoicesController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public ApinvoicesController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/Apinvoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Apinvoice>>> GetApinvoices()
        {
            return await _context.Apinvoices.ToListAsync();
        }

        // GET: api/Apinvoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Apinvoice>> GetApinvoice(int id)
        {
            var apinvoice = await _context.Apinvoices.FindAsync(id);

            if (apinvoice == null)
            {
                return NotFound();
            }

            return apinvoice;
        }

        // PUT: api/Apinvoices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApinvoice(int id, Apinvoice apinvoice)
        {
            if (id != apinvoice.ApinvoiceId)
            {
                return BadRequest();
            }

            _context.Entry(apinvoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApinvoiceExists(id))
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

        // POST: api/Apinvoices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Apinvoice>> PostApinvoice(Apinvoice apinvoice)
        {
            _context.Apinvoices.Add(apinvoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApinvoice", new { id = apinvoice.ApinvoiceId }, apinvoice);
        }

        // DELETE: api/Apinvoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApinvoice(int id)
        {
            var apinvoice = await _context.Apinvoices.FindAsync(id);
            if (apinvoice == null)
            {
                return NotFound();
            }

            _context.Apinvoices.Remove(apinvoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApinvoiceExists(int id)
        {
            return _context.Apinvoices.Any(e => e.ApinvoiceId == id);
        }
    }
}
