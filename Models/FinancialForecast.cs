using System;
using System.Collections.Generic;

namespace ERP_BI_Operations.Models // This is the shared namespace for your DTOs
{
    // DTO for general financial forecasts (Cash Balance, Material Cost, Equipment Cost)
    public class FinancialForecast
    {
        public DateOnly ForecastDate { get; set; }
        public decimal ForecastedValue { get; set; } // Generic for cash, material, equipment
        public decimal LowerBound { get; set; }
        public decimal UpperBound { get; set; }
        public string ForecastDescription { get; set; }
        public string ForecastType { get; set; } // e.g., "CashBalance", "MaterialCost"
    }

    // DTO for What-If Scenario Results
    public class WhatIfScenarioResult
    {
        public string ScenarioDescription { get; set; }
        public List<WhatIfMonthResult> Results { get; set; }
    }

    // DTO for individual month results within a What-If Scenario
    public class WhatIfMonthResult
    {
        public DateOnly Month { get; set; }
        public decimal OriginalValue { get; set; } // Original forecasted value for the adjusted cost type
        public decimal AdjustedValue { get; set; } // Adjusted value for the cost type
        public decimal OriginalCashBalance { get; set; } // Cash balance before adjustment
        public decimal AdjustedCashBalance { get; set; } // Cash balance after adjustment
        public decimal OriginalNetProfit { get; set; } // Net profit before adjustment
        public decimal AdjustedNetProfit { get; set; } // Net profit after adjustment
    }
}
