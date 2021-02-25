namespace Analyzers.Common
{
    public record Order(OrderType Type, double Price);

    public enum OrderType
    {
        Buy,
        Sell
    }
}
