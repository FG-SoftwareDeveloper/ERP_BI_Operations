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
    public class PurchaseOrderLineController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public PurchaseOrderLineController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseOrderLine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderLine>>> GetPurchaseOrderLines()
        {
            return await _context.PurchaseOrderLines.ToListAsync();
        }

        // GET: api/PurchaseOrderLine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderLine>> GetPurchaseOrderLine(int id)
        {
            var purchaseOrderLine = await _context.PurchaseOrderLines.FindAsync(id);

            if (purchaseOrderLine == null)
            {
                return NotFound();
            }

            return purchaseOrderLine;
        }

        // PUT: api/PurchaseOrderLine/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderLine(int id, PurchaseOrderLine purchaseOrderLine)
        {
            if (id != purchaseOrderLine.PolineId)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrderLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderLineExists(id))
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

        // POST: api/PurchaseOrderLine
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PurchaseOrderLine>> PostPurchaseOrderLine(PurchaseOrderLine purchaseOrderLine)
        {
            _context.PurchaseOrderLines.Add(purchaseOrderLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrderLine", new { id = purchaseOrderLine.PolineId }, purchaseOrderLine);
        }

        // DELETE: api/PurchaseOrderLine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrderLine(int id)
        {
            var purchaseOrderLine = await _context.PurchaseOrderLines.FindAsync(id);
            if (purchaseOrderLine == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderLines.Remove(purchaseOrderLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PurchaseOrderLineExists(int id)
        {
            return _context.PurchaseOrderLines.Any(e => e.PolineId == id);
        }
    }
}
