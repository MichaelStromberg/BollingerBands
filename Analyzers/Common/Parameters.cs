using System.IO;

namespace Analyzers.Common
{
    public class Parameters
    {
        public PerformanceResults Results { get; private set; }

        public int NumPeriods;
        public double NumStddevs;
        public double BuyTargetPercent;
        public double SellTargetPercent;

        public Parameters(ParameterRange range)
        {
            NumPeriods        = range.NumPeriods.Begin;
            NumStddevs        = range.NumStddevs.Begin;
            BuyTargetPercent  = range.BuyTargetPercent.Begin;
            SellTargetPercent = range.SellTargetPercent.Begin;
            Results           = new PerformanceResults();
        }

        /// <summary>
        /// clones the given parameters object
        /// </summary>
        public void Update(Parameters other)
        {
            NumPeriods        = other.NumPeriods;
            NumStddevs        = other.NumStddevs;
            BuyTargetPercent  = other.BuyTargetPercent;
            SellTargetPercent = other.SellTargetPercent;
            Results           = other.Results;
        }

        public override string ToString()
        {
            return $"Best annualized rate of return: {Results.AnnualizedRateOfReturn * 100.0:0.00} % with {NumPeriods} periods {NumStddevs:0.0000} stddevs. Buy: {BuyTargetPercent * 100.0:0.000} %, Sell: {SellTargetPercent * 100.0:0.000} %, Profit: {Results.Profit:C}, Trade Span: {Results.TradeSpanPercentage * 100.0:0.00} %";
        }

        public void UpdateResults(IAnalyzerState state) => Results = state.GetPerformanceResults();

        public void Write(BinaryWriter writer)
        {
            writer.Write(NumPeriods);
            writer.Write(NumStddevs);
            writer.Write(BuyTargetPercent);
            writer.Write(SellTargetPercent);
        }
    }
}
