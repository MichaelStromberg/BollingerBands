﻿using System.IO;

namespace Analyzers.Common
{
    public class Parameters
    {
        public PerformanceResults Results { get; private set; }

        public int NumPeriods;
        public double NumStddevs;
        public double BuyTargetPercent;
        public double SellTargetPercent;

        private Parameters(int numPeriods, double numStddevs, double buyTargetPercent, double sellTargetPercent)
        {
            NumPeriods        = numPeriods;
            NumStddevs        = numStddevs;
            BuyTargetPercent  = buyTargetPercent;
            SellTargetPercent = sellTargetPercent;
            Results           = new PerformanceResults();
        }

        public Parameters(ParameterRange range) : this(range.NumPeriods.Begin, range.NumStddevs.Begin,
            range.BuyTargetPercent.Begin, range.SellTargetPercent.Begin)
        {}
        
        public void Update(Parameters other)
        {
            NumPeriods        = other.NumPeriods;
            NumStddevs        = other.NumStddevs;
            BuyTargetPercent  = other.BuyTargetPercent;
            SellTargetPercent = other.SellTargetPercent;
            Results           = other.Results;
        }

        public override string ToString() =>
            $"Best annualized rate of return: {Results.AnnualizedRateOfReturn * 100.0:F2}% with {NumPeriods} periods, {NumStddevs:F5} σ. Buy: {BuyTargetPercent * 100.0:F5}%, Sell: {SellTargetPercent * 100.0:F5}%, Profit: {Results.Profit:C}, Trade Span: {Results.TradeSpanPercentage * 100.0:F2}%";

        public void UpdateResults(IAnalyzerState state) => Results = state.GetPerformanceResults();

        public static Parameters Read(BinaryReader reader)
        {
            int numPeriods           = reader.ReadInt32();
            double numStddevs        = reader.ReadDouble();
            double buyTargetPercent  = reader.ReadDouble();
            double sellTargetPercent = reader.ReadDouble();
            return new Parameters(numPeriods, numStddevs, buyTargetPercent, sellTargetPercent);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(NumPeriods);
            writer.Write(NumStddevs);
            writer.Write(BuyTargetPercent);
            writer.Write(SellTargetPercent);
        }
    }
}
