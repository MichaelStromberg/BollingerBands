using System.IO;

namespace Securities
{
    public interface ISecurity
    {
        string Symbol { get; }
        IPrice[] Prices { get; }
        void Write(BinaryWriter writer);
        ISecurity LastYear();
    }
}
