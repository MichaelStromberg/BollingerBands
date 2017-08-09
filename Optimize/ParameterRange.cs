using System;
using System.Text;

namespace Optimize
{
    public class ParameterRange
    {
        #region members

        public IntParameterRange NumPeriods;
        public DoubleParameterRange NumStddevs;
        public DoubleParameterRange BuyTargetPercent;
        public DoubleParameterRange SellTargetPercent;

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public ParameterRange()
        {
            NumPeriods        = new IntParameterRange    { Begin = 2,   End = 20,                   Min = 2,   Max = 20 };
            NumStddevs        = new DoubleParameterRange { Begin = 0.5, End = 4.0, StepSize = 0.1,  Min = 0.5, Max = 4.0 };
            BuyTargetPercent  = new DoubleParameterRange { Begin = 0.9, End = 1.1, StepSize = 0.01, Min = 0.9, Max = 1.1 };
            SellTargetPercent = new DoubleParameterRange { Begin = 0.9, End = 1.1, StepSize = 0.01, Min = 0.9, Max = 1.1 };
        }

        /// <summary>
        /// updates the parameter range to focus around the best values
        /// </summary>
        public void Update(Parameters bestParameters)
        {
            CalculateNewRange(bestParameters.NumPeriods, NumPeriods);
            CalculateNewRange(bestParameters.NumStddevs, NumStddevs);
            CalculateNewRange(bestParameters.BuyTargetPercent, BuyTargetPercent);
            CalculateNewRange(bestParameters.SellTargetPercent, SellTargetPercent);
        }

        /// <summary>
        /// calculates a new range given integer values
        /// </summary>
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

        /// <summary>
        /// calculates a new range given double values
        /// </summary>
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

        /// <summary>
        /// returns a string representation of our parameter range
        /// </summary>
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

            public int Min;
            public int Max;

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

            public double Min;
            public double Max;

            public override string ToString()
            {
                return $"{Begin} - {End} ({StepSize})";
            }
        }
    }
}
