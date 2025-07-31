namespace ERP_BI_Operations.Models
{
    // This model represents a single data point in your time series
    public class AccountDailyBalance
    {
        // [LoadColumn(0)] // Removed
        public DateTime Date { get; set; }

        // [LoadColumn(1)] // Removed
        public float Balance { get; set; }
    }

}
