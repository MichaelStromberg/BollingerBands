using System;
using System.Collections.Generic;
using System.IO;

namespace Securities
{
    public class Security : ISecurity
    {
        public string Symbol { get; }
        public IPrice[] Prices { get; }

        /// <summary>
        /// constructor
        /// </summary>
        public Security(string symbol, IPrice[] prices)
        {
            Symbol = symbol;
            Prices = prices;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Symbol);
            writer.Write(Prices.Length);
            foreach (IPrice price in Prices) price.Write(writer);
        }

        public ISecurity Filter(DateTime beginDate, DateTime endDate)
        {
            long beginTicks = beginDate.Ticks;
            long endTicks   = endDate.Ticks;
            var newPrices   = new List<IPrice>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (IPrice price in Prices)
            {
                long ticks = price.Date.Ticks;
                if (ticks >= beginTicks && ticks <= endTicks) newPrices.Add(price);
            }

            return new Security(Symbol, newPrices.ToArray());
        }

        public ISecurity LastTwoWeeks(int offset)
        {
            int numDays   = 14 + offset;
            int numPrices = Math.Min(numDays, Prices.Length);
            var newPrices = new IPrice[numPrices];

            for (var i = 0; i < numDays; i++)
            {
                newPrices[i] = Prices[Prices.Length - numPrices + i];
            }

            return new Security(Symbol, newPrices);
        }

        public static ISecurity GetSecurity(BinaryReader reader)
        {
            string symbol = reader.ReadString();
            int numPrices = reader.ReadInt32();

            var prices = new IPrice[numPrices];
            for (var i = 0; i < numPrices; i++) prices[i] = Price.GetPrice(reader);

            return new Security(symbol, prices);
        }
    }
}
