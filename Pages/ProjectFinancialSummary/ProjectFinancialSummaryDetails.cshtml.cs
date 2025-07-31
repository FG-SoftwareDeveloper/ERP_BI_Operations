using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ERP_BI_Operations.Models;
using ERP_BI_Operations.Services;
using System.Threading.Tasks;

namespace MyApp.Namespace
{
    public class ProjectFinancialSummaryDetailsModel : PageModel
    {
        private readonly ProjectFinancialSummaryForecastService _forecastService;

        public ProjectFinancialSummaryDetailsModel(ProjectFinancialSummaryForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        public PriceForecastResult MaterialCostForecast { get; set; }

        public async Task OnGetAsync(int projectId)
        {
            MaterialCostForecast = await _forecastService.ForecastMaterialCostAsync(projectId, 3);
        }
    }
}
