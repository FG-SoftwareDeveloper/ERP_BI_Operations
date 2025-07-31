using ERP_BI_Operations.Models;
using ERP_BI_Operations.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP_BI_Operations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ERP_BIContext _context;
        private readonly ForecastingService _forecastingService; // Inject the service

        public AccountsController(ERP_BIContext context, ForecastingService forecastingService)
        {
            _context = context;
            _forecastingService = forecastingService;
            // Note: For production, consider using an IHostedService to load the ML model
            // on application startup, rather than on the first request to a controller.
            // This ensures the model is ready and avoids synchronous waits in request paths.
        }

        // GET: api/Accounts
        // This endpoint will be used by DataTables initially and when filters are cleared
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts
                .Include(a => a.Company) // Include Company data for display in DataTables
                .ToListAsync();
        }

        // GET: api/Accounts/filter
        // This new endpoint handles filtering based on query parameters
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Account>>> GetFilteredAccounts(
            [FromQuery] string? accountType,
            [FromQuery] int? companyId,
            [FromQuery] decimal? minBalance,
            [FromQuery] decimal? maxBalance)
        {
            IQueryable<Account> accounts = _context.Accounts.Include(a => a.Company);

            if (!string.IsNullOrEmpty(accountType))
            {
                accounts = accounts.Where(a => a.AccountType == accountType);
            }

            if (companyId.HasValue)
            {
                accounts = accounts.Where(a => a.CompanyId == companyId.Value);
            }

            if (minBalance.HasValue)
            {
                accounts = accounts.Where(a => a.Balance >= minBalance.Value);
            }

            if (maxBalance.HasValue)
            {
                accounts = accounts.Where(a => a.Balance <= maxBalance.Value);
            }

            return await accounts.ToListAsync();
        }


        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.Company) // Include Company data if needed for details
                .FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.AccountId)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.AccountId }, account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }

        /*GET: api/Accounts/{id}/forecast
        [HttpGet("{id}/forecast")]
        public ActionResult<IEnumerable<object>> GetAccountForecast(int id, [FromQuery] int horizon = 6)
        {
            // IMPORTANT: This synchronously waits for the model to load/train.
            // For better performance and responsiveness, especially in production,
            // consider using an IHostedService to load the model on application startup.
            try
            {
                _forecastingService.LoadOrCreateForecastingModel().Wait();
            }
            catch (Exception ex)
            {
                // Log the exception if model loading/training fails
                Console.Error.WriteLine($"Error loading or training ML model: {ex.Message}");
                return StatusCode(500, "Failed to load or train forecasting model.");
            }


            var account = _context.Accounts.Find(id);
            if (account == null)
            {
                return NotFound($"Account with ID {id} not found.");
            }

            // Perform the forecast
            var prediction = _forecastingService.ForecastAccountBalance(id, horizon);

            if (prediction == null || prediction.ForecastedBalance == null || !prediction.ForecastedBalance.Any())
            {
                return NoContent(); // Or a more specific message if no forecast can be generated
            }

            // Get historical data for the chart (e.g., last 12 months)
            var currentDate = DateTime.Today;
            var historicalData = new List<object>();

            // Generate dummy historical data for demonstration if actual history is sparse
            // In a real application, you would fetch this from AccountHistory table
            // For now, let's simulate some historical data for charting purposes
            for (int i = 11; i >= 0; i--) // Last 12 months
            {
                // Simulate a decreasing trend for older data, then a slight increase
                decimal simulatedBalance = account.Balance * (1m - (decimal)(0.01m * i));
                if (i < 6) // For recent 6 months, make it closer to current balance
                {
                    simulatedBalance = account.Balance * (1m + (decimal)(0.005m * (6 - i)));
                }
                historicalData.Add(new
                {
                    Date = currentDate.AddMonths(-i).ToString("yyyy-MM-dd"),
                    Balance = (double)simulatedBalance
                });
            }

            // Prepare forecasted data with future dates
            var forecastedData = new List<object>();
            var lastHistoricalDate = historicalData.Any() ? DateTime.Parse(((dynamic)historicalData.Last()).Date) : DateTime.Today;

            for (int i = 0; i < prediction.ForecastedBalance.Length; i++)
            {
                forecastedData.Add(new
                {
                    Date = lastHistoricalDate.AddMonths(i + 1).ToString("yyyy-MM-dd"), // Forecast for next months
                    Balance = (double)prediction.ForecastedBalance[i],
                    LowerBound = (double?)prediction.LowerBound?.ElementAtOrDefault(i),
                    UpperBound = (double?)prediction.UpperBound?.ElementAtOrDefault(i)
                });
            }

            // Combine historical and forecasted data for the chart
            var combinedData = historicalData.Concat(forecastedData).ToList();

            return Ok(combinedData);
        }*/



    }
}
