using System;
using System.IO;
using Securities.Sources;

namespace GoogleFinanceDownload
{
    static class GoogleFinanceMain
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("USAGE: {0} <symbol e.g. NYSEARCA:SCHD> <output path>",
                    Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            var symbol   = args[0];
            var filePath = args[1];

            var iex = new IexTrading();

            Console.Write("- downloading from Google Finance... ");
            var security = iex.DownloadFiveYearsAsync(symbol).Result;
            Console.WriteLine("finished.");

            Console.Write("- writing to disk... ");
            using (var writer = new BinaryWriter(new FileStream(filePath, FileMode.Create)))
            {
                security.Write(writer);
            }
            Console.WriteLine("finished.");
        }
    }
}