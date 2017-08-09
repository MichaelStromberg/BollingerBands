namespace Securities
{
    public interface ITrade
    {
        TradeAction TradeAction { get; }
        IPrice Price { get; }
        double TransactionPrice { get; }
        int NumShares { get; }
    }
}
