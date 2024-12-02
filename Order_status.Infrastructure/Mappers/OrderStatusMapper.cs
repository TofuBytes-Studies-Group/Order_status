using Order_status.Domain.Aggregates;
using Order_status.Infrastructure.Models;

namespace Order_status.Infrastructure.Mappers
{
    public static class OrderStatusMapper
    {
        public static OrderStatus ToDomain(this OrderStatusDTO dto)
        {
            return new OrderStatus
            {
                OrderId = dto.OrderId,
                CustomerUsername = dto.CustomerUsername,
                Status = dto.Status
            };
        }

        public static OrderStatusDTO ToDTO(this OrderStatus domain)
        {
            return new OrderStatusDTO
            {
                OrderId = domain.OrderId,
                CustomerUsername = domain.CustomerUsername,
                Status = domain.Status
            };
        }
    }
}
