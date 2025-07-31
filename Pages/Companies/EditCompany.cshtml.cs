using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models; // Adjust this namespace if your models are elsewhere

namespace MyApp.Namespace
{
    public class EditCompaniesModel : PageModel
    {
        private readonly ERP_BI_Operations.Models.ERP_BIContext _context; // Adjust namespace as needed

        public EditCompaniesModel(ERP_BI_Operations.Models.ERP_BIContext context) // Adjust namespace as needed
        {
            _context = context;
        }

        [BindProperty]
        public Company Company { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }
            Company = company;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(Company.CompanyId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./ListCompanies");
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
    }
}
