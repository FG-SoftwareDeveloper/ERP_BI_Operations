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
    public class FinancialForecastingService
    {
        private readonly ERP_BIContext _context;
        private readonly MLContext _mlContext;
        private ITransformer _cashBalanceModel;
        private ITransformer _materialCostModel;
        private ITransformer _equipmentCostModel;

        public FinancialForecastingService(ERP_BIContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mlContext = new MLContext();
        }

        // Helper to get the last financial snapshot (used for baseline in What-If)
        public async Task<FinancialSnapshot> GetLastFinancialSnapshot()
        {
            return await _context.FinancialSnapshots.OrderByDescending(s => s.SnapshotDate).FirstOrDefaultAsync();
        }

        // --- Cash Balance Forecasting ---
        public async Task TrainCashBalanceModel()
        {
            var historicalData = await _context.FinancialSnapshots
                .OrderBy(s => s.SnapshotDate)
                .Select(s => new FinancialSnapshotData
                {
                    CashBalance = (float)s.CashBalance.GetValueOrDefault(),
                    Timestamp = (float)s.SnapshotDate.ToDateTime(TimeOnly.MinValue).ToOADate()
                })
                .ToListAsync();

            const int minimumHistoricalDataPoints = 10;
            if (historicalData.Count < minimumHistoricalDataPoints)
            {
                Console.WriteLine($"Insufficient historical financial snapshot data for Cash Balance training. Need at least {minimumHistoricalDataPoints} data points, but found {historicalData.Count}.");
                _cashBalanceModel = null;
                return;
            }

            var firstTimestamp = historicalData.Min(d => d.Timestamp);
            foreach (var dataPoint in historicalData) { dataPoint.Timestamp -= firstTimestamp; }

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(historicalData);
            int dynamicWindowSize = Math.Max(2, Math.Min(historicalData.Count / 2, 7));

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedCashBalance",
                inputColumnName: "CashBalance",
                windowSize: dynamicWindowSize,
                seriesLength: historicalData.Count,
                trainSize: historicalData.Count,
                horizon: 3,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundCashBalance",
                confidenceUpperBoundColumn: "UpperBoundCashBalance"
            );

            Console.WriteLine("Training ML.NET Cash Balance Forecasting Model...");
            _cashBalanceModel = forecastingPipeline.Fit(dataView);
            Console.WriteLine("ML.NET Cash Balance Forecasting Model Trained.");
        }

        public async Task<List<FinancialForecast>> PredictCashBalance(int numberOfForecasts = 3)
        {
            if (_cashBalanceModel == null)
            {
                await TrainCashBalanceModel();
                if (_cashBalanceModel == null)
                {
                    Console.WriteLine("ML.NET Cash Balance model not trained. Cannot make predictions.");
                    return new List<FinancialForecast>();
                }
            }

            var forecastEngine = _cashBalanceModel.CreateTimeSeriesEngine<FinancialSnapshotData, CashBalancePrediction>(_mlContext);
            var lastSnapshot = await _context.FinancialSnapshots.OrderByDescending(s => s.SnapshotDate).FirstOrDefaultAsync();

            if (lastSnapshot == null)
            {
                Console.WriteLine("No historical data to start Cash Balance forecasting from.");
                return new List<FinancialForecast>();
            }

            // Prepare input for Predict (not Forecast) - Forecast is deprecated and not recommended
            var input = new FinancialSnapshotData
            {
                CashBalance = (float)lastSnapshot.CashBalance.GetValueOrDefault(),
                Timestamp = (float)lastSnapshot.SnapshotDate.ToDateTime(TimeOnly.MinValue).ToOADate()
            };

            var prediction = forecastEngine.Predict();

            var forecasts = new List<FinancialForecast>();
            var currentForecastDate = lastSnapshot.SnapshotDate;

            for (int i = 0; i < numberOfForecasts; i++)
            {
                currentForecastDate = currentForecastDate.AddMonths(1);
                forecasts.Add(new FinancialForecast
                {
                    ForecastDate = currentForecastDate,
                    ForecastedValue = (decimal)prediction.ForecastedCashBalance[i],
                    LowerBound = (decimal)prediction.LowerBoundCashBalance[i],
                    UpperBound = (decimal)prediction.UpperBoundCashBalance[i],
                    ForecastDescription = $"Cash Balance Forecast (Month {i + 1})",
                    ForecastType = "CashBalance"
                });
            }

            await SaveForecastsToDatabase(forecasts, lastSnapshot.CompanyId, "CashBalance", "ML.NET SSA");
            return forecasts;
        }

        // Ensure that any method returning forecast data to the controller provides objects with Date and Value properties.
        // No direct changes needed here unless you are serializing to JSON directly in this service.

        // --- Material Cost Forecasting ---
        public async Task TrainMaterialCostModel()
        {
            var historicalData = await _context.FinancialSnapshots
                .OrderBy(s => s.SnapshotDate)
                .Select(s => new MaterialCostData
                {
                    MaterialCost = (float)s.MaterialCost.GetValueOrDefault(),
                    Timestamp = (float)s.SnapshotDate.ToDateTime(TimeOnly.MinValue).ToOADate()
                })
                .ToListAsync();

            const int minimumHistoricalDataPoints = 10;
            if (historicalData.Count < minimumHistoricalDataPoints)
            {
                Console.WriteLine($"Insufficient historical financial snapshot data for Material Cost training. Need at least {minimumHistoricalDataPoints} data points, but found {historicalData.Count}.");
                _materialCostModel = null;
                return;
            }

            var firstTimestamp = historicalData.Min(d => d.Timestamp);
            foreach (var dataPoint in historicalData) { dataPoint.Timestamp -= firstTimestamp; }

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(historicalData);
            int dynamicWindowSize = Math.Max(2, Math.Min(historicalData.Count / 2, 7));

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedMaterialCost",
                inputColumnName: "MaterialCost",
                windowSize: dynamicWindowSize,
                seriesLength: historicalData.Count,
                trainSize: historicalData.Count,
                horizon: 3,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundMaterialCost",
                confidenceUpperBoundColumn: "UpperBoundMaterialCost"
            );

            Console.WriteLine("Training ML.NET Material Cost Forecasting Model...");
            _materialCostModel = forecastingPipeline.Fit(dataView);
            Console.WriteLine("ML.NET Material Cost Forecasting Model Trained.");
        }

        public async Task<List<FinancialForecast>> PredictMaterialCost(int numberOfForecasts = 3)
        {
            if (_materialCostModel == null)
            {
                await TrainMaterialCostModel();
                if (_materialCostModel == null)
                {
                    Console.WriteLine("ML.NET Material Cost model not trained. Cannot make predictions.");
                    return new List<FinancialForecast>();
                }
            }

            var forecastEngine = _materialCostModel.CreateTimeSeriesEngine<MaterialCostData, MaterialCostPrediction>(_mlContext);
            var lastSnapshot = await _context.FinancialSnapshots.OrderByDescending(s => s.SnapshotDate).FirstOrDefaultAsync();

            if (lastSnapshot == null)
            {
                Console.WriteLine("No historical data to start Material Cost forecasting from.");
                return new List<FinancialForecast>();
            }

            var prediction = forecastEngine.Predict();

            var forecasts = new List<FinancialForecast>();
            var currentForecastDate = lastSnapshot.SnapshotDate;

            for (int i = 0; i < numberOfForecasts; i++)
            {
                currentForecastDate = currentForecastDate.AddMonths(1);
                forecasts.Add(new FinancialForecast
                {
                    ForecastDate = currentForecastDate,
                    ForecastedValue = (decimal)prediction.ForecastedMaterialCost[i],
                    LowerBound = (decimal)prediction.LowerBoundMaterialCost[i],
                    UpperBound = (decimal)prediction.UpperBoundMaterialCost[i],
                    ForecastDescription = $"Material Cost Forecast (Month {i + 1})",
                    ForecastType = "MaterialCost"
                });
            }

            await SaveForecastsToDatabase(forecasts, lastSnapshot.CompanyId, "MaterialCost", "ML.NET SSA");
            return forecasts;
        }

        // --- Equipment Cost Forecasting ---
        public async Task TrainEquipmentCostModel()
        {
            var historicalData = await _context.FinancialSnapshots
                .OrderBy(s => s.SnapshotDate)
                .Select(s => new EquipmentCostData
                {
                    EquipmentCost = (float)s.EquipmentCost.GetValueOrDefault(),
                    Timestamp = (float)s.SnapshotDate.ToDateTime(TimeOnly.MinValue).ToOADate()
                })
                .ToListAsync();

            const int minimumHistoricalDataPoints = 10;
            if (historicalData.Count < minimumHistoricalDataPoints)
            {
                Console.WriteLine($"Insufficient historical financial snapshot data for Equipment Cost training. Need at least {minimumHistoricalDataPoints} data points, but found {historicalData.Count}.");
                _equipmentCostModel = null;
                return;
            }

            var firstTimestamp = historicalData.Min(d => d.Timestamp);
            foreach (var dataPoint in historicalData) { dataPoint.Timestamp -= firstTimestamp; }

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(historicalData);
            int dynamicWindowSize = Math.Max(2, Math.Min(historicalData.Count / 2, 7));

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedEquipmentCost",
                inputColumnName: "EquipmentCost",
                windowSize: dynamicWindowSize,
                seriesLength: historicalData.Count,
                trainSize: historicalData.Count,
                horizon: 3,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundEquipmentCost",
                confidenceUpperBoundColumn: "UpperBoundEquipmentCost"
            );

            Console.WriteLine("Training ML.NET Equipment Cost Forecasting Model...");
            _equipmentCostModel = forecastingPipeline.Fit(dataView);
            Console.WriteLine("ML.NET Equipment Cost Forecasting Model Trained.");
        }

        public async Task<List<FinancialForecast>> PredictEquipmentCost(int numberOfForecasts = 3)
        {
            if (_equipmentCostModel == null)
            {
                await TrainEquipmentCostModel();
                if (_equipmentCostModel == null)
                {
                    Console.WriteLine("ML.NET Equipment Cost model not trained. Cannot make predictions.");
                    return new List<FinancialForecast>();
                }
            }

            var forecastEngine = _equipmentCostModel.CreateTimeSeriesEngine<EquipmentCostData, EquipmentCostPrediction>(_mlContext);
            var lastSnapshot = await _context.FinancialSnapshots.OrderByDescending(s => s.SnapshotDate).FirstOrDefaultAsync();

            if (lastSnapshot == null)
            {
                Console.WriteLine("No historical data to start Equipment Cost forecasting from.");
                return new List<FinancialForecast>();
            }

            var prediction = forecastEngine.Predict();

            var forecasts = new List<FinancialForecast>();
            var currentForecastDate = lastSnapshot.SnapshotDate;

            for (int i = 0; i < numberOfForecasts; i++)
            {
                currentForecastDate = currentForecastDate.AddMonths(1);
                forecasts.Add(new FinancialForecast
                {
                    ForecastDate = currentForecastDate,
                    ForecastedValue = (decimal)prediction.ForecastedEquipmentCost[i],
                    LowerBound = (decimal)prediction.LowerBoundEquipmentCost[i],
                    UpperBound = (decimal)prediction.UpperBoundEquipmentCost[i],
                    ForecastDescription = $"Equipment Cost Forecast (Month {i + 1})",
                    ForecastType = "EquipmentCost"
                });
            }

            await SaveForecastsToDatabase(forecasts, lastSnapshot.CompanyId, "EquipmentCost", "ML.NET SSA");
            return forecasts;
        }

        // --- What-If Scenario Calculation ---
        public async Task<WhatIfScenarioResult> CalculateWhatIfCostReduction(
            string costType,
            decimal reductionPercentage,
            int forecastMonths = 3)
        {
            await PredictCashBalance(forecastMonths);
            await PredictMaterialCost(forecastMonths);
            await PredictEquipmentCost(forecastMonths);

            var cashForecasts = await _context.ForecastedFinancials
                .Where(f => f.ForecastType == "CashBalance")
                .OrderBy(f => f.TargetDate)
                .Take(forecastMonths)
                .ToListAsync();

            var materialCostForecasts = await _context.ForecastedFinancials
                .Where(f => f.ForecastType == "MaterialCost")
                .OrderBy(f => f.TargetDate)
                .Take(forecastMonths)
                .ToListAsync();

            var equipmentCostForecasts = await _context.ForecastedFinancials
                .Where(f => f.ForecastType == "EquipmentCost")
                .OrderBy(f => f.TargetDate)
                .Take(forecastMonths)
                .ToListAsync();

            var lastSnapshot = await GetLastFinancialSnapshot();
            if (lastSnapshot == null)
            {
                Console.WriteLine("No historical financial data available for what-if analysis baseline.");
                return null;
            }

            var results = new List<WhatIfMonthResult>();
            decimal currentCashBalanceCumulative = lastSnapshot.CashBalance.GetValueOrDefault();

            for (int i = 0; i < forecastMonths; i++)
            {
                var targetMonthDate = lastSnapshot.SnapshotDate.AddMonths(i + 1);

                var cashForecastForMonth = cashForecasts.FirstOrDefault(f => f.TargetDate == targetMonthDate);
                var materialForecastForMonth = materialCostForecasts.FirstOrDefault(f => f.TargetDate == targetMonthDate);
                var equipmentForecastForMonth = equipmentCostForecasts.FirstOrDefault(f => f.TargetDate == targetMonthDate);

                decimal originalForecastedCashBalance = cashForecastForMonth?.ForecastedValue ?? lastSnapshot.CashBalance.GetValueOrDefault();
                decimal originalForecastedMaterialCost = materialForecastForMonth?.ForecastedValue ?? lastSnapshot.MaterialCost.GetValueOrDefault();
                decimal originalForecastedEquipmentCost = equipmentForecastForMonth?.ForecastedValue ?? lastSnapshot.EquipmentCost.GetValueOrDefault();

                decimal forecastedPayrollCost = lastSnapshot.PayrollCost.GetValueOrDefault();
                decimal forecastedOtherExpenses = lastSnapshot.TotalExpenses.GetValueOrDefault() - (lastSnapshot.MaterialCost.GetValueOrDefault() + lastSnapshot.EquipmentCost.GetValueOrDefault() + lastSnapshot.PayrollCost.GetValueOrDefault());
                if (forecastedOtherExpenses < 0) forecastedOtherExpenses = 0;

                decimal adjustedMaterialCost = originalForecastedMaterialCost;
                decimal adjustedEquipmentCost = originalForecastedEquipmentCost;

                if (costType.Equals("MaterialCost", StringComparison.OrdinalIgnoreCase))
                {
                    adjustedMaterialCost = originalForecastedMaterialCost * (1 - (reductionPercentage / 100));
                }
                else if (costType.Equals("EquipmentCost", StringComparison.OrdinalIgnoreCase))
                {
                    adjustedEquipmentCost = originalForecastedEquipmentCost * (1 - (reductionPercentage / 100));
                }

                decimal forecastedRevenue = lastSnapshot.TotalRevenue.GetValueOrDefault();

                decimal originalTotalExpenses = originalForecastedMaterialCost + originalForecastedEquipmentCost + forecastedPayrollCost + forecastedOtherExpenses;
                decimal originalNetProfit = forecastedRevenue - originalTotalExpenses;

                decimal adjustedTotalExpenses = adjustedMaterialCost + adjustedEquipmentCost + forecastedPayrollCost + forecastedOtherExpenses;
                decimal adjustedNetProfit = forecastedRevenue - adjustedTotalExpenses;

                decimal netProfitImprovement = adjustedNetProfit - originalNetProfit;

                decimal adjustedCashBalance = originalForecastedCashBalance + netProfitImprovement;

                results.Add(new WhatIfMonthResult
                {
                    Month = targetMonthDate,
                    OriginalValue = costType.Equals("MaterialCost", StringComparison.OrdinalIgnoreCase) ? originalForecastedMaterialCost : originalForecastedEquipmentCost,
                    AdjustedValue = costType.Equals("MaterialCost", StringComparison.OrdinalIgnoreCase) ? adjustedMaterialCost : adjustedEquipmentCost,
                    OriginalCashBalance = originalForecastedCashBalance,
                    AdjustedCashBalance = adjustedCashBalance,
                    OriginalNetProfit = originalNetProfit,
                    AdjustedNetProfit = adjustedNetProfit
                });
            }

            return new WhatIfScenarioResult
            {
                ScenarioDescription = $"What-If: {reductionPercentage}% reduction in {costType} for {forecastMonths} months.",
                Results = results
            };
        }

        // Save the generated forecasts to the ForecastedFinancial table
        private async Task SaveForecastsToDatabase(List<FinancialForecast> forecasts, int companyId, string forecastType, string modelUsed)
        {
            if (forecasts == null || !forecasts.Any())
            {
                Console.WriteLine($"No {forecastType} forecasts to save.");
                return;
            }

            var existingForecasts = await _context.ForecastedFinancials
                .Where(f => f.CompanyId == companyId && f.ForecastType == forecastType)
                .ToListAsync();
            _context.ForecastedFinancials.RemoveRange(existingForecasts);
            await _context.SaveChangesAsync();

            foreach (var forecast in forecasts)
            {
                var forecastedFinancial = new ForecastedFinancial
                {
                    ForecastDate = DateOnly.FromDateTime(DateTime.Today),
                    TargetDate = forecast.ForecastDate,
                    CompanyId = companyId,
                    ProjectId = null,
                    ForecastType = forecastType,
                    ForecastedValue = forecast.ForecastedValue,
                    ConfidenceIntervalLow = forecast.LowerBound,
                    ConfidenceIntervalHigh = forecast.UpperBound,
                    ModelUsed = modelUsed
                };
                _context.ForecastedFinancials.Add(forecastedFinancial);
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Successfully saved {forecasts.Count} {forecastType} forecasts to the database.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving {forecastType} forecasts to database: {ex.Message}");
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}