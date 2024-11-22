using Order_status.API.Kafka.DTOs;
using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;
using Order_status.Infrastructure.Mappers;
using Order_status.Infrastructure.Models;
using Order_status.Infrastructure.Repositories;

namespace Order_status.API.Services
{
    public class OrderStatusService
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        public OrderStatusService(IOrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        public async void DoStuff()
        {
            await _orderStatusRepository.CreateOrderStatusAsync(new OrderStatusDTO() { OrderId = Guid.NewGuid(), CustomerName = "TestUser1", Status = Status.Accepted });
        }

        public async Task<OrderStatus> GetOrderStatusAsync(Guid orderId)
        {
            OrderStatusDTO orderStatusDTO = await _orderStatusRepository.GetOrderStatusAsync(orderId);
            return OrderStatusMapper.ToDomain(orderStatusDTO);

        }

        public async Task SetOrderStatusAsAcceptedAsync(OrderDTO orderDto)
        {
            await SaveOrderStatusAsync(orderDto.Id, orderDto.CustomerName, Status.Accepted);
        }

        private async Task SaveOrderStatusAsync(Guid orderId, string customerName, Status status)
        {
            OrderStatus orderStatus = new OrderStatus() { OrderId = orderId, CustomerName = customerName, Status = status };
            OrderStatusDTO orderStatusDTO = OrderStatusMapper.ToDTO(orderStatus);
            await _orderStatusRepository.CreateOrderStatusAsync(orderStatusDTO);
        }
    }
}
