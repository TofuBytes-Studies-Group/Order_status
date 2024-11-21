namespace Order_status.API.Kafka.DTOs
{
    // The order status microservice is only interested in the order id and the customer name 
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public required string CustomerName { get; set; }
    }
}
