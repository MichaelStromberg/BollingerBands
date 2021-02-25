using System;

namespace Analyzers.Common
{
    public class Transaction
    {
        public DateTime TimeStamp { get; private set; }
        public bool     IsEnabled { get; private set; }
        public double   Balance   { get; private set; }

        public void Set(DateTime timeStamp, double balance)
        {
            TimeStamp = timeStamp;
            Balance   = balance;
            IsEnabled = true;
        }
    }
}