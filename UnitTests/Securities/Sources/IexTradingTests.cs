using System;
using System.Collections.Generic;
using System.Linq;
using Securities;
using Securities.Sources;
using Xunit;

namespace UnitTests.Securities.Sources
{
    public sealed class IexTradingTests
    {
        [Fact]
        public void DownloadAsync_Nominal()
        {
            var desiredDate = new DateTime(2018, 5, 30);
            var expectedResults = new Price(desiredDate, 267.82, 275.34, 267.82, 272.39);

            var iex = new IexTrading();
            ISecurity security = iex.DownloadFiveYearsAsync("ILMN").Result;

            IPrice observedResults = FindPrice(security.Prices, desiredDate);

            var priceComparer = new PriceComparer();
            Assert.Equal(expectedResults, observedResults, priceComparer);
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static IPrice FindPrice(IPrice[] prices, DateTime desiredDate)
        {
            const string dateFormat = "yyyy-MM-dd";
            return prices.FirstOrDefault(price => price.Date.ToString(dateFormat) == desiredDate.ToString(dateFormat));
        }
    }

    public class PriceComparer : IEqualityComparer<IPrice>
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const double Tolerance = 0.00001;

        public bool Equals(IPrice x, IPrice y)
        {
            return x.Date.ToString(DateFormat) == y.Date.ToString(DateFormat) &&
                   Math.Abs(x.Close - y.Close) < Tolerance &&
                   Math.Abs(x.High - y.High)   < Tolerance &&
                   Math.Abs(x.Low - y.Low)     < Tolerance &&
                   Math.Abs(x.Open - y.Open)   < Tolerance;
        }

        public int GetHashCode(IPrice obj)
        {
            return obj.Date.GetHashCode() ^ obj.Close.GetHashCode() ^ obj.High.GetHashCode() ^ obj.Low.GetHashCode() ^
                   obj.Open.GetHashCode();
        }
    }
}
