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
    public class ProjectFinancialSummaryController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public ProjectFinancialSummaryController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/ProjectFinancialSummary
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectFinancialSummary>>> GetProjectFinancialSummaries()
        {
            return await _context.ProjectFinancialSummaries.ToListAsync();
        }

        // GET: api/ProjectFinancialSummary/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectFinancialSummary>> GetProjectFinancialSummary(int id)
        {
            var projectFinancialSummary = await _context.ProjectFinancialSummaries.FindAsync(id);

            if (projectFinancialSummary == null)
            {
                return NotFound();
            }

            return projectFinancialSummary;
        }

        // PUT: api/ProjectFinancialSummary/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectFinancialSummary(int id, ProjectFinancialSummary projectFinancialSummary)
        {
            if (id != projectFinancialSummary.ProjectSummaryId)
            {
                return BadRequest();
            }

            _context.Entry(projectFinancialSummary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectFinancialSummaryExists(id))
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

        // POST: api/ProjectFinancialSummary
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectFinancialSummary>> PostProjectFinancialSummary(ProjectFinancialSummary projectFinancialSummary)
        {
            _context.ProjectFinancialSummaries.Add(projectFinancialSummary);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectFinancialSummary", new { id = projectFinancialSummary.ProjectSummaryId }, projectFinancialSummary);
        }

        // DELETE: api/ProjectFinancialSummary/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectFinancialSummary(int id)
        {
            var projectFinancialSummary = await _context.ProjectFinancialSummaries.FindAsync(id);
            if (projectFinancialSummary == null)
            {
                return NotFound();
            }

            _context.ProjectFinancialSummaries.Remove(projectFinancialSummary);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectFinancialSummaryExists(int id)
        {
            return _context.ProjectFinancialSummaries.Any(e => e.ProjectSummaryId == id);
        }
    }
}
