using System;
using System.IO;

namespace Securities
{
    public class Price : IPrice
    {
        public DateTime Date { get; }
        public double Open { get; }
        public double High { get; }
        public double Low { get; }
        public double Close { get; }

        /// <summary>
        /// constructor
        /// </summary>
        public Price(DateTime date, double open, double high, double low, double close)
        {
            Date  = date;
            Open  = open;
            High  = high;
            Low   = low;
            Close = close;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Date.Ticks);
            writer.Write(Open);
            writer.Write(High);
            writer.Write(Low);
            writer.Write(Close);
        }

        public static IPrice GetPrice(BinaryReader reader)
        {
            long ticks   = reader.ReadInt64();
            var date     = new DateTime(ticks);
            double open  = reader.ReadDouble();
            double high  = reader.ReadDouble();
            double low   = reader.ReadDouble();
            double close = reader.ReadDouble();

            return new Price(date, open, high, low, close);
        }
    }
}
