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
    public class PayrollRunsController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public PayrollRunsController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/PayrollRuns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PayrollRun>>> GetPayrollRuns()
        {
            return await _context.PayrollRuns.ToListAsync();
        }

        // GET: api/PayrollRuns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PayrollRun>> GetPayrollRun(int id)
        {
            var payrollRun = await _context.PayrollRuns.FindAsync(id);

            if (payrollRun == null)
            {
                return NotFound();
            }

            return payrollRun;
        }

        // PUT: api/PayrollRuns/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayrollRun(int id, PayrollRun payrollRun)
        {
            if (id != payrollRun.PayrollRunId)
            {
                return BadRequest();
            }

            _context.Entry(payrollRun).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayrollRunExists(id))
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

        // POST: api/PayrollRuns
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PayrollRun>> PostPayrollRun(PayrollRun payrollRun)
        {
            _context.PayrollRuns.Add(payrollRun);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayrollRun", new { id = payrollRun.PayrollRunId }, payrollRun);
        }

        // DELETE: api/PayrollRuns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayrollRun(int id)
        {
            var payrollRun = await _context.PayrollRuns.FindAsync(id);
            if (payrollRun == null)
            {
                return NotFound();
            }

            _context.PayrollRuns.Remove(payrollRun);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PayrollRunExists(int id)
        {
            return _context.PayrollRuns.Any(e => e.PayrollRunId == id);
        }
    }
}
