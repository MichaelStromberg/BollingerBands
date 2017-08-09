namespace Optimize
{
    public class Parameters
    {
        public double AnnualizedRateOfReturn;
        public double Profit;

        public int NumPeriods;
        public double NumStddevs;
        public double BuyTargetPercent;
        public double SellTargetPercent;

        public Parameters()
        {
            AnnualizedRateOfReturn = -500.0;
            Profit                 = 0.0;
        }

        public Parameters(ParameterRange range) : this()
        {
            NumPeriods        = range.NumPeriods.Begin;
            NumStddevs        = range.NumStddevs.Begin;
            BuyTargetPercent  = range.BuyTargetPercent.Begin;
            SellTargetPercent = range.SellTargetPercent.Begin;
        }

        /// <summary>
        /// clones the given parameters object
        /// </summary>
        public void Update(Parameters other)
        {
            AnnualizedRateOfReturn = other.AnnualizedRateOfReturn;
            Profit                 = other.Profit;
            NumPeriods             = other.NumPeriods;
            NumStddevs             = other.NumStddevs;
            BuyTargetPercent       = other.BuyTargetPercent;
            SellTargetPercent      = other.SellTargetPercent;
        }

        public override string ToString()
        {
            return $"Best annualized rate of return: {AnnualizedRateOfReturn:0.00} % with {NumPeriods} periods {NumStddevs:0.0000} stddevs. Buy: {BuyTargetPercent * 100.0:0.0} %, Sell: {SellTargetPercent * 100.0:0.0} %, Profit: {Profit:C}";
        }
    }
}
