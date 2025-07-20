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
    public class RFIController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public RFIController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/RFI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rfi>>> GetRfis()
        {
            return await _context.Rfis.ToListAsync();
        }

        // GET: api/RFI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rfi>> GetRfi(int id)
        {
            var rfi = await _context.Rfis.FindAsync(id);

            if (rfi == null)
            {
                return NotFound();
            }

            return rfi;
        }

        // PUT: api/RFI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRfi(int id, Rfi rfi)
        {
            if (id != rfi.Rfiid)
            {
                return BadRequest();
            }

            _context.Entry(rfi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RfiExists(id))
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

        // POST: api/RFI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rfi>> PostRfi(Rfi rfi)
        {
            _context.Rfis.Add(rfi);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRfi", new { id = rfi.Rfiid }, rfi);
        }

        // DELETE: api/RFI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfi(int id)
        {
            var rfi = await _context.Rfis.FindAsync(id);
            if (rfi == null)
            {
                return NotFound();
            }

            _context.Rfis.Remove(rfi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RfiExists(int id)
        {
            return _context.Rfis.Any(e => e.Rfiid == id);
        }
    }
}
