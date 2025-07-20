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
    public class TransmittalsController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public TransmittalsController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/Transmittals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transmittal>>> GetTransmittals()
        {
            return await _context.Transmittals.ToListAsync();
        }

        // GET: api/Transmittals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transmittal>> GetTransmittal(int id)
        {
            var transmittal = await _context.Transmittals.FindAsync(id);

            if (transmittal == null)
            {
                return NotFound();
            }

            return transmittal;
        }

        // PUT: api/Transmittals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransmittal(int id, Transmittal transmittal)
        {
            if (id != transmittal.TransmittalId)
            {
                return BadRequest();
            }

            _context.Entry(transmittal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransmittalExists(id))
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

        // POST: api/Transmittals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transmittal>> PostTransmittal(Transmittal transmittal)
        {
            _context.Transmittals.Add(transmittal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransmittal", new { id = transmittal.TransmittalId }, transmittal);
        }

        // DELETE: api/Transmittals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransmittal(int id)
        {
            var transmittal = await _context.Transmittals.FindAsync(id);
            if (transmittal == null)
            {
                return NotFound();
            }

            _context.Transmittals.Remove(transmittal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransmittalExists(int id)
        {
            return _context.Transmittals.Any(e => e.TransmittalId == id);
        }
    }
}
