namespace Analyzers.Common
{
    public static class InvestmentStatistics
    {
        public static double GetAnnualizedRateOfReturn(double initialBalance, double profit, double numDays)
        {
            double percentGain = profit / initialBalance;
            return percentGain / numDays * 365.0;
        }
    }
}
