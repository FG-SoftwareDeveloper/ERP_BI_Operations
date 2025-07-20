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
    public class EmployeePayDetailsController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public EmployeePayDetailsController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/EmployeePayDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeePayDetail>>> GetEmployeePayDetails()
        {
            return await _context.EmployeePayDetails.ToListAsync();
        }

        // GET: api/EmployeePayDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeePayDetail>> GetEmployeePayDetail(int id)
        {
            var employeePayDetail = await _context.EmployeePayDetails.FindAsync(id);

            if (employeePayDetail == null)
            {
                return NotFound();
            }

            return employeePayDetail;
        }

        // PUT: api/EmployeePayDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeePayDetail(int id, EmployeePayDetail employeePayDetail)
        {
            if (id != employeePayDetail.PayDetailId)
            {
                return BadRequest();
            }

            _context.Entry(employeePayDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeePayDetailExists(id))
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

        // POST: api/EmployeePayDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeePayDetail>> PostEmployeePayDetail(EmployeePayDetail employeePayDetail)
        {
            _context.EmployeePayDetails.Add(employeePayDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployeePayDetail", new { id = employeePayDetail.PayDetailId }, employeePayDetail);
        }

        // DELETE: api/EmployeePayDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeePayDetail(int id)
        {
            var employeePayDetail = await _context.EmployeePayDetails.FindAsync(id);
            if (employeePayDetail == null)
            {
                return NotFound();
            }

            _context.EmployeePayDetails.Remove(employeePayDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeePayDetailExists(int id)
        {
            return _context.EmployeePayDetails.Any(e => e.PayDetailId == id);
        }
    }
}
