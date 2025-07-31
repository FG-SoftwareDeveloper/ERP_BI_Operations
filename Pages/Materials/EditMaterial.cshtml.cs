using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models;

namespace MyApp.Namespace
{
    public class EditMaterialsModel : PageModel
    {
        private readonly ERP_BI_Operations.Models.ERP_BIContext _context;

        public EditMaterialsModel(ERP_BI_Operations.Models.ERP_BIContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Material Material { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == id);
            if (material == null)
            {
                return NotFound();
            }
            Material = material;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Material).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(Material.MaterialId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./ListMaterials");
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.MaterialId == id);
        }
    }
}
