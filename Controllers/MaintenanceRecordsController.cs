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
    public class MaintenanceRecordsController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public MaintenanceRecordsController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/MaintenanceRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceRecord>>> GetMaintenanceRecords()
        {
            return await _context.MaintenanceRecords.ToListAsync();
        }

        // GET: api/MaintenanceRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceRecord>> GetMaintenanceRecord(int id)
        {
            var maintenanceRecord = await _context.MaintenanceRecords.FindAsync(id);

            if (maintenanceRecord == null)
            {
                return NotFound();
            }

            return maintenanceRecord;
        }

        // PUT: api/MaintenanceRecords/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaintenanceRecord(int id, MaintenanceRecord maintenanceRecord)
        {
            if (id != maintenanceRecord.MaintenanceId)
            {
                return BadRequest();
            }

            _context.Entry(maintenanceRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceRecordExists(id))
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

        // POST: api/MaintenanceRecords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaintenanceRecord>> PostMaintenanceRecord(MaintenanceRecord maintenanceRecord)
        {
            _context.MaintenanceRecords.Add(maintenanceRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMaintenanceRecord", new { id = maintenanceRecord.MaintenanceId }, maintenanceRecord);
        }

        // DELETE: api/MaintenanceRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenanceRecord(int id)
        {
            var maintenanceRecord = await _context.MaintenanceRecords.FindAsync(id);
            if (maintenanceRecord == null)
            {
                return NotFound();
            }

            _context.MaintenanceRecords.Remove(maintenanceRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaintenanceRecordExists(int id)
        {
            return _context.MaintenanceRecords.Any(e => e.MaintenanceId == id);
        }
    }
}
