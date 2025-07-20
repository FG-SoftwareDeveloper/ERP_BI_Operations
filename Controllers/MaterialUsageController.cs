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
    public class MaterialUsageController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public MaterialUsageController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/MaterialUsage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialUsage>>> GetMaterialUsages()
        {
            return await _context.MaterialUsages.ToListAsync();
        }

        // GET: api/MaterialUsage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialUsage>> GetMaterialUsage(int id)
        {
            var materialUsage = await _context.MaterialUsages.FindAsync(id);

            if (materialUsage == null)
            {
                return NotFound();
            }

            return materialUsage;
        }

        // PUT: api/MaterialUsage/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaterialUsage(int id, MaterialUsage materialUsage)
        {
            if (id != materialUsage.MaterialUsageId)
            {
                return BadRequest();
            }

            _context.Entry(materialUsage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialUsageExists(id))
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

        // POST: api/MaterialUsage
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaterialUsage>> PostMaterialUsage(MaterialUsage materialUsage)
        {
            _context.MaterialUsages.Add(materialUsage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMaterialUsage", new { id = materialUsage.MaterialUsageId }, materialUsage);
        }

        // DELETE: api/MaterialUsage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialUsage(int id)
        {
            var materialUsage = await _context.MaterialUsages.FindAsync(id);
            if (materialUsage == null)
            {
                return NotFound();
            }

            _context.MaterialUsages.Remove(materialUsage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaterialUsageExists(int id)
        {
            return _context.MaterialUsages.Any(e => e.MaterialUsageId == id);
        }
    }
}
