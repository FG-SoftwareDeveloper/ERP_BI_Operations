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
    public class KPIController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public KPIController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/KPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kpi>>> GetKpis()
        {
            return await _context.Kpis.ToListAsync();
        }

        // GET: api/KPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kpi>> GetKpi(int id)
        {
            var kpi = await _context.Kpis.FindAsync(id);

            if (kpi == null)
            {
                return NotFound();
            }

            return kpi;
        }

        // PUT: api/KPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKpi(int id, Kpi kpi)
        {
            if (id != kpi.Kpiid)
            {
                return BadRequest();
            }

            _context.Entry(kpi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KpiExists(id))
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

        // POST: api/KPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Kpi>> PostKpi(Kpi kpi)
        {
            _context.Kpis.Add(kpi);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKpi", new { id = kpi.Kpiid }, kpi);
        }

        // DELETE: api/KPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKpi(int id)
        {
            var kpi = await _context.Kpis.FindAsync(id);
            if (kpi == null)
            {
                return NotFound();
            }

            _context.Kpis.Remove(kpi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KpiExists(int id)
        {
            return _context.Kpis.Any(e => e.Kpiid == id);
        }
    }
}
