using Securities;

namespace Optimize
{
    public static class AnalyzerCommon
    {
        public static double GetPurchasePrice(int numShares, double purchasePrice, double transactionFee) =>
            numShares * purchasePrice + transactionFee;

        public static double GetSellPrice(int numShares, double sellPrice, double transactionFee) =>
            numShares * sellPrice - transactionFee;

        public static bool HasBollingerBandEvent(IPrice price, BollingerBand bband) =>
            price.Close < bband.LowerBandPrice || price.Close > bband.UpperBandPrice;
    }
}
