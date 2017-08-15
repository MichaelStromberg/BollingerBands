using System;
using System.IO;

namespace Securities
{
    public interface ISecurity
    {
        string Symbol { get; }
        IPrice[] Prices { get; }
        void Write(BinaryWriter writer);
        ISecurity Filter(DateTime beginDate, DateTime endDate);
        ISecurity LastTwoWeeks(int offset);
    }
}
