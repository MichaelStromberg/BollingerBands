using System;
using Securities;

namespace Optimize
{
    internal class Optimizer
    {
        private readonly ISecurity _security;
        private readonly double _startCapital;
        private readonly double _transactionFee;

        public Optimizer(ISecurity security, double startCapital = 100000.0, double transactionFee = 9.99)
        {
            _security       = security;
            _startCapital   = startCapital;
            _transactionFee = transactionFee;
        }

        /// <summary>
        /// returns the optimal parameters given an analyzer, security, and parameter range
        /// </summary>
        private Parameters FindBestParameters(IAnalyzer analyzer, ParameterRange paramRange)
        {
            Console.WriteLine("Parameter estimation using the following parameter ranges:");
            Console.WriteLine(paramRange);

            var currentParameters = new Parameters(paramRange);
            var bestParameters    = new Parameters(paramRange);
            bestParameters.Update(currentParameters);

            for (currentParameters.NumPeriods = paramRange.NumPeriods.Begin; currentParameters.NumPeriods <= paramRange.NumPeriods.End; currentParameters.NumPeriods++)
            {
                for (currentParameters.NumStddevs = paramRange.NumStddevs.Begin; currentParameters.NumStddevs <= paramRange.NumStddevs.End; currentParameters.NumStddevs += paramRange.NumStddevs.StepSize)
                {
                    for (currentParameters.BuyTargetPercent = paramRange.BuyTargetPercent.Begin; currentParameters.BuyTargetPercent <= paramRange.BuyTargetPercent.End; currentParameters.BuyTargetPercent += paramRange.BuyTargetPercent.StepSize)
                    {
                        for (currentParameters.SellTargetPercent = paramRange.SellTargetPercent.Begin; currentParameters.SellTargetPercent < paramRange.SellTargetPercent.End; currentParameters.SellTargetPercent += paramRange.SellTargetPercent.StepSize)
                        {
                            analyzer.CalculatePerformanceResults(currentParameters, _startCapital);
                            if (currentParameters.Results.WeightedProfit <= bestParameters.Results.WeightedProfit) continue;

                            bestParameters.Update(currentParameters);
                            Console.WriteLine(bestParameters);
                        }
                    }
                }
            }

            // refine the parameter range
            paramRange.Update(bestParameters);
            return bestParameters;
        }

        /// <summary>
        /// analyzes each security for Bollinger Band values
        /// </summary>
        public void Optimize()
        {
            const int numDaysUntilSettlement = 5;
            var analyzer              = new SingleAnalyzer(_security, _transactionFee, numDaysUntilSettlement);
            var paramRange            = new ParameterRange();
            Parameters bestParameters = null;

            const int numStages = 5;

            for (int currentStage = 1; currentStage <= numStages; currentStage++)
            {
                Console.WriteLine("Current stage: {0} ({1})", currentStage, _security.Symbol);
                Console.WriteLine("================================");
                bestParameters = FindBestParameters(analyzer, paramRange);
                Console.WriteLine();
            }

            DisplayTransactions(analyzer, bestParameters);

            if (bestParameters == null) return;

            Console.WriteLine("Results:");
            Console.WriteLine("================================================================================");
            Console.WriteLine(bestParameters);
            //parameters.Save(_security.GetBollingerBandPath());
        }

        private void DisplayTransactions(IAnalyzer analyzer, Parameters parameters)
        {
            analyzer.CalculatePerformanceResults(parameters, _startCapital, true);
        }
    }
}
