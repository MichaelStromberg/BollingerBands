using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Securities.Sources
{
    public class GoogleFinance : ISecuritySource
    {
        public async Task<ISecurity> DownloadAsync(string symbol, DateTime begin, DateTime end)
        {
            string beginDate = begin.ToString("MMM+d,+yyyy");
            string endDate   = end.ToString("MMM+d,+yyyy");

            string url = $"http://www.google.com/finance/historical?q={symbol}&startdate={beginDate}&enddate={endDate}&output=csv";
            Console.WriteLine($"url: {url}");
            var prices = new List<IPrice>();

            using (var client   = new HttpClient())
            using (var response = await client.GetAsync(url))
            using (var content  = response.Content)
            using (var stream   = await content.ReadAsStreamAsync())
            using (var reader   = new StreamReader(stream))
            {
                // skip the header
                reader.ReadLine();

                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;

                    var cols  = line.Split(',');
                    FixZeroVolumeDays(cols);

                    var date  = ParsingUtilities.GetDate(cols[0]);
                    var open  = double.Parse(cols[1]);
                    var high  = double.Parse(cols[2]);
                    var low   = double.Parse(cols[3]);
                    var close = double.Parse(cols[4]);

                    prices.Add(new Price(date, open, high, low, close));
                }
            }

            prices.Reverse();

            return new Security(symbol, prices.ToArray()); 
        }

        private static void FixZeroVolumeDays(string[] cols)
        {
            // 13-Jan-12,-,-,-,73.31,0
            RemoveDashPrice(cols, 1);
            RemoveDashPrice(cols, 2);
            RemoveDashPrice(cols, 3);
        }

        private static void RemoveDashPrice(string[] cols, int index)
        {
            if (cols[index] == "-") cols[index] = cols[4];
        }
    }
}
