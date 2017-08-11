using System;
using System.IO;
using Securities;

namespace Optimize
{
    public static class OptimizeMain
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("USAGE: {0} <input path> <output path> <# of years>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            var inputPath  = args[0];
            var outputPath = args[1];
            var numYears   = int.Parse(args[2]);
            var security   = LoadSecurity(inputPath);

            var lastDate  = security.Prices[security.Prices.Length - 1].Date;
            var firstDate = lastDate.AddYears(-numYears);
            security      = security.Filter(firstDate, lastDate);

            var optimizer = new Optimizer(security);
            optimizer.Optimize(outputPath);
        }

        public static ISecurity LoadSecurity(string filePath)
        {
            Console.Write("- loading the security... ");

            ISecurity security;
            using (var reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                security = Security.GetSecurity(reader);
            }

            Console.WriteLine($"{security.Prices.Length} prices loaded.");
            return security;
        }
    }
}