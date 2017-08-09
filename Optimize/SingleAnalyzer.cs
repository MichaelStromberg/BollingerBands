using System;
using Securities;

namespace Optimize
{
    public class SingleAnalyzer : IAnalyzer
    {
        private readonly ISecurity _security;
        private readonly double _transactionFee;
        private readonly int _numDaysUntilSettlement;

        public SingleAnalyzer(ISecurity security, double transactionFee, int numDaysUntilSettlement)
        {
            _security               = security;
            _transactionFee         = transactionFee;
            _numDaysUntilSettlement = numDaysUntilSettlement;
        }

        public void CalculatePerformanceResults(Parameters parameters, double startCapital, bool showOutput = false)
        {
            var totalTicks = _security.Prices[_security.Prices.Length - 1].Date.Ticks - _security.Prices[0].Date.Ticks;
            var state      = new AnalyzerState(parameters, _transactionFee, startCapital, totalTicks, showOutput);

            foreach (var price in _security.Prices)
            {
                if (state.BollingerBand.IsCalculated)
                {
                    state.SellShares(price);
                    state.BuyShares(price, _numDaysUntilSettlement);
                }

                state.BollingerBand.Recalculate(price);
            }

            if (showOutput) state.ShowFinalBollingerBand();

            parameters.UpdateResults(state);
        }
    }
}
