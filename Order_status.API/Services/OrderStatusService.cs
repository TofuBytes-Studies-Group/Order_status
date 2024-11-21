using Order_status.API.Kafka.DTOs;
using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;
using Order_status.Infrastructure.Kafka;

namespace Order_status.API.Services
{
    public class OrderStatusService
    {
        private readonly KafkaProducer _kafkaProducer;
        public OrderStatusService(KafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        public async void DoStuff()
        {
            // Brug KafkaProducer
            await _kafkaProducer.ProduceAsync("topic", "Virker", "From DOSTUFF");
        }

        public OrderStatus? GetOrderStatus(Guid orderId, string username)
        {
            // Get order status from db
            return null;

        }

        public void SetOrderStatusAsAccepted(OrderDTO orderDto)
        {
            SaveOrderStatus(orderDto.Id, orderDto.CustomerName, Status.Accepted);
        }

        private void SaveOrderStatus(Guid orderId, string customerName, Status status)
        {
            OrderStatus orderStatus = new OrderStatus() { OrderId = orderId, CustomerName = customerName, Status = status };
            // add to db
        }
    }
}
