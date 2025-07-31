using Microsoft.ML.Data;

namespace ERP_BI_Operations.Models
{
    public class CashBalancePrediction
    {
        // This is the predicted value for CashBalance.
        // The name "ForecastedCashBalance" must match the name of the output column from the ML model.
        [ColumnName("ForecastedCashBalance")]
        public float[] ForecastedCashBalance { get; set; }

        // This is the lower bound of the prediction interval
        [ColumnName("LowerBoundCashBalance")]
        public float[] LowerBoundCashBalance { get; set; }

        // This is the upper bound of the prediction interval
        [ColumnName("UpperBoundCashBalance")]
        public float[] UpperBoundCashBalance { get; set; }
    }
}
