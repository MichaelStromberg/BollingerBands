using Securities.Sources;
using System;
using System.IO;

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

            var outputDir = args[0];
            var iex = new IexTrading();

            var symbols = new[] { "pej", "pbj", "pho", "rww", "pjp", "heco", "iym", "ixp", "idu", "ilmn", "jkd", "schd" };

            foreach (var symbol in symbols)
            {
                Console.Write($"- downloading {symbol}... ");
                var security = iex.DownloadFiveYearsAsync(symbol).Result;
                Console.WriteLine("finished.");

                var outputFilename = symbol + ".dat";
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