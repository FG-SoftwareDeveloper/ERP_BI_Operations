using ERP_BI_Operations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP_BI_Operations.Services
{
    public class ProjectFinancialSummaryForecastService
    {
        private readonly ERP_BIContext _context;
        private readonly MLContext _mlContext = new();

        public ProjectFinancialSummaryForecastService(ERP_BIContext context)
        {
            _context = context;
        }

        public async Task<PriceForecastResult> ForecastMaterialCostAsync(int projectId, int horizon = 3)
        {
            // 1. Get historical data
            var history = await _context.ProjectFinancialSummaries
                .Where(p => p.ProjectId == projectId && p.MaterialCost != null)
                .OrderBy(p => p.SummaryDate)
                .Select(p => new PriceData { Value = (float)p.MaterialCost.Value, Date = p.SummaryDate.ToDateTime(TimeOnly.MinValue) })
                .ToListAsync();

            if (history.Count < 25)
                return new PriceForecastResult { Success = false, Message = "Not enough data for forecasting." };

            // 2. Prepare data
            var dataView = _mlContext.Data.LoadFromEnumerable(history);

            // 3. Build pipeline
            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(PriceForecast.ForecastedValues),
                inputColumnName: nameof(PriceData.Value),
                windowSize: 12,
                seriesLength: history.Count,
                trainSize: history.Count,
                horizon: horizon,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(PriceForecast.LowerBound),
                confidenceUpperBoundColumn: nameof(PriceForecast.UpperBound)
            );

            // 4. Train model
            var model = pipeline.Fit(dataView);

            // 5. Forecast
            var engine = model.CreateTimeSeriesEngine<PriceData, PriceForecast>(_mlContext);
            var forecast = engine.Predict();

            // 6. Prepare result
            var lastDate = history.Last().Date;
            var results = new List<PriceForecastItem>();
            for (int i = 0; i < horizon; i++)
            {
                results.Add(new PriceForecastItem
                {
                    ForecastDate = lastDate.AddMonths(i + 1),
                    ForecastedValue = forecast.ForecastedValues[i],
                    LowerBound = forecast.LowerBound[i],
                    UpperBound = forecast.UpperBound[i]
                });
            }

            return new PriceForecastResult { Success = true, Forecasts = results };
        }
    }

    public class PriceData
    {
        public float Value { get; set; }
        public DateTime Date { get; set; }
    }

    public class PriceForecast
    {
        public float[] ForecastedValues { get; set; }
        public float[] LowerBound { get; set; }
        public float[] UpperBound { get; set; }
    }

    public class PriceForecastResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<PriceForecastItem> Forecasts { get; set; }
    }

    public class PriceForecastItem
    {
        public DateTime ForecastDate { get; set; }
        public float ForecastedValue { get; set; }
        public float LowerBound { get; set; }
        public float UpperBound { get; set; }
    }
}

// Ensure that any method returning forecast data to the controller provides objects with Date and Value properties.
// No direct changes needed here unless you are serializing to JSON directly in this service.