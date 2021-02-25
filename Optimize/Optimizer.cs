using System;
using System.IO;
using Analyzers;
using Analyzers.Common;
using Securities;

namespace Optimize
{
    public class Optimizer
    {
        private readonly ISecurity _security;
        private readonly double    _startCapital;
        private readonly double    _transactionFee;
        private readonly bool      _showOutput;

        public Optimizer(ISecurity security, double startCapital = 100000.0, double transactionFee = 9.99,
            bool showOutput = true)
        {
            _security       = security;
            _startCapital   = startCapital;
            _transactionFee = transactionFee;
            _showOutput     = showOutput;
        }

        private Parameters FindBestParameters(IAnalyzer analyzer, ParameterRange paramRange)
        {
            if (_showOutput)
            {
                Console.WriteLine("Parameter estimation using the following parameter ranges:");
                Console.WriteLine(paramRange);
            }

            var currentParameters = new Parameters(paramRange);
            var bestParameters    = new Parameters(paramRange);
            bestParameters.Update(currentParameters);

            for (currentParameters.NumPeriods = paramRange.NumPeriods.Begin;
                currentParameters.NumPeriods <= paramRange.NumPeriods.End;
                currentParameters.NumPeriods++)
            {
                for (currentParameters.NumStddevs = paramRange.NumStddevs.Begin;
                    currentParameters.NumStddevs <= paramRange.NumStddevs.End;
                    currentParameters.NumStddevs += paramRange.NumStddevs.StepSize)
                {
                    for (currentParameters.BuyTargetPercent = paramRange.BuyTargetPercent.Begin;
                        currentParameters.BuyTargetPercent <= paramRange.BuyTargetPercent.End;
                        currentParameters.BuyTargetPercent += paramRange.BuyTargetPercent.StepSize)
                    {
                        for (currentParameters.SellTargetPercent = paramRange.SellTargetPercent.Begin;
                            currentParameters.SellTargetPercent < paramRange.SellTargetPercent.End;
                            currentParameters.SellTargetPercent += paramRange.SellTargetPercent.StepSize)
                        {
                            analyzer.CalculatePerformanceResults(currentParameters, _startCapital);
                            if (currentParameters.Results.WeightedProfit <= bestParameters.Results.WeightedProfit)
                                continue;

                            bestParameters.Update(currentParameters);
                            if (_showOutput) Console.WriteLine(bestParameters);
                        }
                    }
                }
            }

            // refine the parameter range
            paramRange.Update(bestParameters);
            return bestParameters;
        }

        public Parameters Optimize(string outputPath)
        {
            const int  numDaysUntilSettlement = 5;
            var        analyzer = new DeferredAnalyzer(_security, _transactionFee, numDaysUntilSettlement);
            var        paramRange = new ParameterRange();
            Parameters bestParameters = null;

            const int numStages = 5;

            for (var currentStage = 1; currentStage <= numStages; currentStage++)
            {
                if (_showOutput)
                {
                    Console.WriteLine("Current stage: {0} ({1})", currentStage, _security.Symbol);
                    Console.WriteLine("================================");
                }

                bestParameters = FindBestParameters(analyzer, paramRange);
            }

            if (_showOutput) DisplayTransactions(analyzer, bestParameters);

            if (bestParameters == null) return null;

            if (_showOutput)
            {
                Console.WriteLine("Results:");
                Console.WriteLine("================================================================================");
                Console.WriteLine(bestParameters);
                Console.WriteLine();
            }

            using var writer = new BinaryWriter(new FileStream(outputPath, FileMode.Create));
            bestParameters.Write(writer);

            return bestParameters;
        }

        private void DisplayTransactions(IAnalyzer analyzer, Parameters parameters) =>
            analyzer.CalculatePerformanceResults(parameters, _startCapital, _showOutput);
    }
}