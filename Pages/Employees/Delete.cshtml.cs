using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models; // Ensure this namespace matches your models
using Microsoft.Extensions.Logging;

namespace MyApp.Namespace
{
    // Using C# 12 Primary Constructor syntax
    public class DeleteEmployeeModel(ERP_BIContext context, ILogger<DeleteEmployeeModel> logger) : PageModel
    {
        // Private readonly fields initialized by the primary constructor
        private readonly ERP_BIContext _context = context;
        private readonly ILogger<DeleteEmployeeModel> _logger = logger;

        [BindProperty]
        public Employee Employee { get; set; } = null!; // Property to display employee details for confirmation

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("DeleteEmployeeModel OnGet: ID was null.");
                return NotFound();
            }

            Employee = await _context.Employees
                .Include(e => e.Department) // Include Department for display
                .Include(e => e.Company)    // Include Company for display
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (Employee == null)
            {
                _logger.LogWarning($"DeleteEmployeeModel OnGet: Employee with ID {id} not found.");
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("DeleteEmployeeModel OnPost: ID was null during post.");
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                _logger.LogWarning($"DeleteEmployeeModel OnPost: Employee with ID {id} not found for deletion.");
                return NotFound();
            }

            try
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Employee with ID {employee.EmployeeId} deleted successfully.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"DeleteEmployeeModel OnPost: An unexpected error occurred while deleting Employee ID {employee.EmployeeId}.");
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the employee. Please try again.");
                return Page(); // Stay on the page showing error
            }

            return RedirectToPage("./ListEmployees"); // Redirect back to the list page
        }
    }
}
