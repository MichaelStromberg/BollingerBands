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


                var security   = LoadSecurity(dataPath);
                var parameters = LoadParameters(bbPath);

                foreach (var price in security.Prices)
                {
                    
                }
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