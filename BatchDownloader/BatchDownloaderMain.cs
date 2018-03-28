using System;
using System.IO;
using Securities.Sources;

namespace BatchDownloader
{
    static class BatchDownloaderMain
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("USAGE: {0} <output directory>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            var outputDir     = args[0];
            var googleFinance = new GoogleFinance();

            var symbols = new[]
            {
                "NYSEARCA:PEJ", "NYSEARCA:PBJ", "NASDAQ:PHO", "NYSEARCA:RWW", "NYSEARCA:PJP", "NYSEARCA:HECO",
                "NYSEARCA:IYM", "NYSEARCA:IXP", "NYSEARCA:IDU", "NASDAQ:ILMN", "NYSEARCA:JKD",
                "NYSEARCA:SCHD", "CURRENCY:BTC"
            };

            foreach (var symbol in symbols)
            {
                Console.Write($"- downloading {symbol}... ");
                var security = googleFinance.DownloadAsync(symbol, new DateTime(2010, 1, 1), DateTime.Now).Result;
                Console.WriteLine("finished.");

                var outputFilename = symbol.Split(':')[1] + ".dat";
                var outputPath     = Path.Combine(outputDir, outputFilename);

                Console.Write("- writing to disk... ");
                using (var writer = new BinaryWriter(new FileStream(outputPath, FileMode.Create)))
                {
                    security.Write(writer);
                }
                Console.WriteLine("finished.");
            }
        }
    }
}