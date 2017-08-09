using System.Collections.Generic;
using Securities;

namespace FindPerfectSellPoints
{
    public class HeuristicTester : ITester
    {
        private readonly IPrice[] _prices;
        private readonly int _beginIndex;
        private readonly int _endIndex;
        private readonly double _transactionFee;

        public HeuristicTester(IPrice[] prices, int beginIndex, int endIndex, double transactionFee)
        {
            _prices         = prices;
            _beginIndex     = beginIndex;
            _endIndex       = endIndex;
            _transactionFee = transactionFee;
        }

        public TradeSet Analyze(double initialCapital)
        {
            int initialIndex = FindInitialMinima(_prices, _beginIndex, _endIndex);
            var tradeSet     = new TradeSet(initialCapital, new List<ITrade>(), 0);

            AddTrades(tradeSet, initialIndex);
            FinalizeTrades(tradeSet);

            return tradeSet;
        }

        public static int FindInitialMinima(IPrice[] prices, int beginIndex, int endIndex)
        {
            int minimaIndex    = beginIndex;
            double minimaClose = prices[beginIndex].Close;

            for (int index = beginIndex + 1; index <= endIndex; index++)
            {
                var price = prices[index];
                if (price.Close > minimaClose) break;

                minimaIndex = index;
                minimaClose = price.Close;
            }

            return minimaIndex;
        }

        private void AddTrades(TradeSet tradeSet, int index)
        {
            var price = _prices[index];
            tradeSet.Buy(price, price.Close, _transactionFee);
            bool sellShares = true;
            ////index++;

            while (index <= _endIndex)
            {
                int nextIndex = sellShares ? GetNextSellIndex(index) : GetNextBuyIndex(index);
                if (nextIndex == -1) break;

                price = _prices[nextIndex];
                if (sellShares) tradeSet.Sell(price, price.Close, _transactionFee);
                else tradeSet.Buy(price, price.Close, _transactionFee);

                sellShares = !sellShares;
                index = nextIndex;
            }
        }

        private int GetNextBuyIndex(int index)
        {
            var originalPrice = _prices[index].Close;

            index += 4;
            int originalIndex = index;

            while (index < _endIndex)
            {
                if (LowestPriceInDays(index, 5) && _prices[index].Close < originalPrice) break;
                index++;
            }

            return index == originalIndex ? -1 : index;
        }

        private bool LowestPriceInDays(int index, int numAdditionalDays)
        {
            double closePrice = _prices[index].Close;
            bool isLowest = true;

            for (int numDays = 1; numDays <= numAdditionalDays; numDays++)
            {
                var nextIndex = index + numDays;

                if (nextIndex < _endIndex && _prices[nextIndex].Close < closePrice)
                {
                    isLowest = false;
                    break;
                }
            }

            return isLowest;
        }

        private int GetNextSellIndex(int index)
        {
            var originalPrice = _prices[index].Close;

            index += 4;
            int originalIndex = index;

            while (index < _endIndex)
            {
                if (HighestPriceInDays(index, 5) && _prices[index].Close > originalPrice) break;
                index++;
            }

            return index == originalIndex ? -1 : index;
        }

        private bool HighestPriceInDays(int index, int numAdditionalDays)
        {
            double closePrice = _prices[index].Close;
            bool isHighest    = true;

            for (int numDays = 1; numDays <= numAdditionalDays; numDays++)
            {
                var nextIndex = index + numDays;

                if (nextIndex < _endIndex && _prices[nextIndex].Close > closePrice)
                {
                    isHighest = false;
                    break;
                }
            }

            return isHighest;
        }

        private void FinalizeTrades(TradeSet tradeSet)
        {
            var finalPrice = GetFinalPrice();
            tradeSet.Sell(finalPrice, finalPrice.Close, _transactionFee);
        }

        private IPrice GetFinalPrice()
        {
            var lastPrice = _prices[_endIndex];
            var finalDate = lastPrice.Date.AddDays(1);
            return new Price(finalDate, lastPrice.Close, lastPrice.Close, lastPrice.Close, lastPrice.Close);
        }
    }
}
