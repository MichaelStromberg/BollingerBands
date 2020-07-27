using Securities.Sources;
using System;
using System.IO;
using Securities;

namespace BatchDownloader
{
    internal static class BatchDownloaderMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("USAGE: {0} <output directory>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            string outputDir = args[0];
            var yahoo = new YahooFinance();

            var symbols = new[] { "pej", "pbj", "pho", "rww", "pjp", "heco", "iym", "ixp", "idu", "ilmn", "jkd", "schd" };

            foreach (string symbol in symbols)
            {
                Console.Write($"- downloading {symbol}... ");
                ISecurity security = yahoo.DownloadFiveYearsAsync(symbol).Result;
                Console.WriteLine("finished.");

                string outputFilename = symbol + ".dat";
                string outputPath     = Path.Combine(outputDir, outputFilename);

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