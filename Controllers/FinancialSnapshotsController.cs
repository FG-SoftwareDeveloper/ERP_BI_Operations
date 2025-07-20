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
    public class FinancialSnapshotsController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public FinancialSnapshotsController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/FinancialSnapshots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinancialSnapshot>>> GetFinancialSnapshots()
        {
            return await _context.FinancialSnapshots.ToListAsync();
        }

        // GET: api/FinancialSnapshots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FinancialSnapshot>> GetFinancialSnapshot(int id)
        {
            var financialSnapshot = await _context.FinancialSnapshots.FindAsync(id);

            if (financialSnapshot == null)
            {
                return NotFound();
            }

            return financialSnapshot;
        }

        // PUT: api/FinancialSnapshots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFinancialSnapshot(int id, FinancialSnapshot financialSnapshot)
        {
            if (id != financialSnapshot.SnapshotId)
            {
                return BadRequest();
            }

            _context.Entry(financialSnapshot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinancialSnapshotExists(id))
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

        // POST: api/FinancialSnapshots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FinancialSnapshot>> PostFinancialSnapshot(FinancialSnapshot financialSnapshot)
        {
            _context.FinancialSnapshots.Add(financialSnapshot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFinancialSnapshot", new { id = financialSnapshot.SnapshotId }, financialSnapshot);
        }

        // DELETE: api/FinancialSnapshots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFinancialSnapshot(int id)
        {
            var financialSnapshot = await _context.FinancialSnapshots.FindAsync(id);
            if (financialSnapshot == null)
            {
                return NotFound();
            }

            _context.FinancialSnapshots.Remove(financialSnapshot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FinancialSnapshotExists(int id)
        {
            return _context.FinancialSnapshots.Any(e => e.SnapshotId == id);
        }
    }
}
