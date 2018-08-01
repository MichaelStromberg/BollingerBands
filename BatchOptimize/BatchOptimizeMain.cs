using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Analyzers.Common;
using Optimize;
using Securities;

namespace BatchOptimize
{
    internal static class BatchOptimizeMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("USAGE: {0} <input directory> <# of years>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            string inputDir            = args[0];
            int numYears            = int.Parse(args[1]);
            var symbolToParameters  = new List<KeyValuePair<string, Parameters>>();

            var lockObject = new object();

            Console.WriteLine("Calculating optimal Bollinger bands:");
            Parallel.ForEach(Directory.GetFiles(inputDir, "*.dat"), securityPath =>
            {
                ISecurity security = OptimizeMain.LoadSecurity(securityPath, false);
                Console.WriteLine($"- {security.Symbol}");

                DateTime lastDate = security.Prices[security.Prices.Length - 1].Date;
                DateTime firstDate = lastDate.AddYears(-numYears);
                security = security.Filter(firstDate, lastDate);

                var optimizer = new Optimizer(security, 100000, 9.99, false);

                string outputPath = Path.Combine(Path.GetDirectoryName(securityPath),
                                     Path.GetFileNameWithoutExtension(securityPath)) + ".bb";
                Parameters securityResults = optimizer.Optimize(outputPath);

                lock (lockObject)
                {
                    symbolToParameters.Add(new KeyValuePair<string, Parameters>(security.Symbol, securityResults));
                }                
            });

            Console.WriteLine();
            foreach (var kvp in symbolToParameters.OrderByDescending(x => x.Value.Results.Profit))
            {
                PerformanceResults results = kvp.Value.Results;
                Console.WriteLine($"{kvp.Key}\t{results.AnnualizedRateOfReturn*100.0:0.00}% return/yr\t{results.Profit:C}\t{results.TradeSpanPercentage*100.0:0.00}% span");
            }
        }
    }
}