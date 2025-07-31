using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using Microsoft.EntityFrameworkCore; // For ToListAsync
using ERP_BI_Operations.Models; // Ensure this namespace matches your models

namespace MyApp.Namespace
{
    public class CreateModel(ERP_BIContext context, ILogger<CreateModel> logger) : PageModel
    {
        private readonly ERP_BIContext _context = context;
        private readonly ILogger<CreateModel> _logger = logger;

        [BindProperty]
        public Account Account { get; set; } = new Account(); // Initialize to prevent null reference warnings

        public SelectList CompanyList { get; set; } = null!; // For the Company dropdown

        public async Task<IActionResult> OnGetAsync()
        {
            // Populate the CompanyList for the dropdown
            CompanyList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove 'Company' from validation if it's a navigation property and not directly set by the form
            // Or ensure your Account model has [Required] on CompanyId if it's mandatory
            ModelState.Remove("Account.Company");

            if (!ModelState.IsValid)
            {
                // If model state is invalid, re-populate CompanyList before returning the page
                CompanyList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName");
                return Page();
            }

            try
            {
                _context.Accounts.Add(Account);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Account {AccountName} created successfully with ID {AccountId}", Account.AccountName, Account.AccountId);
                return RedirectToPage("./ListAccounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account {AccountName}.", Account.AccountName);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the account. Please try again.");
                // Re-populate CompanyList on error
                CompanyList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName");
                return Page();
            }
        }
    }
}