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
    public class JournalEntrieLinesController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public JournalEntrieLinesController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/JournalEntrieLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JournalEntryLine>>> GetJournalEntryLines()
        {
            return await _context.JournalEntryLines.ToListAsync();
        }

        // GET: api/JournalEntrieLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JournalEntryLine>> GetJournalEntryLine(int id)
        {
            var journalEntryLine = await _context.JournalEntryLines.FindAsync(id);

            if (journalEntryLine == null)
            {
                return NotFound();
            }

            return journalEntryLine;
        }

        // PUT: api/JournalEntrieLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJournalEntryLine(int id, JournalEntryLine journalEntryLine)
        {
            if (id != journalEntryLine.JournalEntryLineId)
            {
                return BadRequest();
            }

            _context.Entry(journalEntryLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JournalEntryLineExists(id))
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

        // POST: api/JournalEntrieLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JournalEntryLine>> PostJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            _context.JournalEntryLines.Add(journalEntryLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJournalEntryLine", new { id = journalEntryLine.JournalEntryLineId }, journalEntryLine);
        }

        // DELETE: api/JournalEntrieLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJournalEntryLine(int id)
        {
            var journalEntryLine = await _context.JournalEntryLines.FindAsync(id);
            if (journalEntryLine == null)
            {
                return NotFound();
            }

            _context.JournalEntryLines.Remove(journalEntryLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JournalEntryLineExists(int id)
        {
            return _context.JournalEntryLines.Any(e => e.JournalEntryLineId == id);
        }
    }
}
