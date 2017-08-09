using System;
using System.IO;
using Securities;

namespace Optimize
{
    static class OptimizeMain
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("USAGE: {0} <input path>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            var filePath = args[0];
            var security = LoadSecurity(filePath).LastYear();

            var optimizer = new Optimizer(security);
            optimizer.Optimize();
        }

        private static ISecurity LoadSecurity(string filePath)
        {
            Console.Write("- loading the security... ");

            ISecurity ilmn;
            using (var reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                ilmn = Security.GetSecurity(reader);
            }

            Console.WriteLine($"{ilmn.Prices.Length} prices loaded.");
            return ilmn;
        }
    }
}