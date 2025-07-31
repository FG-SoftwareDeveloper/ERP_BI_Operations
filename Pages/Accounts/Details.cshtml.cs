using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models; // Ensure this namespace matches your models

namespace MyApp.Namespace
{
    public class DetailsModel(ERP_BIContext context, ILogger<DetailsModel> logger) : PageModel
    {
        private readonly ERP_BIContext _context = context;
        private readonly ILogger<DetailsModel> _logger = logger;

        public Account Account { get; set; } = null!; // Initialize with null-forgiving operator

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("DetailsModel OnGet: ID was null.");
                return NotFound();
            }

            // Fetch the account including its related Company
            Account = await _context.Accounts
                .Include(a => a.Company) // Include Company data for display
                .FirstOrDefaultAsync(m => m.AccountId == id);

            if (Account == null)
            {
                _logger.LogWarning($"DetailsModel OnGet: Account with ID {id} not found.");
                return NotFound();
            }
            return Page();
        }
    }
}