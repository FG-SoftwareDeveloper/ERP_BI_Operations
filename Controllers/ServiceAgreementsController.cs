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
    public class ServiceAgreementsController : ControllerBase
    {
        private readonly ERP_BIContext _context;

        public ServiceAgreementsController(ERP_BIContext context)
        {
            _context = context;
        }

        // GET: api/ServiceAgreements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceAgreement>>> GetServiceAgreements()
        {
            return await _context.ServiceAgreements.ToListAsync();
        }

        // GET: api/ServiceAgreements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceAgreement>> GetServiceAgreement(int id)
        {
            var serviceAgreement = await _context.ServiceAgreements.FindAsync(id);

            if (serviceAgreement == null)
            {
                return NotFound();
            }

            return serviceAgreement;
        }

        // PUT: api/ServiceAgreements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceAgreement(int id, ServiceAgreement serviceAgreement)
        {
            if (id != serviceAgreement.AgreementId)
            {
                return BadRequest();
            }

            _context.Entry(serviceAgreement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceAgreementExists(id))
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

        // POST: api/ServiceAgreements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServiceAgreement>> PostServiceAgreement(ServiceAgreement serviceAgreement)
        {
            _context.ServiceAgreements.Add(serviceAgreement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceAgreement", new { id = serviceAgreement.AgreementId }, serviceAgreement);
        }

        // DELETE: api/ServiceAgreements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceAgreement(int id)
        {
            var serviceAgreement = await _context.ServiceAgreements.FindAsync(id);
            if (serviceAgreement == null)
            {
                return NotFound();
            }

            _context.ServiceAgreements.Remove(serviceAgreement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceAgreementExists(int id)
        {
            return _context.ServiceAgreements.Any(e => e.AgreementId == id);
        }
    }
}
