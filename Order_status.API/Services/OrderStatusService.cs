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
            if (orderDto.Id == Guid.Empty || string.IsNullOrEmpty(orderDto.CustomerUsername))
            {
                throw new ArgumentException("The order must have a valid order id and customer name");
            }
            OrderStatus orderStatus = new OrderStatus() { OrderId = orderDto.Id, CustomerUsername = orderDto.CustomerUsername, Status = Status.Accepted };
            await _orderStatusRepository.CreateOrderStatusAsync(orderStatus.ToDTO());
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, Status status)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("The order must have a valid order id");
            }
            await _orderStatusRepository.UpdateOrderStatusAsync(orderId, status);
        }
    }
}
