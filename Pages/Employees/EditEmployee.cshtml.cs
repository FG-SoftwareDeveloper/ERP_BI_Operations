using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using Microsoft.EntityFrameworkCore;
using ERP_BI_Operations.Models; // Ensure this namespace matches your models
using Microsoft.Extensions.Logging;

namespace MyApp.Namespace
{
    // Using C# 12 Primary Constructor syntax
    public class EditEmployeeModel(ERP_BIContext context, ILogger<EditEmployeeModel> logger) : PageModel
    {
        // Private readonly fields initialized by the primary constructor
        private readonly ERP_BIContext _context = context;
        private readonly ILogger<EditEmployeeModel> _logger = logger;

        [BindProperty]
        public Employee Employee { get; set; } = null!; // Property to bind form data

        // SelectLists for dropdowns
        public SelectList DepartmentsList { get; set; } = null!;
        public SelectList CompaniesList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("EditEmployeeModel OnGet: ID was null.");
                return NotFound();
            }

            // Fetch the Employee including related Department and Company
            Employee = await _context.Employees
                .Include(e => e.Department) // Include Department for display/pre-selection
                .Include(e => e.Company)    // Include Company for display/pre-selection
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (Employee == null)
            {
                _logger.LogWarning($"EditEmployeeModel OnGet: Employee with ID {id} not found.");
                return NotFound();
            }

            // Populate the SelectLists for dropdowns
            DepartmentsList = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", Employee.DepartmentId);
            CompaniesList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName", Employee.CompanyId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Re-populate SelectLists if ModelState is invalid, so dropdowns are available on re-render
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("EditEmployeeModel OnPost: ModelState is invalid.");
                DepartmentsList = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", Employee.DepartmentId);
                CompaniesList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName", Employee.CompanyId);
                return Page();
            }

            _context.Attach(Employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Employee with ID {Employee.EmployeeId} updated successfully.");
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!EmployeeExists(Employee.EmployeeId))
                {
                    _logger.LogError(e, $"EditEmployeeModel OnPost: Concurrency conflict. Employee with ID {Employee.EmployeeId} not found during update.");
                    return NotFound();
                }
                else
                {
                    _logger.LogError(e, $"EditEmployeeModel OnPost: Concurrency conflict for Employee ID {Employee.EmployeeId}. Another error occurred.");
                    throw; // Re-throw the exception if it's not a NotFound scenario
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"EditEmployeeModel OnPost: An unexpected error occurred while saving Employee ID {Employee.EmployeeId}.");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the employee. Please try again.");
                // Re-populate SelectLists before returning Page() on error
                DepartmentsList = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", Employee.DepartmentId);
                CompaniesList = new SelectList(await _context.Companies.ToListAsync(), "CompanyId", "CompanyName", Employee.CompanyId);
                return Page();
            }

            return RedirectToPage("./ListEmployees"); // Redirect back to the list page
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
