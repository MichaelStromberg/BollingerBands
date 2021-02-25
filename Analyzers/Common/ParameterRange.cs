using System;
using System.Text;

namespace Analyzers.Common
{
    public class ParameterRange
    {
        public readonly IntParameterRange NumPeriods;
        public readonly DoubleParameterRange NumStddevs;
        public readonly DoubleParameterRange BuyTargetPercent;
        public readonly DoubleParameterRange SellTargetPercent;

        public ParameterRange()
        {
            NumPeriods        = new IntParameterRange(2, 20);
            NumStddevs        = new DoubleParameterRange(0.5, 5, 0.1);
            BuyTargetPercent  = new DoubleParameterRange(0.96, 1.00, 0.005);
            SellTargetPercent = new DoubleParameterRange(1.00, 1.04, 0.005);
        }
        
        public void Update(Parameters bestParameters)
        {
            CalculateNewRange(bestParameters.NumPeriods, NumPeriods);
            CalculateNewRange(bestParameters.NumStddevs, NumStddevs);
            CalculateNewRange(bestParameters.BuyTargetPercent, BuyTargetPercent);
            CalculateNewRange(bestParameters.SellTargetPercent, SellTargetPercent);
        }
        
        private static void CalculateNewRange(int bestValue, IntParameterRange paramRange)
        {
            if (paramRange.Begin == paramRange.End) return;

            int oldNumSteps     = paramRange.End - paramRange.Begin;
            double halfNumSteps = oldNumSteps / 2.0;

            const double newStepSize = 0.1;
            var begin = (int)Math.Floor(bestValue - halfNumSteps * newStepSize);
            var end   = (int)Math.Ceiling(bestValue + halfNumSteps * newStepSize);

            if (begin < paramRange.Min) begin = paramRange.Min;
            if (end   > paramRange.Max) end   = paramRange.Max;

            paramRange.Begin = begin;
            paramRange.End   = end;
        }
        
        private static void CalculateNewRange(double bestValue, DoubleParameterRange paramRange)
        {
            double oldNumSteps  = (paramRange.End - paramRange.Begin) / paramRange.StepSize;
            double halfNumSteps = oldNumSteps / 2.0;

            paramRange.StepSize *= 0.1;
            double begin = bestValue - halfNumSteps * paramRange.StepSize;
            double end   = bestValue + halfNumSteps * paramRange.StepSize;

            if (begin < paramRange.Min) begin = paramRange.Min;
            if (end   > paramRange.Max) end   = paramRange.Max;

            paramRange.Begin = begin;
            paramRange.End   = end;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("NumPeriods:        {0}\n", NumPeriods);
            sb.AppendFormat("NumStddevs:        {0}\n", NumStddevs);
            sb.AppendFormat("BuyTargetPercent:  {0}\n", BuyTargetPercent);
            sb.AppendFormat("SellTargetPercent: {0}\n", SellTargetPercent);
            return sb.ToString();
        }

        public class IntParameterRange
        {
            public int Begin;
            public int End;

            public readonly int Min;
            public readonly int Max;

            public IntParameterRange(int min, int max)
            {
                Min   = min;
                Max   = max;
                Begin = min;
                End   = max;
            }

            public override string ToString()
            {
                return $"{Begin} - {End}";
            }
        }

        public class DoubleParameterRange
        {
            public double Begin;
            public double End;
            public double StepSize;

            public readonly double Min;
            public readonly double Max;

            public DoubleParameterRange(double min, double max, double stepSize)
            {
                Min      = min;
                Max      = max;
                Begin    = min;
                End      = max;
                StepSize = stepSize;
            }

            public override string ToString() => $"{Begin:F6} - {End:F6} ({StepSize:E1})";
        }
    }
}
