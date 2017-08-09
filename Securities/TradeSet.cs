using System.Collections.Generic;

namespace Securities
{
    public class TradeSet
    {
        public double Balance { get; private set; }
        public readonly List<ITrade> Trades;
        private int _numSharesOwned;

        public TradeSet(double balance, List<ITrade> trades, int numSharesOwned)
        {
            Balance         = balance;
            Trades          = trades;
            _numSharesOwned = numSharesOwned;
        }

        public void Buy(IPrice price, double purchasePrice, double transactionFee)
        {
            var numShares = GetMaxShares(Balance - transactionFee, purchasePrice);
            Trades.Add(new Trade(TradeAction.Buy, price, purchasePrice, numShares));
            Balance -= purchasePrice * numShares + transactionFee;
            _numSharesOwned = numShares;
        }

        public void Sell(IPrice price, double salePrice, double transactionFee)
        {
            if (_numSharesOwned == 0) return;
            Trades.Add(new Trade(TradeAction.Sell, price, salePrice, _numSharesOwned));
            Balance += salePrice * _numSharesOwned - transactionFee;
            _numSharesOwned = 0;
        }

        private static int GetMaxShares(double balance, double price) => (int)(balance / price);

        public TradeSet DeepCopy()
        {
            var newTrades = new List<ITrade>();
            newTrades.AddRange(Trades);
            return new TradeSet(Balance, newTrades, _numSharesOwned);
        }
    }
}
