using Analyzers.Common;
using Xunit;

namespace UnitTests.Analyzers
{
    public class InvestmentStatisticsTests
    {
        [Fact]
        public void GetAnnualizedRateOfReturn_OneYear()
        {
            const double expectedResult = 0.20;
            var observedResult = InvestmentStatistics.GetAnnualizedRateOfReturn(100_000, 20_000, 365.0);
            Assert.Equal(expectedResult, observedResult, 2);
        }

        [Fact]
        public void GetAnnualizedRateOfReturn_FourYears()
        {
            const double expectedResult = 0.05;
            var observedResult = InvestmentStatistics.GetAnnualizedRateOfReturn(100_000, 20_000, 4 * 365.0);
            Assert.Equal(expectedResult, observedResult, 2);
        }
    }
}
