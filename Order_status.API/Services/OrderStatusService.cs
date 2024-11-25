using Order_status.API.Kafka.DTOs;
using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;
using Order_status.Infrastructure.Mappers;
using Order_status.Infrastructure.Models;
using Order_status.Infrastructure.Repositories;

namespace Order_status.API.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        public OrderStatusService(IOrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        public async Task<OrderStatus> GetOrderStatusAsync(Guid orderId)
        {
            OrderStatusDTO orderStatusDTO = await _orderStatusRepository.GetOrderStatusAsync(orderId);
            return orderStatusDTO.ToDomain();
        }

        public async Task SetOrderStatusAsAcceptedAsync(OrderDTO orderDto)
        {
            if (orderDto.Id == Guid.Empty || string.IsNullOrEmpty(orderDto.CustomerName))
            {
                throw new ArgumentException("The order must have a valid order id and customer name");
            }
            await SaveOrderStatusAsync(orderDto.Id, orderDto.CustomerName, Status.Accepted);
        }

        private async Task SaveOrderStatusAsync(Guid orderId, string customerName, Status status)
        {
            OrderStatus orderStatus = new OrderStatus() { OrderId = orderId, CustomerName = customerName, Status = status };
            OrderStatusDTO orderStatusDTO = orderStatus.ToDTO();
            await _orderStatusRepository.CreateOrderStatusAsync(orderStatusDTO);
        }
    }
}
