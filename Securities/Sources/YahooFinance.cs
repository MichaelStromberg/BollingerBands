using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Securities.Sources
{
    public class YahooFinance
    {
        private static readonly HttpClient HttpClient = new();

        static YahooFinance() => HttpClient.BaseAddress = new Uri("https://query1.finance.yahoo.com");

        public static async Task<ISecurity> DownloadFiveYearsAsync(string symbol)
        {
            var url    = $"/v7/finance/download/{symbol}?range=5y&interval=1d&events=history";
            var prices = new List<IPrice>();

            await using (Stream stream = await HttpClient.GetStreamAsync(url).ConfigureAwait(false))
            using (var reader = new StreamReader(stream))
            {
                // skip the header
                await reader.ReadLineAsync().ConfigureAwait(false);

                while (true)
                {
                    string line = await reader.ReadLineAsync().ConfigureAwait(false);
                    if (line == null) break;

                    string[] cols = line.Split(',');

                    if (cols[1] == "null" || cols[6] == "0") continue;

                    try
                    {
                        DateTime date  = ParsingUtilities.GetDate(cols[0]);
                        double   open  = double.Parse(cols[1]);
                        double   high  = double.Parse(cols[2]);
                        double   low   = double.Parse(cols[3]);
                        double   close = double.Parse(cols[4]);

                        prices.Add(new Price(date, open, high, low, close));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Message}: {line}");
                        throw;
                    }
                }
            }

            return new Security(symbol, prices.ToArray());
        }
    }
}