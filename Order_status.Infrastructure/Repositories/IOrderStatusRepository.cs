using Order_status.Infrastructure.Models;

namespace Order_status.Infrastructure.Repositories
{
    public interface IOrderStatusRepository
    {
        Task CreateOrderStatusAsync(OrderStatusDTO orderStatus);
        Task<OrderStatusDTO> GetOrderStatusAsync(Guid orderId);
        Task UpdateOrderStatusAsync(OrderStatusDTO orderStatus);
    }
}
