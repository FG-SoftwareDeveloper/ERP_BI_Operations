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
    public class EquipmentUsagesController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public EquipmentUsagesController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/EquipmentUsages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentUsage>>> GetEquipmentUsages()
        {
            return await _context.EquipmentUsages.ToListAsync();
        }

        // GET: api/EquipmentUsages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentUsage>> GetEquipmentUsage(int id)
        {
            var equipmentUsage = await _context.EquipmentUsages.FindAsync(id);

            if (equipmentUsage == null)
            {
                return NotFound();
            }

            return equipmentUsage;
        }

        // PUT: api/EquipmentUsages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipmentUsage(int id, EquipmentUsage equipmentUsage)
        {
            if (id != equipmentUsage.UsageId)
            {
                return BadRequest();
            }

            _context.Entry(equipmentUsage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentUsageExists(id))
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

        // POST: api/EquipmentUsages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EquipmentUsage>> PostEquipmentUsage(EquipmentUsage equipmentUsage)
        {
            _context.EquipmentUsages.Add(equipmentUsage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipmentUsage", new { id = equipmentUsage.UsageId }, equipmentUsage);
        }

        // DELETE: api/EquipmentUsages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipmentUsage(int id)
        {
            var equipmentUsage = await _context.EquipmentUsages.FindAsync(id);
            if (equipmentUsage == null)
            {
                return NotFound();
            }

            _context.EquipmentUsages.Remove(equipmentUsage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipmentUsageExists(int id)
        {
            return _context.EquipmentUsages.Any(e => e.UsageId == id);
        }
    }
}
