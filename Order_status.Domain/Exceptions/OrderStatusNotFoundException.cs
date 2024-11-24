namespace Order_status.Domain.Exceptions
{
    public class OrderStatusNotFoundException : Exception
    {
        public OrderStatusNotFoundException(String message) : base(message) { }
    }
}
