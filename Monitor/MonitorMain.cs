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
            var symbols  = new[] {"ILMN", "RWW", "IYM", "IDU", "PHO"};

            foreach (var symbol in symbols)
            {
                var dataPath = Path.Combine(inputDir, $"{symbol}.dat");
                var bbPath   = Path.Combine(inputDir, $"{symbol}.bb");
                var tsvPath  = Path.Combine(inputDir, $"{symbol}.tsv");

                var parameters = LoadParameters(bbPath);
                var security   = LoadSecurity(dataPath, false).LastTwoWeeks(parameters.NumPeriods);

                var bollingerBand = new BollingerBand(parameters.NumPeriods, parameters.NumStddevs);

                using (var writer = new StreamWriter(new FileStream(tsvPath, FileMode.Create)))
                {
                    writer.WriteLine("date\tbollinger_low\tlow\tclose\thigh\tbollinger_high");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(security.Symbol);
                    Console.ResetColor();

                    Console.WriteLine($"Periods: {parameters.NumPeriods}, Stddevs: {parameters.NumStddevs:0.0000}, Buy: {parameters.BuyTargetPercent * 100.0:0.000} %, Sell: {parameters.SellTargetPercent * 100.0:0.000} %\n");

                    DisplayColor("      date  bb_low     low   close    high bb_high\n", true, ConsoleColor.Yellow);
 
                    foreach (var price in security.Prices)
                    {
                        bollingerBand.Recalculate(price);

                        if (bollingerBand.IsCalculated)
                        {
                            DisplayPrice(price, bollingerBand, parameters);                            
                            writer.WriteLine($"{price.Date:yyyy-MM-dd}\t{bollingerBand.LowerBandPrice * parameters.BuyTargetPercent:C}\t{price.Low:C}\t{price.Close:C}\t{price.High:C}\t{bollingerBand.UpperBandPrice * parameters.SellTargetPercent:C}");
                        }
                    }

                }

                Console.WriteLine();
            }
        }

        private static void DisplayPrice(IPrice price, BollingerBand bollingerBand, Parameters parameters)
        {
            var bollingerLow  = bollingerBand.LowerBandPrice * parameters.BuyTargetPercent;
            var bollingerHigh = bollingerBand.UpperBandPrice * parameters.SellTargetPercent;

            const ConsoleColor lowEventColor  = ConsoleColor.Red;
            const ConsoleColor highEventColor = ConsoleColor.Green;

            bool hasLowEvent  = price.Low <= bollingerLow;
            bool hasHighEvent = price.High >= bollingerHigh;

            DisplayColor($"{price.Date:yyyy-MM-dd} ", true, ConsoleColor.DarkGray);

            DisplayColor($"{bollingerLow,7:C} ", hasLowEvent, lowEventColor);
            DisplayColor($"{price.Low,7:C} ", hasLowEvent, lowEventColor);

            Console.Write($"{ price.Close,7:C} ");

            DisplayColor($"{price.High,7:C} ", hasHighEvent, highEventColor);
            DisplayColor($"{bollingerHigh,7:C}", hasHighEvent, highEventColor);
            Console.WriteLine();
        }

        private static void DisplayColor(string s, bool useColor, ConsoleColor color)
        {
            if (useColor) Console.ForegroundColor = color;
            Console.Write(s);
            if(useColor) Console.ResetColor();
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