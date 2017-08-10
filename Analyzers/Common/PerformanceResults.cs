namespace Analyzers.Common
{
    public class PerformanceResults
    {
        public readonly double AnnualizedRateOfReturn;
        public readonly double Profit;
        public readonly double TradeSpanPercentage;
        public readonly double WeightedProfit;

        public PerformanceResults(double annualizedRateOfReturn = -500.0, double profit = -500.0,
            double tradeSpanPercentage = 0.0)
        {
            AnnualizedRateOfReturn = annualizedRateOfReturn;
            Profit                 = profit;
            TradeSpanPercentage    = tradeSpanPercentage;
            WeightedProfit         = profit * tradeSpanPercentage;
        }
    }
}
