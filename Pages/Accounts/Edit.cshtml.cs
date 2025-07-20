using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models; // Ensure this namespace matches your models
using Microsoft.Extensions.Logging; // For logging

namespace MyApp.Namespace
{
    public class EditModel : PageModel
    {
        private readonly ERP_BIContext _context;
        private readonly ILogger<EditModel> _logger; // Add logger

        public EditModel(ERP_BIContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Account Account { get; set; } // Property to bind form data

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("EditModel OnGet: ID was null.");
                return NotFound();
            }

            Account? account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null)
            {
                _logger.LogWarning($"EditModel OnGet: Account with ID {id} not found.");
                return NotFound();
            }

            Account = account; // Assign only after null check
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("EditModel OnPost: ModelState is invalid.");
                return Page();
            }

            _context.Attach(Account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Account with ID {Account.AccountId} updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(Account.AccountId))
                {
                    _logger.LogError($"EditModel OnPost: Concurrency conflict. Account with ID {Account.AccountId} not found during update.");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"EditModel OnPost: Concurrency conflict for Account ID {Account.AccountId}. Another error occurred.");
                    throw;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"EditModel OnPost: An unexpected error occurred while saving Account ID {Account.AccountId}.", e);
                ModelState.AddModelError(string.Empty, "An error occurred while saving the account. Please try again.");
                return Page();
            }

            return RedirectToPage("./ListAccounts"); // Redirect back to the list page
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
