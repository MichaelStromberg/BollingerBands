using System;
using System.IO;
using Securities;

namespace Optimize
{
    public static class OptimizeMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("USAGE: {0} <input path> <output path> <# of years>",
                    Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            string    inputPath  = args[0];
            string    outputPath = args[1];
            int       numYears   = int.Parse(args[2]);
            ISecurity security   = LoadSecurity(inputPath);

            DateTime lastDate  = security.Prices[^1].Date;
            DateTime firstDate = lastDate.AddYears(-numYears);
            security = security.Filter(firstDate, lastDate);

            var optimizer = new Optimizer(security);
            optimizer.Optimize(outputPath);
        }

        public static ISecurity LoadSecurity(string filePath, bool showOutput = true)
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