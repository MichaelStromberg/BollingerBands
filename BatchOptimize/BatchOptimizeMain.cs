using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Analyzers.Common;
using Optimize;

namespace BatchOptimize
{
    static class BatchOptimizeMain
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("USAGE: {0} <input directory> <# of years>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            var inputDir = args[0];
            var numYears = int.Parse(args[1]);
            var results  = new List<KeyValuePair<string, Parameters>>();

            foreach (var securityPath in Directory.GetFiles(inputDir, "*.dat"))
            {
                var security  = OptimizeMain.LoadSecurity(securityPath);

                var lastDate  = security.Prices[security.Prices.Length - 1].Date;
                var firstDate = lastDate.AddYears(-numYears);
                security      = security.Filter(firstDate, lastDate);

                var optimizer = new Optimizer(security);

                var outputPath = Path.Combine(Path.GetDirectoryName(securityPath),
                                     Path.GetFileNameWithoutExtension(securityPath)) + ".bb";
                var securityResults = optimizer.Optimize(outputPath);

                results.Add(new KeyValuePair<string, Parameters>(security.Symbol, securityResults));
            }

            Console.WriteLine();
            foreach (var result in results.OrderByDescending(x => x.Value.Results.Profit))
            {
                Console.WriteLine($"{result.Key}\t{result.Value.Results.Profit:C}");
            }
        }
    }
}