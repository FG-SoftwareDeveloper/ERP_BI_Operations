namespace ERP_BI_Operations.Models
{
    public class AccountBalancePrediction
    {
        // The predicted values for the future steps
        public float[] ForecastedBalance { get; set; }

        // The lower and upper bounds for the confidence interval of the prediction
        public float[] LowerBound { get; set; }
        public float[] UpperBound { get; set; }
    }
}
