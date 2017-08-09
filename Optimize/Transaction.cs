using System;

namespace Optimize
{
    public class Transaction
    {
        public DateTime TimeStamp { get; private set; }
        public bool IsEnabled { get; private set; }
        public double Balance { get; private set; }

        /// <summary>
        /// sets the transaction timestamp
        /// </summary>
        public void Set(DateTime timeStamp, double balance)
        {
            TimeStamp = timeStamp;
            Balance   = balance;
            IsEnabled = true;
        }
    }
}
