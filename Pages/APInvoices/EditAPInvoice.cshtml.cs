using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models;

namespace MyApp.Namespace
{
    public class EditAPInvoiceModel : PageModel
    {
        private readonly ERP_BI_Operations.Models.ERP_BIContext _context;

        public EditAPInvoiceModel(ERP_BI_Operations.Models.ERP_BIContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Apinvoice Apinvoice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apinvoice = await _context.Apinvoices.FirstOrDefaultAsync(m => m.ApinvoiceId == id);
            if (apinvoice == null)
            {
                return NotFound();
            }
            Apinvoice = apinvoice;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Apinvoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApinvoiceExists(Apinvoice.ApinvoiceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./ListAPInvoice");
        }

        private bool ApinvoiceExists(int id)
        {
            return _context.Apinvoices.Any(e => e.ApinvoiceId == id);
        }
    }
}
