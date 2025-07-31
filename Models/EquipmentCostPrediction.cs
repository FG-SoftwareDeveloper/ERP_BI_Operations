namespace ERP_BI_Operations.Models
{
    public class EquipmentCostPrediction
    {
        // This array will hold the forecasted equipment cost values for the prediction horizon
        public float[] ForecastedEquipmentCost { get; set; }

        // These arrays will hold the lower and upper bounds of the confidence interval
        public float[] LowerBoundEquipmentCost { get; set; }
        public float[] UpperBoundEquipmentCost { get; set; }
    }
}
