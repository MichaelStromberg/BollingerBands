using System;

namespace Optimize
{
    public class ROI
    {
        /// <summary>
        /// returns the annualized rate of return
        /// </summary>
        public static double CalculateAnnualizedRateOfReturn(double initialBalance, double profit, DateTime beginDate,
            DateTime endDate)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (initialBalance == default(double)) return 0.0;

            // calculate the gain and how many days have passed
            var percentGain = profit / initialBalance * 100.0;
            var timeSpan    = new TimeSpan(endDate.Ticks - beginDate.Ticks);

            // return the annualized rate of return
            return percentGain / timeSpan.TotalDays * 365.0;
        }
    }
}
