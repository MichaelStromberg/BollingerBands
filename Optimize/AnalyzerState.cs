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
        private readonly bool _showOutput;

        private readonly long _totalTicks;
        
        private double _balance;
        private double _totalPurchasePrice;
        private int _numSharesOwned;

        public AnalyzerState(Parameters parameters, double transactionFee, double balance, long totalTicks,
            bool showOutput)
        {
            _balance        = balance;
            _initialBalance = balance;
            _parameters     = parameters;
            _transactionFee = transactionFee;
            _totalTicks     = totalTicks;
            _showOutput     = showOutput;

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
            // sanity check: nothing to sell
            if (_numSharesOwned == 0) return;

            double minSellPrice   = BollingerBand.UpperBandPrice * _parameters.SellTargetPercent;
            double totalSellPrice = GetSellPrice(_numSharesOwned, price.Close);

            if (price.Close >= minSellPrice && totalSellPrice > _totalPurchasePrice)
            {
                if (_showOutput)
                {
                    ShowStatus(price);
                    DisplaySale(_numSharesOwned, totalSellPrice, totalSellPrice - _totalPurchasePrice);
                }

                _balance += totalSellPrice;
                _lastSale.Set(price.Date, _balance);
                _numSharesOwned = 0;
            }
        }

        public void BuyShares(IPrice price, int numDaysUntilSettlement)
        {
            var numDaysSinceLastSale = GetNumDaysSinceLastSale(price.Date);

            // sanity check: nothing to buy
            if (_numSharesOwned != 0 || numDaysSinceLastSale < numDaysUntilSettlement) return;

            double maxBuyPrice   = BollingerBand.LowerBandPrice * _parameters.BuyTargetPercent;
            double shareBuyPrice = price.Close;

            if (shareBuyPrice <= maxBuyPrice)
            {
                double purchaseBudget = _balance - _transactionFee;

                _numSharesOwned     = (int)(purchaseBudget / shareBuyPrice);
                _totalPurchasePrice = GetTotalPurchasePrice(_numSharesOwned, shareBuyPrice);
                _balance -= _totalPurchasePrice;

                if (_showOutput)
                {
                    ShowStatus(price);
                    DisplayPurchase(_numSharesOwned, _totalPurchasePrice);
                }

                if (!_firstPurchase.IsEnabled) _firstPurchase.Set(price.Date, _balance);
            }
        }

        private double GetSellPrice(int numShares, double price) => numShares * price - _transactionFee;

        private double GetTotalPurchasePrice(int numShares, double price) => numShares * price + _transactionFee;


        private int GetNumDaysSinceLastSale(DateTime currentDate) => _lastSale.IsEnabled
            ? (int)new TimeSpan(currentDate.Ticks - _lastSale.TimeStamp.Ticks).TotalDays
            : int.MaxValue;

        public PerformanceResults GetPerformanceResults()
        {
            var profit                 = GetProfit();
            var numTradingTicks        = GetTradingTicks();
            var numDays                = GetNumDays(numTradingTicks);
            var annualizedRateOfReturn = InvestmentStatistics.GetAnnualizedRateOfReturn(_initialBalance, profit, numDays);
            var tradeSpanPercentage    = GetTradeSpanPercentage(numTradingTicks);

            return new PerformanceResults(annualizedRateOfReturn, profit, tradeSpanPercentage);
        }

        private void ShowStatus(IPrice price)
        {
            string descriptor = "";
            if (price.Close < BollingerBand.LowerBandPrice) descriptor = "vvv";
            if (price.Close > BollingerBand.UpperBandPrice) descriptor = "^^^";

            var date                   = price.Date.ToString("yyyy-MM-dd HH:mm:ss");
            var weightedLowerBandPrice = BollingerBand.LowerBandPrice * _parameters.BuyTargetPercent;
            var weightedUpperBandPrice = BollingerBand.UpperBandPrice * _parameters.SellTargetPercent;

            Console.WriteLine($"[{date}] " +
                $"bands: [{BollingerBand.LowerBandPrice:C}, {BollingerBand.UpperBandPrice:C}], " +
                $"weighted bands: [{weightedLowerBandPrice:C}, {weightedUpperBandPrice:C}], " +
                $"close: {price.Close:C} " +
                $"{descriptor}");
        }

        private static void DisplayPurchase(int numSharesBought, double totalPurchasePrice)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                      Bought {0} shares (total: {1:C})", numSharesBought, totalPurchasePrice);
            Console.ResetColor();
        }

        private static void DisplaySale(int numSharesSold, double totalSellPrice, double profit)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("                      Sold {0} shares (total: {1:C}, profit: {2:C})", numSharesSold,
                totalSellPrice, profit);
            Console.ResetColor();
        }

        public void ShowFinalBollingerBand()
        {
            var weightedLowerBandPrice = BollingerBand.LowerBandPrice * _parameters.BuyTargetPercent;
            var weightedUpperBandPrice = BollingerBand.UpperBandPrice * _parameters.SellTargetPercent;

            Console.WriteLine("\nFinal bollinger band:");
            Console.WriteLine("=====================");
            Console.WriteLine($"bands: [{BollingerBand.LowerBandPrice:C}, {BollingerBand.UpperBandPrice:C}], " +
                              $"weighted bands: [{weightedLowerBandPrice:C}, {weightedUpperBandPrice:C}]\n");
        }
    }
}
