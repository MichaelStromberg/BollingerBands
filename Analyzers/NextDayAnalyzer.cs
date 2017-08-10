using Analyzers.Common;
using Securities;

namespace Analyzers
{
    public class NextDayAnalyzer : IAnalyzer
    {
        private readonly ISecurity _security;
        private readonly double _transactionFee;
        private readonly int _numDaysUntilSettlement;

        public NextDayAnalyzer(ISecurity security, double transactionFee, int numDaysUntilSettlement)
        {
            _security               = security;
            _transactionFee         = transactionFee;
            _numDaysUntilSettlement = numDaysUntilSettlement;
        }

        public void CalculatePerformanceResults(Parameters parameters, double startCapital, bool showOutput = false)
        {
            var totalTicks = _security.Prices[_security.Prices.Length - 1].Date.Ticks - _security.Prices[0].Date.Ticks;
            var state      = new AnalyzerState(parameters, _transactionFee, startCapital, totalTicks, showOutput);
            Order order    = null;

            foreach (var price in _security.Prices)
            {
                if (order == null) order = CreateOrder(state, price);
                if (order != null) HandleOrder(state, price, order);
            }

            if (showOutput) state.ShowFinalBollingerBand();

            parameters.UpdateResults(state);
        }

        private void HandleOrder(AnalyzerState state, IPrice price, Order order)
        {

            throw new System.NotImplementedException();
        }

        private Order CreateOrder(AnalyzerState state, IPrice price)
        {
            //var order = state.CreateSellOrder(price);
            //if (order != null) return order;

            //state.SellShares(price);
            //state.BuyShares(price, _numDaysUntilSettlement);

            //state.BollingerBand.Recalculate(price);
            return null;
        }
    }
}
