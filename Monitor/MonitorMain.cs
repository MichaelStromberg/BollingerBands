using System;
using System.IO;
using Analyzers.Common;
using Securities;

namespace Monitor
{
    static class MonitorMain
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("USAGE: {0} <input directory>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            var inputDir = args[0];
            var symbols  = new[] {"ILMN", "JKD", "SCHD"};

            foreach (var symbol in symbols)
            {
                var dataPath = Path.Combine(inputDir, $"{symbol}.dat");
                var bbPath   = Path.Combine(inputDir, $"{symbol}.bb");
                var tsvPath  = Path.Combine(inputDir, $"{symbol}.tsv");

                var parameters = LoadParameters(bbPath);
                var security   = LoadSecurity(dataPath).LastTwoWeeks(parameters.NumPeriods);

                var bollingerBand = new BollingerBand(parameters.NumPeriods, parameters.NumStddevs);

                using (var writer = new StreamWriter(new FileStream(tsvPath, FileMode.Create)))
                {
                    writer.WriteLine("date\tbollinger_low\tlow\tclose\thigh\tbollinger_high");

                    Console.WriteLine(security.Symbol);
                    Console.WriteLine(parameters);

                    Console.WriteLine("      date  bb_low     low   close    high bb_high");
                    //Console.WriteLine("2017-07-25 $148.02 $148.65 $148.75 $149.06 $149.04");

                    foreach (var price in security.Prices)
                    {
                        bollingerBand.Recalculate(price);

                        if (bollingerBand.IsCalculated)
                        {
                            Console.WriteLine($"{price.Date:yyyy-MM-dd} {bollingerBand.LowerBandPrice * parameters.BuyTargetPercent,7:C} {price.Low,7:C} {price.Close,7:C} {price.High,7:C} {bollingerBand.UpperBandPrice * parameters.SellTargetPercent,7:C}");
                            writer.WriteLine($"{price.Date:yyyy-MM-dd}\t{bollingerBand.LowerBandPrice * parameters.BuyTargetPercent:C}\t{price.Low:C}\t{price.Close:C}\t{price.High:C}\t{bollingerBand.UpperBandPrice * parameters.SellTargetPercent:C}");
                        }
                    }

                }

                Console.WriteLine();
            }
        }

        private static Parameters LoadParameters(string filePath)
        {
            Parameters parameters;
            using (var reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                parameters = Parameters.Read(reader);
            }
            return parameters;
        }

        private static ISecurity LoadSecurity(string filePath, bool showOutput = true)
        {
            if (showOutput) Console.Write("- loading the security... ");

            ISecurity security;
            using (var reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                security = Security.GetSecurity(reader);
            }

            if (showOutput) Console.WriteLine($"{security.Prices.Length} prices loaded.");
            return security;
        }

    }
}