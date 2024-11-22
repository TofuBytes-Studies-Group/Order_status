using Order_status.API.Kafka.DTOs;
using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;
using Order_status.Infrastructure.Kafka;
using Order_status.Infrastructure.Models;
using Order_status.Infrastructure.Repositories;

namespace Order_status.API.Services
{
    public class OrderStatusService
    {
        private readonly KafkaProducer _kafkaProducer;
        private readonly IOrderStatusRepository _orderStatusRepository;
        public OrderStatusService(KafkaProducer kafkaProducer, IOrderStatusRepository orderStatusRepository)
        {
            _kafkaProducer = kafkaProducer;
            _orderStatusRepository = orderStatusRepository;
        }

        public async void DoStuff()
        {
            // Brug KafkaProducer
            await _orderStatusRepository.CreateOrderStatusAsync(new OrderStatusDTO() { OrderId = Guid.NewGuid(), CustomerName = "TestUser1", Status = Status.Accepted });
            //await _kafkaProducer.ProduceAsync("topic", "Virker", "From DOSTUFF");
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
