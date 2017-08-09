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
