using System;
using System.IO;
using Securities;

namespace FindPerfectSellPoints
{
    class Program
    {
        static void Main()
        {
            //// NYSEARCA:JKD
            //// NYSEARCA:SCHD
            const string filePath = @"D:\Data\ETF\NYSEARCA_SCHD.dat";
            //var googleFinance = new GoogleFinance();
            //var ilmn = googleFinance.DownloadAsync("NYSEARCA:SCHD", new DateTime(2010, 1, 1), DateTime.Now).Result;

            //using (var writer = new BinaryWriter(new FileStream(filePath, FileMode.Create)))
            //{
            //    ilmn.Write(writer);
            //}

            //Environment.Exit(1);
            var security = LoadSecurity(filePath);

            const int maxPrices = 40;
            const double transactionFee = 19.99;

            //for (int i = 0; i < maxPrices; i++)
            //    Console.WriteLine("{0}\t{1}", security.Prices[i].Date.ToString("yyyy-MM-dd"), security.Prices[i].Close);
            //Environment.Exit(1);

            Console.WriteLine("Analyzing: {0}", security.Symbol);
            Console.WriteLine("Date range: {0} - {1}", security.Prices[0].Date.ToString("yyyy-MM-dd"),
                security.Prices[maxPrices - 1].Date.ToString("yyyy-MM-dd"));

            var benchmark = new Benchmark();

            ITester tester = new HeuristicTester(security.Prices, 0, maxPrices - 1, transactionFee);
            var bestTradeSet = tester.Analyze(100000.0);

            Console.WriteLine("Max balance: {0}", bestTradeSet.Balance);
            foreach (var trade in bestTradeSet.Trades) Console.WriteLine($"{trade.Price.Date:yyyy-MM-dd}: {trade.TradeAction} {trade.NumShares} shares @ {trade.TransactionPrice}");

            Console.WriteLine("time: {0}", Benchmark.ToHumanReadable(benchmark.GetElapsedTime()));
        }

        private static ISecurity LoadSecurity(string filePath)
        {
            Console.Write("- loading the security... ");

            ISecurity ilmn;
            using (var reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                ilmn = Security.GetSecurity(reader);
            }

            Console.WriteLine($"{ilmn.Prices.Length} prices loaded.");
            return ilmn;
        }
    }
}