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

        public ISecurity LastYear()
        {
            var thresholdTicks = Prices[Prices.Length - 1].Date.AddYears(-1).Ticks;
            var newPrices = new List<IPrice>();

            foreach (var price in Prices)
            {
                if (price.Date.Ticks >= thresholdTicks) newPrices.Add(price);
            }

            return new Security(Symbol, newPrices.ToArray());
        }

        public static ISecurity GetSecurity(BinaryReader reader)
        {
            var symbol    = reader.ReadString();
            var numPrices = reader.ReadInt32();

            var prices = new IPrice[numPrices];
            for (int i = 0; i < numPrices; i++) prices[i] = Price.GetPrice(reader);

            return new Security(symbol, prices);
        }
    }
}
