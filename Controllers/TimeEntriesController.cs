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
    public class TimeEntriesController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public TimeEntriesController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/TimeEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeEntry>>> GetTimeEntries()
        {
            return await _context.TimeEntries.ToListAsync();
        }

        // GET: api/TimeEntries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeEntry>> GetTimeEntry(int id)
        {
            var timeEntry = await _context.TimeEntries.FindAsync(id);

            if (timeEntry == null)
            {
                return NotFound();
            }

            return timeEntry;
        }

        // PUT: api/TimeEntries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeEntry(int id, TimeEntry timeEntry)
        {
            if (id != timeEntry.TimeEntryId)
            {
                return BadRequest();
            }

            _context.Entry(timeEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeEntryExists(id))
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

        // POST: api/TimeEntries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TimeEntry>> PostTimeEntry(TimeEntry timeEntry)
        {
            _context.TimeEntries.Add(timeEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimeEntry", new { id = timeEntry.TimeEntryId }, timeEntry);
        }

        // DELETE: api/TimeEntries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeEntry(int id)
        {
            var timeEntry = await _context.TimeEntries.FindAsync(id);
            if (timeEntry == null)
            {
                return NotFound();
            }

            _context.TimeEntries.Remove(timeEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TimeEntryExists(int id)
        {
            return _context.TimeEntries.Any(e => e.TimeEntryId == id);
        }
    }
}
