using System;
using Securities;

namespace Optimize
{
    public class AnalyzerState
    {
        public readonly BollingerBand BollingerBand;
        private readonly double _transactionFee;

        private readonly Transaction _firstPurchase = new Transaction();
        private readonly Transaction _lastSale = new Transaction();
        private readonly Parameters _parameters;

        private double _balance;
        private double _totalPurchasePrice;
        private int _numSharesOwned;

        public AnalyzerState(Parameters parameters, double transactionFee, double balance)
        {
            _balance        = balance;
            _parameters     = parameters;
            _transactionFee = transactionFee;
            BollingerBand   = new BollingerBand(parameters.NumPeriods, parameters.NumStddevs);
        }

        /// <summary>
        /// returns the annualized rate of return
        /// </summary>
        public double GetAnnualizedRateOfReturn(double profit)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!_firstPurchase.IsEnabled || !_lastSale.IsEnabled ||
                _firstPurchase.Balance == default(double)) return 0.0;

            // calculate the gain and how many days have passed
            var percentGain = profit / _firstPurchase.Balance * 100.0;
            var timeSpan = new TimeSpan(_lastSale.TimeStamp.Ticks - _firstPurchase.TimeStamp.Ticks);

            return percentGain / timeSpan.TotalDays * 365.0;
        }

        public double GetProfit() => _firstPurchase.IsEnabled && _lastSale.IsEnabled
            ? _lastSale.Balance - _firstPurchase.Balance
            : 0.0;

        public void SellShares(IPrice price)
        {
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
            if (_numSharesOwned != 0) return;

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
    }
}
