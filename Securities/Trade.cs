namespace Securities
{
    public class Trade : ITrade
    {
        public TradeAction TradeAction { get; }
        public IPrice Price { get; }
        public double TransactionPrice { get; }
        public int NumShares { get; }

        /// <summary>
        /// constructor
        /// </summary>
        public Trade(TradeAction tradeAction, IPrice price, double transactionPrice, int numShares)
        {
            TradeAction      = tradeAction;
            Price            = price;
            TransactionPrice = transactionPrice;
            NumShares        = numShares;
        }
    }
}
