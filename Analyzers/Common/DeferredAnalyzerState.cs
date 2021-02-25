using System;
using Securities;

namespace Analyzers.Common
{
    public class DeferredAnalyzerState : IAnalyzerState
    {
        public readonly BollingerBand BollingerBand;
        private readonly double _transactionFee;

        private readonly Transaction _firstPurchase = new();
        private readonly Transaction _lastSale      = new();
        private readonly Parameters  _parameters;
        private readonly double      _initialBalance;
        private readonly bool        _showOutput;
        private readonly int         _numDaysUntilSettlement;

        private readonly long _totalTicks;

        private double _balance;
        private double _totalPurchasePrice;
        private int _numSharesOwned;
        private Order _order;

        public DeferredAnalyzerState(Parameters parameters, double transactionFee, double balance, long totalTicks,
            bool showOutput, int numDaysUntilSettlement)
        {
            _balance                = balance;
            _initialBalance         = balance;
            _parameters             = parameters;
            _transactionFee         = transactionFee;
            _totalTicks             = totalTicks;
            _showOutput             = showOutput;
            _numDaysUntilSettlement = numDaysUntilSettlement;

            BollingerBand = new BollingerBand(parameters.NumPeriods, parameters.NumStddevs);
        }

        private double GetProfit() => _firstPurchase.IsEnabled && _lastSale.IsEnabled
            ? _lastSale.Balance - _firstPurchase.Balance
            : 0.0;

        private long GetTradingTicks() => _firstPurchase.IsEnabled && _lastSale.IsEnabled
            ? _lastSale.TimeStamp.Ticks - _firstPurchase.TimeStamp.Ticks
            : 0;

        private static double GetNumDays(long numTradingTicks) => new TimeSpan(numTradingTicks).TotalDays;

        private double GetTradeSpanPercentage(long numTradingTicks) => numTradingTicks / (double)_totalTicks;

        private double GetSellPrice(int numShares, double price) => numShares * price - _transactionFee;

        private double GetTotalPurchasePrice(int numShares, double price) => numShares * price + _transactionFee;


        private int GetNumDaysSinceLastSale(DateTime currentDate) => _lastSale.IsEnabled
            ? (int)new TimeSpan(currentDate.Ticks - _lastSale.TimeStamp.Ticks).TotalDays
            : int.MaxValue;

        public PerformanceResults GetPerformanceResults()
        {
            double profit                 = GetProfit();
            long numTradingTicks          = GetTradingTicks();
            double numDays                = GetNumDays(numTradingTicks);
            double annualizedRateOfReturn = InvestmentStatistics.GetAnnualizedRateOfReturn(_initialBalance, profit, numDays);
            double tradeSpanPercentage    = GetTradeSpanPercentage(numTradingTicks);

            return new PerformanceResults(annualizedRateOfReturn, profit, tradeSpanPercentage);
        }

        private void ShowStatus(IPrice price)
        {
            var descriptor = "";
            if (price.Close < BollingerBand.LowerBandPrice) descriptor = "vvv";
            if (price.Close > BollingerBand.UpperBandPrice) descriptor = "^^^";

            var date = price.Date.ToString("yyyy-MM-dd HH:mm:ss");
            double weightedLowerBandPrice = BollingerBand.LowerBandPrice * _parameters.BuyTargetPercent;
            double weightedUpperBandPrice = BollingerBand.UpperBandPrice * _parameters.SellTargetPercent;

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
            double weightedLowerBandPrice = BollingerBand.LowerBandPrice * _parameters.BuyTargetPercent;
            double weightedUpperBandPrice = BollingerBand.UpperBandPrice * _parameters.SellTargetPercent;

            Console.WriteLine("\nFinal bollinger band:");
            Console.WriteLine("=====================");
            Console.WriteLine($"bands: [{BollingerBand.LowerBandPrice:C}, {BollingerBand.UpperBandPrice:C}], " +
                              $"weighted bands: [{weightedLowerBandPrice:C}, {weightedUpperBandPrice:C}]\n");
        }

        public void CreateOrder() => _order = _numSharesOwned == 0 ? CreateBuyOrder() : CreateSellOrder();

        private Order CreateSellOrder() => new(OrderType.Sell, BollingerBand.UpperBandPrice * _parameters.SellTargetPercent);

        private Order CreateBuyOrder() => new(OrderType.Buy, BollingerBand.LowerBandPrice * _parameters.BuyTargetPercent);

        public void HandleOrder(IPrice price)
        {
            if (_order == null) return;
            if (_order.Type == OrderType.Buy) BuyShares(price);
            else SellShares(price);
        }

        private void SellShares(IPrice price)
        {
            double totalSellPrice = GetSellPrice(_numSharesOwned, price.Close);
            if (!(price.Close >= _order.Price) || !(totalSellPrice > _totalPurchasePrice)) return;

            if (_showOutput)
            {
                ShowStatus(price);
                DisplaySale(_numSharesOwned, totalSellPrice, totalSellPrice - _totalPurchasePrice);
            }

            _balance += totalSellPrice;
            _lastSale.Set(price.Date, _balance);
            _numSharesOwned = 0;
            _order = null;
        }

        private void BuyShares(IPrice price)
        {
            int numDaysSinceLastSale = GetNumDaysSinceLastSale(price.Date);
            if (numDaysSinceLastSale < _numDaysUntilSettlement) return;

            double closePrice = price.Close;
            if (!(closePrice <= _order.Price)) return;

            double purchaseBudget = _balance - _transactionFee;

            if (!_firstPurchase.IsEnabled) _firstPurchase.Set(price.Date, _balance);

            _order = null;
            _numSharesOwned     = (int)(purchaseBudget / closePrice);
            _totalPurchasePrice = GetTotalPurchasePrice(_numSharesOwned, closePrice);
            _balance -= _totalPurchasePrice;

            if (!_showOutput) return;

            ShowStatus(price);
            DisplayPurchase(_numSharesOwned, _totalPurchasePrice);
        }
    }
}
