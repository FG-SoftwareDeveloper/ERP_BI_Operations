using ERP_BI_Operations.Models;
using ERP_BI_Operations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    [IgnoreAntiforgeryToken] // For JS POST to work without token for now
    [BindProperties]
    public class AdminModel : PageModel
    {
        private readonly ERP_BIContext _context;
        private readonly ILogger<AdminModel> _logger;

        public string ForecastText { get; set; } = "Revenue is expected to increase by 8% next quarter.";


        public AdminModel(ERP_BIContext context, ILogger<AdminModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Property to hold the list of companies for the filter dropdown
        public List<Company> Companies { get; set; } = new List<Company>();

        // Property to hold the total number of users
        public int TotalUsers { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Fetch companies to populate the dropdown filter
                Companies = await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync();

                // Fetch the total number of users
                TotalUsers = await _context.Users.CountAsync();

                // You can also fetch summary KPIs here if they are driven by the database
                // Example: Total Users, Total Employees (if not static)
                // TotalEmployees = await _context.Employees.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data for Admin Dashboard.");
                ModelState.AddModelError(string.Empty, "An error occurred while loading dashboard data. Please try again later.");
            }
        }


    }
}

