using System.Collections.Generic;
using Securities;

namespace FindPerfectSellPoints
{
    public class DeepTester : ITester
    {
        private readonly IPrice[] _prices;
        private readonly int _beginIndex;
        private readonly int _endIndex;
        private readonly double _transactionFee;

        private TradeSet _bestTradeSet = new TradeSet(double.MinValue, null, 0);

        public DeepTester(IPrice[] prices, int beginIndex, int endIndex, double transactionFee)
        {
            _prices         = prices;
            _beginIndex     = beginIndex;
            _endIndex       = endIndex;
            _transactionFee = transactionFee;
        }

        public TradeSet Analyze(double initialCapital)
        {
            var trades = new List<ITrade>();

            for (int index = _beginIndex; index <= _endIndex; index++)
                Buy(index, new TradeSet(initialCapital, trades, 0));

            return _bestTradeSet;
        }

        public void Buy(int priceIndex, TradeSet tradeSet)
        {
            var purchasePrice = _prices[priceIndex].Close;
            tradeSet.Buy(_prices[priceIndex], purchasePrice, _transactionFee);

            for (int index = priceIndex + 1; index <= _endIndex; index++)
                if (_prices[index].Close > purchasePrice) Sell(index, tradeSet.DeepCopy());

            FinalizeTrades(tradeSet);
        }

        private void Sell(int priceIndex, TradeSet tradeSet)
        {
            var salePrice = _prices[priceIndex].Close;
            tradeSet.Sell(_prices[priceIndex], salePrice, _transactionFee);

            for (int index = priceIndex + 1; index <= _endIndex; index++)
                if (_prices[index].Close < salePrice) Buy(index, tradeSet.DeepCopy());

            FinalizeTrades(tradeSet);
        }

        private void FinalizeTrades(TradeSet tradeSet)
        {
            var finalPrice = GetFinalPrice();
            tradeSet.Sell(finalPrice, finalPrice.Close, _transactionFee);
            if (tradeSet.Balance > _bestTradeSet.Balance) _bestTradeSet = tradeSet;
        }

        private IPrice GetFinalPrice()
        {
            var lastPrice = _prices[_endIndex];
            var finalDate = lastPrice.Date.AddDays(1);
            return new Price(finalDate, lastPrice.Close, lastPrice.Close, lastPrice.Close, lastPrice.Close);
        }
    }
}
