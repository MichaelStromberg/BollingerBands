namespace Analyzers.Common
{
    public class Order
    {
        public readonly OrderType Type;
        public readonly double Price;

        public Order(OrderType type, double price)
        {
            Type  = type;
            Price = price;
        }
    }

    public enum OrderType
    {
        Buy,
        Sell
    }
}
