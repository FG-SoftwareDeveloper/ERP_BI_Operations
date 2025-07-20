using ERP_BI_Operations.Models; // Ensure this namespace matches your models
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Added for SelectList
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    // Using C# 12 Primary Constructor syntax
    public class EditModel(ERP_BIContext context, ILogger<EditModel> logger) : PageModel
    {
        // Private readonly fields initialized by the primary constructor
        private readonly ERP_BIContext _context = context;
        private readonly ILogger<EditModel> _logger = logger;

        [BindProperty]
        public Account Account { get; set; } = null!; // Fix CS8618 by initializing with null-forgiving operator

        // Added CompanyList property to hold data for the dropdown
        public SelectList CompanyList { get; set; } = null!; // Initialize with null-forgiving operator

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("EditModel OnGet: ID was null.");
                return NotFound();
            }

            Account = await _context.Accounts
                .Include(a => a.Company) // Include Company if you need to display company details
                .FirstOrDefaultAsync(m => m.AccountId == id);

            if (Account == null)
            {
                _logger.LogWarning($"EditModel OnGet: Account with ID {id} not found.");
                return NotFound();
            }

            // Populate the SelectList for companies
            CompanyList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName", Account.CompanyId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Re-populate CompanyList if ModelState is invalid, so the dropdown is available on re-render
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("EditModel OnPost: ModelState is invalid.");
                // Re-populate CompanyList before returning Page()
                CompanyList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName", Account.CompanyId);
                return Page();
            }

            // Ensure the Account object's state is correctly set to Modified
            _context.Entry(Account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Account with ID {Account.AccountId} updated successfully.");
            }
            catch (DbUpdateConcurrencyException e) // Catch the exception to log it
            {
                if (!AccountExists(Account.AccountId))
                {
                    _logger.LogError(e, $"EditModel OnPost: Concurrency conflict. Account with ID {Account.AccountId} not found during update.");
                    return NotFound();
                }
                else
                {
                    // Corrected to pass the exception 'e' to the logger
                    _logger.LogError(e, $"EditModel OnPost: Concurrency conflict for Account ID {Account.AccountId}. Another error occurred.");
                    throw; // Re-throw the exception if it's not a NotFound scenario
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"EditModel OnPost: An unexpected error occurred while saving Account ID {Account.AccountId}.");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the account. Please try again.");
                // Re-populate CompanyList before returning Page() on error
                CompanyList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName", Account.CompanyId);
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
