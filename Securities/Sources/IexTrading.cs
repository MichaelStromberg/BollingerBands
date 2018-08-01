using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Securities.Sources
{
    public class IexTrading : ISecuritySource
    {
#pragma warning disable 649
#pragma warning disable 169
        // ReSharper disable once ClassNeverInstantiated.Local
        private class IexTradingData
        {
            // ReSharper disable InconsistentNaming
            public DateTime date;            
            public double open;
            public double high;
            public double low;
            public double close;
            public ulong volume;
            public ulong unadjustedVolume;
            public double change;
            public double changePercent;
            public double vwap;
            public string label;
            public double changeOverTime;
            // ReSharper restore InconsistentNaming
        }
#pragma warning restore 169
#pragma warning restore 649

        public async Task<ISecurity> DownloadFiveYearsAsync(string symbol)
        {
            string url = $"https://api.iextrading.com/1.0/stock/{symbol}/chart/5y";            
            string json;

            using (var client                   = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content          = response.Content)
            {
                json = await content.ReadAsStringAsync();
            }

            var prices = GetPrices(json);

            return new Security(symbol, prices.ToArray()); 
        }

        private static List<IPrice> GetPrices(string json)
        {
            var deserializer = new JsonDeserializer();
            var iexPrices    = deserializer.Deserialize<IexTradingData[]>(json);
            var prices       = new List<IPrice>(iexPrices.Length);

            prices.AddRange(iexPrices.Select(iexPrice => new Price(iexPrice.date, iexPrice.open, iexPrice.high, iexPrice.low, iexPrice.close)));
            return prices;
        }
    }
}
