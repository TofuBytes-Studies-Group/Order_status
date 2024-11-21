using Order_status.Domain.Aggregates;

namespace Order_status.Infrastructure.Repositories
{
    public interface IOrderStatusRepository
    {
        OrderStatus GetOrderStatus(Guid orderId);
        void UpdateOrderStatus(OrderStatus orderStatus);
    }
}
