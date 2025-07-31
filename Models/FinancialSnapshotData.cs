using Microsoft.ML.Data;
using System;

namespace ERP_BI_Operations.Models
{
    // Input data for ML.NET model (historical financial snapshots)
    public class FinancialSnapshotData
    {
        // This property will be the value we want to predict (CashBalance)
        // It must be a float for ML.NET time series components.
        [LoadColumn(0)] // Represents the first column in our loaded data
        public float CashBalance { get; set; }

        // This property represents the time series index.
        // It's not directly used in the prediction but helps ML.NET understand the sequence.
        // We'll use the SnapshotDate to create a numerical index for ML.NET.
        [LoadColumn(1)] // Represents the second column
        public float Timestamp { get; set; } // A numerical representation of time
    }

    
 
}
