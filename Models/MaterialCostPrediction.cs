namespace ERP_BI_Operations.Models
{
    public class MaterialCostPrediction
    {

        // This array will hold the forecasted material cost values for the prediction horizon
        public float[] ForecastedMaterialCost { get; set; }

        // These arrays will hold the lower and upper bounds of the confidence interval
        public float[] LowerBoundMaterialCost { get; set; }
        public float[] UpperBoundMaterialCost { get; set; }
    }
}
