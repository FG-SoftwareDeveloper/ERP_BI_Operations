using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models;

namespace MyApp.Namespace
{
    public class EditMaterialUsageModel : PageModel
    {
        private readonly ERP_BI_Operations.Models.ERP_BIContext _context;

        public EditMaterialUsageModel(ERP_BI_Operations.Models.ERP_BIContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MaterialUsage MaterialUsage { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialUsage = await _context.MaterialUsages.FirstOrDefaultAsync(m => m.MaterialUsageId == id);
            if (materialUsage == null)
            {
                return NotFound();
            }
            MaterialUsage = materialUsage;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(MaterialUsage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialUsageExists(MaterialUsage.MaterialUsageId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./ListMaterialUsage");
        }

        private bool MaterialUsageExists(int id)
        {
            return _context.MaterialUsages.Any(e => e.MaterialUsageId == id);
        }
    }
}
