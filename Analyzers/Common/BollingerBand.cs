using System;
using System.Collections.Generic;
using System.Linq;
using Securities;

namespace Analyzers.Common
{
    public class BollingerBand
    {
        public double UpperBandPrice;
        public double LowerBandPrice;
        public bool IsCalculated;

        private readonly double _numStddevs;
        private readonly int _numPeriods;

        private double _currentSum;
        private readonly LinkedList<IPrice> _prices;

        public BollingerBand(int numPeriods, double numStddevs)
        {
            _prices     = new LinkedList<IPrice>();
            _numPeriods = numPeriods;
            _numStddevs = numStddevs;
        }

        /// <summary>
        /// recalculate the upper and lower bands
        /// </summary>
        public void Recalculate(IPrice newPrice)
        {
            // remove the oldest price
            if (_prices.Count == _numPeriods)
            {
                _currentSum -= _prices.First.Value.Close;
                _prices.RemoveFirst();
            }

            // add the newest price
            _prices.AddLast(newPrice);
            _currentSum += newPrice.Close;

            // recalculate the Bollinger band
            if (_prices.Count < _numPeriods) return;

            // calculate the mean
            double mean = _currentSum / _numPeriods;

            // calculate the sum of the differences
            double variance = _prices.Select(price => price.Close - mean).Select(diff => diff * diff).Sum() / _numPeriods;
            double stddev   = Math.Sqrt(variance);

            UpperBandPrice = mean + _numStddevs * stddev;
            LowerBandPrice = mean - _numStddevs * stddev;
            IsCalculated   = true;
        }
    }
}
