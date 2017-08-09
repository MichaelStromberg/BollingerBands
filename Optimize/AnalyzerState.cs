using System;
using Securities;

namespace Optimize
{
    public class AnalyzerState
    {
        public readonly BollingerBand BollingerBand;
        private readonly double _transactionFee;

        private readonly Transaction _firstPurchase = new Transaction();
        private readonly Transaction _lastSale      = new Transaction();
        private readonly Parameters _parameters;
        private readonly double _initialBalance;

        private bool _hasPendingSellTransaction;
        private readonly long _totalTicks;
        
        private double _balance;
        private double _totalPurchasePrice;
        private int _numSharesOwned;

        public AnalyzerState(Parameters parameters, double transactionFee, double balance, long totalTicks)
        {
            _balance        = balance;
            _initialBalance = balance;
            _parameters     = parameters;
            _transactionFee = transactionFee;
            _totalTicks     = totalTicks;

            BollingerBand = new BollingerBand(parameters.NumPeriods, parameters.NumStddevs);
        }

        private double GetProfit() => _firstPurchase.IsEnabled && _lastSale.IsEnabled
            ? _lastSale.Balance - _firstPurchase.Balance
            : 0.0;

        private long GetTradingTicks() => _firstPurchase.IsEnabled && _lastSale.IsEnabled
            ? _lastSale.TimeStamp.Ticks - _firstPurchase.TimeStamp.Ticks
            : 0;

        private static double GetNumDays(long numTradingTicks) => new TimeSpan(numTradingTicks).TotalDays;

        private double GetTradeSpanPercentage(long numTradingTicks) => numTradingTicks / (double) _totalTicks;

        public void SellShares(IPrice price)
        {
            _hasPendingSellTransaction = false;

            // sanity check: nothing to sell
            if (_numSharesOwned == 0) return;

            double minSellPrice   = BollingerBand.UpperBandPrice * _parameters.SellTargetPercent;
            double totalSellPrice = GetSellPrice(_numSharesOwned, price.Close);

            if (price.Close >= minSellPrice && totalSellPrice > _totalPurchasePrice)
            {
                _balance += totalSellPrice;
                _lastSale.Set(price.Date, _balance);
                _numSharesOwned = 0;
            }
        }

        private double GetSellPrice(int numShares, double price) => numShares * price - _transactionFee;

        private double GetTotalPurchasePrice(int numShares, double price) => numShares * price + _transactionFee;

        public void BuyShares(IPrice price)
        {
            // sanity check: nothing to buy
            if (_hasPendingSellTransaction || _numSharesOwned != 0) return;

            double maxBuyPrice   = BollingerBand.LowerBandPrice * _parameters.BuyTargetPercent;
            double shareBuyPrice = price.Close;

            if (shareBuyPrice <= maxBuyPrice)
            {
                double purchaseBudget = _balance - _transactionFee;

                _numSharesOwned     = (int)(purchaseBudget / shareBuyPrice);
                _totalPurchasePrice = GetTotalPurchasePrice(_numSharesOwned, shareBuyPrice);
                _balance -= _totalPurchasePrice;

                if (!_firstPurchase.IsEnabled) _firstPurchase.Set(price.Date, _balance);
            }
        }

        public PerformanceResults GetPerformanceResults()
        {
            var profit                 = GetProfit();
            var numTradingTicks        = GetTradingTicks();
            var numDays                = GetNumDays(numTradingTicks);
            var annualizedRateOfReturn = InvestmentStatistics.GetAnnualizedRateOfReturn(_initialBalance, profit, numDays);
            var tradeSpanPercentage    = GetTradeSpanPercentage(numTradingTicks);

            return new PerformanceResults(annualizedRateOfReturn, profit, tradeSpanPercentage);
        }
    }
}
