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
            foreach (var price in Prices) price.Write(writer);
        }

        public ISecurity Filter(DateTime beginDate, DateTime endDate)
        {
            var beginTicks = beginDate.Ticks;
            var endTicks = endDate.Ticks;
            var newPrices = new List<IPrice>();

            foreach (var price in Prices)
            {
                var ticks = price.Date.Ticks;
                if (ticks >= beginTicks && ticks <= endTicks) newPrices.Add(price);
            }

            return new Security(Symbol, newPrices.ToArray());
        }

        public ISecurity LastTwoWeeks(int offset)
        {
            int numDays   = 14 + offset;
            var numPrices = Math.Min(numDays, Prices.Length);
            var newPrices = new IPrice[numPrices];

            for (int i = 0; i < numDays; i++)
            {
                newPrices[i] = Prices[Prices.Length - numPrices + i];
            }

            return new Security(Symbol, newPrices);
        }

        public static ISecurity GetSecurity(BinaryReader reader)
        {
            var symbol = reader.ReadString();
            var numPrices = reader.ReadInt32();

            var prices = new IPrice[numPrices];
            for (int i = 0; i < numPrices; i++) prices[i] = Price.GetPrice(reader);

            return new Security(symbol, prices);
        }
    }
}
