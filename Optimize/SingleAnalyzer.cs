using Securities;

namespace Optimize
{
    public class SingleAnalyzer : IAnalyzer
    {
        private readonly ISecurity _security;
        private readonly double _transactionFee;

        public SingleAnalyzer(ISecurity security, double transactionFee)
        {
            _security       = security;
            _transactionFee = transactionFee;
        }

        /// <summary>
        /// returns the annualized rate of return given a security and start capital
        /// with optimal Bollinger Bands
        /// </summary>
        public void SetAnnualizedRateOfReturn(Parameters parameters, double startCapital)
        {
            var state = new AnalyzerState(parameters, _transactionFee, startCapital);

            foreach (var price in _security.Prices)
            {
                if (state.BollingerBand.IsCalculated)
                {
                    state.SellShares(price);
                    state.BuyShares(price);
                }

                state.BollingerBand.Recalculate(price);
            }

            parameters.Profit = state.GetProfit();
            parameters.AnnualizedRateOfReturn = state.GetAnnualizedRateOfReturn(parameters.Profit);
        }
    }
}
