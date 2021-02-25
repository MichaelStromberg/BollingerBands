using System;
using System.IO;

namespace Securities
{
    public interface IPrice
    {
        DateTime Date { get; }
        double High { get; }
        double Low { get; }
        double Close { get; }
        void Write(BinaryWriter writer);
    }
}
