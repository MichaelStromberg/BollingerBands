using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            var inputDir            = args[0];
            var numYears            = int.Parse(args[1]);
            var symbolToParameters  = new List<KeyValuePair<string, Parameters>>();

            var lockObject = new object();

            Console.WriteLine("Calculating optimal Bollinger bands:");
            Parallel.ForEach(Directory.GetFiles(inputDir, "*.dat"), securityPath =>
            {
                var security = OptimizeMain.LoadSecurity(securityPath, false);
                Console.WriteLine($"- {security.Symbol}");

                var lastDate = security.Prices[security.Prices.Length - 1].Date;
                var firstDate = lastDate.AddYears(-numYears);
                security = security.Filter(firstDate, lastDate);

                var optimizer = new Optimizer(security, 100000, 9.99, false);

                var outputPath = Path.Combine(Path.GetDirectoryName(securityPath),
                                     Path.GetFileNameWithoutExtension(securityPath)) + ".bb";
                var securityResults = optimizer.Optimize(outputPath);

                lock (lockObject)
                {
                    symbolToParameters.Add(new KeyValuePair<string, Parameters>(security.Symbol, securityResults));
                }                
            });

            Console.WriteLine();
            foreach (var kvp in symbolToParameters.OrderByDescending(x => x.Value.Results.Profit))
            {
                var results = kvp.Value.Results;
                Console.WriteLine($"{kvp.Key}\t{results.AnnualizedRateOfReturn*100.0:0.00}% return/yr\t{results.Profit:C}\t{results.TradeSpanPercentage*100.0:0.00}% span");
            }
        }
    }
}