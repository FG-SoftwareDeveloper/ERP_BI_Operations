using ERP_BI_Operations.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP_BI_Operations.Services
{
    public class ForecastingService(ERP_BIContext dbContext)// Primary constructor for dependency injection
    {
       /* private readonly MLContext _mlContext = new MLContext();
        private readonly ERP_BIContext _dbContext = dbContext;
        private ITransformer _forecastingModel; // This holds the trained ML model
        private readonly string _modelPath = "account_balance_forecast_model.zip"; // Path to save/load the model
       
        /// <summary>
        /// Attempts to load an existing ML model from disk. If not found or loading fails,
        /// it triggers the training and saving of a new model.
        /// </summary>
        public async Task LoadOrCreateForecastingModel()
        {
            Console.WriteLine($"[ForecastingService] Attempting to load or create model. Model path: {_modelPath}");
            if (File.Exists(_modelPath))
            {
                Console.WriteLine($"[ForecastingService] Loading existing ML model from {_modelPath}");
                try
                {
                    // Load the model from the .zip file
                    _forecastingModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);
                    Console.WriteLine("[ForecastingService] Model loaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[ForecastingService] Error loading model: {ex.Message}. Attempting to retrain.");
                    // If loading fails (e.g., schema mismatch, corrupted file), retrain
                    await TrainAndSaveForecastingModel();
                }
            }
            else
            {
                Console.WriteLine("[ForecastingService] No existing model found. Training new ML model...");
                await TrainAndSaveForecastingModel();
            }
        }

        /// <summary>
        /// Trains a new time series forecasting model using historical account balance data.
        /// If insufficient data is found in the database, it generates dummy data for training.
        /// The trained model is then saved to a .zip file.
        /// </summary>
        public async Task TrainAndSaveForecastingModel()
        {
            int targetAccountId = 1; // The AccountId for which to train the model.
                                     // In a more advanced scenario, you might train multiple models or
                                     // use a key column in ML.NET if forecasting multiple series.

            Console.WriteLine($"[ForecastingService] Starting TrainAndSaveForecastingModel for Account ID: {targetAccountId}");

            // 1. Load historical data from the AccountHistory table
            // Ensure AccountHistory table is populated with sufficient data (e.g., 60-120 months)
            var historicalData = await _dbContext.AccountHistories
                .Where(ah => ah.AccountId == targetAccountId)
                .OrderBy(ah => ah.DateRecorded)
                .Select(ah => new AccountDailyBalance { Date = ah.DateRecorded, Balance = (float)ah.Balance })
                .ToListAsync();

            Console.WriteLine($"[ForecastingService] Queried from AccountHistory. Count: {historicalData.Count}");

            // If insufficient historical data is found in the database, generate dummy data for training.
            // ML.NET's SSA algorithm requires the input size for training to be greater than twice the window size.
            // With windowSize: 12, we need > 24 records. We aim for 60-120 for better training.
            if (historicalData.Count < 60)
            {
                Console.WriteLine($"[ForecastingService] Insufficient historical data found in DB ({historicalData.Count}). Generating additional dummy data for training.");
                historicalData.Clear(); // Clear any partial data from the above query

                var startDate = new DateTime(2015, 1, 1); // Start 10 years ago for 120 months of data
                var random = new Random();
                float baseBalance = 100000f; // Starting balance for dummy data

                for (int i = 0; i < 120; i++) // Generate 120 months of data (10 years)
                {
                    // Simulate monthly fluctuations and a slight trend
                    float monthlyChange = (float)(random.NextDouble() * 5000 - 2500); // +/- 2500
                    if (i > 0)
                    {
                        baseBalance += monthlyChange;
                    }
                    // Ensure balance doesn't go too low for demonstration
                    baseBalance = Math.Max(baseBalance, 50000f);

                    historicalData.Add(new AccountDailyBalance
                    {
                        Date = startDate.AddMonths(i),
                        Balance = baseBalance
                    });
                }
                Console.WriteLine($"[ForecastingService] Generated {historicalData.Count} dummy historical data points for training.");
            }
            else
            {
                Console.WriteLine($"[ForecastingService] Using {historicalData.Count} existing historical data points from DB for training.");
            }

            // Ensure data is sorted by date (critical for time series models)
            historicalData = historicalData.OrderBy(d => d.Date).ToList();
            Console.WriteLine($"[ForecastingService] Final historicalData count before training: {historicalData.Count}");

            // Final check for minimum data requirement before training
            if (historicalData.Count < 25) // Minimum required for windowSize=12 is > 24
            {
                Console.Error.WriteLine($"[ForecastingService] ERROR: Still not enough data for training! Required > 24, but got {historicalData.Count}.");
                throw new InvalidOperationException("The input size for training should be greater than twice the window size.");
            }

            // Load the historical data into an IDataView for ML.NET
            IDataView dataView = _mlContext.Data.LoadFromEnumerable(historicalData);

            // 2. Define the forecasting pipeline using SSA (Singular Spectrum Analysis)
            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(AccountBalancePrediction.ForecastedBalance),
                inputColumnName: nameof(AccountDailyBalance.Balance),
                windowSize: 12, // Number of samples in a window (e.g., 12 for monthly seasonality)
                seriesLength: historicalData.Count, // Total length of the time series for pattern detection
                trainSize: historicalData.Count, // Number of samples to train on (all available historical data)
                horizon: 1, // This is a placeholder; the actual forecast horizon is set during prediction
                confidenceLevel: 0.95f, // <-- This enables confidence intervals
                confidenceLowerBoundColumn: nameof(AccountBalancePrediction.LowerBound), // <-- Maps to LowerBound property
                confidenceUpperBoundColumn: nameof(AccountBalancePrediction.UpperBound)  // <-- Maps to UpperBound property
            );

            // 3. Train the model
            _forecastingModel = forecastingPipeline.Fit(dataView);

            // 4. Save the trained model to a .zip file for later use
            _mlContext.Model.Save(_forecastingModel, dataView.Schema, _modelPath);
            Console.WriteLine($"[ForecastingService] ML model trained and saved to {_modelPath}");
        }

        /// <summary>
        /// Generates a forecast for a specific account's balance for a given horizon.
        /// </summary>
        /// <param name="accountId">The ID of the account to forecast.</param>
        /// <param name="horizon">The number of future periods (months) to forecast.</param>
        /// <returns>An AccountBalancePrediction object containing forecasted values and confidence intervals.</returns>
        public AccountBalancePrediction ForecastAccountBalance(int accountId, int horizon)
        {
            // Ensure the forecasting model is loaded or trained before attempting to use it.
            if (_forecastingModel == null)
            {
                Console.Error.WriteLine("[ForecastingService] Model not loaded. Attempting to load/train on demand.");
                LoadOrCreateForecastingModel().Wait();
                if (_forecastingModel == null)
                {
                    Console.Error.WriteLine("[ForecastingService] Model still not available after on-demand load/train.");
                    return null;
                }
            }

            // Create a TimeSeriesPredictionEngine from the trained model, passing MLContext.
            var timeSeriesPredictionEngine = _forecastingModel.CreateTimeSeriesEngine<AccountDailyBalance, AccountBalancePrediction>(_mlContext);

            // The Predict method on the time series engine takes the horizon directly.
            // It does not require an input object in the same way as a standard Predict method.
            var forecast = timeSeriesPredictionEngine.Predict(horizon: horizon);

            Console.WriteLine($"[ForecastingService] Generated forecast for {horizon} periods for Account ID: {accountId}.");
            return forecast;
        }
    }

    // Ensure these models are defined in your Models folder or accessible namespace
    // (These were already provided in previous turns, included here for completeness)
    public class AccountDailyBalance
    {
        // Removed [LoadColumn] attributes as they are not needed when using LoadFromEnumerable
        public DateTime Date { get; set; }
        public float Balance { get; set; }
    }

    public class AccountBalancePrediction
    {
        [ColumnName("ForecastedBalance")] // The name of the output column from the ML model
        public float[] ForecastedBalance { get; set; }

        [ColumnName("LowerBound")] // Lower bound of the prediction confidence interval
        public float[] LowerBound { get; set; }

        [ColumnName("UpperBound")] // Upper bound of the prediction confidence interval
        public float[] UpperBound { get; set; }
    }*/
       }
}
