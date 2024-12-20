﻿using Order_status.API.Kafka.DTOs;
using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;

namespace Order_status.API.Services
{
    public interface IOrderStatusService
    {
        Task<OrderStatus> GetOrderStatusAsync(Guid orderId);
        Task SetOrderStatusAsAcceptedAsync(OrderDTO orderDto);
        Task UpdateOrderStatusAsync(Guid orderId, Status status); // To pretend like the delivery agent can change the order status
    }
}
