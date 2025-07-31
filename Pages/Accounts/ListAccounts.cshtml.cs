using ERP_BI_Operations.Models; // Make sure this namespace matches your models
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    // Using C# 12 Primary Constructor syntax for cleaner injection
    public class IndexModel(ERP_BIContext context, ILogger<IndexModel> logger) : PageModel
    {
        private readonly ERP_BIContext _context = context;
        private readonly ILogger<IndexModel> _logger = logger;

        public IList<Account> Accounts { get; set; } = new List<Account>(); // Initialize to prevent null reference
        public List<Company> Companies { get; set; } = new();

        // Properties for KPIs
        public int TotalAccounts { get; set; }
        public int TotalAssetAccounts { get; set; }
        public int TotalLiabilityAccounts { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Fetch accounts for initial page load (e.g., for KPIs)
                // This data will also be used to populate the card view directly
                Accounts = await _context.Accounts
                                         .Include(a => a.Company) // Assuming you have a navigation property 'Company' in your Account model
                                         .OrderBy(a => a.AccountName) // Order them for better display
                                         .ToListAsync();

                // Add this to fetch companies for the filter dropdown
                Companies = await _context.Companies
                                          .OrderBy(c => c.CompanyName)
                                          .ToListAsync();

                // Calculate KPIs dynamically
                TotalAccounts = Accounts.Count;
                TotalAssetAccounts = Accounts.Count(a => a.AccountType == "Asset");
                TotalLiabilityAccounts = Accounts.Count(a => a.AccountType == "Liability");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accounts data for initial page load.");
                ModelState.AddModelError(string.Empty, "An error occurred while loading account data. Please try again later.");
            }
        }

        // NEW HANDLER: To serve all accounts data for DataTables via AJAX
        public async Task<JsonResult> OnGetAccountsJsonDataAsync()
        {
            try
            {
                var accounts = await _context.Accounts
                                             .Include(a => a.Company) // Include related company data
                                             .Select(a => new
                                             {
                                                 a.AccountId,
                                                 a.AccountName,
                                                 a.AccountType,
                                                 a.Balance,
                                                 a.Company // Assuming Status is a property in your Account model
                                             })
                                             .ToListAsync();

                return new JsonResult(new { data = accounts }); // DataTables expects 'data' property
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accounts data for DataTables.");
                // Return an error status or empty data for DataTables to handle gracefully
                return new JsonResult(new { data = new List<object>(), error = "Failed to load data." });
            }
        }

        // Existing partial view handlers
        public async Task<IActionResult> OnGetDetailsPartialAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();
            return Partial("_AccountDetailsModalContent", account);
        }

        public async Task<IActionResult> OnGetEditPartialAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();
            return Partial("_AccountEditModalContent", account);
        }

        /*public async Task<IActionResult> OnGetDeletePartialAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(AccountId);
            if (account == null) return NotFound();
            return Partial("_AccountDeleteModalContent", account);
        }*/

        // Existing POST handlers for Edit and Delete (AJAX) - ensure these match your actual implementation
        [HttpPost]
        public async Task<IActionResult> OnPostEditAsync([FromBody] Account account) // Use FromBody if sending JSON from client
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            var existing = await _context.Accounts.FindAsync(account.AccountId);
            if (existing == null)
            {
                return NotFound(new { success = false, message = "Account not found for update." });
            }

            // Update properties
            existing.AccountName = account.AccountName;
            existing.AccountType = account.AccountType;
            existing.Balance = account.Balance;
            // Update other properties as needed

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostDeleteAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound(new { success = false, message = "Account not found for deletion." });
            }
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
    }
}