using Microsoft.AspNetCore.Mvc;
using ERP_BI_Operations.Services; // For FinancialForecastingService
using ERP_BI_Operations.Models;   // NEW: For FinancialForecast, WhatIfScenarioResult, WhatIfMonthResult
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using WhatIfScenarioResult = ERP_BI_Operations.Models.WhatIfScenarioResult;
using FinancialForecast = ERP_BI_Operations.Models.FinancialForecast; // Needed for DateOnly

namespace ERP_BI_Operations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialForecastsController : ControllerBase
    {
        private readonly FinancialForecastingService _forecastingService;

        public FinancialForecastsController(FinancialForecastingService forecastingService)
        {
            _forecastingService = forecastingService;
        }

        // GET: api/FinancialForecasts/cashbalance
        [HttpGet("cashbalance")]
        public async Task<ActionResult<IEnumerable<object>>> GetCashBalanceForecast()
        {
            var forecasts = await _forecastingService.PredictCashBalance(3);
            if (forecasts == null || !forecasts.Any())
            {
                return NoContent();
            }
            return Ok(forecasts.Select(x => new { date = x.ForecastDate, value = x.ForecastedValue }));
        }

        // GET: api/FinancialForecasts/materialcost
        [HttpGet("materialcost")]
        public async Task<ActionResult<IEnumerable<object>>> GetMaterialCostForecast()
        {
            var forecasts = await _forecastingService.PredictMaterialCost(3);
            if (forecasts == null || !forecasts.Any())
            {
                return NoContent();
            }
            return Ok(forecasts.Select(x => new { date = x.ForecastDate, value = x.ForecastedValue }));
        }

        // GET: api/FinancialForecasts/equipmentcost
        [HttpGet("equipmentcost")]
        public async Task<ActionResult<IEnumerable<object>>> GetEquipmentCostForecast()
        {
            var forecasts = await _forecastingService.PredictEquipmentCost(3);
            if (forecasts == null || !forecasts.Any())
            {
                return NoContent();
            }
            return Ok(forecasts.Select(x => new { date = x.ForecastDate, value = x.ForecastedValue }));
        }

        // Updated method to fix CS1061 error
        [HttpGet("whatif/costreduction")]
        public async Task<IActionResult> WhatIfCostReduction([FromQuery] string costType, [FromQuery] double reductionPercentage)
        {
            if (string.IsNullOrWhiteSpace(costType) || reductionPercentage <= 0 || reductionPercentage >= 100)
            {
                return BadRequest("Invalid costType or reductionPercentage. Percentage must be between 0 and 100.");
            }

            var decimalReduction = Convert.ToDecimal(reductionPercentage); // 👈 Safe cast

            var resultsTask = _forecastingService.CalculateWhatIfCostReduction(costType, decimalReduction);

            var results = await resultsTask; // Await the Task to get the actual result

            if (results == null || !results.Results.Any()) // Check the Results property of WhatIfScenarioResult
            {
                return NotFound("Could not perform what-if analysis. Ensure historical data is available.");
            }

            return Ok(new
            {
                results = results.Results.Select(x => new { date = x.Month, value = x.AdjustedValue })
            });
        }
    }
}
