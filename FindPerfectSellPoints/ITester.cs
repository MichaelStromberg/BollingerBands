using Securities;

namespace FindPerfectSellPoints
{
    public interface ITester
    {
        TradeSet Analyze(double initialCapital);
    }
}
