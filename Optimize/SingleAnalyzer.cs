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

        public void CalculatePerformanceResults(Parameters parameters, double startCapital)
        {
            var totalTicks = _security.Prices[_security.Prices.Length - 1].Date.Ticks - _security.Prices[0].Date.Ticks;
            var state      = new AnalyzerState(parameters, _transactionFee, startCapital, totalTicks);

            foreach (var price in _security.Prices)
            {
                if (state.BollingerBand.IsCalculated)
                {
                    state.SellShares(price);
                    state.BuyShares(price);
                }

                state.BollingerBand.Recalculate(price);
            }

            parameters.UpdateResults(state);
        }
    }
}
